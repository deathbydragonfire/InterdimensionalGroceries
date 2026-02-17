using UnityEngine;
using UnityEngine.InputSystem;
using InterdimensionalGroceries.PlayerController;

namespace InterdimensionalGroceries.InteractionSystem
{
    public class InteractionController : MonoBehaviour
    {
        [Header("Raycast Settings")]
        [SerializeField] private float maxInteractionDistance = 4f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private Transform cameraTransform;
        
        private InputSystem_Actions inputActions;
        private HUDController hudController;
        private ComputerInteraction currentInteractable;
        private bool isInteracting;
        
        private void Awake()
        {
            inputActions = new InputSystem_Actions();
            
            if (cameraTransform == null)
            {
                cameraTransform = Camera.main?.transform;
            }
            
            hudController = GetComponentInChildren<HUDController>();
        }
        
        private void Start()
        {
            Debug.Log("InteractionController started. Interact action exists: " + (inputActions.Player.Interact != null));
        }
        
        private void OnEnable()
        {
            inputActions.Player.Enable();
            inputActions.Player.Interact1.performed += OnInteract;
            Debug.Log("InteractionController enabled. Subscribed to Interact1 action (F key).");
        }
        
        private void OnDisable()
        {
            inputActions.Player.Interact1.performed -= OnInteract;
            inputActions.Player.Disable();
        }
        
        private void Update()
        {
            CheckForInteractable();
        }
        
        private void CheckForInteractable()
        {
            if (cameraTransform == null)
            {
                Debug.LogWarning("InteractionController: Camera transform is null!");
                return;
            }
            
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, maxInteractionDistance, interactableLayer))
            {
                ComputerInteraction interactable = hit.collider.GetComponent<ComputerInteraction>();
                
                if (interactable != null && interactable.IsPlayerInRange(cameraTransform.position))
                {
                    currentInteractable = interactable;
                    
                    if (hudController != null)
                    {
                        hudController.ShowInteractionTooltip();
                    }
                    return;
                }
            }
            
            currentInteractable = null;
            
            if (hudController != null)
            {
                hudController.HideInteractionTooltip();
            }
        }
        
        private void OnInteract(InputAction.CallbackContext context)
        {
            Debug.Log($"Interact input received! Current interactable: {(currentInteractable != null ? currentInteractable.name : "null")}, isInteracting: {isInteracting}");
            
            if (isInteracting)
            {
                // Close the interaction
                if (currentInteractable != null)
                {
                    currentInteractable.CloseInteraction();
                    isInteracting = false;
                }
            }
            else if (currentInteractable != null)
            {
                // Start the interaction
                currentInteractable.Interact();
                isInteracting = true;
            }
            else
            {
                Debug.Log("No interactable object in range.");
            }
        }
        
        public void OnInteractionClosed()
        {
            isInteracting = false;
        }
    }
}
