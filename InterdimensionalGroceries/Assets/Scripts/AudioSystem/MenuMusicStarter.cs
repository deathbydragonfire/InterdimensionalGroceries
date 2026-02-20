using UnityEngine;

namespace InterdimensionalGroceries.AudioSystem
{
    public class MenuMusicStarter : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool playOnAwake = true;
        [SerializeField] private bool useCrossfade = false;
        
        private void Start()
        {
            if (playOnAwake && MusicManager.Instance != null)
            {
                if (!MusicManager.Instance.IsPlayingMusic())
                {
                    MusicManager.Instance.PlayMenuIntroMusic(useCrossfade);
                    Debug.Log("[MenuMusicStarter] Started menu/intro music");
                }
                else
                {
                    Debug.Log("[MenuMusicStarter] Music already playing, keeping current track");
                }
            }
        }
    }
}
