using UnityEngine;

namespace InterdimensionalGroceries.Core
{
    public class ConstantRotation : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [Tooltip("Rotation speed in degrees per second")]
        public float rotationSpeed = 100f;
        
        [Tooltip("Axis to rotate around (default is Z axis for fan blades)")]
        public Vector3 rotationAxis = Vector3.forward;
        
        [Tooltip("Rotate around parent's center instead of own pivot")]
        public bool rotateAroundParent = false;

        void Update()
        {
            if (rotateAroundParent && transform.parent != null)
            {
                Vector3 worldAxis = transform.parent.TransformDirection(rotationAxis);
                transform.RotateAround(transform.parent.position, worldAxis, rotationSpeed * Time.deltaTime);
            }
            else
            {
                transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
            }
        }
    }
}
