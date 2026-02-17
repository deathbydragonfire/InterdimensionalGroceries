using UnityEngine;

namespace InterdimensionalGroceries.BuildSystem
{
    [CreateAssetMenu(fileName = "SO_NewBuildable", menuName = "Interdimensional Groceries/Buildable Object")]
    public class BuildableObject : ScriptableObject
    {
        [Header("Object Info")]
        [SerializeField] private string objectName;
        [SerializeField] private GameObject prefab;
        [SerializeField] private Sprite icon;
        
        [Header("Placement")]
        [SerializeField] private int placementCost = 0;
        
        [Header("UI")]
        [SerializeField] [TextArea(3, 5)] private string description;
        
        public string ObjectName => objectName;
        public GameObject Prefab => prefab;
        public Sprite Icon => icon;
        public int PlacementCost => placementCost;
        public string Description => description;
    }
}
