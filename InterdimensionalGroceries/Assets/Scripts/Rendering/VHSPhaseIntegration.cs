using UnityEngine;
using InterdimensionalGroceries.PhaseManagement;

namespace InterdimensionalGroceries.Rendering
{
    [RequireComponent(typeof(VHSEffectController))]
    public class VHSPhaseIntegration : MonoBehaviour
    {
        [Header("Phase Intensity Settings")]
        [Range(0f, 2f)]
        [SerializeField] private float inventoryPhaseIntensity = 0.8f;
        [Range(0f, 2f)]
        [SerializeField] private float deliveryPhaseIntensity = 1.2f;
        
        [Header("Transition")]
        [SerializeField] private float transitionDuration = 1f;
        [SerializeField] private bool glitchOnPhaseTransition = true;
        [SerializeField] private float glitchDuration = 0.5f;
        
        private VHSEffectController vhsController;
        private float targetIntensity;
        private float currentIntensity;
        
        private void Awake()
        {
            vhsController = GetComponent<VHSEffectController>();
        }
        
        private void OnEnable()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnInventoryPhaseStarted += OnInventoryPhaseStarted;
                GamePhaseManager.Instance.OnDeliveryPhaseStarted += OnDeliveryPhaseStarted;
                
                currentIntensity = GamePhaseManager.Instance.CurrentPhase == GamePhase.InventoryPhase 
                    ? inventoryPhaseIntensity 
                    : deliveryPhaseIntensity;
                targetIntensity = currentIntensity;
                vhsController.SetGlobalIntensity(currentIntensity);
            }
        }
        
        private void OnDisable()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= OnInventoryPhaseStarted;
                GamePhaseManager.Instance.OnDeliveryPhaseStarted -= OnDeliveryPhaseStarted;
            }
        }
        
        private void Update()
        {
            if (Mathf.Abs(currentIntensity - targetIntensity) > 0.01f)
            {
                currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime / transitionDuration);
                vhsController.SetGlobalIntensity(currentIntensity);
            }
        }
        
        private void OnInventoryPhaseStarted()
        {
            if (glitchOnPhaseTransition)
            {
                vhsController.GlitchOut(glitchDuration);
            }
            
            targetIntensity = inventoryPhaseIntensity;
        }
        
        private void OnDeliveryPhaseStarted()
        {
            if (glitchOnPhaseTransition)
            {
                vhsController.GlitchOut(glitchDuration);
            }
            
            targetIntensity = deliveryPhaseIntensity;
        }
        
        public void TriggerDramaticGlitch(float duration = 1f)
        {
            vhsController.GlitchOut(duration);
        }
    }
}
