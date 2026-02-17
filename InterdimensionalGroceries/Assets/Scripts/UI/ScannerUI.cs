using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InterdimensionalGroceries.ScannerSystem
{
    public class ScannerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text scannerText;


        public void ShowRequest(string itemName)
        {
            SetText($"Bring: {itemName}", Color.white);
        }

        public void ShowScanning()
        {
            SetText("Scanning...", Color.yellow);
        }

        public void ShowCorrect()
        {
            SetText("Accepted", Color.green);
        }

        public void ShowWrong()
        {
            SetText("Rejected", Color.red);
        }

        private void SetText(string text, Color color)
        {
            scannerText.text = text;
            scannerText.color = color;
        }
    }
}
