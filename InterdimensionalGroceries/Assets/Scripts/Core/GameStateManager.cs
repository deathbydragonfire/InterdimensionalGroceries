using UnityEngine;
using UnityEngine.SceneManagement;
using InterdimensionalGroceries.AudioSystem;
using InterdimensionalGroceries.EconomySystem;
using InterdimensionalGroceries.PhaseManagement;

namespace InterdimensionalGroceries.Core
{
    public static class GameStateManager
    {
        public static void ResetGameForMainMenu(bool preserveSaveData = true)
        {
            Debug.Log($"[GameStateManager] Resetting game state (preserveSaveData: {preserveSaveData})");
            
            Time.timeScale = 1f;
            
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.ResetMusicManager();
            }
            
            if (!preserveSaveData && AbilityUpgradeManager.Instance != null)
            {
                AbilityUpgradeManager.Instance.ResetAllUpgrades();
            }
            
            if (GamePhaseManager.Instance != null)
            {
                Object.Destroy(GamePhaseManager.Instance.gameObject);
            }
            
            Debug.Log("[GameStateManager] Game state reset complete");
        }
        
        public static void LoadMainMenu()
        {
            Debug.Log("[GameStateManager] Loading main menu");
            ResetGameForMainMenu(true);
            SceneManager.LoadScene("Intro");
        }
    }
}
