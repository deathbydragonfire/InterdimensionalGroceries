using UnityEngine;
using UnityEngine.InputSystem;
using InterdimensionalGroceries.BuildSystem;
using InterdimensionalGroceries.EconomySystem;

namespace InterdimensionalGroceries.PlayerController
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 10f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float gravity = 20f;

        [Header("Look Settings")]
        [SerializeField] private float lookSensitivity = 2f;
        [SerializeField] private Transform cameraTransform;

        private CharacterController characterController;
        private InputSystem_Actions inputActions;
        private ItemManipulationController itemManipulation;
        private Vector2 moveInput;
        private Vector2 lookInput;
        private bool isSprinting;
        private float verticalRotation = 0f;
        private float verticalVelocity = 0f;
        private bool controlsEnabled = true;

        private float baseMoveSpeed;
        private float baseSprintSpeed;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputActions = new InputSystem_Actions();
            itemManipulation = GetComponent<ItemManipulationController>();
            
            if (cameraTransform == null)
            {
                cameraTransform = Camera.main?.transform;
            }

            baseMoveSpeed = moveSpeed;
            baseSprintSpeed = sprintSpeed;
        }

        private void Start()
        {
            ApplyMovementSpeedUpgrade();

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
            if (upgrade.UpgradeType == UpgradeType.MovementSpeed)
            {
                ApplyMovementSpeedUpgrade();
            }
        }

        private void ApplyMovementSpeedUpgrade()
        {
            if (AbilityUpgradeManager.Instance != null)
            {
                float multiplier = AbilityUpgradeManager.Instance.GetUpgradeMultiplier(UpgradeType.MovementSpeed);
                moveSpeed = baseMoveSpeed * multiplier;
                sprintSpeed = baseSprintSpeed * multiplier;
                Debug.Log($"Movement speed updated: Move={moveSpeed:F2}, Sprint={sprintSpeed:F2} (multiplier={multiplier:F2})");
            }
        }

        private void OnEnable()
        {
            inputActions.Player.Enable();
            inputActions.Player.Move.performed += OnMove;
            inputActions.Player.Move.canceled += OnMove;
            inputActions.Player.Look.performed += OnLook;
            inputActions.Player.Look.canceled += OnLook;
            inputActions.Player.Sprint.performed += OnSprint;
            inputActions.Player.Sprint.canceled += OnSprint;
            inputActions.Player.Jump.performed += OnJump;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            inputActions.Player.Move.performed -= OnMove;
            inputActions.Player.Move.canceled -= OnMove;
            inputActions.Player.Look.performed -= OnLook;
            inputActions.Player.Look.canceled -= OnLook;
            inputActions.Player.Sprint.performed -= OnSprint;
            inputActions.Player.Sprint.canceled -= OnSprint;
            inputActions.Player.Jump.performed -= OnJump;
            inputActions.Player.Disable();
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
        }

        private void OnSprint(InputAction.CallbackContext context)
        {
            isSprinting = context.ReadValueAsButton();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (characterController.isGrounded)
            {
                verticalVelocity = jumpForce;
            }
        }

        private void Update()
        {
            if (!controlsEnabled || (BuildModeController.Instance != null && BuildModeController.Instance.IsBrowsing))
            {
                return;
            }
            
            HandleMovement();
            HandleLook();
        }

        public void SetControlsEnabled(bool enabled)
        {
            controlsEnabled = enabled;
            
            if (!enabled)
            {
                inputActions.Player.Disable();
            }
            else
            {
                inputActions.Player.Enable();
            }
        }

        private void HandleMovement()
        {
            if (!controlsEnabled) return;
            
            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            characterController.Move(move * currentSpeed * Time.deltaTime);
            
            if (characterController.isGrounded)
            {
                if (verticalVelocity < 0f)
                {
                    verticalVelocity = -2f;
                }
            }
            
            verticalVelocity -= gravity * Time.deltaTime;
            characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
        }

        private void HandleLook()
        {
            if (!controlsEnabled) return;
            
            if (itemManipulation != null && itemManipulation.IsRotatingObject)
            {
                return;
            }

            float mouseX = lookInput.x * lookSensitivity;
            float mouseY = lookInput.y * lookSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

            if (cameraTransform != null)
            {
                cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            
            if (body == null || body.isKinematic)
            {
                return;
            }

            if (hit.moveDirection.y < -0.3f)
            {
                return;
            }

            HingeJoint hinge = body.GetComponent<HingeJoint>();
            if (hinge != null)
            {
                float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
                float pushIntensity = moveInput.magnitude * currentSpeed;
                
                if (pushIntensity < 0.1f)
                {
                    return;
                }
                
                Vector3 hingeWorldPos = body.transform.TransformPoint(hinge.anchor);
                Vector3 fromHinge = hit.point - hingeWorldPos;
                fromHinge.y = 0;
                
                Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z).normalized;
                
                Vector3 torqueAxis = Vector3.Cross(fromHinge, pushDirection);
                float torqueMagnitude = torqueAxis.magnitude * 100f * pushIntensity;
                
                Debug.Log($"[Door Push] HingePos: {hingeWorldPos}, HitPoint: {hit.point}, FromHinge: {fromHinge.magnitude:F2}, PushDir: {pushDirection}, TorqueAxis: {torqueAxis}, Torque: {torqueMagnitude:F2}");
                
                body.AddTorque(Vector3.up * Mathf.Sign(torqueAxis.y) * torqueMagnitude, ForceMode.Force);
                
                Vector3 force = pushDirection * 100f * pushIntensity;
                body.AddForceAtPosition(force, hit.point, ForceMode.Force);
            }
            else
            {
                Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
                float pushPower = 10f;
                body.linearVelocity = pushDir * pushPower * characterController.velocity.magnitude;
            }
        }
    }
}
