using UnityEngine;
using UnityEngine.InputSystem;
using InterdimensionalGroceries.Core;
using InterdimensionalGroceries.BuildSystem;
using InterdimensionalGroceries.ItemSystem;
using InterdimensionalGroceries.ScannerSystem;
using InterdimensionalGroceries.AudioSystem;
using InterdimensionalGroceries.EconomySystem;
using InterdimensionalGroceries.InteractionSystem;
using System;

namespace InterdimensionalGroceries.PlayerController
{
    public class ItemManipulationController : MonoBehaviour
    {
        public event Action OnItemPickedUp;

        [Header("Raycast Settings")]
        [SerializeField] private float maxPickupDistance = 3f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private Transform cameraTransform;

        [Header("Held Object Settings")]
        [SerializeField] private float defaultHoldDistance = 2f;
        [SerializeField] private float minHoldDistance = 1f;
        [SerializeField] private float maxHoldDistance = 4f;
        [SerializeField] private float scrollSensitivity = 0.5f;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationSensitivity = 5f;

        [Header("Throw Settings")]
        [SerializeField] private float throwChargeDelay = 0.3f;
        [SerializeField] private float maxChargeTime = 1f;
        [SerializeField] private float minThrowForce = 5f;
        [SerializeField] private float maxThrowForce = 15f;

        private float baseMinThrowForce;
        private float baseMaxThrowForce;

        private InputSystem_Actions inputActions;
        private GameObject heldObject;
        private Rigidbody heldRigidbody;
        private IPickable heldPickable;
        private float currentHoldDistance;
        private bool isCharging;
        private float chargeStartTime;
        private bool isRotating;
        private bool justPickedUp;
        private HUDController hudController;
        private PickupUIController pickupUIController;

        public float ChargePercent { get; private set; }
        public bool IsRotatingObject => isRotating && heldObject != null;
        public bool IsHoldingItem => heldObject != null;

        private void Awake()
        {
            inputActions = new InputSystem_Actions();
            currentHoldDistance = defaultHoldDistance;

            if (cameraTransform == null)
            {
                cameraTransform = Camera.main?.transform;
            }

            hudController = GetComponentInChildren<HUDController>();
            pickupUIController = GetComponentInChildren<PickupUIController>();

            baseMinThrowForce = minThrowForce;
            baseMaxThrowForce = maxThrowForce;
        }

        private void Start()
        {
            if (hudController != null)
            {
                hudController.UpdateChargeBar(0f);
            }

            ApplyThrowingStrengthUpgrade();

            if (AbilityUpgradeManager.Instance != null)
            {
                AbilityUpgradeManager.Instance.OnUpgradePurchased += OnUpgradePurchased;
            }
        }

        private void OnDestroy()
        {
            if (AbilityUpgradeManager.Instance != null)
            {
                AbilityUpgradeManager.Instance.OnUpgradePurchased -= OnUpgradePurchased;
            }
        }

        private void OnUpgradePurchased(AbilityUpgradeData upgrade, int newLevel)
        {
            if (upgrade.UpgradeType == UpgradeType.ThrowingStrength)
            {
                ApplyThrowingStrengthUpgrade();
            }
        }

        private void ApplyThrowingStrengthUpgrade()
        {
            if (AbilityUpgradeManager.Instance != null)
            {
                float multiplier = AbilityUpgradeManager.Instance.GetUpgradeMultiplier(UpgradeType.ThrowingStrength);
                minThrowForce = baseMinThrowForce * multiplier;
                maxThrowForce = baseMaxThrowForce * multiplier;
                Debug.Log($"Throwing strength updated: Min={minThrowForce:F2}, Max={maxThrowForce:F2} (multiplier={multiplier:F2})");
            }
        }

        private void OnEnable()
        {
            inputActions.Player.Enable();
            inputActions.Player.PickUpPlace.performed += OnPickUpPlacePerformed;
            inputActions.Player.PickUpPlace.canceled += OnPickUpPlaceCanceled;
            inputActions.Player.Rotate.performed += OnRotatePerformed;
            inputActions.Player.Rotate.canceled += OnRotateCanceled;
            inputActions.Player.AdjustDistance.performed += OnAdjustDistance;
        }

