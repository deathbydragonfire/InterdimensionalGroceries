using UnityEngine;
using System;

namespace InterdimensionalGroceries.PhaseManagement
{
    public class DeliveryPhaseTimer : MonoBehaviour
    {
        public event Action OnTimerExpired;
        public event Action<float> OnTimerUpdated;

        [SerializeField] private float deliveryDuration = 60f;

        private float remainingTime;
        private bool isRunning;

        private void Awake()
        {
            OnTimerExpired += HandleTimerExpired;
        }

        private void Start()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted += StartTimer;
                GamePhaseManager.Instance.OnInventoryPhaseStarted += StopTimer;
                
                if (GamePhaseManager.Instance.CurrentPhase == GamePhase.DeliveryPhase)
                {
                    StartTimer();
                }
            }
        }

        private void HandleTimerExpired()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.StartInventoryPhase();
            }
        }

        private void OnDestroy()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted -= StartTimer;
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= StopTimer;
            }

            OnTimerExpired -= HandleTimerExpired;
        }

        private void Update()
        {
            if (!isRunning) return;

            remainingTime -= Time.deltaTime;
            OnTimerUpdated?.Invoke(remainingTime);

            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                isRunning = false;
                OnTimerExpired?.Invoke();
            }
        }

        public void StartTimer()
        {
            remainingTime = deliveryDuration;
            isRunning = true;
            OnTimerUpdated?.Invoke(remainingTime);
        }

        public void StopTimer()
        {
            isRunning = false;
            remainingTime = 0f;
        }

        public float GetRemainingTime()
        {
            return remainingTime;
        }
    }
}
