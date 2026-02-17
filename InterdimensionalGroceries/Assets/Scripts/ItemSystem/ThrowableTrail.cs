using UnityEngine;

namespace InterdimensionalGroceries.ItemSystem
{
    [RequireComponent(typeof(TrailRenderer))]
    [RequireComponent(typeof(Rigidbody))]
    public class ThrowableTrail : MonoBehaviour
    {
        [Header("Trail Settings")]
        [SerializeField] private float velocityThreshold = 2f;
        
        private TrailRenderer trailRenderer;
        private Rigidbody rb;
        private bool wasEmitting;

        private void Awake()
        {
            trailRenderer = GetComponent<TrailRenderer>();
            rb = GetComponent<Rigidbody>();
            
            trailRenderer.emitting = false;
            wasEmitting = false;
        }

        private void Update()
        {
            bool shouldEmit = rb.linearVelocity.magnitude > velocityThreshold;

            if (shouldEmit && !wasEmitting)
            {
                trailRenderer.Clear();
                trailRenderer.emitting = true;
                wasEmitting = true;
            }
            else if (!shouldEmit && wasEmitting)
            {
                trailRenderer.emitting = false;
                wasEmitting = false;
            }
        }
    }
}
