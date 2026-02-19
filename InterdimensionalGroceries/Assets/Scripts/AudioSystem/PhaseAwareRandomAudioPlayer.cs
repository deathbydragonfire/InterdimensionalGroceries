using UnityEngine;
using InterdimensionalGroceries.PhaseManagement;
using InterdimensionalGroceries.PlayerController;

namespace InterdimensionalGroceries.AudioSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class PhaseAwareRandomAudioPlayer : MonoBehaviour
    {
        [Header("Audio Configuration")]
        [Tooltip("The collection of audio clips to play")]
        public PhaseAudioClipCollection audioCollection;

        [Header("Playback Control")]
        [Tooltip("Start playing automatically when the game starts")]
        public bool playOnAwake = true;

        [Tooltip("Enable to see debug messages in the console")]
        public bool debugMode = false;

        [Header("Idle Detection")]
        [Tooltip("Enable idle clip playback when player is standing still")]
        public bool enableIdleClips = true;

        [Tooltip("Reference to player idle detector (optional - will find automatically)")]
        public PlayerIdleDetector playerIdleDetector;

        private AudioSource audioSource;
        private float nextPlayTime;
        private bool isPlaying;
        private GamePhase currentPhase;
        private bool playerIsIdle;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            
            if (audioSource == null)
            {
                Debug.LogError($"[PhaseAwareRandomAudioPlayer] No AudioSource found on {gameObject.name}!");
                enabled = false;
                return;
            }

            ConfigureAudioSource();
        }

        private void Start()
        {
            if (GamePhaseManager.Instance == null)
            {
                Debug.LogError("[PhaseAwareRandomAudioPlayer] GamePhaseManager instance not found!");
                enabled = false;
                return;
            }

            currentPhase = GamePhaseManager.Instance.CurrentPhase;

            GamePhaseManager.Instance.OnInventoryPhaseStarted += OnInventoryPhaseStarted;
            GamePhaseManager.Instance.OnDeliveryPhaseStarted += OnDeliveryPhaseStarted;

            if (enableIdleClips)
            {
                SetupIdleDetection();
            }

            if (playOnAwake)
            {
                StartPlayback();
            }
        }

        private void SetupIdleDetection()
        {
            if (playerIdleDetector == null)
            {
                playerIdleDetector = FindFirstObjectByType<PlayerIdleDetector>();
            }

            if (playerIdleDetector != null)
            {
                playerIdleDetector.OnPlayerIdle += OnPlayerIdle;
                playerIdleDetector.OnPlayerActive += OnPlayerActive;

                if (debugMode)
                    Debug.Log("[PhaseAwareRandomAudioPlayer] Idle detection enabled and connected to PlayerIdleDetector");
            }
            else
            {
                if (debugMode)
                    Debug.LogWarning("[PhaseAwareRandomAudioPlayer] PlayerIdleDetector not found - idle clips will not play");
            }
        }

        private void OnDestroy()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= OnInventoryPhaseStarted;
                GamePhaseManager.Instance.OnDeliveryPhaseStarted -= OnDeliveryPhaseStarted;
            }

            if (playerIdleDetector != null)
            {
                playerIdleDetector.OnPlayerIdle -= OnPlayerIdle;
                playerIdleDetector.OnPlayerActive -= OnPlayerActive;
            }
        }

        private void Update()
        {
            if (!isPlaying || audioCollection == null)
                return;

            if (Time.time >= nextPlayTime && !audioSource.isPlaying)
            {
                PlayRandomClip();
            }
        }

        private void ConfigureAudioSource()
        {
            if (audioCollection == null)
                return;

            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = audioCollection.spatialBlend;
            audioSource.minDistance = audioCollection.minDistance;
            audioSource.maxDistance = audioCollection.maxDistance;
            audioSource.rolloffMode = audioCollection.rolloffMode;
            audioSource.dopplerLevel = audioCollection.dopplerLevel;
            audioSource.volume = audioCollection.volume;
        }

        private void PlayRandomClip()
        {
            if (audioCollection == null)
            {
                if (debugMode)
                    Debug.LogWarning("[PhaseAwareRandomAudioPlayer] No audio collection assigned!");
                return;
            }

            AudioClip clip = null;

            if (playerIsIdle && enableIdleClips)
            {
                clip = audioCollection.GetRandomIdleClip();
                
                if (clip != null && debugMode)
                    Debug.Log("[PhaseAwareRandomAudioPlayer] Playing IDLE clip");
            }

            if (clip == null)
            {
                clip = audioCollection.GetRandomClip(currentPhase);
            }
            
            if (clip == null)
            {
                if (debugMode)
                    Debug.LogWarning($"[PhaseAwareRandomAudioPlayer] No clips available for {currentPhase}!");
                
                ScheduleNextPlay();
                return;
            }

            audioSource.clip = clip;
            audioSource.pitch = audioCollection.GetRandomPitch();
            audioSource.Play();

            if (debugMode)
                Debug.Log($"[PhaseAwareRandomAudioPlayer] Playing {clip.name} at pitch {audioSource.pitch:F2} for {currentPhase}{(playerIsIdle ? " (IDLE)" : "")}");

            ScheduleNextPlay();
        }

        private void ScheduleNextPlay()
        {
            float delay = audioCollection != null ? audioCollection.GetRandomDelay() : 10f;
            nextPlayTime = Time.time + delay;

            if (debugMode)
                Debug.Log($"[PhaseAwareRandomAudioPlayer] Next play in {delay:F1} seconds");
        }

        private void OnInventoryPhaseStarted()
        {
            currentPhase = GamePhase.InventoryPhase;
            
            if (debugMode)
                Debug.Log("[PhaseAwareRandomAudioPlayer] Switched to Inventory Phase");

            if (isPlaying)
            {
                audioSource.Stop();
                ScheduleNextPlay();
            }
        }

        private void OnDeliveryPhaseStarted()
        {
            currentPhase = GamePhase.DeliveryPhase;
            
            if (debugMode)
                Debug.Log("[PhaseAwareRandomAudioPlayer] Switched to Delivery Phase");

            if (isPlaying)
            {
                audioSource.Stop();
                ScheduleNextPlay();
            }
        }

        public void StartPlayback()
        {
            if (audioCollection == null)
            {
                Debug.LogWarning("[PhaseAwareRandomAudioPlayer] Cannot start playback - no audio collection assigned!");
                return;
            }

            isPlaying = true;
            ScheduleNextPlay();
            
            if (debugMode)
                Debug.Log("[PhaseAwareRandomAudioPlayer] Playback started");
        }

        public void StopPlayback()
        {
            isPlaying = false;
            audioSource.Stop();
            
            if (debugMode)
                Debug.Log("[PhaseAwareRandomAudioPlayer] Playback stopped");
        }

        public void PlayImmediately()
        {
            if (!isPlaying)
                isPlaying = true;
                
            PlayRandomClip();
        }

        private void OnPlayerIdle()
        {
            playerIsIdle = true;

            if (debugMode)
                Debug.Log("[PhaseAwareRandomAudioPlayer] Player is now idle - will prioritize idle clips");

            if (isPlaying && !audioSource.isPlaying)
            {
                PlayRandomClip();
            }
        }

        private void OnPlayerActive()
        {
            playerIsIdle = false;

            if (debugMode)
                Debug.Log("[PhaseAwareRandomAudioPlayer] Player is now active - back to normal clips");
        }
    }
}
