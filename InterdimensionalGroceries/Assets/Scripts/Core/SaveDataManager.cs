using UnityEngine;
using UnityEngine.SceneManagement;

namespace InterdimensionalGroceries.Core
{
    public static class SaveDataManager
    {
        private const string SAVE_DATA_KEY = "HasSaveData";
        private const string LAST_SCENE_KEY = "LastScene";

        public static bool HasSaveData()
        {
            return PlayerPrefs.GetInt(SAVE_DATA_KEY, 0) == 1;
        }

        public static void MarkSaveDataExists(string sceneName)
        {
            PlayerPrefs.SetInt(SAVE_DATA_KEY, 1);
            PlayerPrefs.SetString(LAST_SCENE_KEY, sceneName);
            PlayerPrefs.Save();
        }

        public static void ClearSaveData()
        {
            PlayerPrefs.DeleteKey(SAVE_DATA_KEY);
            PlayerPrefs.DeleteKey(LAST_SCENE_KEY);
            PlayerPrefs.Save();
        }

        public static void LoadGame()
        {
            if (HasSaveData())
            {
                string lastScene = PlayerPrefs.GetString(LAST_SCENE_KEY, "Tutorial");
                SceneManager.LoadScene(lastScene);
            }
            else
            {
                Debug.LogWarning("[SaveDataManager] No save data found to load");
            }
        }
    }
}
