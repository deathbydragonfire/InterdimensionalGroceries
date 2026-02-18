using UnityEngine;
using System;
using InterdimensionalGroceries.InteractionSystem;
using InterdimensionalGroceries.BuildSystem;
using InterdimensionalGroceries.UI;

namespace TutorialSystem
{
    public class TutorialInteractionTracker : MonoBehaviour
    {
        public event Action OnTerminalOpened;
        public event Action OnShelfPlaced;

        private bool hasOpenedTerminal;
        private bool hasPlacedShelf;
        private bool previousStoreState;

        public bool HasOpenedTerminal => hasOpenedTerminal;
        public bool HasPlacedShelf => hasPlacedShelf;

        private ComputerInteraction computerInteraction;
        private BuildModeController buildModeController;
        private StoreMenuController storeMenuController;

        private void Start()
        {
            computerInteraction = FindFirstObjectByType<ComputerInteraction>();
            buildModeController = FindFirstObjectByType<BuildModeController>();
            storeMenuController = FindFirstObjectByType<StoreMenuController>();
            previousStoreState = false;
        }

        private void Update()
        {
            if (!hasOpenedTerminal && storeMenuController != null)
            {
                CheckTerminalOpened();
            }

            if (!hasPlacedShelf && buildModeController != null)
            {
                CheckShelfPlaced();
            }
        }

        private void CheckTerminalOpened()
        {
            // Check if store menu just opened (transition from closed to open)
            bool isStoreOpen = storeMenuController.IsStoreOpen;
            
            if (isStoreOpen && !previousStoreState)
            {
                hasOpenedTerminal = true;
                Debug.Log("[TutorialInteractionTracker] Terminal opened!");
                OnTerminalOpened?.Invoke();
            }
            
            previousStoreState = isStoreOpen;
        }

        private void CheckShelfPlaced()
        {
            // Safety check: verify Shelf tag exists
            try
            {
                GameObject[] shelves = GameObject.FindGameObjectsWithTag("Shelf");
                if (shelves != null && shelves.Length > 0)
                {
                    // Check if any shelf is actually placed (not a ghost preview)
                    foreach (GameObject shelf in shelves)
                    {
                        // Ignore ghost previews (they have PlacementGhost component)
                        if (shelf.GetComponent<PlacementGhost>() != null)
                        {
                            continue;
                        }
                        
                        // Found a real placed shelf!
                        hasPlacedShelf = true;
                        Debug.Log("[TutorialInteractionTracker] Shelf placed!");
                        OnShelfPlaced?.Invoke();
                        return;
                    }
                }
            }
            catch (UnityException)
            {
                Debug.LogWarning("[TutorialInteractionTracker] 'Shelf' tag is not defined. Please add it in Tags & Layers.");
            }
        }

        public void ResetTracking()
        {
            hasOpenedTerminal = false;
            hasPlacedShelf = false;
        }
    }
}
