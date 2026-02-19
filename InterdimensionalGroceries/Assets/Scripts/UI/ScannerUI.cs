using TMPro;
using UnityEngine;
using UnityEngine.UI;
using InterdimensionalGroceries.PhaseManagement;

namespace InterdimensionalGroceries.ScannerSystem
{
    public class ScannerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text scannerText;
        [SerializeField] private TextMeshPro orderDisplay3D;
        [SerializeField] private Button skipButton;
        [SerializeField] private GameObject scannerContainer;

        private void Awake()
        {
            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted += OnDeliveryPhaseStarted;
                GamePhaseManager.Instance.OnInventoryPhaseStarted += OnInventoryPhaseStarted;
            }
            
            Invoke(nameof(UpdateVisibility), 0.1f);
        }

        private void OnDestroy()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted -= OnDeliveryPhaseStarted;
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= OnInventoryPhaseStarted;
            }
        }

        private void OnDeliveryPhaseStarted()
        {
            ShowScanner();
        }

        private void OnInventoryPhaseStarted()
        {
            HideScanner();
        }

        private void UpdateVisibility()
        {
            if (GamePhaseManager.Instance != null && GamePhaseManager.Instance.CurrentPhase == GamePhase.InventoryPhase)
            {
                HideScanner();
            }
            else
            {
                ShowScanner();
            }
        }

        private void ShowScanner()
        {
            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(true);
            }
        }

        private void HideScanner()
        {
            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(false);
            }
        }

        public void ShowRequest(string itemName)
        {
            SetText($"Bring: {itemName}", Color.white);
            ShowSkipButton(true);
        }

        public void ShowScanning()
        {
            SetText("Scanning...", Color.yellow);
            ShowSkipButton(false);
        }

        public void ShowCorrect()
        {
            SetText("Accepted", Color.green);
            ShowSkipButton(false);
        }

        public void ShowWrong()
        {
            SetText("Rejected", Color.red);
            ShowSkipButton(false);
        }

        public void ShowSkipButton(bool show)
        {
            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(show);
            }
        }

        public void SetSkipButtonClickHandler(System.Action onSkipClicked)
        {
            if (skipButton != null)
            {
                skipButton.onClick.RemoveAllListeners();
                skipButton.onClick.AddListener(() => onSkipClicked?.Invoke());
            }
        }

        private void SetText(string text, Color color)
        {
            if (orderDisplay3D != null)
            {
                orderDisplay3D.text = text;
                orderDisplay3D.color = color;
            }
            else if (scannerText != null)
            {
                scannerText.text = text;
                scannerText.color = color;
            }
        }
    }
}
