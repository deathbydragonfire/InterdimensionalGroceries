using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace TutorialSystem
{
    public class TutorialUIController : MonoBehaviour
    {
        [Header("References")]
        public TutorialManager tutorialManager;
        public LocomotionTracker locomotionTracker;
        public CanvasGroup panelCanvasGroup;
        public TextMeshProUGUI instructionText;

        [Header("Key Visuals")]
        public Image keyW;
        public Image keyA;
        public Image keyS;
        public Image keyD;
        public Image keyShift;
        public Image keySpace;

        [Header("Colors")]
        public Color normalKeyColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        public Color highlightKeyColor = new Color(0.2f, 0.8f, 0.2f, 1f);

        [Header("Animation")]
        public float fadeDuration = 0.5f;

        private bool isPanelVisible;

        private void Start()
        {
            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.alpha = 0f;
                panelCanvasGroup.gameObject.SetActive(true);
            }

            ResetAllKeys();

            if (tutorialManager != null)
            {
                tutorialManager.OnEventStarted += HandleEventStarted;
                Debug.Log("[TutorialUIController] Subscribed to TutorialManager events");
            }
            else
            {
                Debug.LogWarning("[TutorialUIController] TutorialManager reference is null!");
            }
        }

        private void OnDestroy()
        {
            if (tutorialManager != null)
            {
                tutorialManager.OnEventStarted -= HandleEventStarted;
            }
        }

        private void Update()
        {
            if (isPanelVisible && locomotionTracker != null)
            {
                UpdateKeyVisuals();
            }
        }

        private void HandleEventStarted(int eventIndex)
        {
            TutorialEvent currentEvent = tutorialManager.GetCurrentEvent();
            if (currentEvent == null)
            {
                Debug.LogWarning($"[TutorialUIController] Event {eventIndex} is null!");
                return;
            }

            Debug.Log($"[TutorialUIController] Event {eventIndex} started - ShowUI: {currentEvent.showLocomotionUI}, HideUI: {currentEvent.hideLocomotionUI}");

            if (currentEvent.showLocomotionUI)
            {
                ShowPanel();
                if (instructionText != null)
                {
                    instructionText.text = "WASD to move, Shift to sprint, and Space to Jump";
                }
            }

            if (currentEvent.hideLocomotionUI)
            {
                HidePanel();
            }
        }

        private void ShowPanel()
        {
            if (isPanelVisible)
            {
                Debug.Log("[TutorialUIController] Panel already visible");
                return;
            }

            Debug.Log("[TutorialUIController] Showing panel");
            isPanelVisible = true;
            if (panelCanvasGroup != null)
            {
                panelCanvasGroup.gameObject.SetActive(true);
                StartCoroutine(FadePanel(1f));
            }
        }

        private void HidePanel()
        {
            if (!isPanelVisible)
            {
                Debug.Log("[TutorialUIController] Panel already hidden");
                return;
            }

            Debug.Log("[TutorialUIController] Hiding panel");
            isPanelVisible = false;
            if (panelCanvasGroup != null)
            {
                StartCoroutine(FadePanel(0f));
            }
        }

        private IEnumerator FadePanel(float targetAlpha)
        {
            float startAlpha = panelCanvasGroup.alpha;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
                yield return null;
            }

            panelCanvasGroup.alpha = targetAlpha;
            Debug.Log($"[TutorialUIController] Fade complete - Alpha: {targetAlpha}");
        }

        private void UpdateKeyVisuals()
        {
            if (keyW != null)
            {
                keyW.color = locomotionTracker.HasMovedForward ? highlightKeyColor : normalKeyColor;
            }
            if (keyS != null)
            {
                keyS.color = locomotionTracker.HasMovedBack ? highlightKeyColor : normalKeyColor;
            }
            if (keyA != null)
            {
                keyA.color = locomotionTracker.HasMovedLeft ? highlightKeyColor : normalKeyColor;
            }
            if (keyD != null)
            {
                keyD.color = locomotionTracker.HasMovedRight ? highlightKeyColor : normalKeyColor;
            }
            if (keyShift != null)
            {
                keyShift.color = locomotionTracker.HasSprinted ? highlightKeyColor : normalKeyColor;
            }
            if (keySpace != null)
            {
                keySpace.color = locomotionTracker.HasJumped ? highlightKeyColor : normalKeyColor;
            }
        }

        private void ResetAllKeys()
        {
            if (keyW != null) keyW.color = normalKeyColor;
            if (keyA != null) keyA.color = normalKeyColor;
            if (keyS != null) keyS.color = normalKeyColor;
            if (keyD != null) keyD.color = normalKeyColor;
            if (keyShift != null) keyShift.color = normalKeyColor;
            if (keySpace != null) keySpace.color = normalKeyColor;
        }
    }
}
