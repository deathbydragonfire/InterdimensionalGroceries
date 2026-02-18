using UnityEngine;
using UnityEngine.UIElements;
using InterdimensionalGroceries.UI;

namespace TutorialSystem
{
    public class TutorialStoreMenuController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StoreMenuController storeMenuController;
        [SerializeField] private UIDocument mainMenuDocument;
        [SerializeField] private TutorialManager tutorialManager;

        [Header("Settings")]
        [SerializeField] private string placeShelvesTriggerEventName = "Event_PlaceShelves";

        [Header("UI Settings")]
        [SerializeField] private Font pleaseWaitFont;

        private Button suppliesButton;
        private Button abilitiesButton;
        private Button roomsButton;
        private Button furnitureButton;
        private Button closeButton;
        private VisualElement menuGrid;
        private Label pleaseWaitLabel;
        private VisualElement storeContainer;

        private bool hasFurnitureButtonBeenShown;

        private void Start()
        {
            if (tutorialManager != null)
            {
                tutorialManager.OnEventStarted += HandleTutorialEventStarted;
            }

            InitializeTutorialStoreMenu();
        }

        private void OnDestroy()
        {
            if (tutorialManager != null)
            {
                tutorialManager.OnEventStarted -= HandleTutorialEventStarted;
            }
        }

        private void InitializeTutorialStoreMenu()
        {
            if (mainMenuDocument == null)
            {
                Debug.LogError("[TutorialStoreMenuController] Main menu document is null!");
                return;
            }

            var mainRoot = mainMenuDocument.rootVisualElement;
            storeContainer = mainRoot.Q<VisualElement>("StoreContainer");
            menuGrid = mainRoot.Q<VisualElement>("MenuGrid");

            suppliesButton = mainRoot.Q<Button>("SuppliesButton");
            abilitiesButton = mainRoot.Q<Button>("AbilitiesButton");
            roomsButton = mainRoot.Q<Button>("RoomsButton");
            furnitureButton = mainRoot.Q<Button>("FurnitureButton");
            closeButton = mainRoot.Q<Button>("CloseButton");

            HideAllButtons();
            HideCloseButton();

            if (menuGrid != null)
            {
                pleaseWaitLabel = new Label("Please Wait...");
                pleaseWaitLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                pleaseWaitLabel.style.fontSize = 48;
                pleaseWaitLabel.style.color = new Color(1f, 1f, 1f, 1f);
                
                if (pleaseWaitFont != null)
                {
                    pleaseWaitLabel.style.unityFontDefinition = new StyleFontDefinition(pleaseWaitFont);
                }
                
                pleaseWaitLabel.style.flexGrow = 1;
                pleaseWaitLabel.style.alignSelf = Align.Center;
                pleaseWaitLabel.style.justifyContent = Justify.Center;
                pleaseWaitLabel.style.width = Length.Percent(100);
                pleaseWaitLabel.style.height = Length.Percent(100);
                pleaseWaitLabel.style.position = Position.Absolute;
                pleaseWaitLabel.style.left = 0;
                pleaseWaitLabel.style.top = 0;
                pleaseWaitLabel.style.right = 0;
                pleaseWaitLabel.style.bottom = 0;

                menuGrid.Add(pleaseWaitLabel);
            }

            Debug.Log("[TutorialStoreMenuController] Tutorial store menu initialized - all buttons hidden, close button hidden, 'Please Wait...' shown");
        }

        private void HideAllButtons()
        {
            if (suppliesButton != null)
            {
                suppliesButton.style.display = DisplayStyle.None;
            }

            if (abilitiesButton != null)
            {
                abilitiesButton.style.display = DisplayStyle.None;
            }

            if (roomsButton != null)
            {
                roomsButton.style.display = DisplayStyle.None;
            }

            if (furnitureButton != null)
            {
                furnitureButton.style.display = DisplayStyle.None;
            }
        }

        private void HideCloseButton()
        {
            if (closeButton != null)
            {
                closeButton.style.display = DisplayStyle.None;
            }
        }

        private void HandleTutorialEventStarted(int eventIndex)
        {
            if (hasFurnitureButtonBeenShown || tutorialManager == null)
            {
                return;
            }

            TutorialEvent currentEvent = tutorialManager.GetCurrentEvent();
            if (currentEvent == null)
            {
                return;
            }

            Debug.Log($"[TutorialStoreMenuController] Event started: {currentEvent.name} at index {eventIndex}");

            if (currentEvent.name == placeShelvesTriggerEventName)
            {
                ShowFurnitureButton();
            }
        }

        private void ShowFurnitureButton()
        {
            if (hasFurnitureButtonBeenShown)
            {
                return;
            }

            hasFurnitureButtonBeenShown = true;
            Debug.Log("[TutorialStoreMenuController] Event_PlaceShelves triggered - showing Furniture button");

            if (pleaseWaitLabel != null && menuGrid != null)
            {
                menuGrid.Remove(pleaseWaitLabel);
                pleaseWaitLabel = null;
            }

            if (furnitureButton != null)
            {
                furnitureButton.style.display = DisplayStyle.Flex;
            }
        }
    }
}
