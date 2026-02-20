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
                firstPersonController.SetControlsEnabled(false);
                Debug.Log("[MovementEnabler] Player movement disabled");
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

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
                firstPersonController.SetControlsEnabled(true);
                Debug.Log("[MovementEnabler] Player movement enabled via SetControlsEnabled");
            }
        }
    }
}
