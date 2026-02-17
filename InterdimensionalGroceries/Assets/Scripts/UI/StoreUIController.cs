using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using InterdimensionalGroceries.EconomySystem;
using InterdimensionalGroceries.AudioSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace InterdimensionalGroceries.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class StoreUIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StoreInventory storeInventory;
        
        private UIDocument uiDocument;
        private VisualElement storeContainer;
        private Label totalCostLabel;
        private Label currentMoneyLabel;
        private Button purchaseButton;
        private Button backButton;
        private VisualElement itemGrid;
        
        private Dictionary<SupplyItemData, int> selectedItems = new Dictionary<SupplyItemData, int>();
        private Action onBackCallback;
        private bool isOpen;
        private bool isAnimating;
        
        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
        }
        
        private void Start()
        {
            var root = uiDocument.rootVisualElement;
            
            // Load and apply the stylesheet
            var styleSheet = UnityEngine.Resources.Load<StyleSheet>("UI/StoreUI");
            if (styleSheet == null)
            {
                // Try loading from Assets path
                styleSheet = UnityEditor.AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI/StoreUI.uss");
            }
            
            if (styleSheet != null && !root.styleSheets.Contains(styleSheet))
            {
                root.styleSheets.Add(styleSheet);
                Debug.Log("StoreUI stylesheet loaded successfully");
            }
            else if (styleSheet == null)
            {
                Debug.LogWarning("Could not find StoreUI.uss stylesheet!");
            }
            
            storeContainer = root.Q<VisualElement>("StoreContainer");
            totalCostLabel = root.Q<Label>("TotalCost");
            currentMoneyLabel = root.Q<Label>("CurrentMoney");
            purchaseButton = root.Q<Button>("PurchaseButton");
            backButton = root.Q<Button>("BackButton");
            itemGrid = root.Q<VisualElement>("ItemGrid");
            
            if (purchaseButton != null)
            {
                purchaseButton.clicked += OnPurchaseClicked;
            }
            
            if (backButton != null)
            {
                backButton.clicked += OnBackClicked;
            }
            
            if (storeContainer != null)
            {
                storeContainer.style.display = DisplayStyle.None;
            }
            
            Debug.Log($"StoreUIController initialized. isOpen: {isOpen}, storeContainer exists: {storeContainer != null}");
        }
        
        public void OpenSuppliesMenu(Action onBack)
        {
            if (isAnimating) return;
            
            onBackCallback = onBack;
            selectedItems.Clear();
            
            PopulateStoreItems();
            UpdateTotalCost();
            UpdateMoneyDisplay();
            
            if (storeContainer != null)
            {
                storeContainer.style.display = DisplayStyle.Flex;
                StartCoroutine(AnimateSlideIn());
            }
            
            isOpen = true;
            
            Debug.Log("Supplies menu opened successfully");
        }
        
        private void UpdateMoneyDisplay()
        {
            if (currentMoneyLabel != null && MoneyManager.Instance != null)
            {
                float currentMoney = MoneyManager.Instance.GetCurrentMoney();
                currentMoneyLabel.text = $"Balance: ${currentMoney:F2}";
            }
        }
        
        private void PopulateStoreItems()
        {
            if (itemGrid == null || storeInventory == null)
                return;
                
            itemGrid.Clear();
            
            foreach (var item in storeInventory.AvailableItems)
            {
                if (item == null)
                    continue;
                    
                var itemElement = CreateItemElement(item);
                itemGrid.Add(itemElement);
            }
        }
        
        private VisualElement CreateItemElement(SupplyItemData item)
        {
            var container = new VisualElement();
            container.AddToClassList("item-container");
            
            var nameLabel = new Label(item.ItemName);
            nameLabel.AddToClassList("item-name");
            container.Add(nameLabel);
            
            var priceLabel = new Label($"${item.BasePrice:F2}");
            priceLabel.AddToClassList("item-price");
            container.Add(priceLabel);
            
            var quantityContainer = new VisualElement();
            quantityContainer.AddToClassList("quantity-container");
            
            var decreaseButton = new Button(() => AdjustQuantity(item, -1)) { text = "−" };
            decreaseButton.AddToClassList("quantity-button");
            decreaseButton.clicked += PlayButtonClickSound;
            
            var quantityLabel = new Label("0");
            quantityLabel.AddToClassList("quantity-label");
            quantityLabel.name = $"quantity-{item.name}";
            
            var increaseButton = new Button(() => AdjustQuantity(item, 1)) { text = "+" };
            increaseButton.AddToClassList("quantity-button");
            increaseButton.clicked += PlayButtonClickSound;
            
            quantityContainer.Add(decreaseButton);
            quantityContainer.Add(quantityLabel);
            quantityContainer.Add(increaseButton);
            
            container.Add(quantityContainer);
            
            return container;
        }
        
        private void AdjustQuantity(SupplyItemData item, int delta)
        {
            if (!selectedItems.ContainsKey(item))
            {
                selectedItems[item] = 0;
            }
            
            selectedItems[item] = Mathf.Max(0, selectedItems[item] + delta);
            
            var quantityLabel = itemGrid.Q<Label>($"quantity-{item.name}");
            if (quantityLabel != null)
            {
                quantityLabel.text = selectedItems[item].ToString();
            }
            
            UpdateTotalCost();
            UpdateMoneyDisplay();
        }
        
        private void PlayButtonClickSound()
        {
            if (AudioManager.Instance != null)
            {
                Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                AudioManager.Instance.PlaySound(AudioEventType.UIButtonClick, soundPosition);
            }
        }
        
        private void PlayCloseStoreSound()
        {
            if (AudioManager.Instance != null)
            {
                Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                AudioManager.Instance.PlaySound(AudioEventType.UICloseStore, soundPosition);
            }
        }
        
        private void UpdateTotalCost()
        {
            float total = 0f;
            
            foreach (var kvp in selectedItems)
            {
                total += kvp.Key.BasePrice * kvp.Value;
            }
            
            if (totalCostLabel != null)
            {
                totalCostLabel.text = $"Total: ${total:F2}";
            }
        }
        
        private void OnPurchaseClicked()
        {
            float totalCost = 0f;
            List<SupplyItemData> itemsToPurchase = new List<SupplyItemData>();
            
            foreach (var kvp in selectedItems)
            {
                if (kvp.Value > 0)
                {
                    totalCost += kvp.Key.BasePrice * kvp.Value;
                    
                    for (int i = 0; i < kvp.Value; i++)
                    {
                        itemsToPurchase.Add(kvp.Key);
                    }
                }
            }
            
            if (itemsToPurchase.Count == 0)
            {
                Debug.Log("No items selected for purchase.");
                PlayButtonClickSound();
                return;
            }
            
            if (MoneyManager.Instance != null && MoneyManager.Instance.SpendMoney(totalCost))
            {
                Debug.Log($"Purchase successful! Spent ${totalCost:F2} on {itemsToPurchase.Count} items.");
                
                // Play purchase success sound
                if (AudioManager.Instance != null)
                {
                    Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                    AudioManager.Instance.PlaySound(AudioEventType.UIPurchase, soundPosition);
                }
                
                if (CrateSpawner.Instance != null)
                {
                    CrateSpawner.Instance.SpawnCratesWithItems(itemsToPurchase);
                }
                
                GoBack();
            }
            else
            {
                Debug.Log("Insufficient funds for purchase.");
                // Play rejection sound
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
        
        private void GoBack()
        {
            if (isAnimating) return;
            
            StartCoroutine(AnimateSlideOut());
        }
        
        private IEnumerator AnimateSlideIn()
        {
            isAnimating = true;
            
            // Query by class name, not element name
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
                
                // Ease out back (gentle overshoot instead of multiple bounces)
                float eased = EaseOutBack(t);
                
                // Start from bottom (-100%) to center (0%)
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
            
            // Query by class name, not element name
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
                
                // Ease in
                float eased = t * t;
                
                // Move from center (0%) to bottom (100%)
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
            
            Debug.Log("Supplies menu closed, invoking callback");
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
