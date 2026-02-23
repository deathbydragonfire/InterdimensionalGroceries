using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using InterdimensionalGroceries.PhaseManagement;
using InterdimensionalGroceries.EconomySystem;

namespace InterdimensionalGroceries.Core
{
    public class SaveGameLoader : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (SaveDataManager.HasCachedSaveData())
            {
                StartCoroutine(RestoreSaveData());
            }
        }

        private IEnumerator RestoreSaveData()
        {
            yield return new WaitForSeconds(0.5f);
            
            int maxAttempts = 100;
            int attempts = 0;
            
            while (attempts < maxAttempts)
            {
                if (MoneyManager.Instance != null && 
                    AbilityUpgradeManager.Instance != null && 
                    GamePhaseManager.Instance != null)
                {
                    SaveDataManager.ApplySaveData();
                    Debug.Log("[SaveGameLoader] Save data restored successfully");
                    Destroy(gameObject);
                    yield break;
                }
                
                attempts++;
                yield return new WaitForSeconds(0.1f);
            }
            
            Debug.LogWarning("[SaveGameLoader] Failed to restore save data - managers not initialized");
            Destroy(gameObject);
        }
    }
}
