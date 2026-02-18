using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace TutorialSystem
{
    public class LocomotionTracker : MonoBehaviour
    {
        public event Action OnAllLocomotionUsed;

        private InputSystem_Actions inputActions;
        private bool hasMovedForward;
        private bool hasMovedBack;
        private bool hasMovedLeft;
        private bool hasMovedRight;
        private bool hasSprinted;
        private bool hasJumped;
        private bool allLocomotionComplete;

        public bool HasMovedForward => hasMovedForward;
        public bool HasMovedBack => hasMovedBack;
        public bool HasMovedLeft => hasMovedLeft;
        public bool HasMovedRight => hasMovedRight;
        public bool HasSprinted => hasSprinted;
        public bool HasJumped => hasJumped;

        private void Awake()
        {
            inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            inputActions.Player.Enable();
            inputActions.Player.Move.performed += OnMove;
            inputActions.Player.Sprint.performed += OnSprint;
            inputActions.Player.Jump.performed += OnJump;
        }

        private void OnDisable()
        {
            inputActions.Player.Move.performed -= OnMove;
            inputActions.Player.Sprint.performed -= OnSprint;
            inputActions.Player.Jump.performed -= OnJump;
            inputActions.Player.Disable();
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            Vector2 movement = context.ReadValue<Vector2>();

            if (movement.y > 0.5f)
            {
                hasMovedForward = true;
            }
            if (movement.y < -0.5f)
            {
                hasMovedBack = true;
            }
            if (movement.x < -0.5f)
            {
                hasMovedLeft = true;
            }
            if (movement.x > 0.5f)
            {
                hasMovedRight = true;
            }

            CheckAllLocomotionUsed();
        }

        private void OnSprint(InputAction.CallbackContext context)
        {
            hasSprinted = true;
            CheckAllLocomotionUsed();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            hasJumped = true;
            CheckAllLocomotionUsed();
        }

        private void CheckAllLocomotionUsed()
        {
            if (allLocomotionComplete)
            {
                return;
            }

            if (hasMovedForward && hasMovedBack && hasMovedLeft && hasMovedRight && hasSprinted && hasJumped)
            {
                allLocomotionComplete = true;
                Debug.Log("[LocomotionTracker] All locomotion controls used!");
                OnAllLocomotionUsed?.Invoke();
            }
        }

        public void ResetTracking()
        {
            hasMovedForward = false;
            hasMovedBack = false;
            hasMovedLeft = false;
            hasMovedRight = false;
            hasSprinted = false;
            hasJumped = false;
            allLocomotionComplete = false;
        }
    }
}
