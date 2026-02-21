using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using InterdimensionalGroceries.PhaseManagement;

namespace InterdimensionalGroceries.AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(AudioChorusFilter))]
    [RequireComponent(typeof(AudioReverbFilter))]
    public class InventoryPhaseAudioPlayer : MonoBehaviour
    {
        [Header("Audio Clips")]
        [Tooltip("Array of audio clips to play. First clip always plays on the first inventory phase.")]
        public AudioClip[] audioClips;

        [Header("Timing")]
        [Tooltip("Delay in seconds before playing audio after inventory phase starts")]
        public float delayAfterPhaseStart = 5f;

        [Header("Audio Settings")]
        [Range(0f, 1f)]
        public float volume = 1f;

        [Header("Debug")]
        public bool debugMode = false;

        private AudioSource audioSource;
        private AudioChorusFilter chorusFilter;
        private AudioReverbFilter reverbFilter;
        private List<int> availableIndices = new List<int>();
        private int lastPlayedIndex = -1;
        private bool isFirstInventoryPhase = true;
        private Coroutine playbackCoroutine;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            chorusFilter = GetComponent<AudioChorusFilter>();
            reverbFilter = GetComponent<AudioReverbFilter>();

            ConfigureAudioSource();
            ConfigureEffects();
        }

        private void Start()
        {
            if (GamePhaseManager.Instance == null)
            {
                Debug.LogError("[InventoryPhaseAudioPlayer] GamePhaseManager instance not found!");
                enabled = false;
                return;
            }

            GamePhaseManager.Instance.OnInventoryPhaseStarted += OnInventoryPhaseStarted;
        }

        private void OnDestroy()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= OnInventoryPhaseStarted;
            }

            if (playbackCoroutine != null)
            {
                StopCoroutine(playbackCoroutine);
            }
        }

        private void ConfigureAudioSource()
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = volume;
        }

        private void ConfigureEffects()
        {
            chorusFilter.enabled = true;
            chorusFilter.dryMix = 0.5f;
            chorusFilter.wetMix1 = 0.5f;
            chorusFilter.wetMix2 = 0.5f;
            chorusFilter.wetMix3 = 0.5f;
            chorusFilter.delay = 40f;
            chorusFilter.rate = 0.8f;
            chorusFilter.depth = 0.03f;

            reverbFilter.enabled = true;
            reverbFilter.reverbPreset = AudioReverbPreset.Alley;
        }

        private void OnInventoryPhaseStarted()
        {
            if (audioClips == null || audioClips.Length == 0)
            {
                if (debugMode)
                    Debug.LogWarning("[InventoryPhaseAudioPlayer] No audio clips assigned!");
                return;
            }

            if (playbackCoroutine != null)
            {
                StopCoroutine(playbackCoroutine);
            }

            playbackCoroutine = StartCoroutine(PlayDelayedAudio());
        }

        private IEnumerator PlayDelayedAudio()
        {
            yield return new WaitForSeconds(delayAfterPhaseStart);

            AudioClip clipToPlay = GetNextClip();

            if (clipToPlay != null)
            {
                audioSource.clip = clipToPlay;
                audioSource.Play();

                if (debugMode)
                    Debug.Log($"[InventoryPhaseAudioPlayer] Playing {clipToPlay.name} (First phase: {isFirstInventoryPhase})");
            }
            else
            {
                if (debugMode)
                    Debug.LogWarning("[InventoryPhaseAudioPlayer] No clip selected to play!");
            }
        }

        private AudioClip GetNextClip()
        {
            if (audioClips == null || audioClips.Length == 0)
                return null;

            if (audioClips.Length == 1)
                return audioClips[0];

            if (isFirstInventoryPhase)
            {
                isFirstInventoryPhase = false;
                lastPlayedIndex = 0;
                return audioClips[0];
            }

            if (availableIndices.Count == 0)
            {
                for (int i = 0; i < audioClips.Length; i++)
                {
                    if (i != lastPlayedIndex)
                    {
                        availableIndices.Add(i);
                    }
                }
            }

            if (availableIndices.Count == 0)
            {
                if (debugMode)
                    Debug.LogWarning("[InventoryPhaseAudioPlayer] No available clips to play!");
                return null;
            }

            int randomIndex = Random.Range(0, availableIndices.Count);
            int selectedIndex = availableIndices[randomIndex];
            availableIndices.RemoveAt(randomIndex);
            
            lastPlayedIndex = selectedIndex;

            return audioClips[selectedIndex];
        }

        public void ResetFirstPhaseFlag()
        {
            isFirstInventoryPhase = true;
            lastPlayedIndex = -1;
            availableIndices.Clear();
            
            if (debugMode)
                Debug.Log("[InventoryPhaseAudioPlayer] First phase flag reset");
        }
    }
}
