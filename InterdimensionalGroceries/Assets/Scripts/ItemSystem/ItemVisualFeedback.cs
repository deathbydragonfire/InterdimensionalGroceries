using UnityEngine;
using System.Collections;

namespace InterdimensionalGroceries.ItemSystem
{
    public class ItemVisualFeedback : MonoBehaviour
    {
        [Header("Materials")]
        [SerializeField] private Material acceptedGlowMaterial;
        [SerializeField] private Material rejectedFlashMaterial;
        
        [Header("Animation Settings")]
        [SerializeField] private float flashDuration = 0.5f;
        [SerializeField] private int rejectionFlashCount = 3;
        [SerializeField] private float scaleDownDuration = 1.0f;
        [SerializeField] private float greenGlowDuration = 0.2f;
        [SerializeField] private float spinSpeed = 720f;
        [SerializeField] private AnimationCurve elasticCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        private Renderer[] itemRenderers;
        private Material[][] originalMaterials;
        private Vector3 originalScale;
        private Transform itemRootTransform;
        private Coroutine currentEffect;
        private Material acceptedGlowInstance;
        private Material rejectedFlashInstance;

        private void Awake()
        {
            itemRenderers = GetComponentsInChildren<Renderer>();
            
            PickableItem pickableItem = GetComponentInParent<PickableItem>();
            if (pickableItem != null)
            {
                itemRootTransform = pickableItem.transform;
                
                while (itemRootTransform.parent != null)
                {
                    PickableItem parentPickable = itemRootTransform.parent.GetComponentInParent<PickableItem>();
                    if (parentPickable != null && parentPickable != pickableItem)
                    {
                        itemRootTransform = parentPickable.transform;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                itemRootTransform = transform;
            }
            
            originalMaterials = new Material[itemRenderers.Length][];
            for (int i = 0; i < itemRenderers.Length; i++)
            {
                originalMaterials[i] = itemRenderers[i].materials;
            }
            
            if (acceptedGlowMaterial != null)
            {
                acceptedGlowInstance = new Material(acceptedGlowMaterial);
                acceptedGlowInstance.EnableKeyword("_EMISSION");
            }
            
            if (rejectedFlashMaterial != null)
            {
                rejectedFlashInstance = new Material(rejectedFlashMaterial);
                rejectedFlashInstance.EnableKeyword("_EMISSION");
            }
        }
        
        private void Start()
        {
            if (itemRootTransform != null)
            {
                originalScale = itemRootTransform.localScale;
            }
        }

        public void PlayAcceptedEffect(System.Action onComplete = null)
        {
            if (currentEffect != null)
            {
                StopCoroutine(currentEffect);
            }
            currentEffect = StartCoroutine(AcceptedEffectCoroutine(onComplete));
        }

        public void PlayRejectedEffect(System.Action onComplete = null)
        {
            if (currentEffect != null)
            {
                StopCoroutine(currentEffect);
            }
            currentEffect = StartCoroutine(RejectedEffectCoroutine(onComplete));
        }

        private IEnumerator AcceptedEffectCoroutine(System.Action onComplete)
        {
            if (acceptedGlowInstance != null)
            {
                foreach (Renderer rend in itemRenderers)
                {
                    Material[] newMats = new Material[rend.materials.Length + 1];
                    for (int i = 0; i < rend.materials.Length; i++)
                    {
                        newMats[i] = rend.materials[i];
                    }
                    newMats[newMats.Length - 1] = acceptedGlowInstance;
                    rend.materials = newMats;
                }
            }

            yield return new WaitForSeconds(greenGlowDuration);

            float elapsed = 0f;
            Quaternion startRotation = itemRootTransform.rotation;
            
            while (elapsed < scaleDownDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / scaleDownDuration;
                float curveValue = elasticCurve.Evaluate(progress);
                
                float scale = Mathf.Lerp(1f, 0f, curveValue);
                itemRootTransform.localScale = originalScale * scale;
                
                float spinAmount = spinSpeed * elapsed;
                itemRootTransform.rotation = startRotation * Quaternion.Euler(0f, spinAmount, 0f);
                
                yield return null;
            }
            
            itemRootTransform.localScale = Vector3.zero;
            
            onComplete?.Invoke();
            currentEffect = null;
        }

        private IEnumerator RejectedEffectCoroutine(System.Action onComplete)
        {
            float singleFlashDuration = flashDuration / (rejectionFlashCount * 2);
            
            for (int i = 0; i < rejectionFlashCount; i++)
            {
                if (rejectedFlashInstance != null)
                {
                    foreach (Renderer rend in itemRenderers)
                    {
                        Material[] newMats = new Material[rend.materials.Length];
                        for (int j = 0; j < newMats.Length; j++)
                        {
                            newMats[j] = rejectedFlashInstance;
                        }
                        rend.materials = newMats;
                    }
                }

                yield return new WaitForSeconds(singleFlashDuration);

                for (int j = 0; j < itemRenderers.Length; j++)
                {
                    itemRenderers[j].materials = originalMaterials[j];
                }

                yield return new WaitForSeconds(singleFlashDuration);
            }
            
            onComplete?.Invoke();
            currentEffect = null;
        }

        private void OnDestroy()
        {
            if (currentEffect != null)
            {
                StopCoroutine(currentEffect);
            }
            
            if (acceptedGlowInstance != null)
            {
                Destroy(acceptedGlowInstance);
            }
            
            if (rejectedFlashInstance != null)
            {
                Destroy(rejectedFlashInstance);
            }
        }
    }
}
