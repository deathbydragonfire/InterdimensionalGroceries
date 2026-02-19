using UnityEngine;
using InterdimensionalGroceries.AudioSystem;

namespace InterdimensionalGroceries.ItemSystem
{
    public enum ItemType
    {
        Eggs,
        Apples,
        Meat,
        Beans,
        Pebbles,
        Estos,
        Scarabs,
        LongPig,
        Soda,
        Eye,
        Grub,
        Rat,
        Unknown
    }

    [System.Serializable]
    public class ItemData
    {
        [Header("Basic Info")]
        [SerializeField] private string itemName = "Unknown Item";
        [SerializeField] private string tagline = "A mysterious object";

        [Header("Economy")]
        [SerializeField] private float price = 0f;

        [Header("Item Type")]
        [SerializeField] private ItemType itemType = ItemType.Unknown;

        [Header("Audio")]
        [SerializeField] private ItemAudioData itemAudioData;

        public string ItemName => itemName;
        public string Tagline => tagline;
        public float Price => price;
        public ItemType ItemType => itemType;
        public ItemAudioData ItemAudioData => itemAudioData;
    }
}

