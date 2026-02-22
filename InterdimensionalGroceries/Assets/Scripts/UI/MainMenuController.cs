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
        private Button continueButton;
        private Button settingsButton;
        private Button quitButton;
        private VisualElement mainMenuContainer;

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
            
            newGameButton = root.Q<Button>("NewGameButton");
            continueButton = root.Q<Button>("ContinueButton");
            settingsButton = root.Q<Button>("SettingsButton");
            quitButton = root.Q<Button>("QuitButton");

            Debug.Log($"[MainMenuController] Buttons found - New:{newGameButton != null}, Continue:{continueButton != null}, Settings:{settingsButton != null}, Quit:{quitButton != null}");

            if (newGameButton != null)
            {
                newGameButton.RegisterCallback<ClickEvent>(OnNewGameClicked);
                Debug.Log("[MainMenuController] NewGameButton click registered");
            }
            
            if (continueButton != null)
            {
                continueButton.RegisterCallback<ClickEvent>(OnContinueClicked);
            }
            
            if (settingsButton != null)
            {
                settingsButton.RegisterCallback<ClickEvent>(OnSettingsClicked);
            }
            
            if (quitButton != null)
            {
                quitButton.RegisterCallback<ClickEvent>(OnQuitClicked);
            }

            UpdateContinueButton();
        }

        private void OnDisable()
        {
            newGameButton?.UnregisterCallback<ClickEvent>(OnNewGameClicked);
            continueButton?.UnregisterCallback<ClickEvent>(OnContinueClicked);
            settingsButton?.UnregisterCallback<ClickEvent>(OnSettingsClicked);
            quitButton?.UnregisterCallback<ClickEvent>(OnQuitClicked);
        }

        private void UpdateContinueButton()
        {
            bool hasSaveData = SaveDataManager.HasSaveData();
            continueButton?.SetEnabled(hasSaveData);
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
            if (AbilityUpgradeManager.Instance != null)
            {
                AbilityUpgradeManager.Instance.ResetAllUpgrades();
            }
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
