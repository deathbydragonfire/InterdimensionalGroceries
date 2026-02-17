using UnityEngine;

namespace InterdimensionalGroceries.EconomySystem
{
    [CreateAssetMenu(fileName = "SO_SupplyItem", menuName = "InterdimensionalGroceries/Supply Item Data")]
    public class SupplyItemData : ScriptableObject
    {
        [Header("Item Info")]
        [SerializeField] private string itemName;
        [SerializeField] private float basePrice;
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject itemPrefab;
        
        public string ItemName => itemName;
        public float BasePrice => basePrice;
        public Sprite Icon => icon;
        public GameObject ItemPrefab => itemPrefab;
    }
}
