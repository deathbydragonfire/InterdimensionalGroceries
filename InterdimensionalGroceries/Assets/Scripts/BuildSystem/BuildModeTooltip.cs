using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace InterdimensionalGroceries.BuildSystem
{
    public class BuildModeTooltip : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private Text nameText;
        [SerializeField] private Text priceText;
        [SerializeField] private Text descriptionText;
        
        [Header("Settings")]
        [SerializeField] private Vector2 offset = new Vector2(10, 10);
        [SerializeField] private bool followCursor = false;
        
        private RectTransform tooltipRect;
        private Canvas parentCanvas;
        private bool isVisible = false;
        
        private void Awake()
        {
            if (tooltipPanel != null)
            {
                tooltipRect = tooltipPanel.GetComponent<RectTransform>();
                parentCanvas = GetComponentInParent<Canvas>();
            }
            Hide();
        }
        
        public void Show(BuildableObject buildableObject, Vector2 position)
        {
            if (buildableObject == null || tooltipPanel == null) return;
            
            if (nameText != null)
            {
                nameText.text = $"Name: {buildableObject.ObjectName}";
            }
            
            if (priceText != null)
            {
                priceText.text = $"Price: {buildableObject.PlacementCost}$";
            }
            
            if (descriptionText != null)
            {
                descriptionText.text = $"Description: {buildableObject.Description}";
            }
            
            tooltipPanel.SetActive(true);
            isVisible = true;
            UpdatePosition(position);
        }
        
        public void Hide()
        {
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
                isVisible = false;
            }
        }
        
        public void UpdatePosition(Vector2 mousePosition)
        {
            if (tooltipRect == null || parentCanvas == null || !isVisible) return;
            
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                mousePosition,
                parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera,
                out localPoint
            );
            
            tooltipRect.localPosition = localPoint + offset;
        }
        
        private void Update()
        {
            if (followCursor && isVisible)
            {
                UpdatePosition(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            }
        }
    }
}
