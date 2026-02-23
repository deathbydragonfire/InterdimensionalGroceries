using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using InterdimensionalGroceries.PlayerController;
using InterdimensionalGroceries.Core;

namespace InterdimensionalGroceries.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class PauseMenuController : MonoBehaviour
    {
        private UIDocument uiDocument;
        private VisualElement pauseMenuContainer;
        private Button resumeButton;
        private Button saveButton;
        private Button quitButton;
        
        private bool isPaused = false;
        private InputSystem_Actions inputActions;
        private float previousTimeScale = 1f;
        private FirstPersonController playerController;
        private string originalSaveButtonText;

        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            inputActions = new InputSystem_Actions();
            
            playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<FirstPersonController>();
            if (playerController == null)
            {
                Debug.LogWarning("[PauseMenuController] Could not find FirstPersonController on Player");
            }
        }

        private void OnEnable()
        {
            var root = uiDocument.rootVisualElement;
            
            pauseMenuContainer = root.Q<VisualElement>("PauseMenuContainer");
            resumeButton = root.Q<Button>("ResumeButton");
            saveButton = root.Q<Button>("SaveButton");
            quitButton = root.Q<Button>("QuitButton");

            if (resumeButton != null)
            {
                resumeButton.RegisterCallback<ClickEvent>(OnResumeClicked);
                resumeButton.RegisterCallback<MouseEnterEvent>(OnButtonHover);
                resumeButton.RegisterCallback<MouseLeaveEvent>(OnButtonUnhover);
            }
            
            if (saveButton != null)
            {
                originalSaveButtonText = saveButton.text;
                saveButton.RegisterCallback<ClickEvent>(OnSaveClicked);
                saveButton.RegisterCallback<MouseEnterEvent>(OnButtonHover);
                saveButton.RegisterCallback<MouseLeaveEvent>(OnButtonUnhover);
            }
            
            if (quitButton != null)
            {
                quitButton.RegisterCallback<ClickEvent>(OnQuitClicked);
                quitButton.RegisterCallback<MouseEnterEvent>(OnButtonHover);
                quitButton.RegisterCallback<MouseLeaveEvent>(OnButtonUnhover);
            }

            inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            if (resumeButton != null)
            {
                resumeButton.UnregisterCallback<ClickEvent>(OnResumeClicked);
                resumeButton.UnregisterCallback<MouseEnterEvent>(OnButtonHover);
                resumeButton.UnregisterCallback<MouseLeaveEvent>(OnButtonUnhover);
            }
            
            if (saveButton != null)
            {
                saveButton.UnregisterCallback<ClickEvent>(OnSaveClicked);
                saveButton.UnregisterCallback<MouseEnterEvent>(OnButtonHover);
                saveButton.UnregisterCallback<MouseLeaveEvent>(OnButtonUnhover);
            }
            
            if (quitButton != null)
            {
                quitButton.UnregisterCallback<ClickEvent>(OnQuitClicked);
                quitButton.UnregisterCallback<MouseEnterEvent>(OnButtonHover);
                quitButton.UnregisterCallback<MouseLeaveEvent>(OnButtonUnhover);
            }

            if (inputActions != null)
            {
                inputActions.Player.Disable();
            }
        }

        private void OnDestroy()
        {
            inputActions?.Dispose();
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                TogglePause();
            }
        }

        private void TogglePause()
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        private void Pause()
        {
            isPaused = true;
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            
            if (pauseMenuContainer != null)
            {
                pauseMenuContainer.style.display = DisplayStyle.Flex;
            }
            
            if (playerController != null)
            {
                playerController.SetControlsEnabled(false);
            }
            
            ShowCursor();
        }

        private void Resume()
        {
            isPaused = false;
            Time.timeScale = previousTimeScale;
            
            if (pauseMenuContainer != null)
            {
                pauseMenuContainer.style.display = DisplayStyle.None;
            }
            
            if (playerController != null)
            {
                playerController.SetControlsEnabled(true);
            }
            
            HideCursor();
        }

        private void OnResumeClicked(ClickEvent evt)
        {
            Resume();
        }

        private void OnSaveClicked(ClickEvent evt)
        {
            SaveDataManager.SaveGame();
            StartCoroutine(ShowSaveConfirmation());
        }

        private IEnumerator ShowSaveConfirmation()
        {
            if (saveButton != null)
            {
                saveButton.text = "Game Saved!";
                yield return new WaitForSecondsRealtime(2f);
                saveButton.text = originalSaveButtonText;
            }
        }

        private void OnQuitClicked(ClickEvent evt)
        {
            Time.timeScale = 1f;
            
            if (playerController != null)
            {
                playerController.SetControlsEnabled(true);
            }
            
            SceneManager.LoadScene("Intro");
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

        private void ShowCursor()
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }

        private void HideCursor()
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
