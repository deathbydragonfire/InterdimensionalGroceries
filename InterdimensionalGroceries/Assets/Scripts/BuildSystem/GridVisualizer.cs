using UnityEngine;

namespace InterdimensionalGroceries.BuildSystem
{
    public class GridVisualizer : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private float gridSize = 1f;
        [SerializeField] private int gridCellCount = 20;
        [SerializeField] private float gridHeight = 0.01f;
        
        [Header("Appearance")]
        [SerializeField] private Color gridColor = new Color(0f, 1f, 0f, 0.5f);
        [SerializeField] private float lineWidth = 0.02f;
        [SerializeField] private Material lineMaterial;
        
        private GameObject gridContainer;
        private bool isVisible = false;
        
        public void Show(Vector3 centerPosition, float size, Vector3 offset)
        {
            gridSize = size;
            
            if (gridContainer != null)
            {
                Destroy(gridContainer);
            }
            
            CreateGrid(offset, offset);
            isVisible = true;
        }
        
        public void Hide()
        {
            if (gridContainer != null)
            {
                Destroy(gridContainer);
                gridContainer = null;
            }
            isVisible = false;
        }
        
        private void CreateGrid(Vector3 centerPosition, Vector3 offset)
        {
            gridContainer = new GameObject("GridVisualizer");
            gridContainer.transform.position = new Vector3(offset.x, gridHeight, offset.z);
            
            int halfCount = gridCellCount / 2;
            float cellSize = gridSize * 0.5f;
            
            for (int x = -halfCount; x <= halfCount; x++)
            {
                CreateLine(
                    new Vector3(x * cellSize, 0, -halfCount * cellSize),
                    new Vector3(x * cellSize, 0, halfCount * cellSize),
                    gridContainer.transform
                );
            }
            
            for (int z = -halfCount; z <= halfCount; z++)
            {
                CreateLine(
                    new Vector3(-halfCount * cellSize, 0, z * cellSize),
                    new Vector3(halfCount * cellSize, 0, z * cellSize),
                    gridContainer.transform
                );
            }
        }
        
        private void CreateLine(Vector3 start, Vector3 end, Transform parent)
        {
            GameObject lineObj = new GameObject("GridLine");
            lineObj.transform.SetParent(parent);
            lineObj.transform.localPosition = Vector3.zero;
            
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            
            if (lineMaterial != null)
            {
                lr.material = lineMaterial;
            }
            else
            {
                lr.material = new Material(Shader.Find("Sprites/Default"));
            }
            
            lr.startColor = gridColor;
            lr.endColor = gridColor;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.positionCount = 2;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            lr.useWorldSpace = false;
            
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lr.receiveShadows = false;
        }
        
        private Vector3 SnapToGrid(Vector3 position, Vector3 offset)
        {
            return new Vector3(
                Mathf.Round((position.x - offset.x) / gridSize) * gridSize + offset.x,
                position.y,
                Mathf.Round((position.z - offset.z) / gridSize) * gridSize + offset.z
            );
        }
        
        private void OnDestroy()
        {
            Hide();
        }
    }
}
