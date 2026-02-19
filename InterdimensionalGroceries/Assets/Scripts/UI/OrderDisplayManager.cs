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
        
        private bool isFirstTime = true;
        
        private void Start()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnInventoryPhaseStarted += OnInventoryPhaseStarted;
                GamePhaseManager.Instance.OnDeliveryPhaseStarted += OnDeliveryPhaseStarted;
            }
            
            if (orderText == null)
            {
                orderText = GetComponent<TextMeshPro>();
            }
            
            ShowFirstTimeMessage();
        }
        
        private void OnDestroy()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= OnInventoryPhaseStarted;
                GamePhaseManager.Instance.OnDeliveryPhaseStarted -= OnDeliveryPhaseStarted;
            }
        }
        
        private void OnInventoryPhaseStarted()
        {
            ShowInventoryMessage();
        }
        
        private void OnDeliveryPhaseStarted()
        {
            if (orderText != null)
            {
                orderText.text = "";
            }
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
    }
}
