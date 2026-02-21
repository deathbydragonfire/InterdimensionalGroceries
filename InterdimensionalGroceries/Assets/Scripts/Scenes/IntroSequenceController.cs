using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using InterdimensionalGroceries.PlayerController;
using InterdimensionalGroceries.UI;
using InterdimensionalGroceries.AudioSystem;
using TutorialSystem;

namespace InterdimensionalGroceries.Scenes
{
    public class IntroSequenceController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform targetCameraPoint;
        [SerializeField] private GameObject playerGameObject;
        [SerializeField] private Transform walkDestination;
        [SerializeField] private PortalController portalController;
        [SerializeField] private FadeController fadeController;
        [SerializeField] private MenuCameraRotation menuCameraRotation;
        [SerializeField] private VoiceActingSequencer voiceActingSequencer;
        [SerializeField] private IntroPanicSoundManager panicSoundManager;
        [SerializeField] private ScreenGlitchController screenGlitchController;
        
        [Header("Glitch Audio")]
        [SerializeField] private AudioClipData preGlitchWarningSound;
        [SerializeField] private AudioClipData smallGlitchSound;
        [SerializeField] private AudioClipData betweenGlitchesSound;
        [SerializeField] private AudioClipData largeGlitchSound;

        [Header("Phase Timings")]
        [SerializeField] private float uiFadeDuration = 1f;
        [SerializeField] private float cameraRotationDuration = 2f;
        [SerializeField] private float waitBeforeWalk = 0.5f;
        [SerializeField] private float waitBeforePanic = 2.5f;
        [SerializeField] private float waitAfterPanic = 1f;
        [SerializeField] private float portalOpenDuration = 1f;
        [SerializeField] private float waitBeforeFall = 2f;
        [SerializeField] private float fallDuration = 1f;
        [SerializeField] private float fadeToWhiteDuration = 2f;
        
        [Header("Glitch Timings")]
        [SerializeField] private float preGlitchWarningDelay = 1.0f;
        [SerializeField] private float timeBeforeFirstGlitch = 2.0f;
        [SerializeField] private float smallGlitchDuration = 0.2f;
        [SerializeField] private float timeBetweenGlitches = 1.0f;
        [SerializeField] private float largeGlitchDuration = 0.4f;

        [Header("Scene Settings")]
        [SerializeField] private string tutorialSceneName = "Tutorial";

        private FirstPersonController firstPersonController;
        private PlayerAutoWalk playerAutoWalk;
        private CameraLookSequence cameraLookSequence;
        private OrganicCameraLook organicCameraLook;
        private IntroCameraRotation introCameraRotation;
        private bool sequenceRunning = false;

        private void Awake()
        {
            if (cameraTransform == null)
            {
                cameraTransform = Camera.main.transform;
            }

            if (playerGameObject != null)
            {
                firstPersonController = playerGameObject.GetComponent<FirstPersonController>();
                playerAutoWalk = playerGameObject.GetComponent<PlayerAutoWalk>();
                cameraLookSequence = playerGameObject.GetComponent<CameraLookSequence>();
                organicCameraLook = playerGameObject.GetComponent<OrganicCameraLook>();
                introCameraRotation = playerGameObject.GetComponent<IntroCameraRotation>();
                
                if (introCameraRotation == null)
                {
                    introCameraRotation = playerGameObject.AddComponent<IntroCameraRotation>();
                }
            }
        }

        public void StartIntroSequence()
        {
            StartIntroSequence(0f, 10f);
        }

        public void StartIntroSequence(float initialYRotation, float rotationSpeed)
        {
            if (sequenceRunning)
            {
                Debug.LogWarning("[IntroSequenceController] Intro sequence is already running!");
                return;
            }

            if (introCameraRotation != null)
            {
                introCameraRotation.SetRotationSpeed(rotationSpeed);
                introCameraRotation.StartRotation();
            }

            if (voiceActingSequencer != null)
            {
                voiceActingSequencer.StartVoiceSequence();
            }

            StartCoroutine(IntroSequenceCoroutine());
        }

