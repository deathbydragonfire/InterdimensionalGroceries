using UnityEngine;
using System.Collections;

namespace InterdimensionalGroceries.PlayerController
{
    public class OrganicCameraLook : MonoBehaviour
    {
        [Header("Look Settings")]
        [SerializeField] private float minTimeBetweenLooks = 1.5f;
        [SerializeField] private float maxTimeBetweenLooks = 3.5f;
        [SerializeField] private float lookSpeed = 2f;
        [SerializeField] private float returnSpeed = 1.5f;

        [Header("Look Ranges")]
        [SerializeField] private float maxHorizontalAngle = 25f;
        [SerializeField] private float maxVerticalAngle = 15f;

        private Transform cameraTransform;
        private Quaternion targetRotation;
        private Quaternion restRotation;
        private bool isActive = false;
        private Coroutine lookCoroutine;

        private void Awake()
        {
            cameraTransform = Camera.main.transform;
            if (cameraTransform != null)
            {
                restRotation = cameraTransform.localRotation;
            }
        }

        public void StartOrganicLooking()
        {
            if (lookCoroutine != null)
            {
                StopCoroutine(lookCoroutine);
            }
            isActive = true;
            restRotation = cameraTransform.localRotation;
            lookCoroutine = StartCoroutine(OrganicLookRoutine());
        }

        public void StopOrganicLooking()
        {
            isActive = false;
            if (lookCoroutine != null)
            {
                StopCoroutine(lookCoroutine);
                lookCoroutine = null;
            }
        }

        private IEnumerator OrganicLookRoutine()
        {
            while (isActive)
            {
                float waitTime = Random.Range(minTimeBetweenLooks, maxTimeBetweenLooks);
                yield return new WaitForSeconds(waitTime);

                if (!isActive) break;

                float horizontalAngle = Random.Range(-maxHorizontalAngle, maxHorizontalAngle);
                float verticalAngle = Random.Range(-maxVerticalAngle / 2f, maxVerticalAngle);

                targetRotation = restRotation * Quaternion.Euler(verticalAngle, horizontalAngle, 0f);

                float lookDuration = Random.Range(0.5f, 1.2f);
                float elapsed = 0f;

                while (elapsed < lookDuration && isActive)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / lookDuration;
                    float smoothT = Mathf.SmoothStep(0f, 1f, t);
                    
                    cameraTransform.localRotation = Quaternion.Slerp(
                        cameraTransform.localRotation,
                        targetRotation,
                        smoothT * lookSpeed * Time.deltaTime
                    );
                    
                    yield return null;
                }

                if (!isActive) break;

                float holdTime = Random.Range(0.3f, 0.8f);
                yield return new WaitForSeconds(holdTime);

                if (!isActive) break;

                float returnDuration = Random.Range(0.8f, 1.5f);
                elapsed = 0f;

                while (elapsed < returnDuration && isActive)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / returnDuration;
                    float smoothT = Mathf.SmoothStep(0f, 1f, t);
                    
                    cameraTransform.localRotation = Quaternion.Slerp(
                        cameraTransform.localRotation,
                        restRotation,
                        smoothT * returnSpeed * Time.deltaTime
                    );
                    
                    yield return null;
                }
            }
        }

        private void OnDisable()
        {
            StopOrganicLooking();
        }
    }
}
