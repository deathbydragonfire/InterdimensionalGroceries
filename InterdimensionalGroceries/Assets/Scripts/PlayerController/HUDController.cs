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
            
            UpdateChargeBar(0f);
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
    }
}
