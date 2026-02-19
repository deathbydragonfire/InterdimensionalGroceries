using UnityEngine;
using InterdimensionalGroceries.ScannerSystem;

namespace TutorialSystem
{
    public class TutorialScannerController : MonoBehaviour
    {
        [Header("References")]
        public ScannerUI scannerUI;
        public TutorialManager tutorialManager;

        [Header("Tutorial Settings")]
        public string tutorialItemName = "Package";

        private ScannerZone scannerZone;
        private bool tutorialActive = true;
        private GameObject scannerGameObject;

        private void Awake()
        {
            scannerZone = GetComponent<ScannerZone>();
            
            if (scannerUI != null)
            {
                scannerGameObject = scannerUI.gameObject;
                scannerUI.enabled = false;
                scannerGameObject.SetActive(false);
                Debug.Log("[TutorialScannerController] Cached and deactivated ScannerText in Awake");
            }
        }

        private void Start()
        {
            if (tutorialManager != null)
            {
                tutorialManager.OnTutorialComplete += HandleTutorialComplete;
            }
        }

        private void OnDestroy()
        {
            if (tutorialManager != null)
            {
                tutorialManager.OnTutorialComplete -= HandleTutorialComplete;
            }
        }

        private void HandleTutorialComplete()
        {
            tutorialActive = false;
        }

        public bool IsTutorialActive()
        {
            return tutorialActive;
        }

        public void ShowTutorialRequest()
        {
            if (scannerUI != null && tutorialActive && scannerGameObject != null)
            {
                if (!scannerGameObject.activeSelf)
                {
                    scannerGameObject.SetActive(true);
                    Debug.Log("[TutorialScannerController] Activated ScannerText for tutorial request");
                }
                
                scannerUI.enabled = true;
                scannerUI.ShowRequest(tutorialItemName);
                Debug.Log($"[TutorialScannerController] Showing tutorial request: {tutorialItemName}");
            }
        }
        
        public void HideTutorialScanner()
        {
            if (scannerGameObject != null && scannerGameObject.activeSelf)
            {
                if (scannerUI != null)
                {
                    scannerUI.enabled = false;
                }
                scannerGameObject.SetActive(false);
                Debug.Log("[TutorialScannerController] Hidden ScannerText");
            }
        }
    }
}
