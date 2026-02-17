using UnityEngine;
using UnityEngine.UIElements;
using InterdimensionalGroceries.ItemSystem;

namespace InterdimensionalGroceries.PlayerController
{
    [RequireComponent(typeof(UIDocument))]
    public class PickupUIController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        private UIDocument uiDocument;
        private VisualElement controlHintsContainer;
        private Label controlHintLeft;
        private Label controlHintRight;
        private VisualElement infoPanelContainer;
        private Label nameValue;
        private Label taglineValue;
        private Label priceValue;
        private Label pickupHintLabel;

        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Start()
        {
            var root = uiDocument.rootVisualElement;
            
            controlHintsContainer = root.Q<VisualElement>("ControlHintsContainer");
            controlHintLeft = root.Q<Label>("ControlHintLeft");
            controlHintRight = root.Q<Label>("ControlHintRight");
            
            infoPanelContainer = root.Q<VisualElement>("InfoPanelContainer");
            nameValue = root.Q<Label>("NameValue");
            taglineValue = root.Q<Label>("TaglineValue");
            priceValue = root.Q<Label>("PriceValue");
            
            pickupHintLabel = root.Q<Label>("PickupHint");
            
            HideControlHints();
            HideInfoPanel();
            HidePickupHint();
        }

        public void ShowControlHints(Vector3 worldPosition, float holdDistance, float minDistance, float maxDistance)
        {
            if (controlHintsContainer == null || mainCamera == null)
                return;

            controlHintsContainer.style.position = Position.Absolute;
            controlHintsContainer.style.left = new Length(50, LengthUnit.Percent);
            controlHintsContainer.style.top = new Length(50, LengthUnit.Percent);
            controlHintsContainer.style.translate = new Translate(new Length(-50, LengthUnit.Percent), new Length(-40, LengthUnit.Pixel));
            controlHintsContainer.style.display = DisplayStyle.Flex;
            controlHintsContainer.style.visibility = Visibility.Visible;
        }

        public void HideControlHints()
        {
            if (controlHintsContainer != null)
            {
                controlHintsContainer.style.display = DisplayStyle.None;
            }
        }

        public void ShowPickupHint()
        {
            if (pickupHintLabel != null)
            {
                pickupHintLabel.style.display = DisplayStyle.Flex;
            }
        }

        public void HidePickupHint()
        {
            if (pickupHintLabel != null)
            {
                pickupHintLabel.style.display = DisplayStyle.None;
            }
        }

        public void ShowInfoPanel(ItemData itemData)
        {
            if (infoPanelContainer == null || itemData == null)
                return;

            nameValue.text = itemData.ItemName;
            taglineValue.text = itemData.Tagline;
            priceValue.text = $"${itemData.Price:F2}";
            
            infoPanelContainer.AddToClassList("visible");
        }

        public void HideInfoPanel()
        {
            if (infoPanelContainer != null)
            {
                infoPanelContainer.RemoveFromClassList("visible");
            }
        }
    }
}
