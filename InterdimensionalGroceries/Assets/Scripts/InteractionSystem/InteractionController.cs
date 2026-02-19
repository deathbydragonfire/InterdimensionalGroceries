using UnityEngine;
using UnityEngine.InputSystem;
using InterdimensionalGroceries.PlayerController;
using InterdimensionalGroceries.BuildSystem;

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
        private ItemManipulationController itemManipulation;
        
        private void Awake()
        {
            inputActions = new InputSystem_Actions();
            
            if (cameraTransform == null)
            {
                cameraTransform = Camera.main?.transform;
            }
            
            hudController = GetComponentInChildren<HUDController>();
            itemManipulation = FindFirstObjectByType<ItemManipulationController>();
        }
        
        private void Start()
        {
            Debug.Log($"InteractionController started. Interact action exists: {(inputActions.Player.Interact != null)}, isInteracting initial value: {isInteracting}");
        }
        
        private void OnEnable()
        {
            inputActions.Player.Enable();
            inputActions.Player.PickUpPlace.performed += OnInteract;
            Debug.Log("InteractionController enabled. Subscribed to PickUpPlace action (Left Mouse Button).");
        }
        
        private void OnDisable()
        {
            inputActions.Player.PickUpPlace.performed -= OnInteract;
            inputActions.Player.Disable();
        }
        
        private void Update()
        {
            CheckForInteractable();
        }
        
        private void CheckForInteractable()
        {
            // Don't check for interactables if build mode is active
            if (BuildModeController.Instance != null && BuildModeController.Instance.IsActive)
            {
                currentInteractable = null;
                
                if (hudController != null)
                {
                    hudController.HideInteractionTooltip();
                }
                return;
            }
            
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
            // Don't allow interaction if build mode is active
            if (BuildModeController.Instance != null && BuildModeController.Instance.IsActive)
            {
                Debug.Log("Cannot interact - build mode is active");
                return;
            }
            
            // Don't interact with computer if player is holding an item
            if (itemManipulation != null && itemManipulation.IsHoldingItem)
            {
                Debug.Log("Cannot interact with computer - player is holding an item");
                return;
            }
            
            Debug.Log($"Interact input received! Current interactable: {(currentInteractable != null ? currentInteractable.name : "null")}, isInteracting: {isInteracting}");
            
            // Only interact with computer if one is in range
            if (currentInteractable != null && !isInteracting)
            {
                Debug.Log("Branch: Starting interaction (isInteracting was false, interactable found)");
                currentInteractable.Interact();
                isInteracting = true;
            }
        }
        
        public void OnInteractionClosed()
        {
            isInteracting = false;
        }
    }
}
