using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using InterdimensionalGroceries.AudioSystem;
using InterdimensionalGroceries.EconomySystem;
using InterdimensionalGroceries.Core;

namespace InterdimensionalGroceries.BuildSystem
{
    public enum BuildModeState
    {
        Inactive,
        Browsing,
        Placing,
        FurniturePlacement
    }
    
    public class BuildModeController : MonoBehaviour
    {
        public static BuildModeController Instance { get; private set; }
        
        [Header("References")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private BuildModeUI buildModeUI;
        [SerializeField] private GridVisualizer gridVisualizer;
        [SerializeField] private List<BuildableObject> availableObjects = new List<BuildableObject>();
        [SerializeField] private BuildModeCameraController cameraController;
        [SerializeField] private FurnitureInventory furnitureInventory;
        [SerializeField] private FurniturePlacementUI furniturePlacementUI;
        [SerializeField] private BuildModeExitUI buildModeExitUI;
        
        [Header("Placement Settings")]
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private LayerMask collisionCheckMask;
        [SerializeField] private float raycastMaxDistance = 100f;
        
        [Header("Grid Settings")]
        [SerializeField] private bool useGridSnapping = true;
        [SerializeField] private float gridSize = 1f;
        [SerializeField] private Vector3 gridOffset = Vector3.zero;
        [SerializeField] private bool showGridVisualization = false;
        [SerializeField] private bool showGridGizmos = true;
        [SerializeField] private int gridGizmoSize = 20;
        [SerializeField] private Color gridGizmoColor = new Color(0f, 1f, 0f, 0.3f);
        
        [Header("Ghost Materials")]
        [SerializeField] private Material ghostValidMaterial;
        [SerializeField] private Material ghostInvalidMaterial;
        
        private BuildModeState currentState = BuildModeState.Inactive;
        private BuildableObject selectedObject;
        private GameObject currentGhost;
        private PlacementGhost ghostController;
        private InputSystem_Actions inputActions;
        private Vector3 lastValidPosition;
        private bool hasValidPosition;
        private System.Action onExitFurnitureModeCallback;
        
        public bool IsActive => currentState != BuildModeState.Inactive;
        public bool IsBrowsing => currentState == BuildModeState.Browsing;
        public BuildModeState CurrentState => currentState;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            inputActions = new InputSystem_Actions();
            
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
            }
        }
        
        private void OnEnable()
        {
            inputActions.BuildMode.Enable();
            inputActions.BuildMode.ToggleBuildMode.performed += OnToggleBuildMode;
            inputActions.BuildMode.RotateObject.performed += OnRotateObject;
            inputActions.BuildMode.Place.performed += OnPlace;
            inputActions.BuildMode.Cancel.performed += OnCancel;
        }
        
        private void OnDisable()
        {
            inputActions.BuildMode.ToggleBuildMode.performed -= OnToggleBuildMode;
            inputActions.BuildMode.RotateObject.performed -= OnRotateObject;
            inputActions.BuildMode.Place.performed -= OnPlace;
            inputActions.BuildMode.Cancel.performed -= OnCancel;
            inputActions.BuildMode.Disable();
        }
        
        private void Start()
        {
            if (buildModeUI != null)
            {
                buildModeUI.Initialize(this);
                buildModeUI.SetAvailableObjects(availableObjects);
            }
        }
        
        private void Update()
        {
            if (currentState == BuildModeState.Placing || currentState == BuildModeState.FurniturePlacement)
            {
                UpdateGhostPosition();
            }
        }
        
        private void OnToggleBuildMode(InputAction.CallbackContext context)
        {
            if (currentState == BuildModeState.Inactive)
            {
                EnterBuildMode();
            }
            else
            {
                ExitBuildMode();
            }
        }
        
        private void OnRotateObject(InputAction.CallbackContext context)
        {
            if ((currentState == BuildModeState.Placing || currentState == BuildModeState.FurniturePlacement) && ghostController != null)
            {
                ghostController.Rotate90Degrees();
                ghostController.CheckValidPlacement();
            }
        }
        
