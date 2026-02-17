using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using InterdimensionalGroceries.EconomySystem;
using InterdimensionalGroceries.AudioSystem;

namespace InterdimensionalGroceries.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class AbilitiesMenuController : MonoBehaviour
    {
        private UIDocument uiDocument;
        private VisualElement storeContainer;
        private Label currentMoneyLabel;
        private Button backButton;
        private VisualElement abilityList;
        
        private Action onBackCallback;
        private bool isOpen;
        private bool isAnimating;

        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
        }

        private void Start()
        {
            var root = uiDocument.rootVisualElement;

            storeContainer = root.Q<VisualElement>("StoreContainer");
            currentMoneyLabel = root.Q<Label>("CurrentMoney");
            backButton = root.Q<Button>("BackButton");
            abilityList = root.Q<VisualElement>("AbilityList");

            if (backButton != null)
            {
                backButton.clicked += OnBackClicked;
            }

            if (storeContainer != null)
            {
                storeContainer.style.display = DisplayStyle.None;
            }
        }

        public void OpenAbilitiesMenu(Action onBack)
        {
            if (isAnimating) return;

            onBackCallback = onBack;

            PopulateAbilities();
            UpdateMoneyDisplay();

            if (storeContainer != null)
            {
                storeContainer.style.display = DisplayStyle.Flex;
                StartCoroutine(AnimateSlideIn());
            }

            isOpen = true;

            Debug.Log("Abilities menu opened successfully");
        }

        private void UpdateMoneyDisplay()
        {
            if (currentMoneyLabel != null && MoneyManager.Instance != null)
            {
                float currentMoney = MoneyManager.Instance.GetCurrentMoney();
                currentMoneyLabel.text = $"Balance: ${currentMoney:F2}";
            }
        }

        private void PopulateAbilities()
        {
            Debug.Log($"PopulateAbilities called. abilityList null: {abilityList == null}, AbilityUpgradeManager null: {AbilityUpgradeManager.Instance == null}");
            
            if (abilityList == null || AbilityUpgradeManager.Instance == null)
                return;

            abilityList.Clear();

            var availableUpgrades = AbilityUpgradeManager.Instance.GetAvailableUpgrades();
            
            Debug.Log($"Available upgrades count: {availableUpgrades.Count}");

            foreach (var upgrade in availableUpgrades)
            {
                if (upgrade == null)
                {
                    Debug.LogWarning("Null upgrade in available upgrades list");
                    continue;
                }

                Debug.Log($"Creating ability element for: {upgrade.AbilityName}");
                var abilityElement = CreateAbilityElement(upgrade);
                abilityList.Add(abilityElement);
            }
            
            Debug.Log($"PopulateAbilities complete. Added {availableUpgrades.Count} abilities to list");
        }

        private VisualElement CreateAbilityElement(AbilityUpgradeData upgrade)
        {
            var container = new VisualElement();
            container.AddToClassList("ability-container");

            var headerContainer = new VisualElement();
            headerContainer.AddToClassList("ability-header");

            var nameLabel = new Label(upgrade.AbilityName);
            nameLabel.AddToClassList("ability-name");
            headerContainer.Add(nameLabel);

            int currentLevel = AbilityUpgradeManager.Instance.GetUpgradeLevel(upgrade);
            var levelLabel = new Label($"Level {currentLevel}/{upgrade.MaxLevel}");
            levelLabel.AddToClassList("ability-level");
            headerContainer.Add(levelLabel);

            container.Add(headerContainer);

            var descriptionLabel = new Label(upgrade.Description);
            descriptionLabel.AddToClassList("ability-description");
            container.Add(descriptionLabel);

            var footerContainer = new VisualElement();
            footerContainer.AddToClassList("ability-footer");

            bool isMaxLevel = currentLevel >= upgrade.MaxLevel;
            float nextLevelCost = AbilityUpgradeManager.Instance.GetNextLevelCost(upgrade);

            if (isMaxLevel)
            {
                var maxLevelLabel = new Label("MAX LEVEL");
                maxLevelLabel.AddToClassList("max-level-label");
                footerContainer.Add(maxLevelLabel);
            }
            else
            {
                var costLabel = new Label($"Next: ${nextLevelCost:F2}");
                costLabel.AddToClassList("ability-cost");
                footerContainer.Add(costLabel);

                var purchaseButton = new Button(() => OnPurchaseAbility(upgrade)) { text = "Upgrade" };
                purchaseButton.AddToClassList("ability-purchase-button");

                bool canAfford = AbilityUpgradeManager.Instance.CanAffordUpgrade(upgrade);
                purchaseButton.SetEnabled(canAfford);

                if (!canAfford)
                {
                    purchaseButton.AddToClassList("disabled");
                }

                footerContainer.Add(purchaseButton);
            }

            container.Add(footerContainer);

            return container;
        }

        private void OnPurchaseAbility(AbilityUpgradeData upgrade)
        {
            if (AbilityUpgradeManager.Instance == null)
                return;

            if (AbilityUpgradeManager.Instance.PurchaseUpgrade(upgrade))
            {
                if (AudioManager.Instance != null)
                {
                    Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                    AudioManager.Instance.PlaySound(AudioEventType.UIPurchase, soundPosition);
                }

                PopulateAbilities();
                UpdateMoneyDisplay();
            }
            else
            {
                if (AudioManager.Instance != null)
                {
                    Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                    AudioManager.Instance.PlaySound(AudioEventType.Rejection, soundPosition);
                }
            }
        }

        private void OnBackClicked()
        {
            Debug.Log("AbilitiesMenuController: Back button clicked");
            PlayButtonClickSound();
            GoBack();
        }

        private void GoBack()
        {
            Debug.Log("AbilitiesMenuController: GoBack called");
            if (isAnimating)
            {
                Debug.Log("AbilitiesMenuController: GoBack blocked - already animating");
                return;
            }

            StartCoroutine(AnimateSlideOut());
        }

        private void PlayButtonClickSound()
        {
            if (AudioManager.Instance != null)
            {
                Vector3 soundPosition = Camera.main != null ? Camera.main.transform.position : Vector3.zero;
                AudioManager.Instance.PlaySound(AudioEventType.UIButtonClick, soundPosition);
            }
        }

        private IEnumerator AnimateSlideIn()
        {
            isAnimating = true;

            var storeWindow = storeContainer.Query<VisualElement>(className: "store-window").First();
            if (storeWindow == null)
            {
                Debug.LogWarning("Could not find store-window element for animation");
                isAnimating = false;
                yield break;
            }

            float duration = 0.5f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                float eased = EaseOutBack(t);

                float yPercent = Mathf.Lerp(100, 0, eased);

                storeWindow.style.translate = new Translate(0, Length.Percent(yPercent));

                yield return null;
            }

            storeWindow.style.translate = new Translate(0, 0);
            isAnimating = false;
        }

        private IEnumerator AnimateSlideOut()
        {
            isAnimating = true;

            var storeWindow = storeContainer.Query<VisualElement>(className: "store-window").First();
            if (storeWindow == null)
            {
                Debug.LogWarning("Could not find store-window element for animation");
                isAnimating = false;
                yield break;
            }

            float duration = 0.4f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                float eased = t * t;

                float yPercent = Mathf.Lerp(0, 100, eased);

                storeWindow.style.translate = new Translate(0, Length.Percent(yPercent));

                yield return null;
            }

            if (storeContainer != null)
            {
                storeContainer.style.display = DisplayStyle.None;
            }

            isOpen = false;
            isAnimating = false;

            Debug.Log("Abilities menu closed, invoking callback");
            onBackCallback?.Invoke();
        }

        private float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;

            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }
    }
}
