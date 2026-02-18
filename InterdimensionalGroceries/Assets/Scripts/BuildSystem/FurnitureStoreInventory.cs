using UnityEngine;
using System.Collections.Generic;

namespace InterdimensionalGroceries.BuildSystem
{
    [CreateAssetMenu(fileName = "SO_FurnitureStoreInventory", menuName = "InterdimensionalGroceries/Furniture Store Inventory")]
    public class FurnitureStoreInventory : ScriptableObject
    {
        [SerializeField] private List<BuildableObject> availableFurniture = new List<BuildableObject>();
        
        public List<BuildableObject> AvailableFurniture => availableFurniture;
    }
}
