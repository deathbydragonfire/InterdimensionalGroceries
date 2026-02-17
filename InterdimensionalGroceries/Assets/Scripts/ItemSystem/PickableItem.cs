using UnityEngine;
using InterdimensionalGroceries.Core;

namespace InterdimensionalGroceries.ItemSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class PickableItem : MonoBehaviour, IPickable
    {
        [SerializeField] private ItemData itemData;

        private Rigidbody rb;
        private Transform originalParent;
        private int originalLayer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public ItemData GetItemData()
        {
            return itemData;
        }

        public void OnPickedUp()
        {
            originalParent = transform.parent;
            originalLayer = gameObject.layer;

            rb.isKinematic = true;
            rb.useGravity = false;
        }

        public void OnDropped()
        {
            transform.parent = originalParent;
            gameObject.layer = originalLayer;

            rb.isKinematic = false;
            rb.useGravity = true;
        }

        public void OnThrown(float force)
        {
            transform.parent = originalParent;
            gameObject.layer = originalLayer;

            rb.isKinematic = false;
            rb.useGravity = true;

            rb.AddForce(Camera.main.transform.forward * force, ForceMode.Impulse);
        }
    }
}
