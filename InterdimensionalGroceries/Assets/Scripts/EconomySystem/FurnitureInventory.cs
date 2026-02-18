using UnityEngine;
using System.Collections.Generic;
using InterdimensionalGroceries.BuildSystem;

namespace InterdimensionalGroceries.EconomySystem
{
    [CreateAssetMenu(fileName = "SO_FurnitureInventory", menuName = "InterdimensionalGroceries/Furniture Inventory")]
    public class FurnitureInventory : ScriptableObject
    {
        [SerializeField] private List<FurnitureInventoryEntry> ownedFurniture = new List<FurnitureInventoryEntry>();
        
        public void AddFurniture(BuildableObject furniture, int quantity)
        {
            if (furniture == null || quantity <= 0) return;
            
            FurnitureInventoryEntry entry = ownedFurniture.Find(e => e.furniture == furniture);
            
            if (entry != null)
            {
                entry.quantity += quantity;
            }
            else
            {
                ownedFurniture.Add(new FurnitureInventoryEntry { furniture = furniture, quantity = quantity });
            }
        }
        
        public bool RemoveFurniture(BuildableObject furniture, int quantity)
        {
            if (furniture == null || quantity <= 0) return false;
            
            FurnitureInventoryEntry entry = ownedFurniture.Find(e => e.furniture == furniture);
            
            if (entry != null && entry.quantity >= quantity)
            {
                entry.quantity -= quantity;
                
                if (entry.quantity <= 0)
                {
                    ownedFurniture.Remove(entry);
                }
                
                return true;
            }
            
            return false;
        }
        
        public int GetFurnitureCount(BuildableObject furniture)
        {
            if (furniture == null) return 0;
            
            FurnitureInventoryEntry entry = ownedFurniture.Find(e => e.furniture == furniture);
            return entry != null ? entry.quantity : 0;
        }
        
        public List<FurnitureInventoryEntry> GetAllOwnedFurniture()
        {
            return new List<FurnitureInventoryEntry>(ownedFurniture);
        }
        
        public void ClearInventory()
        {
            ownedFurniture.Clear();
        }
    }
    
    [System.Serializable]
    public class FurnitureInventoryEntry
    {
        public BuildableObject furniture;
        public int quantity;
    }
}
