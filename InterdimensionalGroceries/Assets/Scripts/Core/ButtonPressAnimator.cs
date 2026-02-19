using UnityEngine;
using System.Collections;

namespace InterdimensionalGroceries.Core
{
    public class ButtonPressAnimator : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private Vector3 pressDirection = Vector3.back;
        [SerializeField] private float pressDistance = 0.081f;
        [SerializeField] private float pressDuration = 0.1f;
        [SerializeField] private float releaseDuration = 0.15f;
        [SerializeField] private AnimationCurve pressCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        private Vector3 originalPosition;
        private bool isAnimating;

        private void Awake()
        {
            originalPosition = transform.localPosition;
        }

        public void PlayPressAnimation()
        {
            if (isAnimating) return;
            StartCoroutine(PressAnimationCoroutine());
        }

        private IEnumerator PressAnimationCoroutine()
        {
            isAnimating = true;
            
            Vector3 pressedPosition = originalPosition + pressDirection.normalized * pressDistance;
            
            float elapsed = 0f;
            while (elapsed < pressDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / pressDuration;
                float curveValue = pressCurve.Evaluate(t);
                transform.localPosition = Vector3.Lerp(originalPosition, pressedPosition, curveValue);
                yield return null;
            }
            
            transform.localPosition = pressedPosition;
            
            elapsed = 0f;
            while (elapsed < releaseDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / releaseDuration;
                float curveValue = pressCurve.Evaluate(t);
                transform.localPosition = Vector3.Lerp(pressedPosition, originalPosition, curveValue);
                yield return null;
            }
            
            transform.localPosition = originalPosition;
            isAnimating = false;
        }
    }
}
