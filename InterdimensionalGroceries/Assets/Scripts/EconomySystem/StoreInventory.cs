using UnityEngine;
using System.Collections.Generic;

namespace InterdimensionalGroceries.EconomySystem
{
    [CreateAssetMenu(fileName = "SO_StoreInventory", menuName = "InterdimensionalGroceries/Store Inventory")]
    public class StoreInventory : ScriptableObject
    {
        [SerializeField] private List<SupplyItemData> availableItems = new List<SupplyItemData>();
        
        public List<SupplyItemData> AvailableItems => availableItems;
    }
}
