using UnityEngine;
using System.Collections;

public class MoneyUIController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private float shakeIntensity = 10f;
    [SerializeField] private float scaleAmount = 1.2f;
    [SerializeField] private float scaleDuration = 0.3f;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private RectTransform rectTransform;
    private Coroutine currentAnimation;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
    }

    public void PlayMoneyAddedAnimation()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = StartCoroutine(MoneyAddedAnimationCoroutine());
    }

    private IEnumerator MoneyAddedAnimationCoroutine()
    {
        float elapsed = 0f;
        float halfDuration = shakeDuration / 2f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / shakeDuration;

            float shakeX = Random.Range(-shakeIntensity, shakeIntensity) * (1f - progress);
            float shakeY = Random.Range(-shakeIntensity, shakeIntensity) * (1f - progress);
            rectTransform.anchoredPosition = originalPosition + new Vector3(shakeX, shakeY, 0f);

            float scaleProgress = elapsed < halfDuration 
                ? elapsed / halfDuration 
                : 1f - ((elapsed - halfDuration) / halfDuration);
            
            float currentScale = Mathf.Lerp(1f, scaleAmount, Mathf.Sin(scaleProgress * Mathf.PI * 0.5f));
            rectTransform.localScale = originalScale * currentScale;

            yield return null;
        }

        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
        currentAnimation = null;
    }
}
