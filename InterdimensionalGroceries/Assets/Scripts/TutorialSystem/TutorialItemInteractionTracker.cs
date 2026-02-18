using UnityEngine;
using System;
using InterdimensionalGroceries.PlayerController;
using InterdimensionalGroceries.ScannerSystem;
using InterdimensionalGroceries.ItemSystem;

namespace TutorialSystem
{
    public class TutorialItemInteractionTracker : MonoBehaviour
    {
        public event Action OnItemPickedUp;
        public event Action OnItemScanned;

        private bool hasPickedUpItem;
        private bool hasScannedItem;

        public bool HasPickedUpItem => hasPickedUpItem;
        public bool HasScannedItem => hasScannedItem;

        private ItemManipulationController itemController;
        private ScannerZone scannerZone;

        private void Awake()
        {
            itemController = GetComponent<ItemManipulationController>();
        }

        private void Start()
        {
            scannerZone = FindFirstObjectByType<ScannerZone>();
            
            if (scannerZone != null)
            {
                scannerZone.OnItemScanned += HandleItemScanned;
            }
        }

        private void OnEnable()
        {
            if (itemController != null)
            {
                itemController.OnItemPickedUp += HandleItemPickedUp;
            }
        }

        private void OnDisable()
        {
            if (itemController != null)
            {
                itemController.OnItemPickedUp -= HandleItemPickedUp;
            }

            if (scannerZone != null)
            {
                scannerZone.OnItemScanned -= HandleItemScanned;
            }
        }

        private void HandleItemPickedUp()
        {
            if (!hasPickedUpItem)
            {
                hasPickedUpItem = true;
                OnItemPickedUp?.Invoke();
            }
        }

        private void HandleItemScanned(ItemType itemType)
        {
            Debug.Log($"[TutorialItemInteractionTracker] HandleItemScanned called! ItemType: {itemType}, HasScannedItem: {hasScannedItem}");
            
            if (!hasScannedItem)
            {
                hasScannedItem = true;
                Debug.Log("[TutorialItemInteractionTracker] Invoking OnItemScanned!");
                OnItemScanned?.Invoke();
            }
            else
            {
                Debug.Log("[TutorialItemInteractionTracker] Item already scanned, ignoring");
            }
        }

        public void ResetTracking()
        {
            hasPickedUpItem = false;
            hasScannedItem = false;
        }
    }
}
