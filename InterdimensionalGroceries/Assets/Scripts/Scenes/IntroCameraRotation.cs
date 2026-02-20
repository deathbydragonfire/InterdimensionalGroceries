using UnityEngine;

namespace InterdimensionalGroceries.Scenes
{
    public class IntroCameraRotation : MonoBehaviour
    {
        private float rotationSpeed = 10f;
        private bool isRotating = false;

        private void Update()
        {
            if (isRotating)
            {
                float rotationAmount = (360f / rotationSpeed) * Time.deltaTime;
                transform.Rotate(Vector3.up, rotationAmount, Space.World);
            }
        }

        public void SetRotationSpeed(float speed)
        {
            rotationSpeed = speed;
        }

        public void StartRotation()
        {
            isRotating = true;
        }

        public void StopRotation()
        {
            isRotating = false;
        }

        public bool IsRotating()
        {
            return isRotating;
        }
    }
}
