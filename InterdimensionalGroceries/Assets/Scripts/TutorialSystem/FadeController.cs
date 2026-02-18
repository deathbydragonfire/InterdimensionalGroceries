using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace TutorialSystem
{
    public class FadeController : MonoBehaviour
    {
        [SerializeField] private Image fadeImage;

        private void Awake()
        {
            if (fadeImage == null)
            {
                Debug.LogError("[FadeController] Fade Image is not assigned!");
            }
        }

        public void FadeFromBlack(float duration, Action onComplete = null)
        {
            if (fadeImage == null)
            {
                Debug.LogError("[FadeController] Cannot fade: Fade Image is null");
                onComplete?.Invoke();
                return;
            }

            StartCoroutine(FadeFromBlackCoroutine(duration, onComplete));
        }

        public void FadeToBlack(float duration, Action onComplete = null)
        {
            if (fadeImage == null)
            {
                Debug.LogError("[FadeController] Cannot fade: Fade Image is null");
                onComplete?.Invoke();
                return;
            }

            StartCoroutine(FadeToBlackCoroutine(duration, onComplete));
        }

        private IEnumerator FadeFromBlackCoroutine(float duration, Action onComplete)
        {
            fadeImage.gameObject.SetActive(true);
            
            Color color = fadeImage.color;
            color.a = 1f;
            fadeImage.color = color;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                color.a = Mathf.Lerp(1f, 0f, t);
                fadeImage.color = color;
                
                yield return null;
            }

            color.a = 0f;
            fadeImage.color = color;
            fadeImage.gameObject.SetActive(false);

            onComplete?.Invoke();
        }

        private IEnumerator FadeToBlackCoroutine(float duration, Action onComplete)
        {
            fadeImage.gameObject.SetActive(true);
            
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                color.a = Mathf.Lerp(0f, 1f, t);
                fadeImage.color = color;
                
                yield return null;
            }

            color.a = 1f;
            fadeImage.color = color;

            onComplete?.Invoke();
        }
    }
}
