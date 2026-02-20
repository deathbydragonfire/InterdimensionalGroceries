using UnityEngine;

namespace InterdimensionalGroceries.UI
{
    public class MenuCameraRotation : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 10f;
        
        private bool isRotating = true;

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
