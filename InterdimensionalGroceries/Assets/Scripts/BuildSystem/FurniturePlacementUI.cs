using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using InterdimensionalGroceries.EconomySystem;

namespace InterdimensionalGroceries.BuildSystem
{
    [RequireComponent(typeof(UIDocument))]
    public class FurniturePlacementUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private FurnitureInventory furnitureInventory;
        [SerializeField] private BuildModeController buildModeController;
        
        private UIDocument uiDocument;
        private VisualElement root;
        private VisualElement furnitureGrid;
        private Label noFurnitureLabel;
        private BuildableObject currentlySelectedFurniture;
        
        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
        }
        
        private void Start()
        {
            root = uiDocument.rootVisualElement;
            furnitureGrid = root.Q<VisualElement>("FurnitureGrid");
            noFurnitureLabel = root.Q<Label>("NoFurnitureLabel");
            
            Hide();
        }
        
        public void Show()
        {
            if (root != null)
            {
                root.style.display = DisplayStyle.Flex;
                PopulateFurnitureGrid();
            }
        }
        
        public void Hide()
        {
            if (root != null)
            {
                root.style.display = DisplayStyle.None;
            }
        }
        
        private void PopulateFurnitureGrid()
        {
            if (furnitureGrid == null || furnitureInventory == null) return;
            
            furnitureGrid.Clear();
            
            List<FurnitureInventoryEntry> ownedFurniture = furnitureInventory.GetAllOwnedFurniture();
            
            if (ownedFurniture.Count == 0)
            {
                if (noFurnitureLabel != null)
                {
                    noFurnitureLabel.style.display = DisplayStyle.Flex;
                }
                return;
            }
            
            if (noFurnitureLabel != null)
            {
                noFurnitureLabel.style.display = DisplayStyle.None;
            }
            
            foreach (var entry in ownedFurniture)
            {
                if (entry.furniture == null || entry.quantity <= 0) continue;
                
                var furnitureButton = CreateFurnitureButton(entry);
                furnitureGrid.Add(furnitureButton);
            }
        }
        
        private Button CreateFurnitureButton(FurnitureInventoryEntry entry)
        {
            Button button = new Button(() => OnFurnitureSelected(entry.furniture));
            button.AddToClassList("furniture-item-button");
            
            var container = new VisualElement();
            container.AddToClassList("furniture-item-container");
            
            if (entry.furniture.Icon != null)
            {
                var icon = new VisualElement();
                icon.style.backgroundImage = new StyleBackground(entry.furniture.Icon);
                icon.AddToClassList("furniture-icon");
                container.Add(icon);
            }
            
            var textContainer = new VisualElement();
            textContainer.AddToClassList("furniture-text-container");
            
            var nameLabel = new Label(entry.furniture.ObjectName);
            nameLabel.AddToClassList("furniture-name");
            textContainer.Add(nameLabel);
            
            var countLabel = new Label($"Owned: {entry.quantity}");
            countLabel.AddToClassList("furniture-count");
            textContainer.Add(countLabel);
            
            container.Add(textContainer);
            button.Add(container);
            
            return button;
        }
        
        private void OnFurnitureSelected(BuildableObject furniture)
        {
            currentlySelectedFurniture = furniture;
            
            if (buildModeController != null)
            {
                buildModeController.SelectObject(furniture);
            }
            
            Debug.Log($"Selected furniture for placement: {furniture.ObjectName}");
        }
        
        public void RefreshCounts()
        {
            PopulateFurnitureGrid();
        }
    }
}