        private IEnumerator IntroSequenceCoroutine()
        {
            sequenceRunning = true;

            if (menuCameraRotation != null)
            {
                menuCameraRotation.StopRotation();
            }

            yield return new WaitForSeconds(0.5f);

            yield return RotateCameraToTarget();

            if (firstPersonController != null)
            {
                firstPersonController.SetControlsEnabled(false);
            }

            yield return new WaitForSeconds(waitBeforeWalk);

            if (organicCameraLook != null)
            {
                organicCameraLook.StartOrganicLooking();
            }

            if (playerAutoWalk != null && walkDestination != null)
            {
                bool walkCompleted = false;
                playerAutoWalk.StartWalking(walkDestination.position, () => walkCompleted = true);
                
                yield return new WaitUntil(() => walkCompleted);
            }

            if (organicCameraLook != null)
            {
                organicCameraLook.StopOrganicLooking();
            }

            float glitchSequenceTime = preGlitchWarningDelay + timeBeforeFirstGlitch + smallGlitchDuration + timeBetweenGlitches + largeGlitchDuration;
            yield return new WaitForSeconds(waitBeforePanic - glitchSequenceTime);
            
            if (preGlitchWarningSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(preGlitchWarningSound, Camera.main.transform.position);
            }
            
            yield return new WaitForSeconds(preGlitchWarningDelay);
            
            if (screenGlitchController != null)
            {
                screenGlitchController.PlaySmallGlitch(smallGlitchDuration);
                
                if (smallGlitchSound != null && AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound(smallGlitchSound, Camera.main.transform.position);
                }
            }
            
            yield return new WaitForSeconds(smallGlitchDuration);
            
            if (betweenGlitchesSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(betweenGlitchesSound, Camera.main.transform.position);
                
                if (SubtitleController.Instance != null && betweenGlitchesSound.Subtitles != null && betweenGlitchesSound.Subtitles.Length > 0)
                {
                    SubtitleData subtitle = betweenGlitchesSound.GetRandomSubtitle();
                    if (subtitle != null)
                    {
                        AudioClip clip = betweenGlitchesSound.GetRandomClip();
                        float duration = clip != null ? clip.length : 2f;
                        SubtitleController.Instance.ShowSubtitle(subtitle, duration);
                    }
                }
            }
            
            yield return new WaitForSeconds(timeBetweenGlitches);
            
            if (screenGlitchController != null)
            {
                screenGlitchController.PlayLargeGlitch(largeGlitchDuration);
                
                if (largeGlitchSound != null && AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySound(largeGlitchSound, Camera.main.transform.position);
                }
            }
            
            yield return new WaitForSeconds(largeGlitchDuration);

            if (voiceActingSequencer != null)
            {
                voiceActingSequencer.StopVoiceSequence();
                Debug.Log("[IntroSequenceController] Stopped voice acting for panic/portal sequence");
            }

            if (panicSoundManager != null)
            {
                panicSoundManager.StartPanicSoundSequence();
                Debug.Log("[IntroSequenceController] Started panic sound sequence");
            }

            if (cameraLookSequence != null)
            {
                bool panicCompleted = false;
                cameraLookSequence.StartPanicSequence(
                    () => panicCompleted = true,
                    () => {
                        if (portalController != null)
                        {
                            portalController.OpenPortal(portalOpenDuration);
                        }
                        if (panicSoundManager != null)
                        {
                            panicSoundManager.PlayPortalOpening();
                        }
                    }
                );
                
                yield return new WaitUntil(() => panicCompleted);
            }

            yield return new WaitForSeconds(waitAfterPanic);

            yield return SimulateFalling();

            if (fadeController != null)
            {
                bool fadeCompleted = false;
                fadeController.FadeToWhite(fadeToWhiteDuration, () => fadeCompleted = true);
                
                yield return new WaitUntil(() => fadeCompleted);
            }
            else
            {
                yield return new WaitForSeconds(fadeToWhiteDuration);
            }

            yield return new WaitForSeconds(0.5f);

            Debug.Log($"[IntroSequenceController] Loading scene: {tutorialSceneName}");
            SceneManager.LoadScene(tutorialSceneName, LoadSceneMode.Single);

            sequenceRunning = false;
        }

        private IEnumerator RotateCameraToTarget()
        {
            if (targetCameraPoint == null || cameraTransform == null)
            {
                yield break;
            }

            if (introCameraRotation != null)
            {
                introCameraRotation.StopRotation();
            }

            Transform playerTransform = playerGameObject.transform;
            Quaternion startRotation = playerTransform.rotation;
            
            Vector3 directionToTarget = targetCameraPoint.position - cameraTransform.position;
            directionToTarget.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            float elapsed = 0f;
            while (elapsed < cameraRotationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / cameraRotationDuration;
                float smoothT = Mathf.SmoothStep(0f, 1f, t);
                
                playerTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, smoothT);
                yield return null;
            }

            playerTransform.rotation = targetRotation;
        }

        private IEnumerator SimulateFalling()
        {
            if (playerGameObject == null)
            {
                yield break;
            }
            
            if (MusicManager.Instance != null)
            {
                MusicManager.Instance.FadeOutMusic(fallDuration);
                Debug.Log("[IntroSequenceController] Fading out music as player falls into portal");
            }

            if (cameraLookSequence != null)
            {
                StartCoroutine(cameraLookSequence.LookUpWhileFalling());
            }

            Vector3 startPosition = playerGameObject.transform.position;
            Vector3 endPosition = startPosition + Vector3.down * 10f;

            float elapsed = 0f;
            while (elapsed < fallDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fallDuration;
                float easedT = t * t;
                playerGameObject.transform.position = Vector3.Lerp(startPosition, endPosition, easedT);
                yield return null;
            }
        }
    }
}
