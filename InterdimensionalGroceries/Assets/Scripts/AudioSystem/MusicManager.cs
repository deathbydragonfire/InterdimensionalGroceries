using UnityEngine;
using System.Collections;
using InterdimensionalGroceries.PhaseManagement;

namespace InterdimensionalGroceries.AudioSystem
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }
        
        [Header("Music Tracks")]
        [SerializeField] private AudioClipData menuIntroMusic;
        [SerializeField] private AudioClipData tutorialMusic;
        [SerializeField] private AudioClipData deliveryPhaseMusic;
        [SerializeField] private AudioClipData inventoryPhaseMusic;
        
        [Header("Crossfade Settings")]
        [SerializeField] private float crossfadeDuration = 2f;
        [SerializeField, Range(0f, 1f)] private float masterMusicVolume = 1f;
        
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSourceA;
        [SerializeField] private AudioSource musicSourceB;
        
        private AudioSource currentSource;
        private AudioSource fadeOutSource;
        private Coroutine crossfadeCoroutine;
        private bool isPlayingMenuMusic = false;
        
        private void Awake()
        {
            // Traditional singleton - keep the FIRST instance, destroy duplicates
            if (Instance != null && Instance != this)
            {
                Debug.Log($"[MusicManager] Duplicate found in {gameObject.scene.name}. Destroying NEW duplicate and keeping existing instance.");
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"[MusicManager] Initialized as singleton in scene: {gameObject.scene.name}");
            
            SetupAudioSources();
        }
        
        private void SetupAudioSources()
        {
            if (musicSourceA == null)
            {
                musicSourceA = gameObject.AddComponent<AudioSource>();
            }
            
            if (musicSourceB == null)
            {
                musicSourceB = gameObject.AddComponent<AudioSource>();
            }
            
            musicSourceA.loop = true;
            musicSourceA.playOnAwake = false;
            musicSourceA.volume = 0f;
            
            musicSourceB.loop = true;
            musicSourceB.playOnAwake = false;
            musicSourceB.volume = 0f;
            
            currentSource = musicSourceA;
        }
        
        private void Start()
        {
            Debug.Log("[MusicManager] Start called");
            if (GamePhaseManager.Instance != null)
            {
                Debug.Log($"[MusicManager] Subscribing to GamePhaseManager events. Current phase: {GamePhaseManager.Instance.CurrentPhase}");
                GamePhaseManager.Instance.OnDeliveryPhaseStarted += OnDeliveryPhaseStarted;
                GamePhaseManager.Instance.OnInventoryPhaseStarted += OnInventoryPhaseStarted;
                
                if (!isPlayingMenuMusic)
                {
                    Debug.Log($"[MusicManager] Playing music for current phase: {GamePhaseManager.Instance.CurrentPhase}");
                    PlayMusicForPhase(GamePhaseManager.Instance.CurrentPhase, false);
                }
            }
            else
            {
                Debug.LogWarning("[MusicManager] GamePhaseManager.Instance is NULL in Start!");
            }
        }
        
        private void OnDestroy()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted -= OnDeliveryPhaseStarted;
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= OnInventoryPhaseStarted;
            }
        }
        
        private void OnDeliveryPhaseStarted()
        {
            PlayMusicForPhase(GamePhase.DeliveryPhase, true);
        }
        
        private void OnInventoryPhaseStarted()
        {
            PlayMusicForPhase(GamePhase.InventoryPhase, true);
        }
        
        private void PlayMusicForPhase(GamePhase phase, bool useCrossfade)
        {
            AudioClipData targetClipData = phase == GamePhase.DeliveryPhase ? deliveryPhaseMusic : inventoryPhaseMusic;
            
            if (targetClipData == null || targetClipData.Clips == null || targetClipData.Clips.Length == 0)
            {
                Debug.LogWarning($"MusicManager: No music assigned for {phase}");
                return;
            }
            
            AudioClip targetClip = targetClipData.GetRandomClip();
            
            if (currentSource.clip == targetClip && currentSource.isPlaying)
            {
                return;
            }
            
            if (useCrossfade)
            {
                CrossfadeToClip(targetClip, targetClipData);
            }
            else
            {
                PlayClipImmediate(targetClip, targetClipData);
            }
        }
        
        private void PlayClipImmediate(AudioClip clip, AudioClipData clipData)
        {
            if (crossfadeCoroutine != null)
            {
                StopCoroutine(crossfadeCoroutine);
                crossfadeCoroutine = null;
            }
            
            musicSourceA.Stop();
            musicSourceB.Stop();
            
            ApplyAudioClipDataSettings(currentSource, clipData);
            currentSource.clip = clip;
            currentSource.volume = clipData.GetRandomVolume() * masterMusicVolume;
            currentSource.Play();
        }
        
        private void CrossfadeToClip(AudioClip clip, AudioClipData clipData)
        {
            if (crossfadeCoroutine != null)
            {
                StopCoroutine(crossfadeCoroutine);
            }
            
            crossfadeCoroutine = StartCoroutine(CrossfadeCoroutine(clip, clipData));
        }
        
        private IEnumerator CrossfadeCoroutine(AudioClip newClip, AudioClipData clipData)
        {
            fadeOutSource = currentSource;
            currentSource = (currentSource == musicSourceA) ? musicSourceB : musicSourceA;
            
            ApplyAudioClipDataSettings(currentSource, clipData);
            currentSource.clip = newClip;
            currentSource.volume = 0f;
            currentSource.Play();
            
            float elapsed = 0f;
            float startVolume = fadeOutSource.volume;
            float targetVolume = clipData.GetRandomVolume() * masterMusicVolume;
            
            while (elapsed < crossfadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / crossfadeDuration;
                
                fadeOutSource.volume = Mathf.Lerp(startVolume, 0f, t);
                currentSource.volume = Mathf.Lerp(0f, targetVolume, t);
                
                yield return null;
            }
            
            fadeOutSource.volume = 0f;
            fadeOutSource.Stop();
            currentSource.volume = targetVolume;
            
            crossfadeCoroutine = null;
        }
        
        private void ApplyAudioClipDataSettings(AudioSource source, AudioClipData clipData)
        {
            source.pitch = clipData.GetRandomPitch();
            source.spatialBlend = clipData.SpatialBlend;
            source.outputAudioMixerGroup = clipData.MixerGroup;
        }
        
        public void SetMasterMusicVolume(float volume)
        {
            masterMusicVolume = Mathf.Clamp01(volume);
            
            if (currentSource != null && currentSource.isPlaying)
            {
                AudioClipData currentClipData = GetCurrentPhaseMusic();
                if (currentClipData != null)
                {
                    currentSource.volume = currentClipData.GetRandomVolume() * masterMusicVolume;
                }
            }
        }
        
        private AudioClipData GetCurrentPhaseMusic()
        {
            if (GamePhaseManager.Instance != null)
            {
                return GamePhaseManager.Instance.CurrentPhase == GamePhase.DeliveryPhase 
                    ? deliveryPhaseMusic 
                    : inventoryPhaseMusic;
            }
            return null;
        }
        
        public void StopMusic(bool useFade = true)
        {
            if (useFade)
            {
                StartCoroutine(FadeOutAndStop());
            }
            else
            {
                musicSourceA.Stop();
                musicSourceB.Stop();
            }
        }
        
        public void FadeOutMusic(float duration = -1f)
        {
            float fadeDuration = duration > 0f ? duration : crossfadeDuration;
            StartCoroutine(FadeOutAndStopWithDuration(fadeDuration));
        }
        
        private IEnumerator FadeOutAndStop()
        {
            yield return FadeOutAndStopWithDuration(crossfadeDuration);
        }
        
        private IEnumerator FadeOutAndStopWithDuration(float duration)
        {
            float elapsed = 0f;
            float startVolume = currentSource.volume;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                currentSource.volume = Mathf.Lerp(startVolume, 0f, t);
                
                yield return null;
            }
            
            currentSource.Stop();
            currentSource.volume = 0f;
            isPlayingMenuMusic = false;
        }
        
        public void PlayMenuIntroMusic(bool useCrossfade = false)
        {
            if (menuIntroMusic == null || menuIntroMusic.Clips == null || menuIntroMusic.Clips.Length == 0)
            {
                Debug.LogWarning("MusicManager: No menu/intro music assigned");
                return;
            }
            
            AudioClip targetClip = menuIntroMusic.GetRandomClip();
            
            if (currentSource.clip == targetClip && currentSource.isPlaying)
            {
                return;
            }
            
            isPlayingMenuMusic = true;
            
            if (useCrossfade)
            {
                CrossfadeToClip(targetClip, menuIntroMusic);
            }
            else
            {
                PlayClipImmediate(targetClip, menuIntroMusic);
            }
        }
        
        public void PlayTutorialMusic(bool useCrossfade = true)
        {
            if (tutorialMusic == null || tutorialMusic.Clips == null || tutorialMusic.Clips.Length == 0)
            {
                Debug.LogWarning("MusicManager: No tutorial music assigned");
                return;
            }
            
            AudioClip targetClip = tutorialMusic.GetRandomClip();
            
            if (currentSource.clip == targetClip && currentSource.isPlaying)
            {
                return;
            }
            
            isPlayingMenuMusic = false;
            
            if (useCrossfade)
            {
                CrossfadeToClip(targetClip, tutorialMusic);
            }
            else
            {
                PlayClipImmediate(targetClip, tutorialMusic);
            }
        }
        
        public void StopMenuMusic()
        {
            isPlayingMenuMusic = false;
        }
        
        public bool IsPlayingMusic()
        {
            return currentSource != null && currentSource.isPlaying;
        }
        
        public void ResetMusicManager()
        {
            Debug.Log("[MusicManager] Resetting music manager");
            
            if (crossfadeCoroutine != null)
            {
                StopCoroutine(crossfadeCoroutine);
                crossfadeCoroutine = null;
            }
            
            StopAllCoroutines();
            
            musicSourceA.Stop();
            musicSourceB.Stop();
            musicSourceA.volume = 0f;
            musicSourceB.volume = 0f;
            musicSourceA.clip = null;
            musicSourceB.clip = null;
            
            currentSource = musicSourceA;
            fadeOutSource = null;
            isPlayingMenuMusic = false;
            
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnDeliveryPhaseStarted -= OnDeliveryPhaseStarted;
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= OnInventoryPhaseStarted;
            }
            
            Debug.Log("[MusicManager] Reset complete - all music stopped, sources cleared");
        }
    }
}
