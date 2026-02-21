using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using InterdimensionalGroceries.AudioSystem;
using InterdimensionalGroceries.EconomySystem;

namespace InterdimensionalGroceries.UI
{
    public class StoreMenuController : MonoBehaviour
    {
        [Header("UI Documents")]
        [SerializeField] private UIDocument mainMenuDocument;
        [SerializeField] private UIDocument suppliesMenuDocument;
        [SerializeField] private UIDocument abilitiesMenuDocument;
        [SerializeField] private UIDocument furnitureMenuDocument;

        [Header("Controllers")]
        [SerializeField] private StoreUIController suppliesController;
        [SerializeField] private AbilitiesMenuController abilitiesController;
        [SerializeField] private FurnitureStoreUIController furnitureController;

        private VisualElement mainMenuContainer;
        private VisualElement suppliesContainer;
        private VisualElement abilitiesContainer;
        private VisualElement furnitureContainer;
        private VisualElement comingSoonContainer;
        private Label comingSoonText;
        private Label mainMenuMoneyLabel;
        private System.Action onCloseCallback;
        private Coroutine comingSoonCoroutine;

        private enum MenuState
        {
            Closed,
            MainMenu,
            Supplies,
            Abilities,
            Rooms,
            Furniture
        }

        private MenuState currentState = MenuState.Closed;

        public bool IsStoreOpen => currentState != MenuState.Closed;

        private void Start()
        {
            InitializeMenus();
        }

        private void InitializeMenus()
        {
            Debug.Log("StoreMenuController: InitializeMenus called");
            
            if (mainMenuDocument != null)
            {
                var mainRoot = mainMenuDocument.rootVisualElement;
                mainMenuContainer = mainRoot.Q<VisualElement>("StoreContainer");
                mainMenuMoneyLabel = mainRoot.Q<Label>("CurrentMoney");
                comingSoonContainer = mainRoot.Q<VisualElement>("ComingSoonContainer");
                comingSoonText = mainRoot.Q<Label>("ComingSoonText");

                var suppliesButton = mainRoot.Q<Button>("SuppliesButton");
                var abilitiesButton = mainRoot.Q<Button>("AbilitiesButton");
                var toolsButton = mainRoot.Q<Button>("ToolsButton");
                var roomsButton = mainRoot.Q<Button>("RoomsButton");
                var furnitureButton = mainRoot.Q<Button>("FurnitureButton");
                var closeButton = mainRoot.Q<Button>("CloseButton");

                Debug.Log($"StoreMenuController: Buttons found - Supplies:{suppliesButton != null}, Abilities:{abilitiesButton != null}, Tools:{toolsButton != null}, Rooms:{roomsButton != null}, Close:{closeButton != null}");

                if (suppliesButton != null)
                {
                    suppliesButton.clicked += () => { Debug.Log("Supplies button clicked!"); PlayButtonClickSound(); NavigateToSupplies(); };
                }

                if (abilitiesButton != null)
                {
                    abilitiesButton.clicked += () => { Debug.Log("Abilities button clicked!"); PlayButtonClickSound(); NavigateToAbilities(); };
                }

                if (toolsButton != null)
                {
                    toolsButton.clicked += () => { Debug.Log("Tools button clicked!"); PlayButtonClickSound(); ShowComingSoon("Tools"); };
                }

                if (roomsButton != null)
                {
                    roomsButton.clicked += () => { Debug.Log("Rooms button clicked!"); PlayButtonClickSound(); ShowComingSoon("Rooms"); };
                }

                if (furnitureButton != null)
                {
                    furnitureButton.clicked += () => { Debug.Log("Furniture button clicked!"); PlayButtonClickSound(); NavigateToFurniture(); };
                }

                if (closeButton != null)
                {
                    closeButton.clicked += () => { Debug.Log("Close button clicked!"); PlayCloseStoreSound(); CloseStore(); };
                }

                if (mainMenuContainer != null)
                {
                    mainMenuContainer.style.display = DisplayStyle.None;
                }

                if (comingSoonContainer != null)
                {
                    comingSoonContainer.style.display = DisplayStyle.None;
                }
                
                Debug.Log("StoreMenuController: MainMenu initialized, container hidden");
            }

            if (suppliesMenuDocument != null)
            {
                var suppliesRoot = suppliesMenuDocument.rootVisualElement;
                suppliesContainer = suppliesRoot.Q<VisualElement>("StoreContainer");

                if (suppliesContainer != null)
                {
                    suppliesContainer.style.display = DisplayStyle.None;
                }
                
                Debug.Log("StoreMenuController: Supplies menu initialized");
            }

            if (abilitiesMenuDocument != null)
            {
                var abilitiesRoot = abilitiesMenuDocument.rootVisualElement;
                abilitiesContainer = abilitiesRoot.Q<VisualElement>("StoreContainer");

                if (abilitiesContainer != null)
                {
                    abilitiesContainer.style.display = DisplayStyle.None;
                }
                
                Debug.Log("StoreMenuController: Abilities menu initialized");
            }

            if (furnitureMenuDocument != null)
            {
                var furnitureRoot = furnitureMenuDocument.rootVisualElement;
                furnitureContainer = furnitureRoot.Q<VisualElement>("StoreContainer");

                if (furnitureContainer != null)
                {
                    furnitureContainer.style.display = DisplayStyle.None;
                }
                
                Debug.Log("StoreMenuController: Furniture menu initialized");
            }
        }

