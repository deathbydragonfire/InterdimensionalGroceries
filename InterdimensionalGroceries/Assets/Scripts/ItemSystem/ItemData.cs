using UnityEngine;

namespace InterdimensionalGroceries.ItemSystem
{
    [System.Serializable]
    public class ItemData
    {
        [SerializeField] private string itemName = "Unknown Item";
        [SerializeField] private string tagline = "A mysterious object";
        [SerializeField] private float price = 0f;

        public string ItemName => itemName;
        public string Tagline => tagline;
        public float Price => price;
    }
}
