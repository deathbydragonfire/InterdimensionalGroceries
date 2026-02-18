using UnityEngine;
using InterdimensionalGroceries.PlayerController;

namespace TutorialSystem
{
    public class MovementEnabler : MonoBehaviour
    {
        [Header("References")]
        public TutorialManager tutorialManager;
        public FirstPersonController firstPersonController;

        private void Start()
        {
            if (firstPersonController != null)
            {
                firstPersonController.enabled = false;
                Debug.Log("[MovementEnabler] Player movement disabled");
            }

            if (tutorialManager != null)
            {
                tutorialManager.OnEventStarted += HandleEventStarted;
            }
        }

        private void OnDestroy()
        {
            if (tutorialManager != null)
            {
                tutorialManager.OnEventStarted -= HandleEventStarted;
            }
        }

        private void HandleEventStarted(int eventIndex)
        {
            TutorialEvent currentEvent = tutorialManager.GetCurrentEvent();
            if (currentEvent == null)
            {
                return;
            }

            if (currentEvent.showLocomotionUI && firstPersonController != null)
            {
                firstPersonController.enabled = true;
                Debug.Log("[MovementEnabler] Player movement enabled");
            }
        }
    }
}