        private void OnPlace(InputAction.CallbackContext context)
        {
            if ((currentState == BuildModeState.Placing || currentState == BuildModeState.FurniturePlacement) && hasValidPosition)
            {
                if (ghostController != null && ghostController.CheckValidPlacement())
                {
                    bool shiftHeld = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
                    PlaceObject(shiftHeld);
                }
            }
        }
        
        private void OnCancel(InputAction.CallbackContext context)
        {
            if (currentState == BuildModeState.Placing || currentState == BuildModeState.FurniturePlacement)
            {
                CancelPlacement();
            }
            else if (currentState == BuildModeState.Browsing)
            {
                ExitBuildMode();
            }
        }
        
        public void SelectObject(BuildableObject buildableObject)
        {
            selectedObject = buildableObject;
            EnterPlacingState();
        }

        public void EnterFurniturePlacementMode(System.Action onExitCallback = null)
        {
            if (currentState == BuildModeState.FurniturePlacement) return;
            
            currentState = BuildModeState.FurniturePlacement;
            onExitFurnitureModeCallback = onExitCallback;
            
            if (cameraController != null)
            {
                cameraController.EnterBuildModeView();
            }
            
            var firstPersonController = FindFirstObjectByType<InterdimensionalGroceries.PlayerController.FirstPersonController>();
            if (firstPersonController != null)
            {
                firstPersonController.SetControlsEnabled(false);
            }
            
            if (furniturePlacementUI != null)
            {
                furniturePlacementUI.Show();
            }
            
            if (buildModeExitUI != null)
            {
                buildModeExitUI.Show();
            }
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            Debug.Log("Entered furniture placement mode");
        }
        
        private void EnterBuildMode()
        {
            currentState = BuildModeState.Browsing;
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (buildModeUI != null)
            {
                buildModeUI.Show();
            }
        }
        
        public void ExitBuildMode()
        {
            if (currentGhost != null)
            {
                Destroy(currentGhost);
                currentGhost = null;
                ghostController = null;
            }
            
            if (gridVisualizer != null)
            {
                gridVisualizer.Hide();
            }
            
            bool wasFurniturePlacement = currentState == BuildModeState.FurniturePlacement;
            System.Action callbackToInvoke = onExitFurnitureModeCallback;
            
            if (cameraController != null && wasFurniturePlacement)
            {
                cameraController.ExitBuildModeView();
            }
            
            var firstPersonController = FindFirstObjectByType<InterdimensionalGroceries.PlayerController.FirstPersonController>();
            if (firstPersonController != null && wasFurniturePlacement)
            {
                firstPersonController.SetControlsEnabled(true);
            }
            
            if (furniturePlacementUI != null && wasFurniturePlacement)
            {
                furniturePlacementUI.Hide();
            }
            
            if (buildModeExitUI != null && wasFurniturePlacement)
            {
                buildModeExitUI.Hide();
            }
            
            currentState = BuildModeState.Inactive;
            selectedObject = null;
            onExitFurnitureModeCallback = null;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (buildModeUI != null)
            {
                buildModeUI.Hide();
            }
            
            // Invoke callback after cleanup (to reopen furniture store menu)
            if (wasFurniturePlacement && callbackToInvoke != null)
            {
                callbackToInvoke.Invoke();
            }
        }
        
        private void EnterPlacingState()
        {
            if (selectedObject == null || selectedObject.Prefab == null)
            {
                Debug.LogWarning("Cannot enter placing state: no valid object selected");
                return;
            }
            
            if (currentState == BuildModeState.FurniturePlacement)
            {
                currentState = BuildModeState.FurniturePlacement;
            }
            else
            {
                currentState = BuildModeState.Placing;
            }
            
            if (buildModeUI != null)
            {
                buildModeUI.Hide();
            }
            
            CreateGhost();
            
            if (useGridSnapping && showGridVisualization && gridVisualizer != null)
            {
                gridVisualizer.Show(Vector3.zero, gridSize, gridOffset);
            }
        }
        
