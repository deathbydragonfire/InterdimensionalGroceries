using UnityEngine;
using InterdimensionalGroceries.AudioSystem;

namespace InterdimensionalGroceries.ItemSystem
{
    [RequireComponent(typeof(PickableItem))]
    [RequireComponent(typeof(Rigidbody))]
    public class ItemImpactAudio : MonoBehaviour
    {
        [Header("Impact Settings")]
        [SerializeField] private float minimumVelocityThreshold = 2f;
        [SerializeField] private float impactCooldown = 0.2f;
        [SerializeField] private float volumeMultiplier = 1f;

        [Header("Volume Calculation")]
        [SerializeField] private float minImpactForce = 1f;
        [SerializeField] private float maxImpactForce = 20f;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogging = false;

        private PickableItem pickableItem;
        private float lastImpactTime;

        private void Awake()
        {
            pickableItem = GetComponent<PickableItem>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null && rb.isKinematic)
            {
                if (enableDebugLogging)
                    Debug.Log($"[ItemImpactAudio] Collision ignored - Rigidbody is kinematic on {gameObject.name}");
                return;
            }

            if (Time.time - lastImpactTime < impactCooldown)
            {
                if (enableDebugLogging)
                    Debug.Log($"[ItemImpactAudio] Collision ignored - Cooldown active on {gameObject.name}");
                return;
            }

            float impactVelocity = collision.relativeVelocity.magnitude;

            if (impactVelocity < minimumVelocityThreshold)
            {
                if (enableDebugLogging)
                    Debug.Log($"[ItemImpactAudio] Collision ignored - Velocity {impactVelocity:F2} below threshold {minimumVelocityThreshold} on {gameObject.name}");
                return;
            }

            lastImpactTime = Time.time;

            float impactForce = collision.impulse.magnitude;
            float volumeScale = Mathf.InverseLerp(minImpactForce, maxImpactForce, impactForce);
            volumeScale = Mathf.Clamp01(volumeScale) * volumeMultiplier;

            if (enableDebugLogging)
                Debug.Log($"[ItemImpactAudio] Playing impact sound - Velocity: {impactVelocity:F2}, Force: {impactForce:F2}, Volume: {volumeScale:F2} on {gameObject.name}");

            PlayImpactSound(collision, volumeScale);
        }

        private void PlayImpactSound(Collision collision, float volume)
        {
            if (AudioManager.Instance == null)
            {
                if (enableDebugLogging)
                    Debug.LogWarning($"[ItemImpactAudio] AudioManager.Instance is null on {gameObject.name}");
                return;
            }

            ItemData itemData = pickableItem.GetItemData();
            Vector3 impactPosition = collision.contacts.Length > 0 
                ? collision.contacts[0].point 
                : transform.position;

            if (itemData?.ItemAudioData != null && itemData.ItemAudioData.HasCustomImpactSound)
            {
                if (enableDebugLogging)
                    Debug.Log($"[ItemImpactAudio] Playing custom impact sound for {itemData.ItemName}");
                AudioManager.Instance.PlaySound(itemData.ItemAudioData.CustomImpactSound, impactPosition, volume);
            }
            else
            {
                SurfaceType surfaceType = AudioManager.Instance.GetSurfaceTypeFromCollision(collision);
                
                if (enableDebugLogging)
                    Debug.Log($"[ItemImpactAudio] Detected surface type: {surfaceType} on {collision.gameObject.name}");
                
                if (surfaceType == SurfaceType.Default && itemData?.ItemAudioData != null)
                {
                    surfaceType = itemData.ItemAudioData.DefaultSurfaceType;
                    if (enableDebugLogging)
                        Debug.Log($"[ItemImpactAudio] Using fallback surface type: {surfaceType}");
                }

                AudioManager.Instance.PlayImpactSound(surfaceType, impactPosition, volume);
            }
        }
    }
}
