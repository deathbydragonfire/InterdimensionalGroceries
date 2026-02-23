using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using TutorialSystem;
using InterdimensionalGroceries.Core;
using InterdimensionalGroceries.Scenes;
using InterdimensionalGroceries.EconomySystem;

namespace InterdimensionalGroceries.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private IntroSequenceController introSequenceController;
        [SerializeField] private FadeController fadeController;
        [SerializeField] private GameObject mainMenuCamera;
        [SerializeField] private GameObject player;
        [SerializeField] private float uiFadeDuration = 0.7f;

        private UIDocument uiDocument;
        private Button newGameButton;
        private Button savedGameButton;
        private Button settingsButton;
        private Button quitButton;
        private VisualElement mainMenuContainer;
        private VisualElement mainMenuContent;
        
        private VisualElement saveDataSubmenu;
        private Button continueSaveButton;
        private Button deleteSaveButton;
        private Button backButton;

        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
        }

        private void Start()
        {
            Debug.Log("[MainMenuController] Start called");
            
            if (mainMenuCamera == null)
            {
                mainMenuCamera = GameObject.Find("MainMenuCamera");
            }
            
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
            
            if (mainMenuCamera != null)
            {
                mainMenuCamera.SetActive(true);
            }
            
            if (player != null)
            {
                player.SetActive(false);
            }
            
            ShowCursor();
        }

        private void OnEnable()
        {
            var root = uiDocument.rootVisualElement;
            
            Debug.Log($"[MainMenuController] Root element: {root}");
            Debug.Log($"[MainMenuController] Root children count: {root?.childCount}");
            
            mainMenuContainer = root.Q<VisualElement>("MainMenuContainer");
            Debug.Log($"[MainMenuController] MainMenuContainer found: {mainMenuContainer != null}");
            
            mainMenuContent = root.Q<VisualElement>(className: "main-menu-container");
            Debug.Log($"[MainMenuController] mainMenuContent found: {mainMenuContent != null}");
            
            newGameButton = root.Q<Button>("NewGameButton");
            savedGameButton = root.Q<Button>("SavedGameButton");
            settingsButton = root.Q<Button>("SettingsButton");
            quitButton = root.Q<Button>("QuitButton");
            
            saveDataSubmenu = root.Q<VisualElement>("SaveDataSubmenu");
            continueSaveButton = root.Q<Button>("ContinueSaveButton");
            deleteSaveButton = root.Q<Button>("DeleteSaveButton");
            backButton = root.Q<Button>("BackButton");

            Debug.Log($"[MainMenuController] Buttons found - New:{newGameButton != null}, SavedGame:{savedGameButton != null}, Settings:{settingsButton != null}, Quit:{quitButton != null}");

            if (newGameButton != null)
            {
                newGameButton.RegisterCallback<ClickEvent>(OnNewGameClicked);
                Debug.Log("[MainMenuController] NewGameButton click registered");
            }
            
            if (savedGameButton != null)
            {
                savedGameButton.RegisterCallback<ClickEvent>(OnSavedGameClicked);
            }
            
            if (settingsButton != null)
            {
                settingsButton.RegisterCallback<ClickEvent>(OnSettingsClicked);
            }
            
            if (quitButton != null)
            {
                quitButton.RegisterCallback<ClickEvent>(OnQuitClicked);
            }
            
            if (continueSaveButton != null)
            {
                continueSaveButton.RegisterCallback<ClickEvent>(OnContinueSaveClicked);
                continueSaveButton.RegisterCallback<MouseEnterEvent>(OnButtonHover);
                continueSaveButton.RegisterCallback<MouseLeaveEvent>(OnButtonUnhover);
            }
            
            if (deleteSaveButton != null)
            {
                deleteSaveButton.RegisterCallback<ClickEvent>(OnDeleteSaveClicked);
                deleteSaveButton.RegisterCallback<MouseEnterEvent>(OnButtonHover);
                deleteSaveButton.RegisterCallback<MouseLeaveEvent>(OnButtonUnhover);
            }
            
            if (backButton != null)
            {
                backButton.RegisterCallback<ClickEvent>(OnBackClicked);
                backButton.RegisterCallback<MouseEnterEvent>(OnButtonHover);
                backButton.RegisterCallback<MouseLeaveEvent>(OnButtonUnhover);
            }

            UpdateSavedGameButton();
        }

        private void OnDisable()
        {
            newGameButton?.UnregisterCallback<ClickEvent>(OnNewGameClicked);
            savedGameButton?.UnregisterCallback<ClickEvent>(OnSavedGameClicked);
            settingsButton?.UnregisterCallback<ClickEvent>(OnSettingsClicked);
            quitButton?.UnregisterCallback<ClickEvent>(OnQuitClicked);
            
            if (continueSaveButton != null)
            {
                continueSaveButton.UnregisterCallback<ClickEvent>(OnContinueSaveClicked);
                continueSaveButton.UnregisterCallback<MouseEnterEvent>(OnButtonHover);
                continueSaveButton.UnregisterCallback<MouseLeaveEvent>(OnButtonUnhover);
            }
            
            if (deleteSaveButton != null)
            {
                deleteSaveButton.UnregisterCallback<ClickEvent>(OnDeleteSaveClicked);
                deleteSaveButton.UnregisterCallback<MouseEnterEvent>(OnButtonHover);
                deleteSaveButton.UnregisterCallback<MouseLeaveEvent>(OnButtonUnhover);
            }
            
            if (backButton != null)
            {
                backButton.UnregisterCallback<ClickEvent>(OnBackClicked);
                backButton.UnregisterCallback<MouseEnterEvent>(OnButtonHover);
                backButton.UnregisterCallback<MouseLeaveEvent>(OnButtonUnhover);
            }
        }

        private void UpdateSavedGameButton()
        {
            bool hasSaveData = SaveDataManager.HasSaveData();
            savedGameButton?.SetEnabled(hasSaveData);
        }

        private void OnNewGameClicked(ClickEvent evt)
        {
            Debug.Log("[MainMenuController] New Game button clicked!");
            StartCoroutine(StartNewGameSequence());
        }

        private System.Collections.IEnumerator StartNewGameSequence()
        {
            ResetGameState();
            
            float menuCameraRotation = 0f;
            MenuCameraRotation menuRotation = null;
            
            if (mainMenuCamera != null)
            {
                menuRotation = mainMenuCamera.GetComponent<MenuCameraRotation>();
                menuCameraRotation = mainMenuCamera.transform.eulerAngles.y;
            }
            
            yield return StartCoroutine(FadeOutUI());
            
            SwitchToPlayerCamera(menuCameraRotation);
            
            if (menuRotation != null)
            {
                menuRotation.StopRotation();
            }
            
            if (introSequenceController != null)
            {
                float rotationSpeed = menuRotation != null ? menuRotation.GetRotationSpeed() : 10f;
                introSequenceController.StartIntroSequence(menuCameraRotation, rotationSpeed);
            }
        }

        private void ResetGameState()
        {
            SaveDataManager.ClearSaveData();
            
            if (AbilityUpgradeManager.Instance != null)
            {
                AbilityUpgradeManager.Instance.ResetAllUpgrades();
            }
            else
            {
                ResetAbilityUpgrades();
            }
        }

        private void ResetAbilityUpgrades()
        {
            string[] upgradeKeys = new string[]
            {
                "Upgrade_ThrowingStrength_Throwing Strength",
                "Upgrade_MovementSpeed_Movement Speed",
                "Upgrade_DeliveryTime_Delivery Time"
            };

            foreach (string key in upgradeKeys)
            {
                PlayerPrefs.SetInt(key, 0);
            }
            
            PlayerPrefs.Save();
            Debug.Log("[MainMenuController] Reset ability upgrades via PlayerPrefs (manager not yet initialized)");
        }

        private System.Collections.IEnumerator FadeOutUI()
        {
            if (mainMenuContainer == null)
            {
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < uiFadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / uiFadeDuration;
                mainMenuContainer.style.opacity = 1f - t;
                yield return null;
            }
            
            mainMenuContainer.style.opacity = 0f;
            mainMenuContainer.style.display = DisplayStyle.None;
        }

        private void OnContinueClicked(ClickEvent evt)
        {
            Debug.Log("[MainMenuController] Continue button clicked!");
            SaveDataManager.LoadGame();
        }

        private void OnSavedGameClicked(ClickEvent evt)
        {
            Debug.Log("[MainMenuController] Saved Game button clicked!");
            ShowSubmenu();
        }

        private void OnContinueSaveClicked(ClickEvent evt)
        {
            Debug.Log("[MainMenuController] Continue Save button clicked!");
            SaveDataManager.LoadGame();
        }

        private void OnDeleteSaveClicked(ClickEvent evt)
        {
            Debug.Log("[MainMenuController] Delete Save button clicked!");
            
            SaveDataManager.ClearSaveData();
            
            if (AbilityUpgradeManager.Instance != null)
            {
                AbilityUpgradeManager.Instance.ResetAllUpgrades();
            }
            
            UpdateSavedGameButton();
            HideSubmenu();
        }

        private void OnBackClicked(ClickEvent evt)
        {
            Debug.Log("[MainMenuController] Back button clicked!");
            HideSubmenu();
        }

        private void ShowSubmenu()
        {
            if (saveDataSubmenu != null)
            {
                saveDataSubmenu.style.display = DisplayStyle.Flex;
            }
            
            HideMainMenu();
        }

        private void HideSubmenu()
        {
            if (saveDataSubmenu != null)
            {
                saveDataSubmenu.style.display = DisplayStyle.None;
            }
            
            ShowMainMenu();
        }

        private void HideMainMenu()
        {
            if (mainMenuContent != null)
            {
                mainMenuContent.style.display = DisplayStyle.None;
            }
        }

        private void ShowMainMenu()
        {
            if (mainMenuContent != null)
            {
                mainMenuContent.style.display = DisplayStyle.Flex;
            }
        }

        private void OnButtonHover(MouseEnterEvent evt)
        {
            if (evt.target is Button button)
            {
                button.style.textShadow = new TextShadow
                {
                    offset = Vector2.zero,
                    blurRadius = 30f,
                    color = new Color(1f, 1f, 1f, 1f)
                };
            }
        }

        private void OnButtonUnhover(MouseLeaveEvent evt)
        {
            if (evt.target is Button button)
            {
                button.style.textShadow = StyleKeyword.Null;
            }
        }

        private void OnSettingsClicked(ClickEvent evt)
        {
            Debug.Log("[MainMenuController] Settings button clicked!");
        }

        private void OnQuitClicked(ClickEvent evt)
        {
            Debug.Log("[MainMenuController] Quit button clicked!");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public void HideUI()
        {
            if (mainMenuContainer != null)
            {
                mainMenuContainer.style.display = DisplayStyle.None;
            }
        }

        public void ShowUI()
        {
            if (mainMenuContainer != null)
            {
                mainMenuContainer.style.display = DisplayStyle.Flex;
            }
        }

        private void SwitchToPlayerCamera()
        {
            SwitchToPlayerCamera(0f);
        }

        private void SwitchToPlayerCamera(float yRotation)
        {
            if (mainMenuCamera != null)
            {
                mainMenuCamera.SetActive(false);
            }
            
            if (player != null)
            {
                Vector3 currentEulerAngles = player.transform.eulerAngles;
                player.transform.eulerAngles = new Vector3(currentEulerAngles.x, yRotation, currentEulerAngles.z);
                player.SetActive(true);
            }
        }

        private void ShowCursor()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }

        private void DisablePlayerControls()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                var firstPersonController = playerObj.GetComponent<InterdimensionalGroceries.PlayerController.FirstPersonController>();
                if (firstPersonController != null)
                {
                    firstPersonController.SetControlsEnabled(false);
                }
            }
        }
    }
}
