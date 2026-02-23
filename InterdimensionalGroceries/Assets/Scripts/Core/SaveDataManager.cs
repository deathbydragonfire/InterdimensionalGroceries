using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using InterdimensionalGroceries.PhaseManagement;
using InterdimensionalGroceries.EconomySystem;

namespace InterdimensionalGroceries.Core
{
    public static class SaveDataManager
    {
        private const string SAVE_FILE_NAME = "savegame.json";
        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        
        private static GameSaveData cachedSaveData = null;

        public static bool HasSaveData()
        {
            return File.Exists(SaveFilePath);
        }

        public static void SaveGame()
        {
            try
            {
                GameSaveData saveData = new GameSaveData();
                
                if (MoneyManager.Instance != null)
                {
                    saveData.currentMoney = MoneyManager.Instance.GetCurrentMoney();
                }
                
                saveData.currentScene = SceneManager.GetActiveScene().name;
                
                if (GamePhaseManager.Instance != null)
                {
                    saveData.currentPhase = GamePhaseManager.Instance.CurrentPhase.ToString();
                }
                else
                {
                    saveData.currentPhase = GamePhase.InventoryPhase.ToString();
                }
                
                if (AbilityUpgradeManager.Instance != null)
                {
                    var upgradeLevels = AbilityUpgradeManager.Instance.GetAllUpgradeLevels();
                    foreach (var kvp in upgradeLevels)
                    {
                        saveData.upgradeLevels.Add(new UpgradeLevel(kvp.Key, kvp.Value));
                    }
                }
                
                string json = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(SaveFilePath, json);
                
                Debug.Log($"[SaveDataManager] Game saved successfully to {SaveFilePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveDataManager] Failed to save game: {e.Message}");
            }
        }

        public static void LoadGame()
        {
            if (!HasSaveData())
            {
                Debug.LogWarning("[SaveDataManager] No save data found to load");
                return;
            }

            try
            {
                string json = File.ReadAllText(SaveFilePath);
                cachedSaveData = JsonUtility.FromJson<GameSaveData>(json);
                
                if (cachedSaveData != null && !string.IsNullOrEmpty(cachedSaveData.currentScene))
                {
                    Debug.Log($"[SaveDataManager] Loading scene: {cachedSaveData.currentScene}");
                    SceneManager.LoadScene(cachedSaveData.currentScene);
                }
                else
                {
                    Debug.LogWarning("[SaveDataManager] Invalid save data");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveDataManager] Failed to load game: {e.Message}");
            }
        }

        public static void ApplySaveData()
        {
            if (cachedSaveData == null)
            {
                Debug.LogWarning("[SaveDataManager] No cached save data to apply");
                return;
            }

            try
            {
                if (MoneyManager.Instance != null)
                {
                    MoneyManager.Instance.SetMoney(cachedSaveData.currentMoney);
                    Debug.Log($"[SaveDataManager] Restored money: {cachedSaveData.currentMoney}");
                }
                
                if (AbilityUpgradeManager.Instance != null && cachedSaveData.upgradeLevels != null)
                {
                    foreach (var upgrade in cachedSaveData.upgradeLevels)
                    {
                        AbilityUpgradeManager.Instance.SetUpgradeLevelByName(upgrade.upgradeName, upgrade.level);
                    }
                    Debug.Log($"[SaveDataManager] Restored {cachedSaveData.upgradeLevels.Count} upgrade levels");
                }
                
                if (GamePhaseManager.Instance != null && !string.IsNullOrEmpty(cachedSaveData.currentPhase))
                {
                    if (System.Enum.TryParse<GamePhase>(cachedSaveData.currentPhase, out GamePhase phase))
                    {
                        GamePhaseManager.Instance.SetPhase(phase);
                        Debug.Log($"[SaveDataManager] Restored phase: {phase}");
                    }
                }
                
                cachedSaveData = null;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveDataManager] Failed to apply save data: {e.Message}");
            }
        }

        public static void ClearSaveData()
        {
            try
            {
                if (File.Exists(SaveFilePath))
                {
                    File.Delete(SaveFilePath);
                    Debug.Log("[SaveDataManager] Save data cleared");
                }
                
                cachedSaveData = null;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveDataManager] Failed to clear save data: {e.Message}");
            }
        }

        public static bool HasCachedSaveData()
        {
            return cachedSaveData != null;
        }
    }
}
