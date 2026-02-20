using UnityEngine;
using System;

namespace InterdimensionalGroceries.PlayerController
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerAutoWalk : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float arrivalThreshold = 0.2f;
        [SerializeField] private float gravity = 20f;

        private CharacterController characterController;
        private Vector3 targetPosition;
        private Action onArrived;
        private bool isWalking = false;
        private float verticalVelocity = 0f;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (isWalking)
            {
                Vector3 currentPosition = transform.position;
                Vector3 direction = (targetPosition - currentPosition).normalized;
                direction.y = 0f;

                float distance = Vector3.Distance(new Vector3(currentPosition.x, 0f, currentPosition.z), 
                                                  new Vector3(targetPosition.x, 0f, targetPosition.z));

                if (distance > arrivalThreshold)
                {
                    characterController.Move(direction * walkSpeed * Time.deltaTime);
                    
                    if (characterController.isGrounded)
                    {
                        verticalVelocity = -2f;
                    }
                    else
                    {
                        verticalVelocity -= gravity * Time.deltaTime;
                    }
                    
                    characterController.Move(Vector3.up * verticalVelocity * Time.deltaTime);
                }
                else
                {
                    StopWalking();
                    onArrived?.Invoke();
                }
            }
        }

        public void StartWalking(Vector3 target, Action callback = null)
        {
            targetPosition = target;
            onArrived = callback;
            isWalking = true;
            verticalVelocity = 0f;
        }

        public void StopWalking()
        {
            isWalking = false;
        }
    }
}
