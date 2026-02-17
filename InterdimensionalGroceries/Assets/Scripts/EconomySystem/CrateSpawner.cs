using UnityEngine;
using System.Collections.Generic;

namespace InterdimensionalGroceries.EconomySystem
{
    public class CrateSpawner : MonoBehaviour
    {
        public static CrateSpawner Instance { get; private set; }
        
        [Header("Spawn Settings")]
        [SerializeField] private Transform deliveryZone;
        [SerializeField] private GameObject cratePrefab;
        [SerializeField] private int maxItemsPerCrate = 5;
        [SerializeField] private float crateSpacing = 1.5f;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        public void SpawnCratesWithItems(List<SupplyItemData> purchasedItems)
        {
            if (purchasedItems == null || purchasedItems.Count == 0)
            {
                Debug.LogWarning("CrateSpawner: No items to spawn.");
                return;
            }
            
            if (deliveryZone == null)
            {
                Debug.LogError("CrateSpawner: Delivery zone not assigned!");
                return;
            }
            
            if (cratePrefab == null)
            {
                Debug.LogError("CrateSpawner: Crate prefab not assigned!");
                return;
            }
            
            int cratesNeeded = Mathf.CeilToInt((float)purchasedItems.Count / maxItemsPerCrate);
            int itemIndex = 0;
            
            for (int i = 0; i < cratesNeeded; i++)
            {
                Vector3 spawnPosition = deliveryZone.position + new Vector3(i * crateSpacing, 0f, 0f);
                GameObject crateInstance = Instantiate(cratePrefab, spawnPosition, deliveryZone.rotation);
                
                CrateContents contents = crateInstance.GetComponent<CrateContents>();
                if (contents == null)
                {
                    contents = crateInstance.AddComponent<CrateContents>();
                }
                
                for (int j = 0; j < maxItemsPerCrate && itemIndex < purchasedItems.Count; j++)
                {
                    contents.AddItem(purchasedItems[itemIndex]);
                    itemIndex++;
                }
                
                Debug.Log($"Spawned crate {i + 1} with {contents.CurrentCount} items at {spawnPosition}");
            }
        }
    }
}
