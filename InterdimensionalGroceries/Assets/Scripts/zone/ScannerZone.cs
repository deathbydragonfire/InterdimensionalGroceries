using UnityEngine;
using System.Collections;
using InterdimensionalGroceries.ItemSystem;
using InterdimensionalGroceries.PlayerController;

namespace InterdimensionalGroceries.ScannerSystem
{
    public class ScannerZone : MonoBehaviour
    {
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
            GenerateNewRequest();
        }

        private void OnTriggerStay(Collider other)
        {
            if (isBusy) return;

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

            if (data.ItemType == requestedItem)
            {
                scannerUI.ShowCorrect();

                MoneyManager.Instance.AddMoney(data.Price);

                if (moneyNotificationPool != null)
                {
                    moneyNotificationPool.SpawnNotification(data.Price);
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

                ItemVisualFeedback feedback = item.GetComponent<ItemVisualFeedback>();
                if (feedback != null)
                {
                    feedback.PlayRejectedEffect(() => EjectItem(item));
                }
                else
                {
                    EjectItem(item);
                }

                StartCoroutine(NextRequest());
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

        private void GenerateNewRequest()
        {
            ItemType[] values =
                (ItemType[])System.Enum.GetValues(typeof(ItemType));

            requestedItem =
                values[Random.Range(0, values.Length)];

            if (requestedItem == ItemType.Unknown)
            {
                GenerateNewRequest();
                return;
            }

            scannerUI.ShowRequest(requestedItem.ToString());
        }
    }
}
