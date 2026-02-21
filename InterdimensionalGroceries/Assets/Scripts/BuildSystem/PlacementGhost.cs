using UnityEngine;

namespace InterdimensionalGroceries.BuildSystem
{
    public class PlacementGhost : MonoBehaviour
    {
        [Header("Visual Feedback")]
        [SerializeField] private Material validMaterial;
        [SerializeField] private Material invalidMaterial;
        
        [Header("Collision Detection")]
        [SerializeField] private LayerMask collisionCheckMask;
        [SerializeField] private float collisionCheckPadding = 0.1f;
        
        private Renderer[] renderers;
        private BoxCollider referenceCollider;
        private bool isValid = true;
        private Quaternion fixedRotation;
        
        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            referenceCollider = GetComponentInChildren<BoxCollider>();
            fixedRotation = transform.rotation;
            
            if (referenceCollider != null)
            {
                referenceCollider.enabled = false;
            }
        }
        
        private void LateUpdate()
        {
            transform.rotation = fixedRotation;
        }
        
        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
        
        public void Rotate90Degrees()
        {
            fixedRotation *= Quaternion.Euler(0f, 90f, 0f);
            transform.rotation = fixedRotation;
        }
        
        public bool CheckValidPlacement()
        {
            if (referenceCollider == null)
            {
                isValid = false;
                UpdateVisualState();
                return false;
            }
            
            Vector3 worldCenter = referenceCollider.transform.TransformPoint(referenceCollider.center);
            Vector3 worldSize = Vector3.Scale(referenceCollider.size, referenceCollider.transform.lossyScale);
            Vector3 halfExtents = worldSize * 0.5f + Vector3.one * collisionCheckPadding;
            Quaternion worldRotation = referenceCollider.transform.rotation;
            
            Collider[] overlaps = Physics.OverlapBox(worldCenter, halfExtents, worldRotation, collisionCheckMask);
            
            isValid = overlaps.Length == 0;
            UpdateVisualState();
            return isValid;
        }
        
        public void UpdateVisualState()
        {
            Material materialToUse = isValid ? validMaterial : invalidMaterial;
            
            foreach (Renderer rend in renderers)
            {
                Material[] materials = new Material[rend.sharedMaterials.Length];
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = materialToUse;
                }
                rend.sharedMaterials = materials;
            }
        }
        
        public void SetValid(bool valid)
        {
            isValid = valid;
            UpdateVisualState();
        }
        
        private void OnDrawGizmos()
        {
            if (Application.isPlaying && referenceCollider != null)
            {
                Gizmos.color = isValid ? Color.green : Color.red;
                
                Vector3 worldCenter = referenceCollider.transform.TransformPoint(referenceCollider.center);
                Vector3 worldSize = Vector3.Scale(referenceCollider.size, referenceCollider.transform.lossyScale);
                Vector3 paddedSize = worldSize + Vector3.one * (collisionCheckPadding * 2f);
                
                Gizmos.matrix = Matrix4x4.TRS(worldCenter, referenceCollider.transform.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, paddedSize);
                Gizmos.matrix = Matrix4x4.identity;
            }
        }
    }
}
