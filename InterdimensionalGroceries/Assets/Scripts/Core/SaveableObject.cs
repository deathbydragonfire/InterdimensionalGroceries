using UnityEngine;

namespace InterdimensionalGroceries.Core
{
    public enum SaveableObjectType
    {
        Furniture,
        SpawnedItem,
        Interactable
    }

    public class SaveableObject : MonoBehaviour
    {
        [SerializeField] private string prefabIdentifier;
        [SerializeField] private SaveableObjectType objectType;
        [SerializeField] private bool isScenePlaced = false;

        public string PrefabIdentifier
        {
            get => prefabIdentifier;
            set => prefabIdentifier = value;
        }

        public SaveableObjectType ObjectType
        {
            get => objectType;
            set => objectType = value;
        }

        public bool IsScenePlaced
        {
            get => isScenePlaced;
            set => isScenePlaced = value;
        }

        private void OnEnable()
        {
            if (WorldObjectManager.Instance != null)
            {
                WorldObjectManager.Instance.RegisterObject(this);
                Debug.Log($"[SaveableObject] Registered: {prefabIdentifier} (Type: {objectType})");
            }
            else
            {
                Debug.LogWarning($"[SaveableObject] Cannot register {prefabIdentifier} - WorldObjectManager.Instance is null!");
            }
        }

        private void OnDisable()
        {
            if (WorldObjectManager.Instance != null)
            {
                WorldObjectManager.Instance.UnregisterObject(this);
                Debug.Log($"[SaveableObject] Unregistered: {prefabIdentifier}");
            }
        }
    }
}
