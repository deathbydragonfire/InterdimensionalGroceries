using UnityEngine;
using UnityEngine.SceneManagement;

namespace TutorialSystem
{
    public class TutorialSceneInitializer : MonoBehaviour
    {
        private static bool sceneJustLoaded = false;

        private void Awake()
        {
            Debug.Log("[TutorialSceneInitializer] Awake - Scene just loaded via transition");
            sceneJustLoaded = true;
        }

        private void Start()
        {
            Debug.Log("[TutorialSceneInitializer] Start - Checking initialization");
            
            if (sceneJustLoaded)
            {
                Debug.Log("[TutorialSceneInitializer] This scene was loaded from another scene");
                
                TutorialManager tutorialManager = FindFirstObjectByType<TutorialManager>();
                if (tutorialManager == null)
                {
                    Debug.LogError("[TutorialSceneInitializer] TutorialManager not found in scene!");
                }
                else
                {
                    Debug.Log($"[TutorialSceneInitializer] TutorialManager found. Enabled: {tutorialManager.enabled}, GameObject active: {tutorialManager.gameObject.activeInHierarchy}");
                    
                    if (!tutorialManager.enabled)
                    {
                        Debug.LogWarning("[TutorialSceneInitializer] TutorialManager was disabled! Re-enabling it...");
                        tutorialManager.enabled = true;
                    }
                }
            }
            else
            {
                Debug.Log("[TutorialSceneInitializer] Scene was started directly (Play mode in editor)");
            }
            
            sceneJustLoaded = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"[TutorialSceneInitializer] Scene loaded: {scene.name}, mode: {mode}");
            
            if (scene.name == "Tutorial")
            {
                Debug.Log("[TutorialSceneInitializer] Tutorial scene detected in sceneLoaded callback");
                sceneJustLoaded = true;
            }
        }
    }
}