        public void OpenMainMenu(System.Action onClose = null)
        {
            Debug.Log("StoreMenuController: OpenMainMenu called");
            currentState = MenuState.MainMenu;
            onCloseCallback = onClose;

            if (mainMenuContainer != null)
            {
                mainMenuContainer.style.display = DisplayStyle.Flex;
                UpdateMoneyDisplay();
                Debug.Log("StoreMenuController: Main menu container set to Flex");
            }

            HideAllSubmenus();

            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

            var firstPersonController = FindFirstObjectByType<InterdimensionalGroceries.PlayerController.FirstPersonController>();
            if (firstPersonController != null)
            {
                firstPersonController.SetControlsEnabled(false);
            }

            if (AudioManager.Instance != null)
            {
                Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                AudioManager.Instance.PlaySound(AudioEventType.UIOpenStore, soundPosition);
            }
        }

        public void OpenToFurnitureTab()
        {
            Debug.Log("StoreMenuController: OpenToFurnitureTab called");
            
            // Open the main menu first (sets up cursor, disables controls, etc.)
            currentState = MenuState.MainMenu;
            
            if (mainMenuContainer != null)
            {
                mainMenuContainer.style.display = DisplayStyle.None; // Hide main menu
            }

            HideAllSubmenus();

            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;

            var firstPersonController = FindFirstObjectByType<InterdimensionalGroceries.PlayerController.FirstPersonController>();
            if (firstPersonController != null)
            {
                firstPersonController.SetControlsEnabled(false);
            }

            // Navigate directly to furniture tab
            NavigateToFurniture();
        }

        public void NavigateToMainMenu()
        {
            HideAllSubmenus();

            if (mainMenuContainer != null)
            {
                mainMenuContainer.style.display = DisplayStyle.Flex;
                UpdateMoneyDisplay();
            }

            currentState = MenuState.MainMenu;
        }

        private void NavigateToSupplies()
        {
            Debug.Log("StoreMenuController: NavigateToSupplies called");
            
            if (mainMenuContainer != null)
            {
                mainMenuContainer.style.display = DisplayStyle.None;
                Debug.Log("StoreMenuController: Main menu hidden");
            }

            if (suppliesContainer != null)
            {
                suppliesContainer.style.display = DisplayStyle.Flex;
                Debug.Log("StoreMenuController: Supplies container shown");
            }

            if (suppliesController != null)
            {
                suppliesController.OpenSuppliesMenu(() => NavigateToMainMenu());
            }

            currentState = MenuState.Supplies;
            Debug.Log("StoreMenuController: Now in Supplies state");
        }

        private void NavigateToAbilities()
        {
            Debug.Log("StoreMenuController: NavigateToAbilities called");
            
            if (mainMenuContainer != null)
            {
                mainMenuContainer.style.display = DisplayStyle.None;
            }
            
            HideAllSubmenus();

            if (abilitiesContainer != null)
            {
                abilitiesContainer.style.display = DisplayStyle.Flex;
                Debug.Log("StoreMenuController: Abilities container shown");
            }

            if (abilitiesController != null)
            {
                abilitiesController.OpenAbilitiesMenu(() => NavigateToMainMenu());
                Debug.Log("StoreMenuController: AbilitiesController.OpenAbilitiesMenu called");
            }

            currentState = MenuState.Abilities;
        }

