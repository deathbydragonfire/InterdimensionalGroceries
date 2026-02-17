using UnityEngine;
using System.Collections.Generic;

namespace InterdimensionalGroceries.EconomySystem
{
    public class CrateContents : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int maxCapacity = 5;
        
        private List<SupplyItemData> items = new List<SupplyItemData>();
        
        public int MaxCapacity => maxCapacity;
        public int CurrentCount => items.Count;
        public bool IsFull => items.Count >= maxCapacity;
        public List<SupplyItemData> Items => new List<SupplyItemData>(items);
        
        public bool AddItem(SupplyItemData item)
        {
            if (IsFull)
            {
                return false;
            }
            
            items.Add(item);
            return true;
        }
        
        public void ClearContents()
        {
            items.Clear();
        }
    }
}
