using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace InterdimensionalGroceries.UI
{
    public class GlitchLogoFlasher : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image logoImage;
        [SerializeField] private CanvasGroup logoCanvasGroup;
        
        [Header("Settings")]
        [SerializeField] private float flashDuration = 0.1f;
        [SerializeField] private int flashCount = 2;
        [SerializeField] private float timeBetweenFlashes = 0.05f;
        
        private void Awake()
        {
            if (logoImage != null)
            {
                logoImage.enabled = false;
            }
            
            if (logoCanvasGroup != null)
            {
                logoCanvasGroup.alpha = 0f;
            }
        }
        
        public void TriggerLogoFlash(float glitchDuration)
        {
            StartCoroutine(FlashLogoCoroutine(glitchDuration));
        }
        
        private IEnumerator FlashLogoCoroutine(float glitchDuration)
        {
            if (logoImage == null || logoCanvasGroup == null)
            {
                yield break;
            }
            
            float totalFlashTime = (flashDuration + timeBetweenFlashes) * flashCount;
            float delayBeforeFlash = Mathf.Max(0f, (glitchDuration - totalFlashTime) * 0.5f);
            
            yield return new WaitForSeconds(delayBeforeFlash);
            
            for (int i = 0; i < flashCount; i++)
            {
                logoImage.enabled = true;
                logoCanvasGroup.alpha = 1f;
                
                yield return new WaitForSeconds(flashDuration);
                
                logoImage.enabled = false;
                logoCanvasGroup.alpha = 0f;
                
                if (i < flashCount - 1)
                {
                    yield return new WaitForSeconds(timeBetweenFlashes);
                }
            }
        }
    }
}
