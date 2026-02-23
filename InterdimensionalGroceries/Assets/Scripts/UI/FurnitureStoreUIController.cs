using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using InterdimensionalGroceries.BuildSystem;
using InterdimensionalGroceries.EconomySystem;
using InterdimensionalGroceries.AudioSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace InterdimensionalGroceries.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class FurnitureStoreUIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private FurnitureStoreInventory furnitureStoreInventory;
        [SerializeField] private FurnitureInventory furnitureInventory;
        [SerializeField] private MoneyNotification notificationPrefab;
        
        private UIDocument uiDocument;
        private VisualElement storeContainer;
        private Label totalCostLabel;
        private Label currentMoneyLabel;
        private Button purchaseButton;
        private Button backButton;
        private Button placeFurnitureButton;
        private VisualElement itemGrid;
        
        private Dictionary<BuildableObject, int> selectedItems = new Dictionary<BuildableObject, int>();
        private Action onBackCallback;
        private bool isOpen;
        private bool isAnimating;
        private MoneyNotification notificationInstance;
        
        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
        }
        
        private void Start()
        {
            var root = uiDocument.rootVisualElement;
            
            var styleSheet = UnityEngine.Resources.Load<StyleSheet>("UI/StoreUI");
            if (styleSheet == null)
            {
#if UNITY_EDITOR
                styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI/StoreUI.uss");
#endif
            }
            
            if (styleSheet != null && !root.styleSheets.Contains(styleSheet))
            {
                root.styleSheets.Add(styleSheet);
            }
            
            storeContainer = root.Q<VisualElement>("StoreContainer");
            totalCostLabel = root.Q<Label>("TotalCost");
            currentMoneyLabel = root.Q<Label>("CurrentMoney");
            purchaseButton = root.Q<Button>("PurchaseButton");
            backButton = root.Q<Button>("BackButton");
            placeFurnitureButton = root.Q<Button>("PlaceFurnitureButton");
            itemGrid = root.Q<VisualElement>("ItemGrid");
            
            if (purchaseButton != null)
            {
                purchaseButton.style.display = DisplayStyle.None;
            }
            
            if (totalCostLabel != null)
            {
                totalCostLabel.style.display = DisplayStyle.None;
            }
            
            if (backButton != null)
            {
                backButton.clicked += OnBackClicked;
            }
            
            if (placeFurnitureButton != null)
            {
                placeFurnitureButton.clicked += OnPlaceFurnitureClicked;
            }
            
            if (storeContainer != null)
            {
                storeContainer.style.display = DisplayStyle.None;
            }
            
            if (notificationPrefab != null)
            {
                Canvas rootCanvas = FindRootCanvas();
                if (rootCanvas != null)
                {
                    notificationInstance = Instantiate(notificationPrefab, rootCanvas.transform);
                }
                else
                {
                    notificationInstance = Instantiate(notificationPrefab);
                }
                
                Canvas notificationCanvas = notificationInstance.GetComponent<Canvas>();
                if (notificationCanvas == null)
                {
                    notificationCanvas = notificationInstance.gameObject.AddComponent<Canvas>();
                    notificationCanvas.overrideSorting = true;
                }
                notificationCanvas.sortingOrder = 1000;
                
                if (notificationInstance.GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
                {
                    notificationInstance.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                }
                
                notificationInstance.gameObject.SetActive(false);
            }
        }
        
        private Canvas FindRootCanvas()
        {
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (Canvas canvas in canvases)
            {
                if (canvas.isRootCanvas)
                {
                    return canvas;
                }
            }
            return null;
        }
        
        public void OpenFurnitureMenu(Action onBack)
        {
            if (isAnimating) return;
            
            onBackCallback = onBack;
            selectedItems.Clear();
            
            PopulateFurnitureItems();
            UpdateTotalCost();
            UpdateMoneyDisplay();
            
            if (storeContainer != null)
            {
                storeContainer.style.display = DisplayStyle.Flex;
                StartCoroutine(AnimateSlideIn());
            }
            
            isOpen = true;
            
            Debug.Log("Furniture menu opened successfully");
        }
        
        private void UpdateMoneyDisplay()
        {
            if (currentMoneyLabel != null && MoneyManager.Instance != null)
            {
                float currentMoney = MoneyManager.Instance.GetCurrentMoney();
                currentMoneyLabel.text = $"Balance: ${currentMoney:F2}";
            }
        }
        
        private void PopulateFurnitureItems()
        {
            if (itemGrid == null || furnitureStoreInventory == null)
                return;
                
            itemGrid.Clear();
            
            foreach (var furniture in furnitureStoreInventory.AvailableFurniture)
            {
                if (furniture == null)
                    continue;
                    
                var itemElement = CreateItemElement(furniture);
                itemGrid.Add(itemElement);
            }
        }
        
        private VisualElement CreateItemElement(BuildableObject furniture)
        {
            var container = new VisualElement();
            container.AddToClassList("item-container");
            
            var nameLabel = new Label(furniture.ObjectName);
            nameLabel.AddToClassList("item-name");
            container.Add(nameLabel);
            
            var priceLabel = new Label($"${furniture.PlacementCost:F2}");
            priceLabel.AddToClassList("item-price");
            container.Add(priceLabel);
            
            var quantityContainer = new VisualElement();
            quantityContainer.AddToClassList("quantity-container");
            
            var decreaseButton = new Button(() => { AdjustQuantity(furniture, -1); RefreshFurnitureDisplay(); }) { text = "−" };
            decreaseButton.AddToClassList("quantity-button");
            decreaseButton.clicked += PlayButtonClickSound;
            
            int currentOwned = furnitureInventory != null ? furnitureInventory.GetFurnitureCount(furniture) : 0;
            var quantityLabel = new Label($"Owned: {currentOwned}");
            quantityLabel.AddToClassList("quantity-label");
            quantityLabel.name = $"quantity-{furniture.name}";
            
            var increaseButton = new Button(() => { AdjustQuantity(furniture, 1); RefreshFurnitureDisplay(); }) { text = "+" };
            increaseButton.AddToClassList("quantity-button");
            increaseButton.clicked += PlayButtonClickSound;
            
            quantityContainer.Add(decreaseButton);
            quantityContainer.Add(quantityLabel);
            quantityContainer.Add(increaseButton);
            
            container.Add(quantityContainer);
            
            return container;
        }
        
        private void RefreshFurnitureDisplay()
        {
            PopulateFurnitureItems();
        }
        
        private void AdjustQuantity(BuildableObject furniture, int delta)
        {
            if (furniture == null) return;
            
            if (delta > 0)
            {
                float cost = furniture.PlacementCost;
                
                if (MoneyManager.Instance != null)
                {
                    if (MoneyManager.Instance.SpendMoney(cost))
                    {
                        if (furnitureInventory != null)
                        {
                            furnitureInventory.AddFurniture(furniture, 1);
                        }
                        
                        if (AudioManager.Instance != null)
                        {
                            Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                            AudioManager.Instance.PlaySound(AudioEventType.UIPurchase, soundPosition);
                        }
                        
                        Debug.Log($"Purchased {furniture.ObjectName} for ${cost:F2}");
                    }
                    else
                    {
                        Debug.Log("Insufficient funds for purchase.");
                        
                        if (AudioManager.Instance != null)
                        {
                            Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                            AudioManager.Instance.PlaySound(AudioEventType.Rejection, soundPosition);
                        }
                        
                        ShowInsufficientFundsNotification(cost);
                    }
                }
            }
            else if (delta < 0)
            {
                if (furnitureInventory != null && furnitureInventory.GetFurnitureCount(furniture) > 0)
                {
                    float refund = furniture.PlacementCost;
                    
                    if (furnitureInventory.RemoveFurniture(furniture, 1))
                    {
                        if (MoneyManager.Instance != null)
                        {
                            MoneyManager.Instance.AddMoney(refund);
                        }
                        
                        if (AudioManager.Instance != null)
                        {
                            Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                            AudioManager.Instance.PlaySound(AudioEventType.UIPurchase, soundPosition);
                        }
                        
                        Debug.Log($"Sold {furniture.ObjectName} for ${refund:F2} refund");
                    }
                }
                else
                {
                    if (AudioManager.Instance != null)
                    {
                        Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                        AudioManager.Instance.PlaySound(AudioEventType.Rejection, soundPosition);
                    }
                }
            }
            
            UpdateMoneyDisplay();
        }
        
        private void ShowInsufficientFundsNotification(float requiredAmount)
        {
            if (notificationInstance != null)
            {
                float currentMoney = MoneyManager.Instance != null ? MoneyManager.Instance.GetCurrentMoney() : 0f;
                float shortfall = requiredAmount - currentMoney;
                notificationInstance.ShowCustomMessage($"Insufficient Funds! Need ${shortfall:F2} more");
            }
        }
        
        private void PlayButtonClickSound()
        {
            if (AudioManager.Instance != null)
            {
                Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                AudioManager.Instance.PlaySound(AudioEventType.UIButtonClick, soundPosition);
            }
        }
        
        private void UpdateTotalCost()
        {
            float total = 0f;
            
            foreach (var kvp in selectedItems)
            {
                total += kvp.Key.PlacementCost * kvp.Value;
            }
            
            if (totalCostLabel != null)
            {
                totalCostLabel.text = $"Total: ${total:F2}";
            }
        }
        
        private void OnPurchaseClicked()
        {
            float totalCost = 0f;
            Dictionary<BuildableObject, int> itemsToPurchase = new Dictionary<BuildableObject, int>();
            
            foreach (var kvp in selectedItems)
            {
                if (kvp.Value > 0)
                {
                    totalCost += kvp.Key.PlacementCost * kvp.Value;
                    itemsToPurchase[kvp.Key] = kvp.Value;
                }
            }
            
            if (itemsToPurchase.Count == 0)
            {
                Debug.Log("No furniture selected for purchase.");
                PlayButtonClickSound();
                return;
            }
            
            if (MoneyManager.Instance != null && MoneyManager.Instance.SpendMoney(totalCost))
            {
                Debug.Log($"Purchase successful! Spent ${totalCost:F2} on furniture.");
                
                if (AudioManager.Instance != null)
                {
                    Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                    AudioManager.Instance.PlaySound(AudioEventType.UIPurchase, soundPosition);
                }
                
                if (furnitureInventory != null)
                {
                    foreach (var kvp in itemsToPurchase)
                    {
                        furnitureInventory.AddFurniture(kvp.Key, kvp.Value);
                    }
                }
                
                GoBack();
            }
            else
            {
                Debug.Log("Insufficient funds for purchase.");
                
                if (AudioManager.Instance != null)
                {
                    Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                    AudioManager.Instance.PlaySound(AudioEventType.Rejection, soundPosition);
                }
            }
        }
        
        private void OnBackClicked()
        {
            PlayButtonClickSound();
            GoBack();
        }
        
        private void OnPlaceFurnitureClicked()
        {
            PlayButtonClickSound();
            
            // Close the furniture menu immediately
            if (storeContainer != null)
            {
                storeContainer.style.display = DisplayStyle.None;
            }
            isOpen = false;
            
            // Close the entire store menu
            var storeMenuController = FindFirstObjectByType<StoreMenuController>();
            if (storeMenuController != null)
            {
                storeMenuController.CloseStore();
            }
            
            // Enter build mode with callback to reopen furniture store
            if (BuildModeController.Instance != null)
            {
                BuildModeController.Instance.EnterFurniturePlacementMode(() => 
                {
                    // When exiting build mode, reopen the store to furniture tab
                    if (storeMenuController != null)
                    {
                        storeMenuController.OpenToFurnitureTab();
                    }
                });
            }
        }
        
        private void GoBack()
        {
            if (isAnimating) return;
            
            StartCoroutine(AnimateSlideOut());
        }
        
        private IEnumerator AnimateSlideIn()
        {
            isAnimating = true;
            
            var storeWindow = storeContainer.Query<VisualElement>(className: "store-window").First();
            if (storeWindow == null)
            {
                Debug.LogWarning("Could not find store-window element for animation");
                isAnimating = false;
                yield break;
            }
            
            float duration = 0.5f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                float eased = EaseOutBack(t);
                float yPercent = Mathf.Lerp(100, 0, eased);
                
                storeWindow.style.translate = new Translate(0, Length.Percent(yPercent));
                
                yield return null;
            }
            
            storeWindow.style.translate = new Translate(0, 0);
            isAnimating = false;
        }
        
        private IEnumerator AnimateSlideOut()
        {
            isAnimating = true;
            
            var storeWindow = storeContainer.Query<VisualElement>(className: "store-window").First();
            if (storeWindow == null)
            {
                Debug.LogWarning("Could not find store-window element for animation");
                isAnimating = false;
                yield break;
            }
            
            float duration = 0.4f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                float eased = t * t;
                float yPercent = Mathf.Lerp(0, 100, eased);
                
                storeWindow.style.translate = new Translate(0, Length.Percent(yPercent));
                
                yield return null;
            }
            
            if (storeContainer != null)
            {
                storeContainer.style.display = DisplayStyle.None;
            }
            
            isOpen = false;
            isAnimating = false;
            
            Debug.Log("Furniture menu closed, invoking callback");
            onBackCallback?.Invoke();
        }
        
        private float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            
            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }
    }
}
