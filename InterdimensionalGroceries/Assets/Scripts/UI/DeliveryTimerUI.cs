using UnityEngine;
using TMPro;
using InterdimensionalGroceries.PhaseManagement;

namespace InterdimensionalGroceries.UI
{
    public class DeliveryTimerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private GameObject timerContainer;
        [SerializeField] private float lowTimeThreshold = 10f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color lowTimeColor = Color.red;

        private DeliveryPhaseTimer deliveryTimer;

        private void Awake()
        {
            deliveryTimer = FindFirstObjectByType<DeliveryPhaseTimer>();

            if (deliveryTimer != null)
            {
                deliveryTimer.OnTimerUpdated += UpdateTimerDisplay;
            }
        }

        private void Start()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted += ShowTimer;
                GamePhaseManager.Instance.OnInventoryPhaseStarted += HideTimer;
            }
            
            Invoke(nameof(UpdateVisibility), 0.1f);
        }

        private void UpdateVisibility()
        {
            if (GamePhaseManager.Instance != null)
            {
                if (GamePhaseManager.Instance.CurrentPhase == GamePhase.InventoryPhase)
                {
                    HideTimer();
                }
                else
                {
                    ShowTimer();
                }
            }
            else
            {
                ShowTimer();
            }
        }

        private void OnDestroy()
        {
            if (deliveryTimer != null)
            {
                deliveryTimer.OnTimerUpdated -= UpdateTimerDisplay;
            }

            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted -= ShowTimer;
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= HideTimer;
            }
        }

        private void UpdateTimerDisplay(float remainingTime)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);

            if (timerText != null)
            {
                timerText.text = $"{minutes:00}:{seconds:00}";

                if (remainingTime <= lowTimeThreshold)
                {
                    timerText.color = lowTimeColor;
                }
                else
                {
                    timerText.color = normalColor;
                }
            }
        }

        private void ShowTimer()
        {
            if (timerContainer != null)
            {
                timerContainer.SetActive(true);
            }
        }

        private void HideTimer()
        {
            if (timerContainer != null)
            {
                timerContainer.SetActive(false);
            }
        }
    }
}
