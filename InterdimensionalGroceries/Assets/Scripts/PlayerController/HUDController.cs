using UnityEngine;
using UnityEngine.UIElements;

namespace InterdimensionalGroceries.PlayerController
{
    [RequireComponent(typeof(UIDocument))]
    public class HUDController : MonoBehaviour
    {
        private UIDocument uiDocument;
        private VisualElement crosshair;
        private VisualElement chargeBarContainer;
        private VisualElement chargeBarFill;
        private Label interactionTooltip;

        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
        }

        private void Start()
        {
            var root = uiDocument.rootVisualElement;
            
            crosshair = root.Q<VisualElement>("Crosshair");
            chargeBarContainer = root.Q<VisualElement>("ChargeBarContainer");
            chargeBarFill = root.Q<VisualElement>("ChargeBarFill");
            interactionTooltip = root.Q<Label>("InteractionTooltip");
            
            UpdateChargeBar(0f);
            HideInteractionTooltip();
        }

        public void SetCrosshairVisible(bool visible)
        {
            if (crosshair != null)
            {
                crosshair.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        public void UpdateChargeBar(float chargePercent)
        {
            if (chargeBarContainer != null && chargeBarFill != null)
            {
                if (chargePercent <= 0f)
                {
                    chargeBarContainer.style.display = DisplayStyle.None;
                }
                else
                {
                    chargeBarContainer.style.display = DisplayStyle.Flex;
                    chargeBarFill.style.width = Length.Percent(chargePercent * 100f);
                }
            }
        }

        public void ShowInteractionTooltip(string text = "[Left Click] to Interact")
        {
            if (interactionTooltip != null)
            {
                interactionTooltip.text = text;
                interactionTooltip.style.display = DisplayStyle.Flex;
            }
            else
            {
                Debug.LogWarning("InteractionTooltip element not found in UI!");
            }
        }

        public void HideInteractionTooltip()
        {
            if (interactionTooltip != null)
            {
                interactionTooltip.style.display = DisplayStyle.None;
            }
        }
    }
}
