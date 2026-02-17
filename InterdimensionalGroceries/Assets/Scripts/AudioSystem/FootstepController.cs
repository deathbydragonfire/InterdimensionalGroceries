using UnityEngine;

namespace InterdimensionalGroceries.AudioSystem
{
    [RequireComponent(typeof(CharacterController))]
    public class FootstepController : MonoBehaviour
    {
        [Header("Footstep Settings")]
        [SerializeField] private float walkStepInterval = 0.5f;
        [SerializeField] private float sprintStepInterval = 0.3f;
        [SerializeField] private float volumeMultiplier = 0.8f;

        [Header("Movement Detection")]
        [SerializeField] private float minimumVelocity = 0.1f;
        [SerializeField] private bool requireGrounded = true;

        [Header("Surface Detection")]
        [SerializeField] private bool useSurfaceDetection = true;
        [SerializeField] private float raycastDistance = 1.5f;
        [SerializeField] private LayerMask groundLayerMask = -1;

        [Header("Sprint Detection")]
        [SerializeField] private float sprintVelocityThreshold = 6f;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogging = false;

        private CharacterController characterController;
        private float stepTimer;
        private bool wasMovingLastFrame;
        private Vector3 lastPosition;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            lastPosition = transform.position;
        }

        private void Update()
        {
            if (AudioManager.Instance == null)
                return;

            bool isGrounded = characterController.isGrounded;
            Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;
            float horizontalSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;
            
            bool isMoving = horizontalSpeed > minimumVelocity;
            bool shouldPlayFootstep = isMoving && (!requireGrounded || isGrounded);

            if (shouldPlayFootstep)
            {
                bool isSprinting = horizontalSpeed > sprintVelocityThreshold;
                float currentStepInterval = isSprinting ? sprintStepInterval : walkStepInterval;

                stepTimer += Time.deltaTime;

                if (stepTimer >= currentStepInterval)
                {
                    PlayFootstep(isSprinting);
                    stepTimer = 0f;
                }
            }
            else
            {
                stepTimer = 0f;
            }

            wasMovingLastFrame = isMoving;
            lastPosition = transform.position;
        }

        private void PlayFootstep(bool isSprinting)
        {
            if (AudioManager.Instance == null)
                return;

            Vector3 footstepPosition = transform.position;

            if (useSurfaceDetection)
            {
                SurfaceType surfaceType = DetectSurface();
                
                if (enableDebugLogging)
                {
                    string speedType = isSprinting ? "Sprint" : "Walk";
                    Debug.Log($"[FootstepController] Playing {speedType} footstep on {surfaceType} surface");
                }

                AudioManager.Instance.PlayImpactSound(surfaceType, footstepPosition, volumeMultiplier);
            }
            else
            {
                if (enableDebugLogging)
                {
                    string speedType = isSprinting ? "Sprint" : "Walk";
                    Debug.Log($"[FootstepController] Playing {speedType} footstep (generic)");
                }

                AudioManager.Instance.PlaySound(AudioEventType.Footstep, footstepPosition);
            }
        }

        private SurfaceType DetectSurface()
        {
            RaycastHit hit;
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastDistance, groundLayerMask))
            {
                if (hit.collider.TryGetComponent<SurfaceIdentifier>(out var identifier))
                {
                    return identifier.SurfaceType;
                }
            }

            return SurfaceType.Default;
        }

        private void OnDrawGizmosSelected()
        {
            if (!useSurfaceDetection)
                return;

            Gizmos.color = Color.yellow;
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * raycastDistance);
        }
    }
}
