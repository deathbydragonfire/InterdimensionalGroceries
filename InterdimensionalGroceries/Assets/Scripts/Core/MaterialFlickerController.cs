using UnityEngine;

namespace InterdimensionalGroceries.Core
{
    [RequireComponent(typeof(Renderer))]
    public class MaterialFlickerController : MonoBehaviour
    {
        [Header("Flicker Settings")]
        [SerializeField] private FlickerMode flickerMode = FlickerMode.Random;
        [SerializeField] private Color minEmissionColor = new Color(2f, 1.2f, 0.4f);
        [SerializeField] private Color maxEmissionColor = new Color(4f, 2.4f, 0.8f);

        [Header("Random Mode Settings")]
        [SerializeField] private float flickerSpeed = 2f;
        [SerializeField] [Range(0f, 1f)] private float stutterChance = 0.05f;
        [SerializeField] [Range(0f, 1f)] private float stutterIntensityMultiplier = 0.7f;

        [Header("High Frequency Mode Settings")]
        [SerializeField] private float flickerFrequency = 15f;

        [Header("Audio Settings")]
        [SerializeField] private AudioClip stutterSound;
        [SerializeField] [Range(0f, 1f)] private float stutterVolume = 0.5f;

        private Renderer rendererComponent;
        private Material materialInstance;
        private AudioSource audioSource;
        private float noiseOffset;
        private float timeAccumulator;

        private static readonly int EmissionColorProperty = Shader.PropertyToID("_EmissionColor");

        private void Awake()
        {
            rendererComponent = GetComponent<Renderer>();
            materialInstance = rendererComponent.material;
            noiseOffset = Random.Range(0f, 1000f);

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null && stutterSound != null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.maxDistance = 10f;
            }
        }

        private void Update()
        {
            switch (flickerMode)
            {
                case FlickerMode.Random:
                    UpdateRandomFlicker();
                    break;
                case FlickerMode.HighFrequency:
                    UpdateHighFrequencyFlicker();
                    break;
            }
        }

        private void UpdateRandomFlicker()
        {
            float perlinValue = Mathf.PerlinNoise(Time.time * flickerSpeed + noiseOffset, 0f);
            Color targetColor = Color.Lerp(minEmissionColor, maxEmissionColor, perlinValue);

            if (Random.value < stutterChance * Time.deltaTime * 60f)
            {
                targetColor = minEmissionColor * stutterIntensityMultiplier;
                PlayStutterSound();
            }

            materialInstance.SetColor(EmissionColorProperty, targetColor);
        }

        private void UpdateHighFrequencyFlicker()
        {
            timeAccumulator += Time.deltaTime * flickerFrequency;
            
            if (timeAccumulator >= 1f)
            {
                timeAccumulator -= 1f;
                float randomValue = Random.value;
                Color targetColor = Color.Lerp(minEmissionColor, maxEmissionColor, randomValue);
                materialInstance.SetColor(EmissionColorProperty, targetColor);
            }
        }

        private void PlayStutterSound()
        {
            if (audioSource != null && stutterSound != null)
            {
                audioSource.PlayOneShot(stutterSound, stutterVolume);
            }
        }

        private void OnDestroy()
        {
            if (materialInstance != null)
            {
                Destroy(materialInstance);
            }
        }
    }
}
