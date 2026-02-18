using UnityEngine;

namespace InterdimensionalGroceries.Core
{
    public enum FlickerMode
    {
        Random,
        HighFrequency
    }

    [RequireComponent(typeof(Light))]
    public class LightFlickerController : MonoBehaviour
    {
        [Header("Flicker Settings")]
        [SerializeField] private FlickerMode flickerMode = FlickerMode.Random;
        [SerializeField] private float minIntensity = 0.8f;
        [SerializeField] private float maxIntensity = 1.2f;

        [Header("Random Mode Settings")]
        [SerializeField] private float flickerSpeed = 2f;
        [SerializeField] [Range(0f, 1f)] private float stutterChance = 0.05f;

        [Header("High Frequency Mode Settings")]
        [SerializeField] private float flickerFrequency = 15f;

        private Light lightComponent;
        private float baseIntensity;
        private float noiseOffset;
        private float timeAccumulator;

        private void Awake()
        {
            lightComponent = GetComponent<Light>();
            baseIntensity = lightComponent.intensity;
            noiseOffset = Random.Range(0f, 1000f);
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
            float targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, perlinValue);

            if (Random.value < stutterChance * Time.deltaTime * 60f)
            {
                targetIntensity = minIntensity * 0.5f;
            }

            lightComponent.intensity = targetIntensity;
        }

        private void UpdateHighFrequencyFlicker()
        {
            timeAccumulator += Time.deltaTime * flickerFrequency;
            
            if (timeAccumulator >= 1f)
            {
                timeAccumulator -= 1f;
                float randomValue = Random.value;
                lightComponent.intensity = Mathf.Lerp(minIntensity, maxIntensity, randomValue);
            }
        }
    }
}