        private void NavigateToFurniture()
        {
            Debug.Log("StoreMenuController: NavigateToFurniture called");
            
            if (mainMenuContainer != null)
            {
                mainMenuContainer.style.display = DisplayStyle.None;
            }
            
            HideAllSubmenus();

            if (furnitureContainer != null)
            {
                furnitureContainer.style.display = DisplayStyle.Flex;
                Debug.Log("StoreMenuController: Furniture container shown");
            }

            if (furnitureController != null)
            {
                furnitureController.OpenFurnitureMenu(() => NavigateToMainMenu());
                Debug.Log("StoreMenuController: FurnitureController.OpenFurnitureMenu called");
            }

            currentState = MenuState.Furniture;
        }

        private void ShowComingSoon(string featureName)
        {
            Debug.Log($"{featureName} feature coming soon!");
            
            if (comingSoonCoroutine != null)
            {
                StopCoroutine(comingSoonCoroutine);
            }
            
            comingSoonCoroutine = StartCoroutine(ComingSoonAnimation());
        }

        private IEnumerator ComingSoonAnimation()
        {
            if (comingSoonContainer == null || comingSoonText == null)
            {
                yield break;
            }

            comingSoonContainer.style.display = DisplayStyle.Flex;
            comingSoonText.style.translate = new Translate(new Length(-100, LengthUnit.Percent), 0);

            float duration = 0.5f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                float easedT = EaseOutCubic(t);
                
                float xPosition = Mathf.Lerp(-100f, 0f, easedT);
                comingSoonText.style.translate = new Translate(new Length(xPosition, LengthUnit.Percent), 0);
                
                yield return null;
            }

            comingSoonText.style.translate = new Translate(0, 0);

            yield return new WaitForSeconds(1.5f);

            elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                float easedT = EaseInCubic(t);
                
                float xPosition = Mathf.Lerp(0f, 100f, easedT);
                comingSoonText.style.translate = new Translate(new Length(xPosition, LengthUnit.Percent), 0);
                
                yield return null;
            }

            comingSoonContainer.style.display = DisplayStyle.None;
            comingSoonText.style.translate = new Translate(new Length(-100, LengthUnit.Percent), 0);
            
            comingSoonCoroutine = null;
        }

        private float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        private float EaseInCubic(float t)
        {
            return t * t * t;
        }

        private void HideAllSubmenus()
        {
            if (suppliesContainer != null)
            {
                suppliesContainer.style.display = DisplayStyle.None;
            }

            if (abilitiesContainer != null)
            {
                abilitiesContainer.style.display = DisplayStyle.None;
            }

            if (furnitureContainer != null)
            {
                furnitureContainer.style.display = DisplayStyle.None;
            }
        }

        public void CloseStore()
        {
            Debug.Log("StoreMenuController: CloseStore called");
            
            if (mainMenuContainer != null)
            {
                mainMenuContainer.style.display = DisplayStyle.None;
            }

            HideAllSubmenus();

            currentState = MenuState.Closed;

            var firstPersonController = FindFirstObjectByType<InterdimensionalGroceries.PlayerController.FirstPersonController>();
            if (firstPersonController != null)
            {
                firstPersonController.SetControlsEnabled(true);
            }

            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
            
            Debug.Log("StoreMenuController: Store closed, invoking callback");
            onCloseCallback?.Invoke();
        }

        private void UpdateMoneyDisplay()
        {
            if (mainMenuMoneyLabel != null && MoneyManager.Instance != null)
            {
                float currentMoney = MoneyManager.Instance.GetCurrentMoney();
                mainMenuMoneyLabel.text = $"Balance: ${currentMoney:F2}";
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

        private void PlayCloseStoreSound()
        {
            if (AudioManager.Instance != null)
            {
                Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                AudioManager.Instance.PlaySound(AudioEventType.UICloseStore, soundPosition);
            }
        }
    }
}