        private void OnDisable()
        {
            inputActions.Player.PickUpPlace.performed -= OnPickUpPlacePerformed;
            inputActions.Player.PickUpPlace.canceled -= OnPickUpPlaceCanceled;
            inputActions.Player.Rotate.performed -= OnRotatePerformed;
            inputActions.Player.Rotate.canceled -= OnRotateCanceled;
            inputActions.Player.AdjustDistance.performed -= OnAdjustDistance;
            inputActions.Player.Disable();
        }

        private void OnPickUpPlacePerformed(InputAction.CallbackContext context)
        {
            if (heldObject == null)
            {
                TryPickupObject();
            }
            else if (!justPickedUp)
            {
                isCharging = true;
                chargeStartTime = Time.time;
            }
        }

        private void OnPickUpPlaceCanceled(InputAction.CallbackContext context)
        {
            if (justPickedUp)
            {
                justPickedUp = false;
                return;
            }

            if (heldObject != null)
            {
                float holdTime = Time.time - chargeStartTime;
                
                if (holdTime < throwChargeDelay)
                {
                    PlaceObject();
                }
                else if (isCharging)
                {
                    ThrowObject();
                }
                else
                {
                    PlaceObject();
                }
                
                isCharging = false;
                ChargePercent = 0f;
                hudController?.UpdateChargeBar(0f);
            }
        }

        private void OnRotatePerformed(InputAction.CallbackContext context)
        {
            isRotating = true;
        }

        private void OnRotateCanceled(InputAction.CallbackContext context)
        {
            isRotating = false;
        }

        private void OnAdjustDistance(InputAction.CallbackContext context)
        {
            if (heldObject != null)
            {
                float scrollValue = context.ReadValue<Vector2>().y;
                currentHoldDistance += scrollValue * scrollSensitivity * 0.1f;
                currentHoldDistance = Mathf.Clamp(currentHoldDistance, minHoldDistance, maxHoldDistance);
            }
        }

        private void Update()
        {
            if (BuildModeController.Instance != null && BuildModeController.Instance.IsBrowsing)
            {
                if (pickupUIController != null)
                {
                    pickupUIController.HidePickupHint();
                }
                return;
            }
            
            if (heldObject != null)
            {
                UpdateHeldObjectPosition();

                bool inScannerRange = IsInScannerRange();
                
                if (pickupUIController != null)
                {
                    if (inScannerRange)
                    {
                        pickupUIController.HideControlHints();
                        pickupUIController.ShowScannerHint();
                    }
                    else
                    {
                        Vector3 hintPosition = heldObject.transform.position;
                        pickupUIController.ShowControlHints(hintPosition, currentHoldDistance, minHoldDistance, maxHoldDistance);
                        pickupUIController.HideScannerHint();
                    }
                    pickupUIController.HidePickupHint();
                }

                if (isRotating)
                {
                    RotateHeldObject();
                }

                if (inputActions.Player.PickUpPlace.IsPressed() && !justPickedUp)
                {
                    float holdTime = Time.time - chargeStartTime;
                    
                    if (holdTime >= throwChargeDelay)
                    {
                        if (!isCharging)
                        {
                            isCharging = true;
                        }
                        UpdateChargeBar();
                    }
                }
                else if (!inputActions.Player.PickUpPlace.IsPressed())
                {
                    if (hudController != null && ChargePercent > 0)
                    {
                        ChargePercent = 0f;
                        hudController.UpdateChargeBar(0f);
                    }
                }
            }
            else
            {
                CheckForPickupTarget();
            }
        }

        private bool IsInScannerRange()
        {
            if (heldObject == null) return false;
            
            ScannerZone[] scanners = FindObjectsByType<ScannerZone>(FindObjectsSortMode.None);
            foreach (var scanner in scanners)
            {
                if (scanner.IsObjectInRange(heldObject))
                {
                    return true;
                }
            }
            return false;
        }

