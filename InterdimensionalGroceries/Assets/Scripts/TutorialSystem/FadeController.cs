using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace TutorialSystem
{
    public class FadeController : MonoBehaviour
    {
        [SerializeField] private Image fadeImage;
        [SerializeField] private bool startWithBlackScreen = true;
        [SerializeField] private bool startWithWhiteScreen = false;

        private void Awake()
        {
            if (fadeImage == null)
            {
                Debug.LogError("[FadeController] Fade Image is not assigned!");
                return;
            }

            if (startWithWhiteScreen)
            {
                fadeImage.gameObject.SetActive(true);
                fadeImage.color = new Color(1f, 1f, 1f, 1f);
                Debug.Log("[FadeController] Initialized with white screen");
            }
            else if (startWithBlackScreen)
            {
                fadeImage.gameObject.SetActive(true);
                fadeImage.color = new Color(0f, 0f, 0f, 1f);
                Debug.Log("[FadeController] Initialized with black screen");
            }
        }

        public void FadeFromWhite(float duration, Action onComplete = null)
        {
            if (fadeImage == null)
            {
                Debug.LogError("[FadeController] Cannot fade: Fade Image is null");
                onComplete?.Invoke();
                return;
            }

            StartCoroutine(FadeFromWhiteCoroutine(duration, onComplete));
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

        public void FadeToWhite(float duration, Action onComplete = null)
        {
            if (fadeImage == null)
            {
                Debug.LogError("[FadeController] Cannot fade: Fade Image is null");
                onComplete?.Invoke();
                return;
            }

            StartCoroutine(FadeToWhiteCoroutine(duration, onComplete));
        }

        private IEnumerator FadeFromWhiteCoroutine(float duration, Action onComplete)
        {
            Debug.Log($"[FadeController] Starting FadeFromWhite, duration: {duration}s");
            fadeImage.gameObject.SetActive(true);
            
            Color color = Color.white;
            color.a = 1f;
            fadeImage.color = color;
            Debug.Log($"[FadeController] Initial color: {color}");

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
            Debug.Log("[FadeController] FadeFromWhite complete");

            onComplete?.Invoke();
        }

        private IEnumerator FadeFromBlackCoroutine(float duration, Action onComplete)
        {
            Debug.Log($"[FadeController] Starting FadeFromBlack, duration: {duration}s");
            fadeImage.gameObject.SetActive(true);
            
            Color color = fadeImage.color;
            color.r = 0f;
            color.g = 0f;
            color.b = 0f;
            color.a = 1f;
            fadeImage.color = color;
            Debug.Log($"[FadeController] Initial color: {color}");

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
            Debug.Log("[FadeController] FadeFromBlack complete");

            onComplete?.Invoke();
        }

        private IEnumerator FadeToBlackCoroutine(float duration, Action onComplete)
        {
            Debug.Log($"[FadeController] Starting FadeToBlack, duration: {duration}s");
            fadeImage.gameObject.SetActive(true);
            
            Color color = new Color(0f, 0f, 0f, 0f);
            fadeImage.color = color;
            Debug.Log($"[FadeController] Initial color: {color}");

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
            Debug.Log("[FadeController] FadeToBlack complete");

            onComplete?.Invoke();
        }

        private IEnumerator FadeToWhiteCoroutine(float duration, Action onComplete)
        {
            fadeImage.gameObject.SetActive(true);
            
            Color whiteColor = new Color(1f, 1f, 1f, 0f);
            fadeImage.color = whiteColor;

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                whiteColor.a = Mathf.Lerp(0f, 1f, t);
                fadeImage.color = whiteColor;
                
                yield return null;
            }

            whiteColor.a = 1f;
            fadeImage.color = whiteColor;

            onComplete?.Invoke();
        }
    }
}
