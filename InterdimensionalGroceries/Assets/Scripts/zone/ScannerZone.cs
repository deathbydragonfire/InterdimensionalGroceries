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

        [Header("References")]
        [SerializeField] private ScannerUI scannerUI;

        private ItemType requestedItem;
        private bool isBusy;

        private void Start()
        {
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
                StartCoroutine(ScanItem(item));
            }
        }

        private IEnumerator ScanItem(PickableItem item)
        {
            isBusy = true;

            Rigidbody rb = item.GetComponent<Rigidbody>();

            // Lock in place
            rb.isKinematic = true;
            rb.useGravity = false;

            item.transform.position = snapPoint.position;
            item.transform.rotation = snapPoint.rotation;
            item.transform.parent = snapPoint;

            // Show scanning
            scannerUI.ShowScanning();

            yield return new WaitForSeconds(scanTime);

            CheckItem(item);
        }

        private void CheckItem(PickableItem item)
        {
            ItemData data = item.GetItemData();

            if (data.ItemType == requestedItem)
            {
                scannerUI.ShowCorrect();

                // Add money for this item
                MoneyManager.Instance.AddMoney(data.Price);

                Destroy(item.gameObject);

                StartCoroutine(NextRequest());
            }

            else
            {
                scannerUI.ShowWrong();

                EjectItem(item);

                StartCoroutine(NextRequest());
            }
        }

        private void EjectItem(PickableItem item)
        {
            Rigidbody rb = item.GetComponent<Rigidbody>();

            item.transform.parent = null;

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
