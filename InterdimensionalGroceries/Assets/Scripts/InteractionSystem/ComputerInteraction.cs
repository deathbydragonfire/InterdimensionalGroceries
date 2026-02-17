using UnityEngine;
using InterdimensionalGroceries.UI;

namespace InterdimensionalGroceries.InteractionSystem
{
    public class ComputerInteraction : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float minInteractionDistance = 1f;
        [SerializeField] private float maxInteractionDistance = 4f;
        
        [Header("References")]
        [SerializeField] private StoreUIController storeUIController;
        
        private bool isPlayerInRange;
        private bool isStoreOpen;
        private InteractionController interactionController;
        
        private void Start()
        {
            interactionController = FindFirstObjectByType<InteractionController>();
            
            if (interactionController == null)
            {
                Debug.LogWarning("InteractionController not found in scene.");
            }
        }
        
        public bool IsPlayerInRange(Vector3 playerPosition)
        {
            float distance = Vector3.Distance(transform.position, playerPosition);
            isPlayerInRange = distance >= minInteractionDistance && distance <= maxInteractionDistance;
            return isPlayerInRange;
        }
        
        public void Interact()
        {
            Debug.Log($"ComputerInteraction.Interact() called. isPlayerInRange: {isPlayerInRange}, isStoreOpen: {isStoreOpen}");
            
            if (!isPlayerInRange || isStoreOpen)
            {
                Debug.Log("Interaction blocked: " + (!isPlayerInRange ? "player not in range" : "store already open"));
                return;
            }
                
            OpenStore();
        }
        
        private void OpenStore()
        {
            Debug.Log("OpenStore() called");
            
            if (storeUIController != null)
            {
                storeUIController.OpenStore(OnStoreClosed);
                isStoreOpen = true;
            }
            else
            {
                Debug.LogWarning("StoreUIController not assigned to ComputerInteraction.");
            }
        }
        
        private void OnStoreClosed()
        {
            Debug.Log("OnStoreClosed() callback");
            isStoreOpen = false;
            
            if (interactionController != null)
            {
                interactionController.OnInteractionClosed();
            }
        }
        
        public void CloseInteraction()
        {
            Debug.Log("CloseInteraction() called");
            
            if (storeUIController != null)
            {
                storeUIController.ForceClose();
            }
        }
    }
}
