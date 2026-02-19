using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using InterdimensionalGroceries.PhaseManagement;
using InterdimensionalGroceries.AudioSystem;

namespace InterdimensionalGroceries.UI
{
    public class BeginDeliveriesButton : MonoBehaviour
    {
        [SerializeField] private Button beginButton;
        [SerializeField] private GameObject buttonContainer;

        private Keyboard keyboard;

        private void Awake()
        {
            if (beginButton != null)
            {
                beginButton.onClick.AddListener(OnBeginDeliveriesClicked);
            }

            keyboard = Keyboard.current;
        }

        private void Start()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted += HideButton;
                GamePhaseManager.Instance.OnInventoryPhaseStarted += ShowButton;
            }
            
            Invoke(nameof(UpdateButtonVisibility), 0.1f);
        }

        private void Update()
        {
            if (keyboard == null)
            {
                keyboard = Keyboard.current;
                return;
            }

            if (GamePhaseManager.Instance == null || GamePhaseManager.Instance.CurrentPhase != GamePhase.InventoryPhase)
            {
                return;
            }

            if (keyboard.xKey.wasPressedThisFrame)
            {
                OnBeginDeliveriesClicked();
            }
        }

        private void OnDestroy()
        {
            if (beginButton != null)
            {
                beginButton.onClick.RemoveListener(OnBeginDeliveriesClicked);
            }

            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted -= HideButton;
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= ShowButton;
            }
        }

        private void OnBeginDeliveriesClicked()
        {
            if (AudioManager.Instance != null)
            {
                Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                AudioManager.Instance.PlaySound(AudioEventType.UIButtonClick, soundPosition);
            }

            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.StartDeliveryPhase();
            }
        }

        private void UpdateButtonVisibility()
        {
            if (GamePhaseManager.Instance != null && GamePhaseManager.Instance.CurrentPhase == GamePhase.InventoryPhase)
            {
                ShowButton();
            }
            else
            {
                HideButton();
            }
        }

        private void ShowButton()
        {
            if (buttonContainer != null)
            {
                buttonContainer.SetActive(true);
            }
        }

        private void HideButton()
        {
            if (buttonContainer != null)
            {
                buttonContainer.SetActive(false);
            }
        }
    }
}
