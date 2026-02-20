using UnityEngine;
using System.Collections;

namespace InterdimensionalGroceries.Scenes
{
    public class PortalController : MonoBehaviour
    {
        [SerializeField] private GameObject portalVisual;
        [SerializeField] private ParticleSystem portalParticles;
        [SerializeField] private float openDuration = 0.5f;
        [SerializeField] private float springiness = 1.2f;

        private Coroutine openCoroutine;

        private void Awake()
        {
            if (portalVisual != null)
            {
                portalVisual.SetActive(false);
                portalVisual.transform.localScale = Vector3.zero;
                
                MeshRenderer renderer = portalVisual.GetComponent<MeshRenderer>();
                if (renderer != null && renderer.sharedMaterial != null)
                {
                    renderer.sharedMaterial.EnableKeyword("_EMISSION");
                }
            }
            
            if (portalParticles != null)
            {
                portalParticles.Stop();
            }
        }

        public void OpenPortal(float duration)
        {
            if (openCoroutine != null)
            {
                StopCoroutine(openCoroutine);
            }
            openCoroutine = StartCoroutine(OpenPortalCoroutine());
        }

        private IEnumerator OpenPortalCoroutine()
        {
            if (portalVisual != null)
            {
                portalVisual.SetActive(true);
                Debug.Log("[PortalController] Portal visual activated, starting scale animation");
            }

            Vector3 targetScale = new Vector3(4f, 0.2f, 3f);
            Vector3 startScale = Vector3.zero;
            
            float elapsed = 0f;
            while (elapsed < openDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / openDuration;
                
                float spring = Mathf.Sin(t * Mathf.PI);
                float overshoot = 1f + (spring * (springiness - 1f));
                
                if (portalVisual != null)
                {
                    portalVisual.transform.localScale = Vector3.Lerp(startScale, targetScale * overshoot, t);
                }
                
                yield return null;
            }
            
            if (portalVisual != null)
            {
                portalVisual.transform.localScale = targetScale;
                Debug.Log($"[PortalController] Portal final scale: {portalVisual.transform.localScale}");
            }

            if (portalParticles != null)
            {
                portalParticles.Play();
            }

            Debug.Log("[PortalController] Portal opened");
        }

        public void ClosePortal()
        {
            if (portalVisual != null)
            {
                portalVisual.SetActive(false);
            }

            if (portalParticles != null)
            {
                portalParticles.Stop();
            }
        }
    }
}
