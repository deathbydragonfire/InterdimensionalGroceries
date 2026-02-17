using UnityEngine;
using TMPro;
using InterdimensionalGroceries.AudioSystem;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private MoneyUIController moneyUIController;

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
        
        if (moneyUIController != null)
        {
            moneyUIController.PlayMoneyAddedAnimation();
        }

        if (AudioManager.Instance != null)
        {
            Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
            AudioManager.Instance.PlaySound(AudioEventType.MoneyGained, soundPosition);
        }
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
