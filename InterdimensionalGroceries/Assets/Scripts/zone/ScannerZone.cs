using UnityEngine;
using System.Collections;
using System;
using InterdimensionalGroceries.ItemSystem;
using InterdimensionalGroceries.PlayerController;
using InterdimensionalGroceries.AudioSystem;
using InterdimensionalGroceries.PhaseManagement;
using InterdimensionalGroceries.CustomerSystem;

namespace InterdimensionalGroceries.ScannerSystem
{
    public class ScannerZone : MonoBehaviour
    {
        public event Action<ItemType> OnItemScanned;
        [Header("Scanner Settings")]
        [SerializeField] private Transform snapPoint;
        [SerializeField] private float scanTime = 2f;
        [SerializeField] private float ejectForce = 8f;

        [Header("Zone Colliders")]
        [SerializeField] private Collider throwZoneCollider;

        [Header("References")]
        [SerializeField] private ScannerUI scannerUI;
        [SerializeField] private ScanProgressBar scanProgressBarPrefab;
        [SerializeField] private MoneyNotificationPool moneyNotificationPool;
        
        [Header("Visual Feedback Settings")]
        [SerializeField] private float acceptedDestroyDelay = 0.5f;

        private ItemType requestedItem;
        private bool isBusy;
        private Collider placementZoneCollider;
        private ScanProgressBar currentProgressBar;
        private TutorialSystem.TutorialScannerController tutorialController;
        private bool isSkipping;

        public bool IsObjectInRange(GameObject obj)
        {
            if (obj == null) return false;
            
            PickableItem item = obj.GetComponent<PickableItem>();
            if (item == null) return false;
            
            Collider zoneToCheck = item.WasThrown ? throwZoneCollider : placementZoneCollider;
            
            if (zoneToCheck == null || !zoneToCheck.isTrigger) return false;
            
            return zoneToCheck.bounds.Contains(obj.transform.position);
        }

        private void Start()
        {
            placementZoneCollider = GetComponent<Collider>();
            tutorialController = GetComponent<TutorialSystem.TutorialScannerController>();
            
            if (scannerUI != null)
            {
                scannerUI.SetSkipButtonClickHandler(SkipCurrentRequest);
            }
            
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted += OnDeliveryPhaseStarted;
                GamePhaseManager.Instance.OnInventoryPhaseStarted += OnInventoryPhaseStarted;
            }
            
            if (tutorialController == null || !tutorialController.IsTutorialActive())
            {
                if (GamePhaseManager.Instance == null || GamePhaseManager.Instance.CurrentPhase == GamePhase.DeliveryPhase)
                {
                    GenerateNewRequest();
                }
            }
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
            if (tutorialController == null || !tutorialController.IsTutorialActive())
            {
                StartCoroutine(GenerateRequestDelayed());
            }
        }
        
        private IEnumerator GenerateRequestDelayed()
        {
            yield return new WaitForEndOfFrame();
            GenerateNewRequest();
        }

        private void OnInventoryPhaseStarted()
        {
            requestedItem = ItemType.Unknown;
            scannerUI.ShowSkipButton(false);
        }

        public void SkipCurrentRequest()
        {
            if (isSkipping || isBusy) return;
            if (requestedItem == ItemType.Unknown) return;
            if (GamePhaseManager.Instance != null && GamePhaseManager.Instance.CurrentPhase != GamePhase.DeliveryPhase) return;

            StartCoroutine(SkipRequestCoroutine());
        }

        private IEnumerator SkipRequestCoroutine()
        {
            isSkipping = true;

            float itemPrice = GetItemPrice(requestedItem);
            float penaltyCost = itemPrice / 2f;

            MoneyManager.Instance.DeductMoney(penaltyCost);

            if (moneyNotificationPool != null)
            {
                moneyNotificationPool.SpawnNotification(-penaltyCost);
            }

            scannerUI.ShowRequest("Skipped - Generating New Request...");
            scannerUI.ShowSkipButton(false);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(AudioEventType.Rejection, snapPoint.position);
            }

            yield return new WaitForSeconds(1.5f);

            GenerateNewRequest();

