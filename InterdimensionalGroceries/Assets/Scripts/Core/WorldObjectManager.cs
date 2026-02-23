using UnityEngine;
using System.Collections.Generic;

namespace InterdimensionalGroceries.Core
{
    public class WorldObjectManager : MonoBehaviour
    {
        public static WorldObjectManager Instance { get; private set; }

        private HashSet<SaveableObject> trackedObjects = new HashSet<SaveableObject>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void RegisterObject(SaveableObject saveableObject)
        {
            if (saveableObject != null && !string.IsNullOrEmpty(saveableObject.PrefabIdentifier))
            {
                trackedObjects.Add(saveableObject);
                Debug.Log($"[WorldObjectManager] Registered: {saveableObject.PrefabIdentifier} (Type: {saveableObject.ObjectType}). Total tracked: {trackedObjects.Count}");
            }
            else
            {
                if (saveableObject == null)
                {
                    Debug.LogWarning("[WorldObjectManager] Attempted to register null SaveableObject");
                }
                else if (string.IsNullOrEmpty(saveableObject.PrefabIdentifier))
                {
                    Debug.LogWarning($"[WorldObjectManager] Rejected registration - empty identifier on {saveableObject.gameObject.name}");
                }
            }
        }

        public void UnregisterObject(SaveableObject saveableObject)
        {
            if (saveableObject != null)
            {
                trackedObjects.Remove(saveableObject);
            }
        }

        public List<SaveableObject> GetAllSaveableObjects()
        {
            trackedObjects.RemoveWhere(obj => obj == null);
            Debug.Log($"[WorldObjectManager] GetAllSaveableObjects called. Tracked count: {trackedObjects.Count}");
            foreach (var obj in trackedObjects)
            {
                Debug.Log($"  - {obj.PrefabIdentifier} ({obj.ObjectType})");
            }
            return new List<SaveableObject>(trackedObjects);
        }

        public void ClearAllTrackedObjects()
        {
            List<SaveableObject> objectsToDestroy = new List<SaveableObject>();
            
            foreach (SaveableObject obj in trackedObjects)
            {
                if (obj != null && !obj.IsScenePlaced)
                {
                    objectsToDestroy.Add(obj);
                }
            }
            
            foreach (SaveableObject obj in objectsToDestroy)
            {
                if (obj != null && obj.gameObject != null)
                {
                    Destroy(obj.gameObject);
                }
            }
            
            trackedObjects.RemoveWhere(obj => obj == null || !obj.IsScenePlaced);
            
            Debug.Log($"[WorldObjectManager] Cleared {objectsToDestroy.Count} dynamic objects. {trackedObjects.Count} scene-placed objects remain.");
        }
    }
}
