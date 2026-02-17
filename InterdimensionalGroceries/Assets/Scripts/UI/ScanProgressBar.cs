using UnityEngine;
using System.Collections;

public class ScanProgressBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer[] targetRenderers;
    
    [Header("Animation Settings")]
    [SerializeField] private Color scanColor = Color.cyan;
    [SerializeField] private float maxEmissionIntensity = 15f;
    [SerializeField] private int pulseCount = 3;
    
    private Material[] scanMaterials;
    private Material[][] originalMaterials;
    private Coroutine scanCoroutine;

    public void SetTarget(GameObject target)
    {
        if (target != null)
        {
            targetRenderers = target.GetComponentsInChildren<Renderer>();
        }
    }

    public void StartScanning(float duration)
    {
        if (scanCoroutine != null)
        {
            StopCoroutine(scanCoroutine);
        }
        
        if (targetRenderers == null || targetRenderers.Length == 0)
        {
            return;
        }
        
        CreateScanMaterials();
        ApplyScanMaterials();
        scanCoroutine = StartCoroutine(ScanAnimationCoroutine(duration));
    }

    private void ApplyScanMaterials()
    {
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            if (scanMaterials[i] != null)
            {
                targetRenderers[i].material = scanMaterials[i];
            }
        }
    }

    public void StopScanning()
    {
        if (scanCoroutine != null)
        {
            StopCoroutine(scanCoroutine);
            scanCoroutine = null;
        }
        
        RestoreOriginalMaterials();
    }

    private void CreateScanMaterials()
    {
        scanMaterials = new Material[targetRenderers.Length];
        originalMaterials = new Material[targetRenderers.Length][];
        
        for (int i = 0; i < targetRenderers.Length; i++)
        {
            Material[] mats = targetRenderers[i].materials;
            originalMaterials[i] = mats;
            
            scanMaterials[i] = new Material(mats[0]);
            scanMaterials[i].EnableKeyword("_EMISSION");
            scanMaterials[i].SetColor("_EmissionColor", Color.black);
        }
    }

    private IEnumerator ScanAnimationCoroutine(float duration)
    {
        float pulseDuration = duration / pulseCount;
        
        for (int pulse = 0; pulse < pulseCount; pulse++)
        {
            float elapsed = 0f;
            
            while (elapsed < pulseDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / pulseDuration;
                
                float intensity = Mathf.Sin(progress * Mathf.PI) * maxEmissionIntensity;
                Color emissionColor = scanColor * intensity;
                
                for (int i = 0; i < targetRenderers.Length; i++)
                {
                    if (scanMaterials[i] != null)
                    {
                        scanMaterials[i].SetColor("_EmissionColor", emissionColor);
                        targetRenderers[i].material = scanMaterials[i];
                    }
                }
                
                yield return null;
            }
            
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                if (scanMaterials[i] != null)
                {
                    scanMaterials[i].SetColor("_EmissionColor", Color.black);
                    targetRenderers[i].material = scanMaterials[i];
                }
            }
        }
        
        RestoreOriginalMaterials();
        scanCoroutine = null;
    }

    private void RestoreOriginalMaterials()
    {
        if (originalMaterials != null && targetRenderers != null)
        {
            for (int i = 0; i < targetRenderers.Length && i < originalMaterials.Length; i++)
            {
                if (targetRenderers[i] != null && originalMaterials[i] != null)
                {
                    targetRenderers[i].materials = originalMaterials[i];
                }
            }
            originalMaterials = null;
        }
        
        if (scanMaterials != null)
        {
            for (int i = 0; i < scanMaterials.Length; i++)
            {
                if (scanMaterials[i] != null)
                {
                    Destroy(scanMaterials[i]);
                }
            }
            scanMaterials = null;
        }
    }

    private void OnDestroy()
    {
        RestoreOriginalMaterials();
    }
}
