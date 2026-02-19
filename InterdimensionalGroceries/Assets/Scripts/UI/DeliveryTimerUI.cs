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
                deliveryTimer.OnTimerExpired += ShowTimesUp;
            }
        }

        private void Start()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted += OnDeliveryPhaseStarted;
                GamePhaseManager.Instance.OnInventoryPhaseStarted += OnInventoryPhaseStarted;
            }
            
            Invoke(nameof(UpdateVisibility), 0.1f);
        }

        private void UpdateVisibility()
        {
            if (timerContainer != null)
            {
                timerContainer.SetActive(true);
            }

            if (GamePhaseManager.Instance != null)
            {
                if (GamePhaseManager.Instance.CurrentPhase == GamePhase.InventoryPhase)
                {
                    OnInventoryPhaseStarted();
                }
                else
                {
                    OnDeliveryPhaseStarted();
                }
            }
        }

        private void OnDestroy()
        {
            if (deliveryTimer != null)
            {
                deliveryTimer.OnTimerUpdated -= UpdateTimerDisplay;
                deliveryTimer.OnTimerExpired -= ShowTimesUp;
            }

            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted -= OnDeliveryPhaseStarted;
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= OnInventoryPhaseStarted;
            }
        }

        private void UpdateTimerDisplay(float remainingTime)
        {
            if (timerText != null)
            {
                if (remainingTime <= 0f)
                {
                    timerText.text = "TIME'S UP!";
                    timerText.color = lowTimeColor;
                }
                else
                {
                    int minutes = Mathf.FloorToInt(remainingTime / 60f);
                    int seconds = Mathf.FloorToInt(remainingTime % 60f);

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
        }

        private void ShowTimesUp()
        {
            if (timerText != null)
            {
                timerText.text = "TIME'S UP!";
                timerText.color = lowTimeColor;
            }
        }

        private void OnDeliveryPhaseStarted()
        {
            if (timerContainer != null)
            {
                timerContainer.SetActive(true);
            }
        }

        private void OnInventoryPhaseStarted()
        {
            if (timerContainer != null)
            {
                timerContainer.SetActive(true);
            }
            
            ShowTimesUp();
        }
    }
}
