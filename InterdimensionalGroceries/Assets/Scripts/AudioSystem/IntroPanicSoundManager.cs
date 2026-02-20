using UnityEngine;
using System.Collections;

namespace InterdimensionalGroceries.AudioSystem
{
    public class IntroPanicSoundManager : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClipData voiceActing;
        [SerializeField] private AudioClipData tensionSound;
        [SerializeField] private AudioClipData risingStingSound;
        [SerializeField] private AudioClipData portalOpeningSound;

        [Header("Timing Delays (seconds)")]
        [Tooltip("Delay before voice acting starts. Negative values start before panic sequence, positive values delay after sequence starts.")]
        [SerializeField] private float voiceActingDelay = 0f;
        
        [Tooltip("Delay before tension sound starts. Negative values start before panic sequence, positive values delay after sequence starts.")]
        [SerializeField] private float tensionDelay = -1f;
        
        [Tooltip("Delay before rising sting starts. Negative values start before panic sequence, positive values delay after sequence starts.")]
        [SerializeField] private float risingStingDelay = 0.5f;
        
        [Tooltip("Delay before portal opening sound starts. This is typically triggered by portal opening event, but can be offset.")]
        [SerializeField] private float portalOpeningDelay = 0f;

        [Header("Volume Controls")]
        [SerializeField, Range(0f, 1f)] private float voiceActingVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float tensionVolume = 0.8f;
        [SerializeField, Range(0f, 1f)] private float risingStingVolume = 0.9f;
        [SerializeField, Range(0f, 1f)] private float portalOpeningVolume = 1f;

        [Header("Audio Settings")]
        [SerializeField, Range(0f, 1f)] private float spatialBlend = 0f;
        [SerializeField] private Transform audioSourcePosition;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogging = false;

        private AudioSource voiceActingSource;
        private AudioSource tensionSource;
        private AudioSource risingStingSource;
        private AudioSource portalSource;

        private Coroutine panicSequenceCoroutine;

        private void Awake()
        {
            if (audioSourcePosition == null)
            {
                audioSourcePosition = transform;
            }

            CreateAudioSources();
        }

        private void CreateAudioSources()
        {
            voiceActingSource = CreateAudioSource("VoiceActingSource");
            tensionSource = CreateAudioSource("TensionSource");
            risingStingSource = CreateAudioSource("RisingStingSource");
            portalSource = CreateAudioSource("PortalSource");
        }

        private AudioSource CreateAudioSource(string sourceName)
        {
            GameObject sourceObject = new GameObject(sourceName);
            sourceObject.transform.SetParent(audioSourcePosition);
            sourceObject.transform.localPosition = Vector3.zero;
            
            AudioSource source = sourceObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = spatialBlend;
            
            return source;
        }

        public void StartPanicSoundSequence()
        {
            if (panicSequenceCoroutine != null)
            {
                StopCoroutine(panicSequenceCoroutine);
            }
            panicSequenceCoroutine = StartCoroutine(PanicSoundSequenceCoroutine());
        }

        public void StopPanicSoundSequence()
        {
            if (panicSequenceCoroutine != null)
            {
                StopCoroutine(panicSequenceCoroutine);
                panicSequenceCoroutine = null;
            }

            StopAllSounds();
        }

        private IEnumerator PanicSoundSequenceCoroutine()
        {
            if (enableDebugLogging)
            {
                Debug.Log("[IntroPanicSoundManager] Starting panic sound sequence");
            }

            float sequenceStartTime = Time.time;

            if (voiceActingDelay < 0f)
            {
                PlayVoiceActing();
            }

            if (tensionDelay < 0f)
            {
                PlayTension();
            }

            if (risingStingDelay < 0f)
            {
                PlayRisingSting();
            }

            yield return null;

            if (voiceActingDelay >= 0f)
            {
                StartCoroutine(PlaySoundAfterDelay(voiceActingDelay, PlayVoiceActing));
            }

            if (tensionDelay >= 0f)
            {
                StartCoroutine(PlaySoundAfterDelay(tensionDelay, PlayTension));
            }

            if (risingStingDelay >= 0f)
            {
                StartCoroutine(PlaySoundAfterDelay(risingStingDelay, PlayRisingSting));
            }
        }

