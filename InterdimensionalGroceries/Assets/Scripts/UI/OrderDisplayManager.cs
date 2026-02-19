using TMPro;
using UnityEngine;
using InterdimensionalGroceries.PhaseManagement;

namespace InterdimensionalGroceries.UI
{
    public class OrderDisplayManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshPro orderText;
        
        [Header("Messages")]
        [SerializeField] private string firstTimeMessage = "Make Sure You Order Enough Supplies";
        [SerializeField] private string[] inventoryMessages = new string[]
        {
            "Time to Restock",
            "Check Your Inventory",
            "Order More Supplies",
            "Replenish Stock Now",
            "Review Your Shelves",
            "Purchase New Items",
            "Manage Your Store",
            "Stock Up For Sales"
        };
        
        [Header("Countdown Settings")]
        [SerializeField] private float countdownFontSize = 72f;
        
        private bool isFirstTime = true;
        private float originalFontSize;
        private VerticalAlignmentOptions originalVerticalAlignment;
        private bool originalAutoSizing;
        
        private void Start()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnInventoryPhaseStarted += OnInventoryPhaseStarted;
            }
            
            if (orderText == null)
            {
                orderText = GetComponent<TextMeshPro>();
            }
            
            if (orderText != null)
            {
                originalFontSize = orderText.fontSize;
                originalVerticalAlignment = orderText.verticalAlignment;
                originalAutoSizing = orderText.enableAutoSizing;
            }
            
            ShowFirstTimeMessage();
        }
        
        private void OnDestroy()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= OnInventoryPhaseStarted;
            }
        }
        
        private void OnInventoryPhaseStarted()
        {
            ShowInventoryMessage();
        }
        
        private void ShowFirstTimeMessage()
        {
            if (orderText != null && isFirstTime)
            {
                orderText.text = firstTimeMessage;
            }
        }
        
        private void ShowInventoryMessage()
        {
            if (orderText == null) return;
            
            if (isFirstTime)
            {
                orderText.text = firstTimeMessage;
                isFirstTime = false;
            }
            else
            {
                int randomIndex = Random.Range(0, inventoryMessages.Length);
                orderText.text = inventoryMessages[randomIndex];
            }
        }
        
        public void SetMessage(string message)
        {
            if (orderText != null)
            {
                orderText.text = message;
            }
        }
        
        public void SetCountdownMode(bool enabled)
        {
            if (orderText == null) return;
            
            if (enabled)
            {
                orderText.enableAutoSizing = false;
                orderText.fontSize = countdownFontSize;
                orderText.verticalAlignment = VerticalAlignmentOptions.Middle;
            }
            else
            {
                orderText.enableAutoSizing = originalAutoSizing;
                orderText.fontSize = originalFontSize;
                orderText.verticalAlignment = originalVerticalAlignment;
            }
        }
    }
}