            isSkipping = false;
        }

        private float GetItemPrice(ItemType itemType)
        {
            PickableItem[] allItems = FindObjectsByType<PickableItem>(FindObjectsSortMode.None);
            
            foreach (PickableItem item in allItems)
            {
                ItemData data = item.GetItemData();
                if (data != null && data.ItemType == itemType)
                {
                    return data.Price;
                }
            }

            return 10f;
        }

        private void OnTriggerStay(Collider other)
        {
            if (isBusy) return;

            if (GamePhaseManager.Instance != null && GamePhaseManager.Instance.CurrentPhase != GamePhase.DeliveryPhase)
            {
                return;
            }

            PickableItem item = other.GetComponent<PickableItem>();

            if (item == null) return;

            Rigidbody rb = item.GetComponent<Rigidbody>();

            // Only scan if item is NOT being held
            if (rb.isKinematic == false)
            {
                // Check if item is in the correct zone based on whether it was thrown
                Collider zoneToCheck = item.WasThrown ? throwZoneCollider : placementZoneCollider;
                
                if (zoneToCheck != null && zoneToCheck.bounds.Contains(item.transform.position))
                {
                    StartCoroutine(ScanItem(item));
                }
            }
        }

        private IEnumerator ScanItem(PickableItem item)
        {
            isBusy = true;
            item.IsBeingScanned = true;

            Rigidbody rb = item.GetComponent<Rigidbody>();

            rb.isKinematic = true;
            rb.useGravity = false;

            item.transform.position = snapPoint.position;
            item.transform.rotation = snapPoint.rotation;
            item.transform.parent = snapPoint;

            scannerUI.ShowScanning();

            if (scanProgressBarPrefab != null)
            {
                currentProgressBar = Instantiate(scanProgressBarPrefab);
                currentProgressBar.SetTarget(item.gameObject);
                currentProgressBar.StartScanning(scanTime);
            }

            yield return new WaitForSeconds(scanTime);

            if (currentProgressBar != null)
            {
                currentProgressBar.StopScanning();
                Destroy(currentProgressBar.gameObject);
                currentProgressBar = null;
            }

            CheckItem(item);
        }

        private void CheckItem(PickableItem item)
        {
            ItemData data = item.GetItemData();

            bool isCorrect = false;
            
            // Tutorial mode: accept "Tutorial Cube"
            if (tutorialController != null && tutorialController.IsTutorialActive())
            {
                isCorrect = item.gameObject.name == "Tutorial Cube";
            }
            else
            {
                isCorrect = data.ItemType == requestedItem;
            }

            if (isCorrect)
            {
                // Fire OnItemScanned only when item is accepted
                Debug.Log($"[ScannerZone] Item ACCEPTED! Invoking OnItemScanned for {data.ItemType}");
                OnItemScanned?.Invoke(data.ItemType);
                
                scannerUI.ShowCorrect();

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound(AudioEventType.Acceptance, snapPoint.position);
                }

                MoneyManager.Instance.AddMoney(data.Price);

                if (moneyNotificationPool != null)
                {
                    // Tutorial mode: show custom message
                    if (tutorialController != null && tutorialController.IsTutorialActive())
                    {
                        moneyNotificationPool.SpawnCustomNotification("Package Delivered!");
                    }
                    else
                    {
                        moneyNotificationPool.SpawnNotification(data.Price);
                    }
                }

                ItemVisualFeedback feedback = item.GetComponent<ItemVisualFeedback>();
                if (feedback != null)
                {
                    feedback.PlayAcceptedEffect(() => 
                    {
                        if (item != null && item.gameObject != null)
                        {
                            Destroy(item.gameObject);
                        }
                    });
                    
                    StartCoroutine(DelayedNextRequest(acceptedDestroyDelay));
                }
                else
                {
                    Destroy(item.gameObject);
                    StartCoroutine(NextRequest());
                }
            }

            else
            {
                scannerUI.ShowWrong();

                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound(AudioEventType.Rejection, snapPoint.position);
                }

                ItemVisualFeedback feedback = item.GetComponent<ItemVisualFeedback>();
                if (feedback != null)
                {
                    feedback.PlayRejectedEffect(() => EjectItem(item));
                }
                else
                {
                    EjectItem(item);
                }

                StartCoroutine(RejectionComplete());
            }
        }

        private IEnumerator DelayedNextRequest(float delay)
        {
            yield return new WaitForSeconds(delay);
            GenerateNewRequest();
            isBusy = false;
        }

        private void EjectItem(PickableItem item)
        {
            Rigidbody rb = item.GetComponent<Rigidbody>();

            item.transform.parent = null;
            item.IsBeingScanned = false;

            rb.isKinematic = false;
            rb.useGravity = true;

            // Stronger force
            rb.AddForce(transform.forward * ejectForce, ForceMode.Impulse);
        }

        private IEnumerator NextRequest()
        {
            yield return new WaitForSeconds(1.5f);

            GenerateNewRequest();

            isBusy = false;
        }

        private IEnumerator RejectionComplete()
        {
            yield return new WaitForSeconds(1.5f);

            scannerUI.ShowRequest(requestedItem.ToString());

            isBusy = false;
        }

        private void GenerateNewRequest()
        {
            if (GamePhaseManager.Instance != null && GamePhaseManager.Instance.CurrentPhase == GamePhase.InventoryPhase)
            {
                return;
            }

            if (tutorialController != null && tutorialController.IsTutorialActive())
            {
                tutorialController.ShowTutorialRequest();
                return;
            }

            ItemType[] values =
                (ItemType[])System.Enum.GetValues(typeof(ItemType));

            requestedItem =
                values[UnityEngine.Random.Range(0, values.Length)];

            if (requestedItem == ItemType.Unknown)
            {
                GenerateNewRequest();
                return;
            }

            string displayText;
            if (CustomerCommentManager.Instance != null)
            {
                displayText = CustomerCommentManager.Instance.GetRandomComment(requestedItem);
            }
            else
            {
                displayText = requestedItem.ToString();
            }

            scannerUI.ShowRequest(displayText);
        }
    }
}
