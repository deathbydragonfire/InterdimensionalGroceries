using UnityEngine;
using System.Collections;

namespace InterdimensionalGroceries.PlayerController
{
    public class CameraLookSequence : MonoBehaviour
    {
        [Header("Look Sequence Angles")]
        [SerializeField] private float lookLeftAngleMin = -70f;
        [SerializeField] private float lookLeftAngleMax = -50f;
        [SerializeField] private float lookRightAngleMin = 50f;
        [SerializeField] private float lookRightAngleMax = 70f;
        [SerializeField] private float lookDownAngle = 80f;
        [SerializeField] private float lookUpAngleMin = -75f;
        [SerializeField] private float lookUpAngleMax = -60f;

        [Header("Timing")]
        [SerializeField] private float lookDurationMin = 0.4f;
        [SerializeField] private float lookDurationMax = 0.7f;
        [SerializeField] private float pauseBetweenLooksMin = 0.15f;
        [SerializeField] private float pauseBetweenLooksMax = 0.35f;
        [SerializeField] private float lookDownDuration = 2.5f;

        [Header("Turn Around Settings")]
        [SerializeField] private Transform turnAroundTarget;
        [SerializeField] private float turnAroundDuration = 1.5f;
        [SerializeField] private float pauseBeforeTurnAround = 0.3f;

        [Header("Camera Shake")]
        [SerializeField] private float shakeIntensity = 0.15f;
        [SerializeField] private float shakeFrequency = 25f;

        private Transform cameraTransform;
        private Transform playerTransform;
        private Coroutine sequenceCoroutine;
        private Vector3 originalLocalPosition;

        private void Awake()
        {
            cameraTransform = Camera.main.transform;
            if (cameraTransform != null)
            {
                originalLocalPosition = cameraTransform.localPosition;
                playerTransform = cameraTransform.parent;
            }
        }

        public void StartPanicSequence(System.Action onComplete = null, System.Action onLookingDown = null)
        {
            if (sequenceCoroutine != null)
            {
                StopCoroutine(sequenceCoroutine);
            }
            sequenceCoroutine = StartCoroutine(PanicSequenceCoroutine(onComplete, onLookingDown));
        }

        private IEnumerator PanicSequenceCoroutine(System.Action onComplete, System.Action onLookingDown)
        {
            Quaternion originalRotation = cameraTransform.localRotation;

            if (turnAroundTarget != null)
            {
                yield return new WaitForSeconds(pauseBeforeTurnAround);
                yield return TurnAroundToTarget();
            }

            float leftAngle = Random.Range(lookLeftAngleMin, lookLeftAngleMax);
            float leftDuration = Random.Range(lookDurationMin, lookDurationMax);
            yield return LookAtAngleWithShake(leftAngle, 0f, leftDuration);
            yield return new WaitForSeconds(Random.Range(pauseBetweenLooksMin, pauseBetweenLooksMax));

            float rightAngle = Random.Range(lookRightAngleMin, lookRightAngleMax);
            float rightDuration = Random.Range(lookDurationMin, lookDurationMax);
            yield return LookAtAngleWithShake(rightAngle, 0f, rightDuration);
            yield return new WaitForSeconds(Random.Range(pauseBetweenLooksMin, pauseBetweenLooksMax));

            yield return LookAtAngleWithShake(0f, lookDownAngle, 0.6f);
            
            yield return new WaitForSeconds(0.5f);
            
            onLookingDown?.Invoke();
            
            yield return new WaitForSeconds(1.4f);

            float upAngle = Random.Range(lookUpAngleMin, lookUpAngleMax);
            yield return LookAtAngleWithShake(0f, upAngle, 0.5f);

            onComplete?.Invoke();
        }

        private IEnumerator TurnAroundToTarget()
        {
            if (playerTransform == null || turnAroundTarget == null)
            {
                yield break;
            }

            Quaternion startRotation = playerTransform.rotation;
            
            Vector3 directionToTarget = turnAroundTarget.position - playerTransform.position;
            directionToTarget.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            float elapsed = 0f;
            while (elapsed < turnAroundDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / turnAroundDuration;
                float smoothT = Mathf.SmoothStep(0f, 1f, t);
                
                playerTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, smoothT);
                
                float shakeX = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) - 0.5f) * shakeIntensity;
                float shakeY = (Mathf.PerlinNoise(0f, Time.time * shakeFrequency) - 0.5f) * shakeIntensity;
                
                cameraTransform.localPosition = originalLocalPosition + new Vector3(shakeX, shakeY, 0f);
                
                yield return null;
            }

            playerTransform.rotation = targetRotation;
            cameraTransform.localRotation = Quaternion.identity;
        }

        private IEnumerator LookAtAngleWithShake(float yaw, float pitch, float duration)
        {
            Quaternion startRotation = cameraTransform.localRotation;
            Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float smoothT = Mathf.SmoothStep(0f, 1f, t);
                
                cameraTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, smoothT);
                
                float shakeX = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) - 0.5f) * shakeIntensity;
                float shakeY = (Mathf.PerlinNoise(0f, Time.time * shakeFrequency) - 0.5f) * shakeIntensity;
                
                cameraTransform.localPosition = originalLocalPosition + new Vector3(shakeX, shakeY, 0f);
                
                yield return null;
            }

            cameraTransform.localRotation = targetRotation;
        }

        public void StopSequence()
        {
            if (sequenceCoroutine != null)
            {
                StopCoroutine(sequenceCoroutine);
                sequenceCoroutine = null;
            }
            
            if (cameraTransform != null)
            {
                cameraTransform.localPosition = originalLocalPosition;
            }
        }
    }
}