        private IEnumerator PlaySoundAfterDelay(float delay, System.Action playAction)
        {
            yield return new WaitForSeconds(delay);
            playAction?.Invoke();
        }

        public void PlayVoiceActing()
        {
            PlaySound(voiceActingSource, voiceActing, voiceActingVolume, "Voice Acting");
        }

        public void PlayTension()
        {
            PlaySound(tensionSource, tensionSound, tensionVolume, "Tension");
        }

        public void PlayRisingSting()
        {
            PlaySound(risingStingSource, risingStingSound, risingStingVolume, "Rising Sting");
        }

        public void PlayPortalOpening()
        {
            if (portalOpeningDelay > 0f)
            {
                StartCoroutine(PlaySoundAfterDelay(portalOpeningDelay, () => PlaySound(portalSource, portalOpeningSound, portalOpeningVolume, "Portal Opening")));
            }
            else if (portalOpeningDelay < 0f)
            {
                if (enableDebugLogging)
                {
                    Debug.LogWarning("[IntroPanicSoundManager] Portal opening delay is negative, playing immediately instead");
                }
                PlaySound(portalSource, portalOpeningSound, portalOpeningVolume, "Portal Opening");
            }
            else
            {
                PlaySound(portalSource, portalOpeningSound, portalOpeningVolume, "Portal Opening");
            }
        }

        private void PlaySound(AudioSource source, AudioClipData clipData, float volume, string soundName)
        {
            if (source == null)
            {
                if (enableDebugLogging)
                {
                    Debug.LogWarning($"[IntroPanicSoundManager] AudioSource for {soundName} is null");
                }
                return;
            }

            if (clipData == null || clipData.Clips == null || clipData.Clips.Length == 0)
            {
                if (enableDebugLogging)
                {
                    Debug.LogWarning($"[IntroPanicSoundManager] No audio clips assigned for {soundName}");
                }
                return;
            }

            AudioClip clip = clipData.GetRandomClip();
            source.clip = clip;
            source.volume = clipData.GetRandomVolume() * volume;
            source.pitch = clipData.GetRandomPitch();
            source.outputAudioMixerGroup = clipData.MixerGroup;

            source.Play();

            if (enableDebugLogging)
            {
                Debug.Log($"[IntroPanicSoundManager] Playing {soundName}: {clip.name} | Volume: {source.volume:F2} | Pitch: {source.pitch:F2}");
            }
        }

        public void StopAllSounds()
        {
            if (voiceActingSource != null) voiceActingSource.Stop();
            if (tensionSource != null) tensionSource.Stop();
            if (risingStingSource != null) risingStingSource.Stop();
            if (portalSource != null) portalSource.Stop();

            if (enableDebugLogging)
            {
                Debug.Log("[IntroPanicSoundManager] Stopped all sounds");
            }
        }

        public void StopVoiceActing()
        {
            if (voiceActingSource != null)
            {
                voiceActingSource.Stop();
                if (enableDebugLogging)
                {
                    Debug.Log("[IntroPanicSoundManager] Stopped voice acting");
                }
            }
        }

        public void StopTension()
        {
            if (tensionSource != null)
            {
                tensionSource.Stop();
                if (enableDebugLogging)
                {
                    Debug.Log("[IntroPanicSoundManager] Stopped tension sound");
                }
            }
        }

        public void StopRisingSting()
        {
            if (risingStingSource != null)
            {
                risingStingSource.Stop();
                if (enableDebugLogging)
                {
                    Debug.Log("[IntroPanicSoundManager] Stopped rising sting");
                }
            }
        }

        private void OnDestroy()
        {
            StopAllSounds();
        }
    }
}
