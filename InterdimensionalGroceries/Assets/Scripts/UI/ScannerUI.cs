using TMPro;
using UnityEngine;
using UnityEngine.UI;
using InterdimensionalGroceries.PhaseManagement;
using InterdimensionalGroceries.UI;
using System.Collections;

namespace InterdimensionalGroceries.ScannerSystem
{
    public class ScannerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text scannerText;
        [SerializeField] private TextMeshPro orderDisplay3D;
        [SerializeField] private Button skipButton;
        [SerializeField] private GameObject scannerContainer;

        [Header("Customer Integration")]
        [SerializeField] private CustomerScreenEyeAnimator customerAnimator;

        [Header("Typewriter Effect")]
        [SerializeField] private float typewriterSpeed = 0.05f;
        [SerializeField] private AudioClip typewriterSound;
        [SerializeField][Range(0f, 1f)] private float typewriterVolume = 0.3f;

        private AudioSource audioSource;
        private Coroutine typewriterCoroutine;

        private void Awake()
        {
            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(false);
            }

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = true;
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
            if (GamePhaseManager.Instance != null &&
                GamePhaseManager.Instance.CurrentPhase == GamePhase.InventoryPhase)
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
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
            }

            typewriterCoroutine = StartCoroutine(TypewriterEffect(itemName, Color.white));
            ShowSkipButton(true);
        }

        public void ShowScanning()
        {
            SetText("Scanning...", Color.yellow);
            ShowSkipButton(false);

            customerAnimator?.ShowCustomerScanning();
        }

        public void ShowCorrect()
        {
            SetText("Accepted", Color.green);
            ShowSkipButton(false);

            customerAnimator?.ShowCustomerCorrect();
        }

        public void ShowWrong()
        {
            SetText("Rejected", Color.red);
            ShowSkipButton(false);

            customerAnimator?.ShowCustomerWrong();
        }

        public void ShowSkipButton(bool show)
        {
            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(show);
            }
        }

        // 🔥 This was missing before — restored
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

        private IEnumerator TypewriterEffect(string fullText, Color color)
        {
            if (typewriterSound != null && audioSource != null)
            {
                audioSource.clip = typewriterSound;
                audioSource.volume = typewriterVolume;
                audioSource.Play();
            }

            if (orderDisplay3D != null)
            {
                orderDisplay3D.text = "";
                orderDisplay3D.color = color;
            }
            else if (scannerText != null)
            {
                scannerText.text = "";
                scannerText.color = color;
            }

            for (int i = 0; i <= fullText.Length; i++)
            {
                string currentText = fullText.Substring(0, i);

                if (orderDisplay3D != null)
                {
                    orderDisplay3D.text = currentText;
                }
                else if (scannerText != null)
                {
                    scannerText.text = currentText;
                }

                yield return new WaitForSeconds(typewriterSpeed);
            }

            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
