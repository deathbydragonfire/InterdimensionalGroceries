using UnityEngine;
using UnityEngine.Events;

namespace InterdimensionalGroceries.Core
{
    public class ClickableButton : MonoBehaviour, IClickable
    {
        [SerializeField] private UnityEvent onButtonClicked;
        
        private ButtonPressAnimator pressAnimator;

        private void Awake()
        {
            pressAnimator = GetComponent<ButtonPressAnimator>();
        }

        public void OnClick()
        {
            if (pressAnimator != null)
            {
                pressAnimator.PlayPressAnimation();
            }
            
            onButtonClicked?.Invoke();
        }
    }
}
