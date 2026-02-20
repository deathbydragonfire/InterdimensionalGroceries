using UnityEngine;
using System.Collections;
using InterdimensionalGroceries.AudioSystem;

namespace InterdimensionalGroceries.Weather
{
    public class LightningController : MonoBehaviour
    {
        [Header("Lightning Timing")]
        [SerializeField] private float minInterval = 5f;
        [SerializeField] private float maxInterval = 15f;
        
        [Header("Flash Settings")]
        [SerializeField] private float flashDuration = 0.2f;
        [SerializeField] private int minFlashesPerEvent = 1;
        [SerializeField] private int maxFlashesPerEvent = 3;
        [SerializeField] private float timeBetweenFlashes = 0.1f;
        [SerializeField] private float maxFlashIntensity = 3f;
        
        [Header("Thunder Settings")]
        [SerializeField] private float minThunderDelay = 0.5f;
        [SerializeField] private float maxThunderDelay = 2f;
        
        [Header("References")]
        [SerializeField] private Light lightningLight;
        [SerializeField] private AudioClipData thunderSound;
        
        private float nextLightningTime;
        
        private void Start()
        {
            if (lightningLight == null)
            {
                lightningLight = GetComponent<Light>();
            }
            
            if (lightningLight != null)
            {
                lightningLight.intensity = 0f;
            }
            
            ScheduleNextLightning();
        }
        
        private void Update()
        {
            if (Time.time >= nextLightningTime)
            {
                StartCoroutine(LightningSequence());
                ScheduleNextLightning();
            }
        }
        
        private void ScheduleNextLightning()
        {
            nextLightningTime = Time.time + Random.Range(minInterval, maxInterval);
        }
        
        private IEnumerator LightningSequence()
        {
            int flashCount = Random.Range(minFlashesPerEvent, maxFlashesPerEvent + 1);
            
            for (int i = 0; i < flashCount; i++)
            {
                yield return StartCoroutine(Flash());
                
                if (i < flashCount - 1)
                {
                    yield return new WaitForSeconds(timeBetweenFlashes);
                }
            }
            
            float thunderDelay = Random.Range(minThunderDelay, maxThunderDelay);
            yield return new WaitForSeconds(thunderDelay);
            
            PlayThunder();
        }
        
        private IEnumerator Flash()
        {
            if (lightningLight == null) yield break;
            
            float elapsed = 0f;
            float halfDuration = flashDuration * 0.5f;
            
            while (elapsed < flashDuration)
            {
                elapsed += Time.deltaTime;
                
                float progress = elapsed / flashDuration;
                float intensity;
                
                if (progress < 0.5f)
                {
                    intensity = Mathf.Lerp(0f, maxFlashIntensity, progress * 2f);
                }
                else
                {
                    intensity = Mathf.Lerp(maxFlashIntensity, 0f, (progress - 0.5f) * 2f);
                }
                
                lightningLight.intensity = intensity;
                
                yield return null;
            }
            
            lightningLight.intensity = 0f;
        }
        
        private void PlayThunder()
        {
            if (thunderSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(thunderSound, transform.position);
            }
        }
    }
}
