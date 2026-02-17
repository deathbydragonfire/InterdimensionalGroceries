using UnityEngine;
using System;

namespace InterdimensionalGroceries.PlayerController
{
    public class CameraZoomController : MonoBehaviour
    {
        [Header("Zoom Settings")]
        [SerializeField] private float zoomDuration = 1f;
        [SerializeField] private AnimationCurve zoomCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        private Camera playerCamera;
        private FirstPersonController firstPersonController;
        private Vector3 originalWorldPosition;
        private Quaternion originalWorldRotation;
        
        private bool isZooming;
        private bool isZoomedIn;
        private float zoomProgress;
        private Transform targetTransform;
        private Action onZoomCompleteCallback;
        
        private void Awake()
        {
            playerCamera = GetComponentInChildren<Camera>();
            firstPersonController = GetComponent<FirstPersonController>();
            
            if (playerCamera == null)
            {
                Debug.LogError("CameraZoomController: No Camera found in children!");
            }
        }
        
        public void ZoomToTarget(Transform target, Action onComplete = null)
        {
            if (isZoomedIn || isZooming)
            {
                Debug.LogWarning("CameraZoomController: Already zooming or zoomed in!");
                return;
            }
                
            if (target == null)
            {
                Debug.LogError("CameraZoomController: Target transform is null!");
                return;
            }
            
            targetTransform = target;
            onZoomCompleteCallback = onComplete;
            
            originalWorldPosition = playerCamera.transform.position;
            originalWorldRotation = playerCamera.transform.rotation;
            
            isZooming = true;
            isZoomedIn = false;
            zoomProgress = 0f;
            
            Debug.Log($"Starting zoom to {target.name} at position {target.position}");
            
            if (firstPersonController != null)
            {
                firstPersonController.SetControlsEnabled(false);
            }
        }
        
        public void ZoomOut(Action onComplete = null)
        {
            if (!isZoomedIn || isZooming)
                return;
                
            onZoomCompleteCallback = onComplete;
            isZooming = true;
            isZoomedIn = false;
            zoomProgress = 0f;
        }
        
        private void Update()
        {
            if (!isZooming)
                return;
                
            zoomProgress += Time.deltaTime / zoomDuration;
            float curvedProgress = zoomCurve.Evaluate(Mathf.Clamp01(zoomProgress));
            
            if (targetTransform != null && !isZoomedIn)
            {
                // Zooming to target
                playerCamera.transform.position = Vector3.Lerp(originalWorldPosition, targetTransform.position, curvedProgress);
                playerCamera.transform.rotation = Quaternion.Lerp(originalWorldRotation, targetTransform.rotation, curvedProgress);
                
                if (zoomProgress >= 1f)
                {
                    isZooming = false;
                    isZoomedIn = true;
                    Debug.Log("Zoom complete!");
                    onZoomCompleteCallback?.Invoke();
                    onZoomCompleteCallback = null;
                }
            }
            else if (isZoomedIn)
            {
                // Zooming back out
                playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, originalWorldPosition, curvedProgress);
                playerCamera.transform.rotation = Quaternion.Lerp(playerCamera.transform.rotation, originalWorldRotation, curvedProgress);
                
                if (zoomProgress >= 1f)
                {
                    isZooming = false;
                    isZoomedIn = false;
                    Debug.Log("Zoom out complete!");
                    
                    if (firstPersonController != null)
                    {
                        firstPersonController.SetControlsEnabled(true);
                    }
                    
                    onZoomCompleteCallback?.Invoke();
                    onZoomCompleteCallback = null;
                }
            }
        }
    }
}
