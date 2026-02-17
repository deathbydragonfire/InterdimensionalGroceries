using UnityEngine;
using UnityEngine.UI;

namespace InterdimensionalGroceries.BuildSystem
{
    public class BuildableObjectButton : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text nameLabel;
        [SerializeField] private Text priceLabel;
        [SerializeField] private Text descriptionLabel;
        
        public void Initialize(BuildableObject buildableObject)
        {
            if (buildableObject == null) return;
            
            if (nameLabel != null)
            {
                nameLabel.text = buildableObject.ObjectName;
            }
            
            if (priceLabel != null)
            {
                priceLabel.text = $"${buildableObject.PlacementCost}";
            }
            
            if (descriptionLabel != null)
            {
                descriptionLabel.text = buildableObject.Description;
            }
        }
    }
}
