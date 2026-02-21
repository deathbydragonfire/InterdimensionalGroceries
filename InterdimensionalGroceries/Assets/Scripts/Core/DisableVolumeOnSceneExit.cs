using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace InterdimensionalGroceries.Core
{
    public class DisableVolumeOnSceneExit : MonoBehaviour
    {
        private Volume volume;

        private void Awake()
        {
            volume = GetComponent<Volume>();
        }

        private void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (volume != null)
            {
                volume.enabled = false;
            }
        }
    }
}