        private void CancelPlacement()
        {
            if (currentGhost != null)
            {
                Destroy(currentGhost);
                currentGhost = null;
                ghostController = null;
            }
            
            if (gridVisualizer != null)
            {
                gridVisualizer.Hide();
            }
            
            bool wasFurniturePlacement = currentState == BuildModeState.FurniturePlacement;
            
            if (wasFurniturePlacement)
            {
                ExitBuildMode();
            }
            else
            {
                currentState = BuildModeState.Browsing;
                selectedObject = null;
                
                if (buildModeUI != null)
                {
                    buildModeUI.Show();
                }
            }
        }
        
        private void CreateGhost()
        {
            if (currentGhost != null)
            {
                Destroy(currentGhost);
            }
            
            currentGhost = Instantiate(selectedObject.Prefab);
            
            currentGhost.transform.rotation = Quaternion.identity;
            
            Collider[] colliders = currentGhost.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }
            
            Rigidbody[] rigidbodies = currentGhost.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rigidbodies)
            {
                Destroy(rb);
            }
            
            ghostController = currentGhost.AddComponent<PlacementGhost>();
            
            typeof(PlacementGhost)
                .GetField("validMaterial", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ghostController, ghostValidMaterial);
            typeof(PlacementGhost)
                .GetField("invalidMaterial", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ghostController, ghostInvalidMaterial);
            typeof(PlacementGhost)
                .GetField("collisionCheckMask", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(ghostController, collisionCheckMask);
            
            ghostController.UpdateVisualState();
        }
        
        private void UpdateGhostPosition()
        {
            if (ghostController == null) return;
            
            Camera activeCamera = playerCamera;
            if (currentState == BuildModeState.FurniturePlacement && cameraController != null && cameraController.IsInBuildMode)
            {
                activeCamera = cameraController.BuildModeCamera;
            }
            
            if (activeCamera == null)
            {
                Debug.LogWarning("No active camera found for ghost positioning");
                return;
            }
            
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = activeCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, raycastMaxDistance, groundLayerMask))
            {
                Vector3 position = hit.point;
                
                if (useGridSnapping)
                {
                    position = SnapToGrid(position);
                }
                
                ghostController.SetPosition(position);
                hasValidPosition = true;
                ghostController.CheckValidPlacement();
                lastValidPosition = position;
            }
            else
            {
                hasValidPosition = false;
                ghostController.SetValid(false);
            }
        }
        
        private Vector3 SnapToGrid(Vector3 position)
        {
            Vector3 snapped = new Vector3(
                Mathf.Round((position.x - gridOffset.x) / gridSize) * gridSize + gridOffset.x,
                position.y,
                Mathf.Round((position.z - gridOffset.z) / gridSize) * gridSize + gridOffset.z
            );
            return snapped;
        }
        
        private void PlaceObject(bool continueMultiPlacement)
        {
            if (selectedObject == null || selectedObject.Prefab == null) return;
            
            bool isFromFurnitureInventory = currentState == BuildModeState.FurniturePlacement;
            
            if (isFromFurnitureInventory && furnitureInventory != null)
            {
                if (furnitureInventory.GetFurnitureCount(selectedObject) <= 0)
                {
                    Debug.Log("No furniture available in inventory to place.");
                    return;
                }
            }
            
            Vector3 placementPosition = currentGhost.transform.position;
            GameObject placedObject = Instantiate(selectedObject.Prefab, placementPosition, currentGhost.transform.rotation);
            
            SaveableObject saveableComponent = placedObject.GetComponent<SaveableObject>();
            if (saveableComponent == null)
            {
                saveableComponent = placedObject.AddComponent<SaveableObject>();
            }
            else
            {
                if (WorldObjectManager.Instance != null)
                {
                    WorldObjectManager.Instance.UnregisterObject(saveableComponent);
                }
            }
            
            saveableComponent.PrefabIdentifier = selectedObject.ObjectName;
            saveableComponent.ObjectType = SaveableObjectType.Furniture;
            
            if (WorldObjectManager.Instance != null)
            {
                WorldObjectManager.Instance.RegisterObject(saveableComponent);
            }
            
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(AudioEventType.BuildModePlace, placementPosition);
            }
            
            if (isFromFurnitureInventory && furnitureInventory != null)
            {
                furnitureInventory.RemoveFurniture(selectedObject, 1);
                Debug.Log($"Placed furniture: {selectedObject.ObjectName}. Remaining: {furnitureInventory.GetFurnitureCount(selectedObject)}");
                
                if (furniturePlacementUI != null)
                {
                    furniturePlacementUI.RefreshCounts();
                }
                
                // Check if we just placed the last item
                if (furnitureInventory.GetFurnitureCount(selectedObject) <= 0)
                {
                    Debug.Log($"No more {selectedObject.ObjectName} left. Clearing selection.");
                    ClearGhostAndSelection();
                    return;
                }
            }
            
            // In furniture placement mode, always continue placement (don't exit)
            // In regular build mode, respect the continueMultiPlacement flag (Shift key)
            if (isFromFurnitureInventory || continueMultiPlacement)
            {
                CreateGhost();
            }
            else
            {
                ExitPlacementMode();
            }
        }
        
        private void ExitPlacementMode()
        {
            if (currentGhost != null)
            {
                Destroy(currentGhost);
                currentGhost = null;
                ghostController = null;
            }
            
            if (gridVisualizer != null)
            {
                gridVisualizer.Hide();
            }
            
            bool wasFurniturePlacement = currentState == BuildModeState.FurniturePlacement;
            
            currentState = BuildModeState.Inactive;
            selectedObject = null;
            
            if (wasFurniturePlacement && cameraController != null)
            {
                cameraController.ExitBuildModeView();
            }
            
            var firstPersonController = FindFirstObjectByType<InterdimensionalGroceries.PlayerController.FirstPersonController>();
            if (firstPersonController != null && wasFurniturePlacement)
            {
                firstPersonController.SetControlsEnabled(true);
            }
            
            if (furniturePlacementUI != null && wasFurniturePlacement)
            {
                furniturePlacementUI.Hide();
            }
            
            if (buildModeExitUI != null && wasFurniturePlacement)
            {
                buildModeExitUI.Hide();
            }
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (buildModeUI != null)
            {
                buildModeUI.Hide();
            }
        }
        
        private void ClearGhostAndSelection()
        {
            if (currentGhost != null)
            {
                Destroy(currentGhost);
                currentGhost = null;
                ghostController = null;
            }
            
            selectedObject = null;
            hasValidPosition = false;
        }
        
        private void OnDrawGizmos()
        {
            if (!showGridGizmos || !useGridSnapping || (currentState != BuildModeState.Placing && currentState != BuildModeState.FurniturePlacement)) return;
            
            Gizmos.color = gridGizmoColor;
            
            Vector3 center = gridOffset;
            if (ghostController != null)
            {
                center = ghostController.transform.position;
            }
            
            int halfSize = gridGizmoSize / 2;
            
            for (int x = -halfSize; x <= halfSize; x++)
            {
                Vector3 start = new Vector3(center.x + x * gridSize, 0.01f, center.z - halfSize * gridSize);
                Vector3 end = new Vector3(center.x + x * gridSize, 0.01f, center.z + halfSize * gridSize);
                Gizmos.DrawLine(start, end);
            }
            
            for (int z = -halfSize; z <= halfSize; z++)
            {
                Vector3 start = new Vector3(center.x - halfSize * gridSize, 0.01f, center.z + z * gridSize);
                Vector3 end = new Vector3(center.x + halfSize * gridSize, 0.01f, center.z + z * gridSize);
                Gizmos.DrawLine(start, end);
            }
        }
    }
}
