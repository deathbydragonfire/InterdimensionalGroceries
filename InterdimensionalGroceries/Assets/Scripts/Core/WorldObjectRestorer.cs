using UnityEngine;
using System.Collections.Generic;
using InterdimensionalGroceries.BuildSystem;
using InterdimensionalGroceries.EconomySystem;

namespace InterdimensionalGroceries.Core
{
    public class WorldObjectRestorer : MonoBehaviour
    {
        public static WorldObjectRestorer Instance { get; private set; }

        [Header("Prefab Sources")]
        [SerializeField] private FurnitureStoreInventory furnitureInventory;
        [SerializeField] private List<SupplyItemData> supplyItems = new List<SupplyItemData>();

        private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            BuildPrefabDictionary();
        }

        private void BuildPrefabDictionary()
        {
            prefabDictionary.Clear();

            if (furnitureInventory != null && furnitureInventory.AvailableFurniture != null)
            {
                foreach (BuildableObject furniture in furnitureInventory.AvailableFurniture)
                {
                    if (furniture != null && furniture.Prefab != null && !string.IsNullOrEmpty(furniture.ObjectName))
                    {
                        prefabDictionary[furniture.ObjectName] = furniture.Prefab;
                    }
                }
                Debug.Log($"[WorldObjectRestorer] Registered {furnitureInventory.AvailableFurniture.Count} furniture prefabs");
            }

            if (supplyItems != null)
            {
                foreach (SupplyItemData item in supplyItems)
                {
                    if (item != null && item.ItemPrefab != null && !string.IsNullOrEmpty(item.ItemName))
                    {
                        prefabDictionary[item.ItemName] = item.ItemPrefab;
                    }
                }
                Debug.Log($"[WorldObjectRestorer] Registered {supplyItems.Count} supply item prefabs");
            }

            Debug.Log($"[WorldObjectRestorer] Total prefabs in dictionary: {prefabDictionary.Count}");
        }

        public void RestoreWorldObjects(List<WorldObjectData> worldObjects)
        {
            if (worldObjects == null || worldObjects.Count == 0)
            {
                Debug.Log("[WorldObjectRestorer] No world objects to restore");
                return;
            }

            Debug.Log($"[WorldObjectRestorer] Starting restoration of {worldObjects.Count} objects");

            int restoredCount = 0;
            int failedCount = 0;

            foreach (WorldObjectData data in worldObjects)
            {
                if (string.IsNullOrEmpty(data.prefabIdentifier))
                {
                    Debug.LogWarning("[WorldObjectRestorer] Skipping object with empty prefab identifier");
                    failedCount++;
                    continue;
                }

                if (!prefabDictionary.TryGetValue(data.prefabIdentifier, out GameObject prefab))
                {
                    Debug.LogWarning($"[WorldObjectRestorer] Prefab not found for identifier: {data.prefabIdentifier}");
                    failedCount++;
                    continue;
                }

                Vector3 position = data.position.ToVector3();
                Quaternion rotation = data.rotation.ToQuaternion();
                Vector3 scale = data.scale.ToVector3();

                GameObject restoredObject = Instantiate(prefab, position, rotation);
                restoredObject.transform.localScale = scale;

                SaveableObject saveableComponent = restoredObject.GetComponent<SaveableObject>();
                if (saveableComponent == null)
                {
                    saveableComponent = restoredObject.AddComponent<SaveableObject>();
                }
                else
                {
                    if (WorldObjectManager.Instance != null)
                    {
                        WorldObjectManager.Instance.UnregisterObject(saveableComponent);
                    }
                }

                saveableComponent.PrefabIdentifier = data.prefabIdentifier;
                
                if (System.Enum.TryParse<SaveableObjectType>(data.objectType, out SaveableObjectType type))
                {
                    saveableComponent.ObjectType = type;
                }

                if (WorldObjectManager.Instance != null)
                {
                    WorldObjectManager.Instance.RegisterObject(saveableComponent);
                }

                Rigidbody rb = restoredObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                Debug.Log($"[WorldObjectRestorer] Restored and registered: {data.prefabIdentifier} at {position}");
                restoredCount++;
            }

            Debug.Log($"[WorldObjectRestorer] Restoration complete: {restoredCount} objects restored, {failedCount} failed");
        }
    }
}
