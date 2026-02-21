using UnityEngine;
using System.Collections;
using InterdimensionalGroceries.UI;
using InterdimensionalGroceries.AudioSystem;

namespace InterdimensionalGroceries.PhaseManagement
{
    public class DeliveryPhaseCountdown : MonoBehaviour
    {
        [Header("Countdown Settings")]
        [SerializeField] private int countdownFrom = 5;
        [SerializeField] private float countdownInterval = 1f;
        [SerializeField] private string deliverText = "DELIVER!";
        
        [Header("References")]
        [SerializeField] private OrderDisplayManager orderDisplay;
        
        private bool isCountingDown;

        private void Start()
        {
            if (orderDisplay == null)
            {
                orderDisplay = FindFirstObjectByType<OrderDisplayManager>();
            }
        }

        public void StartCountdown()
        {
            if (isCountingDown) return;
            StartCoroutine(CountdownCoroutine());
        }

        private IEnumerator CountdownCoroutine()
        {
            isCountingDown = true;

            if (orderDisplay != null)
            {
                orderDisplay.SetCountdownMode(true);
            }

            for (int i = countdownFrom; i >= 1; i--)
            {
                if (orderDisplay != null)
                {
                    orderDisplay.SetMessage(i.ToString());
                }

                if (AudioManager.Instance != null)
                {
                    Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                    AudioManager.Instance.PlaySound(AudioEventType.CountdownTick, soundPosition);
                    Debug.Log($"[DeliveryPhaseCountdown] Playing tick sound {i}");
                }
                else
                {
                    Debug.LogWarning($"[DeliveryPhaseCountdown] AudioManager.Instance is NULL on tick {i}!");
                }

                yield return new WaitForSeconds(countdownInterval);
            }

            if (orderDisplay != null)
            {
                orderDisplay.SetCountdownMode(false);
                orderDisplay.SetMessage(deliverText);
            }

            if (AudioManager.Instance != null)
            {
                Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                AudioManager.Instance.PlaySound(AudioEventType.CountdownTick, soundPosition);
            }

            yield return new WaitForSeconds(countdownInterval);

            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.StartDeliveryPhase();
            }

            isCountingDown = false;
        }
    }
}
