using UnityEngine;
using System;
using InterdimensionalGroceries.AudioSystem;

namespace InterdimensionalGroceries.PhaseManagement
{
    public class DeliveryPhaseTimer : MonoBehaviour
    {
        public event Action OnTimerExpired;
        public event Action<float> OnTimerUpdated;

        [SerializeField] private float deliveryDuration = 60f;
        [SerializeField] private float lowTimeThreshold = 10f;

        private float remainingTime;
        private bool isRunning;
        private int lastSecondPlayed = -1;

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

            if (remainingTime <= lowTimeThreshold && remainingTime > 0f)
            {
                int currentSecond = Mathf.FloorToInt(remainingTime);
                if (currentSecond != lastSecondPlayed)
                {
                    lastSecondPlayed = currentSecond;
                    PlayCountdownTick();
                }
            }

            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                isRunning = false;
                OnTimerExpired?.Invoke();
            }
        }

        private void PlayCountdownTick()
        {
            if (AudioManager.Instance != null)
            {
                Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                AudioManager.Instance.PlaySound(AudioEventType.CountdownTick, soundPosition);
            }
        }

        public void StartTimer()
        {
            remainingTime = deliveryDuration;
            isRunning = true;
            lastSecondPlayed = -1;
            OnTimerUpdated?.Invoke(remainingTime);
        }

        public void StopTimer()
        {
            isRunning = false;
            remainingTime = 0f;
            lastSecondPlayed = -1;
        }

        public float GetRemainingTime()
        {
            return remainingTime;
        }
    }
}
