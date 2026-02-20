using UnityEngine;

namespace InterdimensionalGroceries.AudioSystem
{
    public class TutorialMusicStarter : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool useCrossfade = true;
        [SerializeField] private float delayBeforeStart = 0.5f;
        
        private void Start()
        {
            if (delayBeforeStart > 0f)
            {
                Invoke(nameof(StartMusic), delayBeforeStart);
            }
            else
            {
                StartMusic();
            }
        }
        
        private void StartMusic()
        {
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.PlayTutorialMusic(useCrossfade);
                Debug.Log("[TutorialMusicStarter] Started tutorial music with crossfade");
            }
            else
            {
                Debug.LogWarning("[TutorialMusicStarter] MusicManager instance not found");
            }
        }
    }
}
