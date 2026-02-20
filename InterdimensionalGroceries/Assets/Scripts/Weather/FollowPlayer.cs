using UnityEngine;

namespace InterdimensionalGroceries.Weather
{
    public class FollowPlayer : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float heightAbovePlayer = 20f;
        [SerializeField] private bool followX = true;
        [SerializeField] private bool followZ = true;
        [SerializeField] private bool smoothFollow = true;
        [SerializeField] private float smoothSpeed = 5f;
        
        private void Start()
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                }
                else
                {
                    Debug.LogWarning("[FollowPlayer] No player found with 'Player' tag. Weather effects will not follow player.");
                }
            }
        }
        
        private void LateUpdate()
        {
            if (playerTransform == null) return;
            
            Vector3 targetPosition = transform.position;
            
            if (followX)
            {
                targetPosition.x = playerTransform.position.x;
            }
            
            if (followZ)
            {
                targetPosition.z = playerTransform.position.z;
            }
            
            targetPosition.y = playerTransform.position.y + heightAbovePlayer;
            
            if (smoothFollow)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }
}
