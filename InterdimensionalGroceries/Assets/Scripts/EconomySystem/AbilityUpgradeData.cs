using UnityEngine;

namespace InterdimensionalGroceries.EconomySystem
{
    public enum UpgradeType
    {
        ThrowingStrength,
        MovementSpeed
    }

    [CreateAssetMenu(fileName = "SO_AbilityUpgrade", menuName = "Interdimensional Groceries/Ability Upgrade Data")]
    public class AbilityUpgradeData : ScriptableObject
    {
        [Header("Ability Information")]
        [SerializeField] private string abilityName;
        [SerializeField] [TextArea(3, 5)] private string description;
        [SerializeField] private UpgradeType upgradeType;

        [Header("Upgrade Progression")]
        [SerializeField] private int maxLevel = 5;
        [SerializeField] private float baseCost = 100f;
        [SerializeField] private float costMultiplier = 1.5f;

        public string AbilityName => abilityName;
        public string Description => description;
        public UpgradeType UpgradeType => upgradeType;
        public int MaxLevel => maxLevel;
        public float BaseCost => baseCost;
        public float CostMultiplier => costMultiplier;

        public float GetCostForLevel(int level)
        {
            if (level <= 0 || level > maxLevel)
                return 0f;

            return baseCost * Mathf.Pow(costMultiplier, level - 1);
        }
    }
}
