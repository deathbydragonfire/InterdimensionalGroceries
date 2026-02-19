using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace InterdimensionalGroceries.AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Pool Settings")]
        [SerializeField] private int poolSize = 10;
        [SerializeField] private bool prewarmPool = true;

        [Header("Audio Settings")]
        [SerializeField] private AudioClipData pickupSound;
        [SerializeField] private AudioClipData throwSound;
        [SerializeField] private AudioClipData placeSound;
        [SerializeField] private AudioClipData scanSound;
        [SerializeField] private AudioClipData acceptanceSound;
        [SerializeField] private AudioClipData rejectionSound;
        [SerializeField] private AudioClipData moneyGainedSound;
        [SerializeField] private AudioClipData buildModePlaceSound;
        [SerializeField] private AudioClipData footstepSound;
        
        [Header("UI Sounds")]
        [SerializeField] private AudioClipData uiButtonClickSound;
        [SerializeField] private AudioClipData uiPurchaseSound;
        [SerializeField] private AudioClipData uiOpenStoreSound;
        [SerializeField] private AudioClipData uiCloseStoreSound;
        
        [Header("Phase Transition Sounds")]
        [SerializeField] private AudioClipData deliveryPhaseStartSound;
        [SerializeField] private AudioClipData inventoryPhaseStartSound;
        
        [Header("Item Spawning Sounds")]
        [SerializeField] private AudioClipData itemSpawnSound;

        [Header("Surface Sounds")]
        [SerializeField] private SurfaceAudioData surfaceAudioData;

        [Header("Volume Controls")]
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        [SerializeField] private AudioMixerGroup uiMixerGroup;
        [SerializeField] private AudioMixerGroup ambientMixerGroup;
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogging = false;

        private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
        private List<AudioSource> activeAudioSources = new List<AudioSource>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (prewarmPool)
            {
                InitializePool();
            }
        }

        private void InitializePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                CreateNewAudioSource();
            }

            if (enableDebugLogging)
            {
                Debug.Log($"[AudioManager] Pool initialized with {poolSize} AudioSources");
            }
        }

        private AudioSource CreateNewAudioSource()
        {
            GameObject audioSourceObj = new GameObject($"PooledAudioSource_{audioSourcePool.Count}");
            audioSourceObj.transform.SetParent(transform);
            AudioSource audioSource = audioSourceObj.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSourcePool.Enqueue(audioSource);
            return audioSource;
        }

        private AudioSource GetAudioSource()
        {
            AudioSource source = null;

            if (audioSourcePool.Count > 0)
            {
                source = audioSourcePool.Dequeue();
            }
            else
            {
                source = CreateNewAudioSource();
                if (enableDebugLogging)
                {
                    Debug.LogWarning("[AudioManager] Pool exhausted, creating new AudioSource");
                }
            }

            activeAudioSources.Add(source);
            return source;
        }

        private void ReturnAudioSource(AudioSource source)
        {
            if (source == null) return;

            source.Stop();
            source.clip = null;
            source.loop = false;
            activeAudioSources.Remove(source);
            audioSourcePool.Enqueue(source);
        }

        public void PlaySound(AudioEventType eventType, Vector3 position)
        {
            AudioClipData clipData = GetClipDataForEvent(eventType);
            
            if (clipData != null)
            {
                PlaySound(clipData, position);
            }
            else if (enableDebugLogging)
            {
                Debug.LogWarning($"[AudioManager] No audio data assigned for event: {eventType}");
            }
        }

        public void PlayImpactSound(SurfaceType surfaceType, Vector3 position, float impactVolume = 1f)
        {
            if (surfaceAudioData == null)
            {
                if (enableDebugLogging)
                {
                    Debug.LogWarning("[AudioManager] No SurfaceAudioData assigned");
                }
                return;
            }

            AudioClipData clipData = surfaceAudioData.GetAudioForSurface(surfaceType);
            
            if (clipData != null)
            {
                PlaySound(clipData, position, impactVolume);
            }
            else if (enableDebugLogging)
            {
                Debug.LogWarning($"[AudioManager] No audio data for surface: {surfaceType}");
            }
        }

        public void PlaySound(AudioClipData clipData, Vector3 position, float volumeMultiplier = 1f)
        {
            if (clipData == null || clipData.Clips == null || clipData.Clips.Length == 0)
            {
                if (enableDebugLogging)
                {
                    Debug.LogWarning("[AudioManager] No audio clips in AudioClipData");
                }
                return;
            }

            AudioSource source = GetAudioSource();
            source.transform.position = position;

            AudioClip clip = clipData.GetRandomClip();
            source.clip = clip;
            source.volume = clipData.GetRandomVolume() * volumeMultiplier * masterVolume;
            source.pitch = clipData.GetRandomPitch();
            source.spatialBlend = clipData.SpatialBlend;
            source.maxDistance = clipData.MaxDistance;
            source.outputAudioMixerGroup = clipData.MixerGroup;

            source.Play();

            if (enableDebugLogging)
            {
                Debug.Log($"[AudioManager] Playing {clip.name} at {position} | Volume: {source.volume:F2} | Pitch: {source.pitch:F2}");
            }

            StartCoroutine(ReturnToPoolAfterPlay(source, clip.length / source.pitch));
        }

        private System.Collections.IEnumerator ReturnToPoolAfterPlay(AudioSource source, float duration)
        {
            yield return new WaitForSeconds(duration);
            ReturnAudioSource(source);
        }

        private AudioClipData GetClipDataForEvent(AudioEventType eventType)
        {
            return eventType switch
            {
                AudioEventType.Pickup => pickupSound,
                AudioEventType.Throw => throwSound,
                AudioEventType.Place => placeSound,
                AudioEventType.Scan => scanSound,
                AudioEventType.Acceptance => acceptanceSound,
                AudioEventType.Rejection => rejectionSound,
                AudioEventType.MoneyGained => moneyGainedSound,
                AudioEventType.BuildModePlace => buildModePlaceSound,
                AudioEventType.Footstep => footstepSound,
                AudioEventType.UIButtonClick => uiButtonClickSound,
                AudioEventType.UIPurchase => uiPurchaseSound,
                AudioEventType.UIOpenStore => uiOpenStoreSound,
                AudioEventType.UICloseStore => uiCloseStoreSound,
                AudioEventType.DeliveryPhaseStart => deliveryPhaseStartSound,
                AudioEventType.InventoryPhaseStart => inventoryPhaseStartSound,
                AudioEventType.ItemSpawn => itemSpawnSound,
                _ => null
            };
        }

        public SurfaceType GetSurfaceTypeFromCollision(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<SurfaceIdentifier>(out var identifier))
            {
                return identifier.SurfaceType;
            }
            return SurfaceType.Default;
        }

        private void Update()
        {
            if (enableDebugLogging && Keyboard.current != null && Keyboard.current.f10Key.wasPressedThisFrame)
            {
                Debug.Log($"[AudioManager] Active: {activeAudioSources.Count} | Pooled: {audioSourcePool.Count}");
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !enableDebugLogging) return;

            foreach (var source in activeAudioSources)
            {
                if (source != null && source.isPlaying)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(source.transform.position, 0.3f);
                    Gizmos.DrawWireSphere(source.transform.position, source.maxDistance);
                }
            }
        }
    }
}
