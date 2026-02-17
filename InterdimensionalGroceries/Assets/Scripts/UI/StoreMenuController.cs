using UnityEngine;
using UnityEngine.UIElements;
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

        [Header("Controllers")]
        [SerializeField] private StoreUIController suppliesController;
        [SerializeField] private AbilitiesMenuController abilitiesController;

        private VisualElement mainMenuContainer;
        private VisualElement suppliesContainer;
        private VisualElement abilitiesContainer;
        private Label mainMenuMoneyLabel;
        private System.Action onCloseCallback;

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

                var suppliesButton = mainRoot.Q<Button>("SuppliesButton");
                var abilitiesButton = mainRoot.Q<Button>("AbilitiesButton");
                var roomsButton = mainRoot.Q<Button>("RoomsButton");
                var furnitureButton = mainRoot.Q<Button>("FurnitureButton");
                var closeButton = mainRoot.Q<Button>("CloseButton");

                Debug.Log($"StoreMenuController: Buttons found - Supplies:{suppliesButton != null}, Abilities:{abilitiesButton != null}, Close:{closeButton != null}");

                if (suppliesButton != null)
                {
                    suppliesButton.clicked += () => { Debug.Log("Supplies button clicked!"); PlayButtonClickSound(); NavigateToSupplies(); };
                }

                if (abilitiesButton != null)
                {
                    abilitiesButton.clicked += () => { Debug.Log("Abilities button clicked!"); PlayButtonClickSound(); NavigateToAbilities(); };
                }

                if (roomsButton != null)
                {
                    roomsButton.clicked += () => { Debug.Log("Rooms button clicked!"); PlayButtonClickSound(); ShowComingSoon("Rooms"); };
                }

                if (furnitureButton != null)
                {
                    furnitureButton.clicked += () => { Debug.Log("Furniture button clicked!"); PlayButtonClickSound(); ShowComingSoon("Furniture"); };
                }

                if (closeButton != null)
                {
                    closeButton.clicked += () => { Debug.Log("Close button clicked!"); PlayCloseStoreSound(); CloseStore(); };
                }

                if (mainMenuContainer != null)
                {
                    mainMenuContainer.style.display = DisplayStyle.None;
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

        private void ShowComingSoon(string featureName)
        {
            Debug.Log($"{featureName} feature coming soon!");
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
