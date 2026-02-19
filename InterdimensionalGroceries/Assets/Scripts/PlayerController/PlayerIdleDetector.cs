using UnityEngine;
using System;
using InterdimensionalGroceries.UI;

namespace InterdimensionalGroceries.PlayerController
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerIdleDetector : MonoBehaviour
    {
        [Header("Idle Detection Settings")]
        [Tooltip("Time in seconds before player is considered idle")]
        [SerializeField] private float idleThreshold = 10f;

        [Tooltip("Minimum movement distance to reset idle timer")]
        [SerializeField] private float movementThreshold = 0.01f;

        [Header("References")]
        [Tooltip("Reference to the store menu controller (optional - will find automatically)")]
        [SerializeField] private StoreMenuController storeMenuController;

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;

        public event Action OnPlayerIdle;
        public event Action OnPlayerActive;

        private CharacterController characterController;
        private Vector3 lastPosition;
        private float idleTimer;
        private bool isIdle;

        public bool IsIdle => isIdle;
        public float IdleTime => idleTimer;
        public float TimeUntilIdle => Mathf.Max(0, idleThreshold - idleTimer);

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            lastPosition = transform.position;
            idleTimer = 0f;
            isIdle = false;

            if (storeMenuController == null)
            {
                storeMenuController = FindFirstObjectByType<StoreMenuController>();
                
                if (storeMenuController == null && debugMode)
                {
                    Debug.LogWarning("[PlayerIdleDetector] StoreMenuController not found - idle detection will still work but won't check for store interaction");
                }
            }
        }

        private void Update()
        {
            bool isUsingComputer = IsPlayerUsingComputer();
            
            if (isUsingComputer)
            {
                ResetIdleTimer();
                return;
            }

            bool hasMovedThisFrame = HasPlayerMoved();

            if (hasMovedThisFrame)
            {
                ResetIdleTimer();
            }
            else
            {
                idleTimer += Time.deltaTime;

                if (!isIdle && idleTimer >= idleThreshold)
                {
                    SetIdleState(true);
                }
            }

            lastPosition = transform.position;
        }

        private bool HasPlayerMoved()
        {
            float distanceMoved = Vector3.Distance(transform.position, lastPosition);
            return distanceMoved > movementThreshold;
        }

        private bool IsPlayerUsingComputer()
        {
            if (storeMenuController == null)
                return false;

            return storeMenuController.IsStoreOpen;
        }

        private void ResetIdleTimer()
        {
            if (isIdle)
            {
                SetIdleState(false);
            }

            idleTimer = 0f;
        }

        private void SetIdleState(bool idle)
        {
            if (isIdle == idle)
                return;

            isIdle = idle;

            if (idle)
            {
                if (debugMode)
                    Debug.Log($"[PlayerIdleDetector] Player is now IDLE (idle for {idleTimer:F1} seconds)");

                OnPlayerIdle?.Invoke();
            }
            else
            {
                if (debugMode)
                    Debug.Log("[PlayerIdleDetector] Player is now ACTIVE");

                OnPlayerActive?.Invoke();
            }
        }

        public void ForceResetIdleTimer()
        {
            ResetIdleTimer();
        }

        private void OnGUI()
        {
            if (!debugMode)
                return;

            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 14;
            style.normal.textColor = isIdle ? Color.red : Color.green;

            string status = isIdle ? "IDLE" : "ACTIVE";
            string timeInfo = isIdle ? $"Idle for: {idleTimer:F1}s" : $"Idle in: {TimeUntilIdle:F1}s";
            string computerInfo = IsPlayerUsingComputer() ? " (Using Computer)" : "";

            GUI.Label(new Rect(10, 100, 300, 60), 
                $"Player Status: {status}\n{timeInfo}{computerInfo}", 
                style);
        }
    }
}
