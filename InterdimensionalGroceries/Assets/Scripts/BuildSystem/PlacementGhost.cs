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
        private Bounds combinedBounds;
        private bool isValid = true;
        private Quaternion fixedRotation;
        
        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            fixedRotation = transform.rotation;
            CalculateCombinedBounds();
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
            CalculateCombinedBounds();
        }
        
        public bool CheckValidPlacement()
        {
            Vector3 worldCenter = transform.TransformPoint(combinedBounds.center);
            Vector3 halfExtents = combinedBounds.extents + Vector3.one * collisionCheckPadding;
            
            Collider[] overlaps = Physics.OverlapBox(worldCenter, halfExtents, transform.rotation, collisionCheckMask);
            
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
        
        private void CalculateCombinedBounds()
        {
            if (renderers.Length == 0)
            {
                combinedBounds = new Bounds(Vector3.zero, Vector3.one);
                return;
            }
            
            Vector3 min = Vector3.positiveInfinity;
            Vector3 max = Vector3.negativeInfinity;
            
            foreach (Renderer rend in renderers)
            {
                MeshFilter meshFilter = rend.GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    Mesh mesh = meshFilter.sharedMesh;
                    Vector3[] vertices = mesh.vertices;
                    
                    foreach (Vector3 vertex in vertices)
                    {
                        Vector3 worldVertex = rend.transform.TransformPoint(vertex);
                        Vector3 localVertex = transform.InverseTransformPoint(worldVertex);
                        
                        min = Vector3.Min(min, localVertex);
                        max = Vector3.Max(max, localVertex);
                    }
                }
            }
            
            combinedBounds = new Bounds();
            combinedBounds.SetMinMax(min, max);
        }
        
        private void OnDrawGizmos()
        {
            if (Application.isPlaying && renderers != null && renderers.Length > 0)
            {
                Gizmos.color = isValid ? Color.green : Color.red;
                
                Vector3 worldCenter = transform.TransformPoint(combinedBounds.center);
                Vector3 paddedSize = combinedBounds.size + Vector3.one * (collisionCheckPadding * 2f);
                
                Gizmos.matrix = Matrix4x4.TRS(worldCenter, transform.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, paddedSize);
                Gizmos.matrix = Matrix4x4.identity;
            }
        }
    }
}
