using UnityEngine;
using TMPro;
using System.Collections;

public class MoneyNotification : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Animation Settings")]
    [SerializeField] private float slideDistance = 100f;
    [SerializeField] private float slideInDuration = 0.3f;
    [SerializeField] private float displayDuration = 1.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private AnimationCurve slideInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private RectTransform rectTransform;
    private Vector2 targetPosition;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
        
        if (notificationText == null)
        {
            notificationText = GetComponent<TMP_Text>();
        }
    }

    public void Show(float amount, System.Action onComplete = null)
    {
        gameObject.SetActive(true);
        
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        notificationText.text = $"+${amount:0.00}";
        animationCoroutine = StartCoroutine(AnimationCoroutine(onComplete));
    }

    public void ShowCustomMessage(string message, System.Action onComplete = null)
    {
        gameObject.SetActive(true);
        
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        
        notificationText.text = message;
        animationCoroutine = StartCoroutine(AnimationCoroutine(onComplete));
    }

    private IEnumerator AnimationCoroutine(System.Action onComplete)
    {
        targetPosition = rectTransform.anchoredPosition;
        Vector2 startPosition = targetPosition - new Vector2(slideDistance, 0f);
        
        rectTransform.anchoredPosition = startPosition;
        canvasGroup.alpha = 1f;

        float elapsed = 0f;
        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float progress = slideInCurve.Evaluate(elapsed / slideInDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, progress);
            yield return null;
        }
        
        rectTransform.anchoredPosition = targetPosition;

        yield return new WaitForSeconds(displayDuration);

        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - (elapsed / fadeOutDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
        
        onComplete?.Invoke();
        animationCoroutine = null;
    }

    public void ForceHide()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        
        gameObject.SetActive(false);
    }
}
