using UnityEngine;

namespace InterdimensionalGroceries.BuildSystem
{
    public class BuildModeCameraController : MonoBehaviour
    {
        [Header("Camera References")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Camera buildModeCamera;
        
        private bool isInBuildMode = false;
        
        public Camera BuildModeCamera => buildModeCamera;
        public Camera PlayerCamera => playerCamera;
        
        private void Awake()
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }
            
            if (buildModeCamera != null)
            {
                buildModeCamera.gameObject.SetActive(false);
            }
        }
        
        public void EnterBuildModeView()
        {
            if (isInBuildMode) return;
            
            isInBuildMode = true;
            
            if (buildModeCamera != null && playerCamera != null)
            {
                playerCamera.gameObject.SetActive(false);
                buildModeCamera.gameObject.SetActive(true);
            }
            
            Debug.Log("Entered bird's eye build mode view");
        }
        
        public void ExitBuildModeView()
        {
            if (!isInBuildMode) return;
            
            isInBuildMode = false;
            
            if (buildModeCamera != null && playerCamera != null)
            {
                buildModeCamera.gameObject.SetActive(false);
                playerCamera.gameObject.SetActive(true);
            }
            
            Debug.Log("Exited bird's eye build mode view");
        }
        
        public bool IsInBuildMode => isInBuildMode;
    }
}
