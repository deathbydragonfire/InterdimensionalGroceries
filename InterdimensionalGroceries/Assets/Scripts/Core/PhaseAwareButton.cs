using UnityEngine;
using UnityEngine.Events;
using InterdimensionalGroceries.PhaseManagement;

namespace InterdimensionalGroceries.Core
{
    public class PhaseAwareButton : MonoBehaviour, IClickable
    {
        [Header("Button Settings")]
        [SerializeField] private UnityEvent onButtonClicked;
        [SerializeField] private GamePhase activePhase = GamePhase.InventoryPhase;
        [SerializeField] private bool onlyActiveOnce = false;
        
        [Header("Tooltip Settings")]
        [SerializeField] private string activeTooltip = "[LMB to Click]";
        [SerializeField] private Color activeTooltipColor = Color.white;
        [SerializeField] private string inactiveTooltip = "[Not Available]";
        [SerializeField] private Color inactiveTooltipColor = Color.red;
        
        private ButtonPressAnimator pressAnimator;
        private bool hasBeenClicked = false;
        private bool isSubscribed = false;

        private void Awake()
        {
            pressAnimator = GetComponent<ButtonPressAnimator>();
        }

        private void Start()
        {
            TrySubscribeToPhaseEvents();
        }

        public void OnClick()
        {
            if (!CanClick())
                return;

            if (pressAnimator != null)
            {
                pressAnimator.PlayPressAnimation();
            }
            
            onButtonClicked?.Invoke();
            
            if (onlyActiveOnce)
            {
                hasBeenClicked = true;
            }
        }

        public bool CanClick()
        {
            if (GamePhaseManager.Instance == null)
                return false;

            if (onlyActiveOnce && hasBeenClicked)
                return false;

            return GamePhaseManager.Instance.CurrentPhase == activePhase;
        }

        public string GetTooltipText()
        {
            return CanClick() ? activeTooltip : inactiveTooltip;
        }

        public Color GetTooltipColor()
        {
            return CanClick() ? activeTooltipColor : inactiveTooltipColor;
        }

        public void ResetClickState()
        {
            hasBeenClicked = false;
        }

        private void OnEnable()
        {
            TrySubscribeToPhaseEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromPhaseEvents();
        }

        private void TrySubscribeToPhaseEvents()
        {
            if (isSubscribed || GamePhaseManager.Instance == null)
                return;

            GamePhaseManager.Instance.OnInventoryPhaseStarted += OnInventoryPhaseStarted;
            GamePhaseManager.Instance.OnDeliveryPhaseStarted += OnDeliveryPhaseStarted;
            isSubscribed = true;
        }

        private void UnsubscribeFromPhaseEvents()
        {
            if (!isSubscribed || GamePhaseManager.Instance == null)
                return;

            GamePhaseManager.Instance.OnInventoryPhaseStarted -= OnInventoryPhaseStarted;
            GamePhaseManager.Instance.OnDeliveryPhaseStarted -= OnDeliveryPhaseStarted;
            isSubscribed = false;
        }

        private void OnInventoryPhaseStarted()
        {
            if (onlyActiveOnce && activePhase == GamePhase.InventoryPhase)
            {
                hasBeenClicked = false;
                Debug.Log($"[PhaseAwareButton] {gameObject.name} reset for Inventory Phase");
            }
        }

        private void OnDeliveryPhaseStarted()
        {
            if (onlyActiveOnce && activePhase == GamePhase.DeliveryPhase)
            {
                hasBeenClicked = false;
                Debug.Log($"[PhaseAwareButton] {gameObject.name} reset for Delivery Phase");
            }
        }
    }
}
