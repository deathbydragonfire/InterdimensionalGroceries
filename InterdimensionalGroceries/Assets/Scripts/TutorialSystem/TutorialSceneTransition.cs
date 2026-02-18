using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using InterdimensionalGroceries.PlayerController;

namespace TutorialSystem
{
    public class TutorialSceneTransition : MonoBehaviour
    {
        [Header("References")]
        public FadeController fadeController;
        public FirstPersonController playerController;
        public GameObject proceedButtonCanvas;

        [Header("Settings")]
        public string targetSceneName = "scene!";
        public float fadeToBlackDuration = 2f;

        private TutorialManager tutorialManager;

        private void Awake()
        {
            tutorialManager = GetComponent<TutorialManager>();
        }

        private void OnEnable()
        {
            if (tutorialManager != null)
            {
                tutorialManager.OnTutorialComplete += HandleTutorialComplete;
            }
        }

        private void OnDisable()
        {
            if (tutorialManager != null)
            {
                tutorialManager.OnTutorialComplete -= HandleTutorialComplete;
            }
        }

        private void HandleTutorialComplete()
        {
            Debug.Log("[TutorialSceneTransition] Tutorial complete - starting transition");
            StartCoroutine(TransitionSequence());
        }

        private IEnumerator TransitionSequence()
        {
            // Start fade to black
            if (fadeController != null)
            {
                fadeController.FadeToBlack(fadeToBlackDuration, null);
            }
            else
            {
                Debug.LogError("[TutorialSceneTransition] FadeController is null!");
            }

            // Wait for the final audio clip to finish
            TutorialEvent currentEvent = tutorialManager.GetCurrentEvent();
            if (currentEvent != null && currentEvent.audioClip != null)
            {
                float audioDuration = currentEvent.audioClip.length;
                Debug.Log($"[TutorialSceneTransition] Waiting {audioDuration}s for audio to finish");
                yield return new WaitForSeconds(audioDuration);
            }

            // Show proceed button after audio finishes
            ShowProceedButton();
        }

        private void ShowProceedButton()
        {
            Debug.Log("[TutorialSceneTransition] Showing proceed button");

            if (playerController != null)
            {
                playerController.SetControlsEnabled(false);
            }

            if (proceedButtonCanvas != null)
            {
                proceedButtonCanvas.SetActive(true);
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void OnProceedButtonClicked()
        {
            Debug.Log($"[TutorialSceneTransition] Loading scene: {targetSceneName}");
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
