using UnityEngine;

namespace InterdimensionalGroceries.EconomySystem
{
    public class ChuteSpawnPoint : MonoBehaviour
    {
        [Header("Trajectory")]
        [SerializeField] private Transform trajectoryObject;
        
        [Header("Ejection Settings")]
        [SerializeField] private float minForce = 3f;
        [SerializeField] private float maxForce = 6f;
        
        [Header("Direction Randomization")]
        [SerializeField, Range(0f, 45f)] private float horizontalRandomAngle = 15f;
        [SerializeField, Range(0f, 45f)] private float verticalRandomAngle = 10f;
        
        public Vector3 GetRandomEjectionForce()
        {
            Vector3 baseDirection = trajectoryObject != null ? -trajectoryObject.forward : transform.forward;
            
            float randomHorizontalAngle = Random.Range(-horizontalRandomAngle, horizontalRandomAngle);
            float randomVerticalAngle = Random.Range(-verticalRandomAngle, verticalRandomAngle);
            
            Quaternion randomRotation = Quaternion.Euler(randomVerticalAngle, randomHorizontalAngle, 0f);
            Vector3 direction = randomRotation * baseDirection;
            
            float force = Random.Range(minForce, maxForce);
            return direction * force;
        }
        
        public Vector3 SpawnPosition => transform.position;
    }
}
