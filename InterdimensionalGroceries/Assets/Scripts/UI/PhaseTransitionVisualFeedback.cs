using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using InterdimensionalGroceries.PhaseManagement;

namespace InterdimensionalGroceries.UI
{
    public class PhaseTransitionVisualFeedback : MonoBehaviour
    {
        [Header("Flash Settings")]
        [SerializeField] private Image flashImage;
        [SerializeField] private Color deliveryPhaseFlashColor = new Color(1f, 0.5f, 0f, 0.3f);
        [SerializeField] private Color inventoryPhaseFlashColor = new Color(0f, 0.5f, 1f, 0.3f);
        [SerializeField] private float flashDuration = 0.5f;

        private void Awake()
        {
            if (flashImage != null)
            {
                Color transparent = flashImage.color;
                transparent.a = 0f;
                flashImage.color = transparent;
            }

            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted += OnDeliveryPhaseStarted;
                GamePhaseManager.Instance.OnInventoryPhaseStarted += OnInventoryPhaseStarted;
            }
        }

        private void OnDestroy()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted -= OnDeliveryPhaseStarted;
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= OnInventoryPhaseStarted;
            }
        }

        private void OnDeliveryPhaseStarted()
        {
            StartCoroutine(FlashScreen(deliveryPhaseFlashColor));
        }

        private void OnInventoryPhaseStarted()
        {
            StartCoroutine(FlashScreen(inventoryPhaseFlashColor));
        }

        private IEnumerator FlashScreen(Color flashColor)
        {
            if (flashImage == null) yield break;

            float elapsed = 0f;
            
            while (elapsed < flashDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / flashDuration;
                
                float alpha = Mathf.Sin(progress * Mathf.PI);
                
                Color currentColor = flashColor;
                currentColor.a = alpha * flashColor.a;
                flashImage.color = currentColor;
                
                yield return null;
            }
            
            Color transparent = flashColor;
            transparent.a = 0f;
            flashImage.color = transparent;
        }
    }
}