        private void CheckForPickupTarget()
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxPickupDistance, interactableLayer))
            {
                IClickable clickable = hit.collider.GetComponent<IClickable>();
                if (clickable != null)
                {
                    if (pickupUIController != null)
                    {
                        pickupUIController.ShowButtonHint();
                        pickupUIController.HidePickupHint();
                    }
                    return;
                }

                IPickable pickable = hit.collider.GetComponent<IPickable>();
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                PickableItem pickableItem = hit.collider.GetComponent<PickableItem>();

                if (pickable != null && rb != null && pickableItem != null && !pickableItem.IsBeingScanned)
                {
                    if (pickupUIController != null)
                    {
                        pickupUIController.ShowPickupHint();
                        pickupUIController.HideButtonHint();
                    }
                    return;
                }
            }
            
            if (pickupUIController != null)
            {
                pickupUIController.HidePickupHint();
                pickupUIController.HideButtonHint();
            }
        }

        private void TryPickupObject()
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxPickupDistance, interactableLayer))
            {
                ComputerInteraction computerInteraction = hit.collider.GetComponent<ComputerInteraction>();
                if (computerInteraction != null && computerInteraction.IsPlayerInRange(cameraTransform.position))
                {
                    return;
                }

                IClickable clickable = hit.collider.GetComponent<IClickable>();
                if (clickable != null)
                {
                    clickable.OnClick();
                    return;
                }

                GameObject hitObject = hit.collider.gameObject;
                IPickable pickable = hitObject.GetComponent<IPickable>();
                Rigidbody rb = hitObject.GetComponent<Rigidbody>();
                PickableItem pickableItem = hitObject.GetComponent<PickableItem>();

                if (pickable != null && rb != null && pickableItem != null && !pickableItem.IsBeingScanned)
                {
                    heldObject = hitObject;
                    heldRigidbody = rb;
                    heldPickable = pickable;
                    currentHoldDistance = defaultHoldDistance;
                    justPickedUp = true;

                    heldPickable.OnPickedUp();
                    heldObject.transform.parent = cameraTransform;
                    
                    OnItemPickedUp?.Invoke();
                    
                    if (pickupUIController != null)
                    {
                        var itemData = heldPickable.GetItemData();
                        pickupUIController.ShowInfoPanel(itemData);
                        pickupUIController.ShowControlHints(heldObject.transform.position, currentHoldDistance, minHoldDistance, maxHoldDistance);
                    }

                    if (AudioManager.Instance != null)
                    {
                        AudioManager.Instance.PlaySound(AudioEventType.Pickup, heldObject.transform.position);
                    }
                }
            }
        }

        private void UpdateHeldObjectPosition()
        {
            if (heldObject != null)
            {
                Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * currentHoldDistance;
                heldObject.transform.position = targetPosition;
            }
        }

        private void RotateHeldObject()
        {
            if (heldObject != null)
            {
                Vector2 lookDelta = inputActions.Player.Look.ReadValue<Vector2>();
                heldObject.transform.Rotate(cameraTransform.up, lookDelta.x * rotationSensitivity, Space.World);
                heldObject.transform.Rotate(cameraTransform.right, -lookDelta.y * rotationSensitivity, Space.World);
            }
        }

        private void UpdateChargeBar()
        {
            float chargeTime = Time.time - chargeStartTime - throwChargeDelay;
            ChargePercent = Mathf.Clamp01(chargeTime / maxChargeTime);
            hudController?.UpdateChargeBar(ChargePercent);
        }

        private void PlaceObject()
        {
            if (heldObject != null && heldPickable != null)
            {
                Vector3 placePosition = heldObject.transform.position;
                
                heldPickable.OnDropped();
                heldObject = null;
                heldRigidbody = null;
                heldPickable = null;
                
                if (pickupUIController != null)
                {
                    pickupUIController.HideControlHints();
                    pickupUIController.HideInfoPanel();
                    pickupUIController.HideScannerHint();
                }

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound(AudioEventType.Place, placePosition);
                }
            }
        }

        private void ThrowObject()
        {
            if (heldObject != null && heldPickable != null)
            {
                Vector3 throwPosition = heldObject.transform.position;
                
                float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, ChargePercent);
                heldPickable.OnThrown(throwForce);
                heldObject = null;
                heldRigidbody = null;
                heldPickable = null;
                
                if (pickupUIController != null)
                {
                    pickupUIController.HideControlHints();
                    pickupUIController.HideInfoPanel();
                    pickupUIController.HideScannerHint();
                }

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound(AudioEventType.Throw, throwPosition);
                }
            }
        }
    }
}
