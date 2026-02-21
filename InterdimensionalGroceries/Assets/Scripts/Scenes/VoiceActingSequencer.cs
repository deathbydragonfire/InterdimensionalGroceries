using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InterdimensionalGroceries.UI;

namespace InterdimensionalGroceries.Scenes
{
    public class VoiceActingSequencer : MonoBehaviour
    {
        [Header("Voice Lines")]
        [SerializeField] private AudioClip[] voiceLines;
        
        [Header("Subtitles")]
        [SerializeField] private SubtitleData[] subtitles;
        
        [Header("Timing Settings")]
        [SerializeField] private float firstLineDelay = 0f;
        [SerializeField] private float minDelayBetweenLines = 3f;
        [SerializeField] private float maxDelayBetweenLines = 7f;
        
        [Header("Audio Settings")]
        [SerializeField] private float volume = 1f;
        [SerializeField] private bool use3DAudio = false;
        [SerializeField] private float maxDistance = 50f;
        
        [Header("Audio Effects")]
        [SerializeField] private bool enableReverb = true;
        [SerializeField] private AudioReverbPreset reverbPreset = AudioReverbPreset.Alley;
        [SerializeField] private bool enableChorus = true;
        
        private AudioSource audioSource;
        private AudioReverbFilter reverbFilter;
        private AudioChorusFilter chorusFilter;
        private bool isPlaying = false;
        private Coroutine sequenceCoroutine;

        private void Awake()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            SetupAudioEffects();
            ConfigureAudioSource();
        }

        private void SetupAudioEffects()
        {
            if (enableReverb)
            {
                reverbFilter = gameObject.GetComponent<AudioReverbFilter>();
                if (reverbFilter == null)
                {
                    reverbFilter = gameObject.AddComponent<AudioReverbFilter>();
                }
                reverbFilter.reverbPreset = reverbPreset;
                Debug.Log($"[VoiceActingSequencer] Added reverb effect with preset: {reverbPreset}");
            }
            
            if (enableChorus)
            {
                chorusFilter = gameObject.GetComponent<AudioChorusFilter>();
                if (chorusFilter == null)
                {
                    chorusFilter = gameObject.AddComponent<AudioChorusFilter>();
                }
                Debug.Log("[VoiceActingSequencer] Added chorus effect with default settings");
            }
        }

        private void ConfigureAudioSource()
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.volume = volume;
            
            if (use3DAudio)
            {
                audioSource.spatialBlend = 1f;
                audioSource.maxDistance = maxDistance;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
            }
            else
            {
                audioSource.spatialBlend = 0f;
            }
        }

        public void StartVoiceSequence()
        {
            if (isPlaying)
            {
                Debug.LogWarning("[VoiceActingSequencer] Voice sequence is already playing!");
                return;
            }

            if (voiceLines == null || voiceLines.Length == 0)
            {
                Debug.LogWarning("[VoiceActingSequencer] No voice lines assigned!");
                return;
            }

            sequenceCoroutine = StartCoroutine(PlayVoiceSequence());
        }

        public void StopVoiceSequence()
        {
            if (sequenceCoroutine != null)
            {
                StopCoroutine(sequenceCoroutine);
                sequenceCoroutine = null;
            }

            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            if (SubtitleController.Instance != null)
            {
                SubtitleController.Instance.HideSubtitle();
            }

            isPlaying = false;
            Debug.Log("[VoiceActingSequencer] Voice sequence stopped");
        }

        private IEnumerator PlayVoiceSequence()
        {
            isPlaying = true;
            Debug.Log("[VoiceActingSequencer] Starting voice sequence");

            yield return new WaitForSeconds(firstLineDelay);

            for (int i = 0; i < voiceLines.Length; i++)
            {
                if (voiceLines[i] == null)
                {
                    Debug.LogWarning($"[VoiceActingSequencer] Voice line {i} is null, skipping");
                    continue;
                }

                Debug.Log($"[VoiceActingSequencer] Playing voice line {i + 1}/{voiceLines.Length}");
                audioSource.clip = voiceLines[i];
                audioSource.Play();

                if (subtitles != null && i < subtitles.Length && subtitles[i] != null && SubtitleController.Instance != null)
                {
                    SubtitleController.Instance.ShowSubtitle(subtitles[i], voiceLines[i].length);
                }

                yield return new WaitForSeconds(voiceLines[i].length);

                if (SubtitleController.Instance != null)
                {
                    SubtitleController.Instance.HideSubtitle();
                }

                if (i < voiceLines.Length - 1)
                {
                    float delay = Random.Range(minDelayBetweenLines, maxDelayBetweenLines);
                    Debug.Log($"[VoiceActingSequencer] Waiting {delay:F2}s before next line");
                    yield return new WaitForSeconds(delay);
                }
            }

            isPlaying = false;
            Debug.Log("[VoiceActingSequencer] Voice sequence completed");
        }

        public bool IsPlaying()
        {
            return isPlaying;
        }

        private void OnValidate()
        {
            if (maxDelayBetweenLines < minDelayBetweenLines)
            {
                maxDelayBetweenLines = minDelayBetweenLines;
            }
        }
    }
}
