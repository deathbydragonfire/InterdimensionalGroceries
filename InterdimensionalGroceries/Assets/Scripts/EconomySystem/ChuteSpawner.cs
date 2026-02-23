using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InterdimensionalGroceries.AudioSystem;
using InterdimensionalGroceries.Core;

namespace InterdimensionalGroceries.EconomySystem
{
    public class ChuteSpawner : MonoBehaviour
    {
        public static ChuteSpawner Instance { get; private set; }
        
        [Header("Spawn Settings")]
        [SerializeField] private float spawnDelay = 1f;
        
        [Header("Rotation Settings")]
        [SerializeField] private float minAngularVelocity = 2f;
        [SerializeField] private float maxAngularVelocity = 8f;
        
        private List<ChuteSpawnPoint> chuteSpawnPoints = new List<ChuteSpawnPoint>();
        private int lastUsedChuteIndex = -1;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        
        private void Start()
        {
            FindAllChuteSpawnPoints();
        }
        
        private void FindAllChuteSpawnPoints()
        {
            chuteSpawnPoints.Clear();
            ChuteSpawnPoint[] foundPoints = FindObjectsByType<ChuteSpawnPoint>(FindObjectsSortMode.None);
            chuteSpawnPoints.AddRange(foundPoints);
            
            if (chuteSpawnPoints.Count == 0)
            {
                Debug.LogWarning("ChuteSpawner: No ChuteSpawnPoint components found in the scene!");
            }
            else
            {
                Debug.Log($"ChuteSpawner: Found {chuteSpawnPoints.Count} chute spawn points");
            }
        }
        
        public void SpawnItemsFromChutes(List<SupplyItemData> purchasedItems)
        {
            if (purchasedItems == null || purchasedItems.Count == 0)
            {
                Debug.LogWarning("ChuteSpawner: No items to spawn.");
                return;
            }
            
            if (chuteSpawnPoints.Count == 0)
            {
                Debug.LogError("ChuteSpawner: No chute spawn points available!");
                return;
            }
            
            StartCoroutine(SpawnItemsWithDelay(purchasedItems));
        }
        
        private IEnumerator SpawnItemsWithDelay(List<SupplyItemData> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                SupplyItemData itemData = items[i];
                
                if (itemData == null || itemData.ItemPrefab == null)
                {
                    Debug.LogWarning($"ChuteSpawner: Item {i} has null data or prefab, skipping.");
                    continue;
                }
                
                ChuteSpawnPoint selectedChute = GetNextChute();
                SpawnItemAtChute(itemData, selectedChute);
                
                if (i < items.Count - 1)
                {
                    yield return new WaitForSeconds(spawnDelay);
                }
            }
        }
        
        private ChuteSpawnPoint GetNextChute()
        {
            if (chuteSpawnPoints.Count == 1)
            {
                return chuteSpawnPoints[0];
            }
            
            int selectedIndex;
            
            if (lastUsedChuteIndex == -1)
            {
                selectedIndex = Random.Range(0, chuteSpawnPoints.Count);
            }
            else
            {
                do
                {
                    selectedIndex = Random.Range(0, chuteSpawnPoints.Count);
                }
                while (selectedIndex == lastUsedChuteIndex);
            }
            
            lastUsedChuteIndex = selectedIndex;
            return chuteSpawnPoints[selectedIndex];
        }
        
        private void SpawnItemAtChute(SupplyItemData itemData, ChuteSpawnPoint spawnPoint)
        {
            GameObject itemInstance = Instantiate(itemData.ItemPrefab, spawnPoint.SpawnPosition, Quaternion.identity);
            
            SaveableObject saveableComponent = itemInstance.GetComponent<SaveableObject>();
            if (saveableComponent == null)
            {
                saveableComponent = itemInstance.AddComponent<SaveableObject>();
                Debug.Log($"[ChuteSpawner] Added SaveableObject to {itemData.ItemName}");
            }
            else
            {
                if (WorldObjectManager.Instance != null)
                {
                    WorldObjectManager.Instance.UnregisterObject(saveableComponent);
                }
            }
            
            saveableComponent.PrefabIdentifier = itemData.ItemName;
            saveableComponent.ObjectType = SaveableObjectType.SpawnedItem;
            
            if (WorldObjectManager.Instance != null)
            {
                WorldObjectManager.Instance.RegisterObject(saveableComponent);
            }
            
            Debug.Log($"[ChuteSpawner] Configured and registered: {itemData.ItemName}");
            
            Rigidbody rb = itemInstance.GetComponentInChildren<Rigidbody>();
            if (rb != null)
            {
                Collider[] colliders = rb.GetComponents<Collider>();
                
                StartCoroutine(DisableCollidersTemporarily(colliders, 0.3f));
                
                Vector3 ejectionForce = spawnPoint.GetRandomEjectionForce();
                rb.AddForce(ejectionForce, ForceMode.VelocityChange);
                
                Vector3 randomAngularVelocity = new Vector3(
                    Random.Range(-maxAngularVelocity, maxAngularVelocity),
                    Random.Range(-maxAngularVelocity, maxAngularVelocity),
                    Random.Range(-maxAngularVelocity, maxAngularVelocity)
                );
                
                float angularSpeed = Random.Range(minAngularVelocity, maxAngularVelocity);
                rb.angularVelocity = randomAngularVelocity.normalized * angularSpeed;
            }
            else
            {
                Debug.LogWarning($"ChuteSpawner: Item {itemData.ItemName} does not have a Rigidbody component!");
            }
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(AudioEventType.ItemSpawn, spawnPoint.SpawnPosition);
            }
        }
        
        private IEnumerator DisableCollidersTemporarily(Collider[] colliders, float duration)
        {
            foreach (Collider col in colliders)
            {
                if (col != null)
                {
                    col.enabled = false;
                }
            }
            
            yield return new WaitForSeconds(duration);
            
            foreach (Collider col in colliders)
            {
                if (col != null)
                {
                    col.enabled = true;
                }
            }
        }
    }
}
