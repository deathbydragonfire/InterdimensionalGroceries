using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace InterdimensionalGroceries.UI
{
    public class MenuCameraRotation : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 10f;
        [SerializeField] private Volume postProcessVolume;
        [SerializeField] private float menuBloomIntensity = 2.76f;
        
        private bool isRotating = true;
        private Bloom bloom;

        private void Awake()
        {
            if (postProcessVolume != null && postProcessVolume.sharedProfile != null)
            {
                if (postProcessVolume.sharedProfile.TryGet(out bloom))
                {
                    bloom.intensity.overrideState = true;
                    bloom.intensity.value = menuBloomIntensity;
                }
            }
        }

        private void Update()
        {
            if (isRotating)
            {
                float rotationAmount = (360f / rotationSpeed) * Time.deltaTime;
                transform.Rotate(Vector3.up, rotationAmount, Space.World);
            }
        }

        public void SetRotating(bool rotating)
        {
            isRotating = rotating;
        }

        public void StopRotation()
        {
            isRotating = false;
        }

        public void StartRotation()
        {
            isRotating = true;
        }

        public float GetRotationSpeed()
        {
            return rotationSpeed;
        }
    }
}
