using UnityEngine;
using UnityEngine.UIElements;

namespace InterdimensionalGroceries.BuildSystem
{
    [RequireComponent(typeof(UIDocument))]
    public class BuildModeExitUI : MonoBehaviour
    {
        private UIDocument uiDocument;
        private VisualElement root;
        private Button exitButton;
        
        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
        }
        
        private void Start()
        {
            root = uiDocument.rootVisualElement;
            
            exitButton = root.Q<Button>("ExitBuildModeButton");
            
            if (exitButton != null)
            {
                exitButton.clicked += OnExitClicked;
            }
            
            Hide();
        }
        
        private void OnExitClicked()
        {
            if (BuildModeController.Instance != null)
            {
                BuildModeController.Instance.ExitBuildMode();
            }
            
            Hide();
        }
        
        public void Show()
        {
            if (root != null)
            {
                root.style.display = DisplayStyle.Flex;
            }
        }
        
        public void Hide()
        {
            if (root != null)
            {
                root.style.display = DisplayStyle.None;
            }
        }
    }
}
