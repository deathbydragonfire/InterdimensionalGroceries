using UnityEngine;
using InterdimensionalGroceries.PhaseManagement;

namespace InterdimensionalGroceries.UI
{
    public class PhaseBasedVisibility : MonoBehaviour
    {
        [SerializeField] private GamePhase visibleDuringPhase = GamePhase.DeliveryPhase;
        
        private SpriteRenderer spriteRenderer;
        private Canvas canvas;
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            canvas = GetComponent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted += OnPhaseChanged;
                GamePhaseManager.Instance.OnInventoryPhaseStarted += OnPhaseChanged;
                
                UpdateVisibility(GamePhaseManager.Instance.CurrentPhase);
            }
        }

        private void OnDisable()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted -= OnPhaseChanged;
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= OnPhaseChanged;
            }
        }

        private void Start()
        {
            if (GamePhaseManager.Instance != null)
            {
                UpdateVisibility(GamePhaseManager.Instance.CurrentPhase);
            }
        }

        private void OnPhaseChanged()
        {
            if (GamePhaseManager.Instance != null)
            {
                UpdateVisibility(GamePhaseManager.Instance.CurrentPhase);
            }
        }

        private void UpdateVisibility(GamePhase currentPhase)
        {
            bool shouldBeVisible = currentPhase == visibleDuringPhase;
            
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = shouldBeVisible;
            }
            
            if (canvas != null)
            {
                canvas.enabled = shouldBeVisible;
            }
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = shouldBeVisible ? 1f : 0f;
                canvasGroup.interactable = shouldBeVisible;
                canvasGroup.blocksRaycasts = shouldBeVisible;
            }
        }
    }
}
