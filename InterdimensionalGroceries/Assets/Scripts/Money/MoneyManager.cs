using UnityEngine;
using TMPro;
using InterdimensionalGroceries.AudioSystem;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private MoneyUIController moneyUIController;
    [SerializeField] private Color positiveMoneyColor = Color.white;
    [SerializeField] private Color negativeMoneyColor = Color.red;

    private float currentMoney = 20f;

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

    public bool SpendMoney(float amount)
    {
        if (amount < 0f)
        {
            Debug.LogWarning("Cannot spend negative money amount.");
            return false;
        }
        
        currentMoney -= amount;
        UpdateUI();
        return true;
    }

    public void DeductMoney(float amount)
    {
        currentMoney -= amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"$ {currentMoney:0.00}";
            
            if (currentMoney < 0)
            {
                moneyText.color = negativeMoneyColor;
            }
            else
            {
                moneyText.color = positiveMoneyColor;
            }
        }
    }

    public float GetCurrentMoney()
    {
        return currentMoney;
    }
}
