using UnityEngine;
using UnityEngine.InputSystem;

namespace InterdimensionalGroceries.PlayerController
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 10f;

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

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            inputActions = new InputSystem_Actions();
            itemManipulation = GetComponent<ItemManipulationController>();
            
            if (cameraTransform == null)
            {
                cameraTransform = Camera.main?.transform;
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

        private void Update()
        {
            HandleMovement();
            HandleLook();
        }

        private void HandleMovement()
        {
            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            characterController.Move(move * currentSpeed * Time.deltaTime);
            
            if (!characterController.isGrounded)
            {
                characterController.Move(Vector3.down * 9.81f * Time.deltaTime);
            }
        }

        private void HandleLook()
        {
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
    }
}
