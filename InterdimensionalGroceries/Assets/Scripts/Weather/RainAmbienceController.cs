using UnityEngine;
using InterdimensionalGroceries.AudioSystem;

namespace InterdimensionalGroceries.Weather
{
    public class RainAmbienceController : MonoBehaviour
    {
        [Header("Rain Ambience")]
        [SerializeField] private AudioClipData rainAmbienceSound;
        
        private AudioSource ambienceSource;
        
        private void Start()
        {
            PlayRainAmbience();
        }
        
        private void PlayRainAmbience()
        {
            if (rainAmbienceSound == null || rainAmbienceSound.Clips == null || rainAmbienceSound.Clips.Length == 0)
            {
                Debug.LogWarning("[RainAmbienceController] No rain ambience audio clips assigned");
                return;
            }
            
            GameObject ambienceObj = new GameObject("RainAmbienceSource");
            ambienceObj.transform.SetParent(transform);
            ambienceSource = ambienceObj.AddComponent<AudioSource>();
            
            ambienceSource.clip = rainAmbienceSound.GetRandomClip();
            ambienceSource.volume = rainAmbienceSound.GetRandomVolume();
            ambienceSource.pitch = rainAmbienceSound.GetRandomPitch();
            ambienceSource.spatialBlend = rainAmbienceSound.SpatialBlend;
            ambienceSource.loop = true;
            ambienceSource.outputAudioMixerGroup = rainAmbienceSound.MixerGroup;
            ambienceSource.Play();
        }
        
        private void OnDestroy()
        {
            if (ambienceSource != null && ambienceSource.isPlaying)
            {
                ambienceSource.Stop();
            }
        }
    }
}
