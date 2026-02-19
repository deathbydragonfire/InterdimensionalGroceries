using UnityEngine;
using TutorialSystem;

namespace Core
{
    public class SceneFadeOnLoad : MonoBehaviour
    {
        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 1.5f;
        
        [Header("References")]
        [SerializeField] private FadeController fadeController;

        private void Start()
        {
            if (fadeController == null)
            {
                Debug.LogError("[SceneFadeOnLoad] FadeController is not assigned!");
                return;
            }

            fadeController.FadeFromBlack(fadeDuration);
        }
    }
}
