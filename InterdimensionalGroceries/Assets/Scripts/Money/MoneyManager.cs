using UnityEngine;
using TMPro; // Assuming you use TextMeshPro for UI

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text moneyText;

    private float currentMoney = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        UpdateUI();
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (moneyText != null)
            moneyText.text = $"$ {currentMoney:0.00}";
    }

    public float GetCurrentMoney()
    {
        return currentMoney;
    }
}
