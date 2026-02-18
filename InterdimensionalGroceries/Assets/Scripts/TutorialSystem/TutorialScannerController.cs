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

        private void Awake()
        {
            scannerZone = GetComponent<ScannerZone>();
        }

        private void Start()
        {
            if (scannerUI != null)
            {
                scannerUI.ShowRequest(tutorialItemName);
            }

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
            if (scannerUI != null && tutorialActive)
            {
                scannerUI.ShowRequest(tutorialItemName);
            }
        }
    }
}
