using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace InterdimensionalGroceries.BuildSystem
{
    public class BuildModeUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject selectionPanel;
        [SerializeField] private Transform gridContainer;
        [SerializeField] private GameObject buttonPrefab;
        
        [Header("Settings")]
        [SerializeField] private List<BuildableObject> availableObjects = new List<BuildableObject>();
        
        private BuildModeController buildModeController;
        private List<GameObject> instantiatedButtons = new List<GameObject>();
        
        public void Initialize(BuildModeController controller)
        {
            buildModeController = controller;
            CreateObjectButtons();
            Hide();
        }
        
        public void Show()
        {
            if (selectionPanel != null)
            {
                selectionPanel.SetActive(true);
            }
        }
        
        public void Hide()
        {
            if (selectionPanel != null)
            {
                selectionPanel.SetActive(false);
            }
        }
        
        public void SetAvailableObjects(List<BuildableObject> objects)
        {
            availableObjects = objects;
            CreateObjectButtons();
        }
        
        private void CreateObjectButtons()
        {
            foreach (GameObject btn in instantiatedButtons)
            {
                Destroy(btn);
            }
            instantiatedButtons.Clear();
            
            if (gridContainer == null || buttonPrefab == null) return;
            
            foreach (BuildableObject buildableObj in availableObjects)
            {
                if (buildableObj == null) continue;
                
                GameObject buttonObj = Instantiate(buttonPrefab, gridContainer);
                instantiatedButtons.Add(buttonObj);
                
                Button button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    BuildableObject obj = buildableObj;
                    button.onClick.AddListener(() => OnObjectSelected(obj));
                }
                
                BuildableObjectButton buttonHandler = buttonObj.GetComponent<BuildableObjectButton>();
                if (buttonHandler == null)
                {
                    buttonHandler = buttonObj.AddComponent<BuildableObjectButton>();
                }
                buttonHandler.Initialize(buildableObj);
                
                Image icon = buttonObj.transform.Find("Icon")?.GetComponent<Image>();
                if (icon != null && buildableObj.Icon != null)
                {
                    icon.sprite = buildableObj.Icon;
                }
            }
        }
        
        private void OnObjectSelected(BuildableObject buildableObject)
        {
            if (buildModeController != null)
            {
                buildModeController.SelectObject(buildableObject);
            }
        }
    }
}
