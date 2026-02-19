using UnityEngine;
using System.Collections;
using InterdimensionalGroceries.PhaseManagement;

namespace InterdimensionalGroceries.UI
{
    [System.Serializable]
    public class CustomerSprites
    {
        public string customerName;

        public Sprite idleSprite;
        public Sprite scanningSprite;
        public Sprite correctSprite;
        public Sprite wrongSprite;
    }

    public class CustomerScreenEyeAnimator : MonoBehaviour
    {
        [Header("Eye References")]
        [SerializeField] private SpriteRenderer eye1SpriteRenderer;
        [SerializeField] private SpriteRenderer eye2SpriteRenderer;
        [SerializeField] private GameObject eye1GameObject;
        [SerializeField] private GameObject eye2GameObject;

        [Header("Eye Animation Settings")]
        [SerializeField] private float minAnimationInterval = 3f;
        [SerializeField] private float maxAnimationInterval = 5f;

        [Header("Customer System")]
        [SerializeField] private SpriteRenderer customerSpriteRenderer;
        [SerializeField] private GameObject customerGameObject;
        [SerializeField] private CustomerSprites[] customers;
        [SerializeField] private float customerSwitchDelay = 1.5f;

        private Coroutine eyeCoroutine;
        private CustomerSprites currentCustomer;

        private void Start()
        {
            SubscribeToPhaseEvents();

            if (GamePhaseManager.Instance != null &&
                GamePhaseManager.Instance.CurrentPhase == GamePhase.InventoryPhase)
            {
                StartEyeAnimation();
                SetEyeVisibility(true);
                SetCustomerVisibility(false);
            }
            else
            {
                SetEyeVisibility(false);
                StartNewCustomer();
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromPhaseEvents();
        }

        #region Phase Handling

        private void SubscribeToPhaseEvents()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnInventoryPhaseStarted += OnInventoryPhaseStarted;
                GamePhaseManager.Instance.OnDeliveryPhaseStarted += OnDeliveryPhaseStarted;
            }
        }

        private void UnsubscribeFromPhaseEvents()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= OnInventoryPhaseStarted;
                GamePhaseManager.Instance.OnDeliveryPhaseStarted -= OnDeliveryPhaseStarted;
            }
        }

        private void OnInventoryPhaseStarted()
        {
            StopAllCoroutines();

            SetCustomerVisibility(false);

            SetEyeVisibility(true);
            StartEyeAnimation();
        }

        private void OnDeliveryPhaseStarted()
        {
            StopEyeAnimation();
            SetEyeVisibility(false);

            StartNewCustomer();
        }

        #endregion

        #region Eye Animation

        private void StartEyeAnimation()
        {
            if (eyeCoroutine != null)
                StopCoroutine(eyeCoroutine);

            eyeCoroutine = StartCoroutine(AnimateEyes());
        }

        private void StopEyeAnimation()
        {
            if (eyeCoroutine != null)
            {
                StopCoroutine(eyeCoroutine);
                eyeCoroutine = null;
            }
        }

        private IEnumerator AnimateEyes()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minAnimationInterval, maxAnimationInterval));

                if (eye1SpriteRenderer != null)
                    eye1SpriteRenderer.flipX = !eye1SpriteRenderer.flipX;

                if (eye2SpriteRenderer != null)
                    eye2SpriteRenderer.flipX = !eye2SpriteRenderer.flipX;
            }
        }

        private void SetEyeVisibility(bool visible)
        {
            if (eye1GameObject != null)
                eye1GameObject.SetActive(visible);

            if (eye2GameObject != null)
                eye2GameObject.SetActive(visible);
        }

        #endregion

        #region Customer System

        private void StartNewCustomer()
        {
            if (customers == null || customers.Length == 0)
                return;

            currentCustomer = customers[Random.Range(0, customers.Length)];

            SetCustomerVisibility(true);
            SetCustomerSprite(currentCustomer.idleSprite);
        }

        public void ShowCustomerScanning()
        {
            if (currentCustomer != null)
                SetCustomerSprite(currentCustomer.scanningSprite);
        }

        public void ShowCustomerCorrect()
        {
            if (currentCustomer != null)
                SetCustomerSprite(currentCustomer.correctSprite);

            StartCoroutine(SwitchCustomer());
        }

        public void ShowCustomerWrong()
        {
            if (currentCustomer != null)
                SetCustomerSprite(currentCustomer.wrongSprite);

            StartCoroutine(SwitchCustomer());
        }

        private IEnumerator SwitchCustomer()
        {
            yield return new WaitForSeconds(customerSwitchDelay);

            SetCustomerVisibility(false);

            yield return new WaitForSeconds(0.5f);

            StartNewCustomer();
        }

        private void SetCustomerSprite(Sprite sprite)
        {
            if (customerSpriteRenderer != null)
                customerSpriteRenderer.sprite = sprite;
        }

        private void SetCustomerVisibility(bool visible)
        {
            if (customerGameObject != null)
                customerGameObject.SetActive(visible);
        }

        #endregion
    }
}
