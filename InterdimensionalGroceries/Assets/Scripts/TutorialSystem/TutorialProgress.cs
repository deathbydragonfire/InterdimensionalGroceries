using UnityEngine;

namespace TutorialSystem
{
    public class TutorialProgress : MonoBehaviour
    {
        private const string TUTORIAL_COMPLETE_KEY = "TutorialComplete";

        public static bool IsTutorialComplete()
        {
            return PlayerPrefs.GetInt(TUTORIAL_COMPLETE_KEY, 0) == 1;
        }

        public static void MarkTutorialComplete()
        {
            PlayerPrefs.SetInt(TUTORIAL_COMPLETE_KEY, 1);
            PlayerPrefs.Save();
        }

        public static void ResetTutorial()
        {
            PlayerPrefs.DeleteKey(TUTORIAL_COMPLETE_KEY);
            PlayerPrefs.Save();
        }

        private void Start()
        {
            TutorialManager tutorialManager = GetComponent<TutorialManager>();
            if (tutorialManager != null)
            {
                tutorialManager.OnTutorialComplete += HandleTutorialComplete;
            }
        }

        private void OnDestroy()
        {
            TutorialManager tutorialManager = GetComponent<TutorialManager>();
            if (tutorialManager != null)
            {
                tutorialManager.OnTutorialComplete -= HandleTutorialComplete;
            }
        }

        private void HandleTutorialComplete()
        {
            MarkTutorialComplete();
        }
    }
}
