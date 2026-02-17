using UnityEngine;
using UnityEngine.UIElements;

namespace InterdimensionalGroceries.PlayerController
{
    [RequireComponent(typeof(UIDocument))]
    public class UIFontApplier : MonoBehaviour
    {
        [SerializeField] private Font customFont;
        
        private void Start()
        {
            if (customFont == null)
            {
                Debug.LogWarning($"No font assigned to UIFontApplier on {gameObject.name}");
                return;
            }

            var uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null || uiDocument.rootVisualElement == null)
            {
                Debug.LogWarning($"No UIDocument found on {gameObject.name}");
                return;
            }

            ApplyFontToAllLabels(uiDocument.rootVisualElement);
            Debug.Log($"Applied font '{customFont.name}' to all labels in {gameObject.name}");
        }

        private void ApplyFontToAllLabels(VisualElement root)
        {
            var labels = root.Query<Label>().ToList();
            foreach (var label in labels)
            {
                label.style.unityFont = new StyleFont(customFont);
            }
        }
    }
}
