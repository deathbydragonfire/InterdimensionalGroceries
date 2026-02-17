using UnityEngine;

namespace InterdimensionalGroceries.PlayerController
{
    public class CameraBob : MonoBehaviour
    {
        [Header("Bob Settings")]
        [SerializeField] private float bobFrequency = 10f;
        [SerializeField] private float bobHorizontalAmplitude = 0.05f;
        [SerializeField] private float bobVerticalAmplitude = 0.08f;
        [SerializeField] private float smoothTransition = 5f;

        [Header("References")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private Transform cameraTransform;

        private Vector3 originalLocalPosition;
        private float bobTimer = 0f;
        private float currentBobIntensity = 0f;
        private Vector3 lastPosition;

        private void Awake()
        {
            if (characterController == null)
            {
                characterController = GetComponentInParent<CharacterController>();
            }

            if (cameraTransform == null)
            {
                cameraTransform = Camera.main?.transform;
            }

            if (cameraTransform != null)
            {
                originalLocalPosition = cameraTransform.localPosition;
            }

            if (characterController != null)
            {
                lastPosition = characterController.transform.position;
            }
        }

        private void LateUpdate()
        {
            if (cameraTransform == null || characterController == null)
            {
                Debug.LogWarning("CameraBob: Missing references!");
                return;
            }

            Vector3 currentPosition = characterController.transform.position;
            Vector3 positionDelta = currentPosition - lastPosition;
            positionDelta.y = 0f;
            float horizontalSpeed = positionDelta.magnitude / Time.deltaTime;
            lastPosition = currentPosition;

            float targetIntensity = 0f;
            bool isGrounded = characterController.isGrounded;

            if (isGrounded && horizontalSpeed > 0.1f)
            {
                targetIntensity = 1f;
            }

            currentBobIntensity = Mathf.Lerp(currentBobIntensity, targetIntensity, smoothTransition * Time.deltaTime);

            if (currentBobIntensity > 0.01f)
            {
                if (targetIntensity > 0f)
                {
                    bobTimer += Time.deltaTime * bobFrequency;
                }

                float horizontalOffset = Mathf.Sin(bobTimer) * bobHorizontalAmplitude * currentBobIntensity;
                float verticalOffset = Mathf.Sin(bobTimer * 2f) * bobVerticalAmplitude * currentBobIntensity;

                Vector3 targetPosition = originalLocalPosition + new Vector3(horizontalOffset, verticalOffset, 0f);
                cameraTransform.localPosition = targetPosition;
            }
            else
            {
                cameraTransform.localPosition = Vector3.Lerp(
                    cameraTransform.localPosition,
                    originalLocalPosition,
                    smoothTransition * Time.deltaTime
                );
                bobTimer = 0f;
            }
        }

        private void OnDisable()
        {
            if (cameraTransform != null)
            {
                cameraTransform.localPosition = originalLocalPosition;
            }
        }
    }
}
