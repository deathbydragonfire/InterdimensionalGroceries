using UnityEngine;
using System;
using InterdimensionalGroceries.AudioSystem;

namespace InterdimensionalGroceries.PhaseManagement
{
    public enum GamePhase
    {
        InventoryPhase,
        DeliveryPhase
    }

    public class GamePhaseManager : MonoBehaviour
    {
        public static GamePhaseManager Instance { get; private set; }

        public event Action OnDeliveryPhaseStarted;
        public event Action OnDeliveryPhaseEnded;
        public event Action OnInventoryPhaseStarted;

        private GamePhase currentPhase = GamePhase.InventoryPhase;

        public GamePhase CurrentPhase => currentPhase;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.Log($"[GamePhaseManager] Destroying duplicate in scene {gameObject.scene.name}");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Debug.Log($"[GamePhaseManager] Initialized in scene: {gameObject.scene.name}");
        }

        private void Start()
        {
            Debug.Log("[GamePhaseManager] Start - Invoking OnInventoryPhaseStarted");
            OnInventoryPhaseStarted?.Invoke();
        }

        public void StartDeliveryPhase()
        {
            if (currentPhase == GamePhase.DeliveryPhase) return;

            currentPhase = GamePhase.DeliveryPhase;
            
            if (AudioManager.Instance != null)
            {
                Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                AudioManager.Instance.PlaySound(AudioEventType.DeliveryPhaseStart, soundPosition);
            }
            
            OnDeliveryPhaseStarted?.Invoke();
        }

        public void StartInventoryPhase()
        {
            if (currentPhase == GamePhase.InventoryPhase) return;

            currentPhase = GamePhase.InventoryPhase;
            OnDeliveryPhaseEnded?.Invoke();
            
            if (AudioManager.Instance != null)
            {
                Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                AudioManager.Instance.PlaySound(AudioEventType.InventoryPhaseStart, soundPosition);
            }
            
            OnInventoryPhaseStarted?.Invoke();
        }
    }
}
