using UnityEngine;
using System.Collections;

namespace InterdimensionalGroceries.AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class AmbientAudioController : MonoBehaviour
    {
        [Header("Ambient Audio Configuration")]
        [Tooltip("The ambient audio data to play")]
        public AmbientAudioData ambientData;

        [Header("Playback Control")]
        [Tooltip("Start playing automatically on awake")]
        public bool playOnAwake = true;

        [Tooltip("Allow runtime volume adjustment")]
        [Range(0f, 1f)]
        public float volumeMultiplier = 1f;

        [Tooltip("Allow runtime pitch adjustment")]
        [Range(0.1f, 3f)]
        public float pitchMultiplier = 1f;

        private AudioSource audioSource;
        private Coroutine fadeCoroutine;
        private float targetVolume;
        private bool isPlaying;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            
            if (ambientData != null)
            {
                ConfigureAudioSource();
                
                if (playOnAwake)
                {
                    Play();
                }
            }
            else
            {
                Debug.LogWarning($"AmbientAudioController on {gameObject.name} has no AmbientAudioData assigned!", this);
            }
        }

        private void ConfigureAudioSource()
        {
            if (ambientData == null || audioSource == null) return;

            audioSource.clip = ambientData.clip;
            audioSource.loop = ambientData.loop;
            audioSource.spatialBlend = ambientData.spatialBlend;
            audioSource.maxDistance = ambientData.maxDistance;
            audioSource.pitch = ambientData.GetRandomizedPitch() * pitchMultiplier;
            audioSource.outputAudioMixerGroup = ambientData.mixerGroup;
            audioSource.playOnAwake = false;
            
            targetVolume = ambientData.volume * volumeMultiplier;
        }

        public void Play()
        {
            if (ambientData == null || audioSource == null)
            {
                Debug.LogWarning("Cannot play ambient audio: missing data or audio source", this);
                return;
            }

            if (isPlaying) return;

            isPlaying = true;

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            if (ambientData.fadeInDuration > 0f)
            {
                fadeCoroutine = StartCoroutine(FadeIn());
            }
            else
            {
                audioSource.volume = targetVolume;
                audioSource.Play();
            }
        }

        public void Stop()
        {
            if (!isPlaying) return;

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            if (ambientData != null && ambientData.fadeOutDuration > 0f)
            {
                fadeCoroutine = StartCoroutine(FadeOut());
            }
            else
            {
                audioSource.Stop();
                isPlaying = false;
            }
        }

        public void SetVolume(float volume)
        {
            volumeMultiplier = Mathf.Clamp01(volume);
            targetVolume = ambientData != null ? ambientData.volume * volumeMultiplier : volumeMultiplier;
            
            if (isPlaying && fadeCoroutine == null)
            {
                audioSource.volume = targetVolume;
            }
        }

        public void SetPitch(float pitch)
        {
            pitchMultiplier = Mathf.Clamp(pitch, 0.1f, 3f);
            
            if (ambientData != null)
            {
                audioSource.pitch = ambientData.GetRandomizedPitch() * pitchMultiplier;
            }
        }

        private IEnumerator FadeIn()
        {
            audioSource.volume = 0f;
            audioSource.Play();

            float elapsed = 0f;
            
            while (elapsed < ambientData.fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / ambientData.fadeInDuration;
                audioSource.volume = Mathf.Lerp(0f, targetVolume, t);
                yield return null;
            }

            audioSource.volume = targetVolume;
            fadeCoroutine = null;
        }

        private IEnumerator FadeOut()
        {
            float startVolume = audioSource.volume;
            float elapsed = 0f;

            while (elapsed < ambientData.fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / ambientData.fadeOutDuration;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }

            audioSource.volume = 0f;
            audioSource.Stop();
            isPlaying = false;
            fadeCoroutine = null;
        }

        private void OnValidate()
        {
            if (audioSource != null && isPlaying && ambientData != null)
            {
                targetVolume = ambientData.volume * volumeMultiplier;
                audioSource.volume = targetVolume;
                audioSource.pitch = ambientData.GetRandomizedPitch() * pitchMultiplier;
            }
        }

        private void OnDestroy()
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
        }
    }
}
