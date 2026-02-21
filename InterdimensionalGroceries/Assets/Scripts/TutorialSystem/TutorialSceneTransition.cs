using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using InterdimensionalGroceries.PlayerController;
using InterdimensionalGroceries.BuildSystem;
using InterdimensionalGroceries.AudioSystem;
using InterdimensionalGroceries.EconomySystem;

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
        public float proceedButtonFadeDuration = 0.5f;

        private TutorialManager tutorialManager;
        private CanvasGroup proceedButtonCanvasGroup;

        private void Awake()
        {
            tutorialManager = GetComponent<TutorialManager>();
            
            if (proceedButtonCanvas != null)
            {
                proceedButtonCanvasGroup = proceedButtonCanvas.GetComponent<CanvasGroup>();
                if (proceedButtonCanvasGroup == null)
                {
                    proceedButtonCanvasGroup = proceedButtonCanvas.AddComponent<CanvasGroup>();
                }
            }
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
            // Exit build mode if active
            if (BuildModeController.Instance != null && BuildModeController.Instance.IsActive)
            {
                Debug.Log("[TutorialSceneTransition] Exiting build mode");
                BuildModeController.Instance.ExitBuildMode();
            }

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
                if (proceedButtonCanvasGroup != null)
                {
                    proceedButtonCanvasGroup.alpha = 0f;
                    StartCoroutine(FadeInProceedButton());
                }
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private IEnumerator FadeInProceedButton()
        {
            float elapsed = 0f;
            while (elapsed < proceedButtonFadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / proceedButtonFadeDuration;
                proceedButtonCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }
            proceedButtonCanvasGroup.alpha = 1f;
        }

        public void OnProceedButtonClicked()
        {
            Debug.Log($"[TutorialSceneTransition] Proceed button clicked, starting fade out");
            StartCoroutine(FadeOutAndLoadScene());
        }

        private IEnumerator FadeOutAndLoadScene()
        {
            if (proceedButtonCanvasGroup != null)
            {
                float elapsed = 0f;
                while (elapsed < proceedButtonFadeDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / proceedButtonFadeDuration;
                    proceedButtonCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                    yield return null;
                }
                proceedButtonCanvasGroup.alpha = 0f;
            }

            // Explicitly destroy the Intro scene's AudioManager and MusicManager
            // so the Actual Delivery Scene can use its own configured managers
            if (AudioManager.Instance != null)
            {
                Debug.Log("[TutorialSceneTransition] Destroying AudioManager from Intro/Tutorial before loading Actual Delivery Scene");
                Destroy(AudioManager.Instance.gameObject);
            }
            
            if (MusicManager.Instance != null)
            {
                Debug.Log("[TutorialSceneTransition] Destroying MusicManager from Intro/Tutorial before loading Actual Delivery Scene");
                Destroy(MusicManager.Instance.gameObject);
            }
            
            if (AbilityUpgradeManager.Instance != null)
            {
                Debug.Log("[TutorialSceneTransition] Destroying AbilityUpgradeManager from Intro/Tutorial before loading Actual Delivery Scene");
                Destroy(AbilityUpgradeManager.Instance.gameObject);
            }

            Debug.Log($"[TutorialSceneTransition] Loading scene: {targetSceneName}");
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
