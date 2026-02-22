using UnityEngine;
using System;
using System.Collections.Generic;

namespace InterdimensionalGroceries.EconomySystem
{
    public class AbilityUpgradeManager : MonoBehaviour
    {
        public static AbilityUpgradeManager Instance { get; private set; }

        [Header("Available Upgrades")]
        [SerializeField] private List<AbilityUpgradeData> availableUpgrades = new List<AbilityUpgradeData>();

        private Dictionary<AbilityUpgradeData, int> upgradeLevels = new Dictionary<AbilityUpgradeData, int>();

        public event Action<AbilityUpgradeData, int> OnUpgradePurchased;

        private void Awake()
        {
            // Check if there's already an instance
            if (Instance != null)
            {
                // If the existing instance is this object, we're good
                if (Instance == this)
                {
                    // Already set, do nothing
                }
                // If the existing instance is a different valid object, destroy this new one
                else if (Instance != null) // Unity's null check - false if destroyed
                {
                    Destroy(gameObject);
                    return;
                }
                // If the existing instance was destroyed, replace it
                else
                {
                    Instance = this;
                }
            }
            else
            {
                // No existing instance, set this as the instance
                Instance = this;
            }

            DontDestroyOnLoad(gameObject);

            LoadUpgradeLevels();
        }

        private void LoadUpgradeLevels()
        {
            foreach (var upgrade in availableUpgrades)
            {
                if (upgrade == null)
                    continue;

                string key = GetPlayerPrefsKey(upgrade);
                int level = PlayerPrefs.GetInt(key, 0);
                upgradeLevels[upgrade] = level;
            }

            Debug.Log($"Loaded {upgradeLevels.Count} upgrade levels from PlayerPrefs");
        }

        private void SaveUpgradeLevel(AbilityUpgradeData upgrade, int level)
        {
            string key = GetPlayerPrefsKey(upgrade);
            PlayerPrefs.SetInt(key, level);
            PlayerPrefs.Save();
        }

        private string GetPlayerPrefsKey(AbilityUpgradeData upgrade)
        {
            return $"Upgrade_{upgrade.UpgradeType}_{upgrade.AbilityName}";
        }

        public int GetUpgradeLevel(AbilityUpgradeData upgrade)
        {
            if (upgrade == null)
                return 0;

            if (!upgradeLevels.ContainsKey(upgrade))
            {
                upgradeLevels[upgrade] = 0;
            }

            return upgradeLevels[upgrade];
        }

        public bool CanAffordUpgrade(AbilityUpgradeData upgrade)
        {
            if (upgrade == null)
                return false;

            int currentLevel = GetUpgradeLevel(upgrade);

            if (currentLevel >= upgrade.MaxLevel)
                return false;

            int nextLevel = currentLevel + 1;
            float cost = upgrade.GetCostForLevel(nextLevel);

            return MoneyManager.Instance != null && MoneyManager.Instance.GetCurrentMoney() >= cost;
        }

        public bool PurchaseUpgrade(AbilityUpgradeData upgrade)
        {
            if (upgrade == null)
                return false;

            int currentLevel = GetUpgradeLevel(upgrade);

            if (currentLevel >= upgrade.MaxLevel)
            {
                Debug.Log($"{upgrade.AbilityName} is already at max level");
                return false;
            }

            int nextLevel = currentLevel + 1;
            float cost = upgrade.GetCostForLevel(nextLevel);

            if (MoneyManager.Instance != null && MoneyManager.Instance.SpendMoney(cost))
            {
                upgradeLevels[upgrade] = nextLevel;
                SaveUpgradeLevel(upgrade, nextLevel);

                Debug.Log($"Purchased {upgrade.AbilityName} level {nextLevel} for ${cost:F2}");

                OnUpgradePurchased?.Invoke(upgrade, nextLevel);
                return true;
            }

            Debug.Log($"Insufficient funds to purchase {upgrade.AbilityName}");
            return false;
        }

        public float GetNextLevelCost(AbilityUpgradeData upgrade)
        {
            if (upgrade == null)
                return 0f;

            int currentLevel = GetUpgradeLevel(upgrade);

            if (currentLevel >= upgrade.MaxLevel)
                return 0f;

            return upgrade.GetCostForLevel(currentLevel + 1);
        }

        public List<AbilityUpgradeData> GetAvailableUpgrades()
        {
            return new List<AbilityUpgradeData>(availableUpgrades);
        }

        public float GetUpgradeMultiplier(UpgradeType upgradeType)
        {
            foreach (var upgrade in availableUpgrades)
            {
                if (upgrade != null && upgrade.UpgradeType == upgradeType)
                {
                    int level = GetUpgradeLevel(upgrade);
                    return GetMultiplierForUpgradeType(upgradeType, level);
                }
            }

            return 1f;
        }

        private float GetMultiplierForUpgradeType(UpgradeType upgradeType, int level)
        {
            switch (upgradeType)
            {
                case UpgradeType.ThrowingStrength:
                    return 1f + (level * 0.2f);
                case UpgradeType.MovementSpeed:
                    return 1f + (level * 0.15f);
                default:
                    return 1f;
            }
        }

        public void ResetAllUpgrades()
        {
            foreach (var upgrade in availableUpgrades)
            {
                if (upgrade == null)
                    continue;

                upgradeLevels[upgrade] = 0;
                SaveUpgradeLevel(upgrade, 0);
            }

            Debug.Log("All ability upgrades have been reset to base level");

            foreach (var upgrade in availableUpgrades)
            {
                if (upgrade != null)
                {
                    OnUpgradePurchased?.Invoke(upgrade, 0);
                }
            }
        }
    }
}
