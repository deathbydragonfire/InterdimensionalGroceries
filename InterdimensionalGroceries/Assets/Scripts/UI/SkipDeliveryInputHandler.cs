using UnityEngine;
using UnityEngine.InputSystem;
using InterdimensionalGroceries.ScannerSystem;
using InterdimensionalGroceries.PhaseManagement;

namespace InterdimensionalGroceries.UI
{
    public class SkipDeliveryInputHandler : MonoBehaviour
    {
        private ScannerZone scannerZone;
        private Keyboard keyboard;

        private void Awake()
        {
            scannerZone = FindFirstObjectByType<ScannerZone>();
            keyboard = Keyboard.current;
        }

        private void Update()
        {
            if (keyboard == null)
            {
                keyboard = Keyboard.current;
                return;
            }

            if (GamePhaseManager.Instance == null || GamePhaseManager.Instance.CurrentPhase != GamePhase.DeliveryPhase)
            {
                return;
            }

            if (keyboard.xKey.wasPressedThisFrame)
            {
                if (scannerZone != null)
                {
                    scannerZone.SkipCurrentRequest();
                }
            }
        }
    }
}
