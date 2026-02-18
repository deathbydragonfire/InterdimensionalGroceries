using UnityEngine;
using System.Collections.Generic;

namespace TutorialSystem
{
    public class TutorialObjectManager : MonoBehaviour
    {
        [Header("Objects to Deactivate on Start")]
        [Tooltip("These objects will be deactivated when the tutorial starts")]
        public GameObject[] objectsToDeactivate;

        private Dictionary<string, GameObject> objectCache = new Dictionary<string, GameObject>();

        private void Awake()
        {
            CacheObjects();
        }

        private void Start()
        {
            DeactivateObjects();
        }

        private void CacheObjects()
        {
            if (objectsToDeactivate == null || objectsToDeactivate.Length == 0)
            {
                return;
            }

            objectCache.Clear();
            foreach (GameObject obj in objectsToDeactivate)
            {
                if (obj != null)
                {
                    objectCache[obj.name] = obj;
                    Debug.Log($"[TutorialObjectManager] Cached: {obj.name}");
                }
            }
        }

        private void DeactivateObjects()
        {
            if (objectsToDeactivate == null || objectsToDeactivate.Length == 0)
            {
                return;
            }

            foreach (GameObject obj in objectsToDeactivate)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                    Debug.Log($"[TutorialObjectManager] Deactivated: {obj.name}");
                }
            }
        }

        public GameObject GetCachedObject(string objectName)
        {
            if (objectCache.TryGetValue(objectName, out GameObject obj))
            {
                return obj;
            }
            return null;
        }

        public bool HasCachedObject(string objectName)
        {
            return objectCache.ContainsKey(objectName);
        }
    }
}
