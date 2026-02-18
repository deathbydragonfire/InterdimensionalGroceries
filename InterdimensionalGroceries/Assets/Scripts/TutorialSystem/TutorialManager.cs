using UnityEngine;
using System.Collections;
using System;
using InterdimensionalGroceries.AudioSystem;

namespace TutorialSystem
{
    public class TutorialManager : MonoBehaviour
    {
        public event Action<int> OnEventStarted;
        public event Action OnTutorialComplete;

        [Header("Configuration")]
        public TutorialPlaylist playlist;

        [Header("References")]
        public LocomotionTracker locomotionTracker;
        public TutorialItemInteractionTracker itemInteractionTracker;
        public TutorialInteractionTracker interactionTracker;
        public FadeController fadeController;

        [Header("Fade Settings")]
        public float fadeFromBlackDuration = 2f;

        private AudioManager audioManager;
        private TutorialObjectManager objectManager;
        private int currentEventIndex = -1;
        private bool isProcessingEvent;
        private AudioSource currentAudioSource;

        private void Awake()
        {
            audioManager = FindFirstObjectByType<AudioManager>();
            objectManager = GetComponent<TutorialObjectManager>();
        }

        private void Start()
        {
            if (locomotionTracker != null)
            {
                locomotionTracker.OnAllLocomotionUsed += HandleLocomotionComplete;
            }

            if (itemInteractionTracker != null)
            {
                itemInteractionTracker.OnItemPickedUp += HandleItemPickedUp;
                itemInteractionTracker.OnItemScanned += HandleItemScanned;
            }

            if (interactionTracker != null)
            {
                interactionTracker.OnTerminalOpened += HandleTerminalOpened;
                interactionTracker.OnShelfPlaced += HandleShelfPlaced;
            }

            if (fadeController != null)
            {
                fadeController.FadeFromBlack(fadeFromBlackDuration, () => StartCoroutine(ProcessNextEvent()));
            }
            else
            {
                StartCoroutine(ProcessNextEvent());
            }
        }

        private void OnDestroy()
        {
            if (locomotionTracker != null)
            {
                locomotionTracker.OnAllLocomotionUsed -= HandleLocomotionComplete;
            }

            if (itemInteractionTracker != null)
            {
                itemInteractionTracker.OnItemPickedUp -= HandleItemPickedUp;
                itemInteractionTracker.OnItemScanned -= HandleItemScanned;
            }

            if (interactionTracker != null)
            {
                interactionTracker.OnTerminalOpened -= HandleTerminalOpened;
                interactionTracker.OnShelfPlaced -= HandleShelfPlaced;
            }
        }

        private IEnumerator ProcessNextEvent()
        {
            if (isProcessingEvent || playlist == null || playlist.events == null)
            {
                yield break;
            }

            currentEventIndex++;

            if (currentEventIndex >= playlist.events.Length)
            {
                Debug.Log("[TutorialManager] Tutorial complete!");
                OnTutorialComplete?.Invoke();
                yield break;
            }

            isProcessingEvent = true;
            TutorialEvent currentEvent = playlist.events[currentEventIndex];

            Debug.Log($"[TutorialManager] Processing event {currentEventIndex}: {currentEvent.name} (Trigger: {currentEvent.triggerType})");

            if (currentEvent.delayBeforeTrigger > 0)
            {
                Debug.Log($"[TutorialManager] Waiting {currentEvent.delayBeforeTrigger}s before triggering");
                yield return new WaitForSeconds(currentEvent.delayBeforeTrigger);
            }

            OnEventStarted?.Invoke(currentEventIndex);

            if (currentEvent.audioClip != null)
            {
                Debug.Log($"[TutorialManager] Playing audio clip: {currentEvent.audioClip.name}");
                PlayAudioClip(currentEvent.audioClip);
            }

            if (currentEvent.gameObjectNamesToActivate != null && currentEvent.gameObjectNamesToActivate.Length > 0)
            {
                Debug.Log($"[TutorialManager] Activating {currentEvent.gameObjectNamesToActivate.Length} GameObjects");
                ActivateGameObjects(currentEvent.gameObjectNamesToActivate);
            }

            if (currentEvent.triggerType == TutorialEventTrigger.SceneLoad || 
                currentEvent.triggerType == TutorialEventTrigger.AudioClipFinished)
            {
                if (currentEvent.audioClip != null)
                {
                    Debug.Log($"[TutorialManager] Waiting for audio to finish ({currentEvent.audioClip.length}s)");
                    yield return new WaitForSeconds(currentEvent.audioClip.length);
                }

                isProcessingEvent = false;
                StartCoroutine(ProcessNextEvent());
            }
            else if (currentEvent.triggerType == TutorialEventTrigger.OrientationComplete)
            {
                Debug.Log("[TutorialManager] OrientationComplete event - triggering tutorial complete immediately");
                OnTutorialComplete?.Invoke();
                
                if (currentEvent.audioClip != null)
                {
                    Debug.Log($"[TutorialManager] Playing final audio in background ({currentEvent.audioClip.length}s)");
                }
                
                yield break;
            }
            else
            {
                Debug.Log($"[TutorialManager] Waiting for trigger: {currentEvent.triggerType}");
                isProcessingEvent = false;
            }
        }

        private void HandleLocomotionComplete()
        {
            Debug.Log("[TutorialManager] Locomotion complete event received");
            if (currentEventIndex >= 0 && currentEventIndex < playlist.events.Length)
            {
                TutorialEvent currentEvent = playlist.events[currentEventIndex];
                Debug.Log($"[TutorialManager] Current event trigger: {currentEvent.triggerType}");
                if (currentEvent.triggerType == TutorialEventTrigger.LocomotionComplete)
                {
                    Debug.Log("[TutorialManager] Advancing to next event");
                    StartCoroutine(ProcessNextEvent());
                }
            }
        }

        private void HandleItemPickedUp()
        {
            if (currentEventIndex >= 0 && currentEventIndex < playlist.events.Length)
            {
                TutorialEvent currentEvent = playlist.events[currentEventIndex];
                if (currentEvent.triggerType == TutorialEventTrigger.ItemPickedUp)
                {
                    StartCoroutine(ProcessNextEvent());
                }
            }
        }

        private void HandleItemScanned()
        {
            if (currentEventIndex >= 0 && currentEventIndex < playlist.events.Length)
            {
                TutorialEvent currentEvent = playlist.events[currentEventIndex];
                if (currentEvent.triggerType == TutorialEventTrigger.ItemScanned)
                {
                    StartCoroutine(ProcessNextEvent());
                }
            }
        }

        private void HandleTerminalOpened()
        {
            Debug.Log("[TutorialManager] Terminal opened event received");
            if (currentEventIndex >= 0 && currentEventIndex < playlist.events.Length)
            {
                TutorialEvent currentEvent = playlist.events[currentEventIndex];
                Debug.Log($"[TutorialManager] Current event trigger: {currentEvent.triggerType}");
                if (currentEvent.triggerType == TutorialEventTrigger.TerminalOpened)
                {
                    Debug.Log("[TutorialManager] Advancing to next event");
                    StartCoroutine(ProcessNextEvent());
                }
            }
        }

        private void HandleShelfPlaced()
        {
            Debug.Log("[TutorialManager] Shelf placed event received");
            if (currentEventIndex >= 0 && currentEventIndex < playlist.events.Length)
            {
                TutorialEvent currentEvent = playlist.events[currentEventIndex];
                Debug.Log($"[TutorialManager] Current event trigger: {currentEvent.triggerType}");
                if (currentEvent.triggerType == TutorialEventTrigger.ShelfPlaced)
                {
                    Debug.Log("[TutorialManager] Advancing to next event");
                    StartCoroutine(ProcessNextEvent());
                }
            }
        }

        private void PlayAudioClip(AudioClip clip)
        {
            if (audioManager != null && clip != null)
            {
                AudioSource source = audioManager.GetComponent<AudioSource>();
                if (source == null)
                {
                    source = audioManager.gameObject.AddComponent<AudioSource>();
                }
                
                currentAudioSource = source;
                currentAudioSource.clip = clip;
                currentAudioSource.volume = 1f;
                currentAudioSource.spatialBlend = 0f;
                currentAudioSource.Play();
            }
        }

        private void ActivateGameObjects(string[] objectNames)
        {
            foreach (string objectName in objectNames)
            {
                GameObject obj = null;

                // First try to get from cache (for objects deactivated by TutorialObjectManager)
                if (objectManager != null && objectManager.HasCachedObject(objectName))
                {
                    obj = objectManager.GetCachedObject(objectName);
                    if (obj != null)
                    {
                        obj.SetActive(true);
                        Debug.Log($"[TutorialManager] Activated GameObject (cached): {objectName}");
                        continue;
                    }
                }

                // Try to find active root-level objects
                obj = GameObject.Find(objectName);
                if (obj != null)
                {
                    obj.SetActive(true);
                    Debug.Log($"[TutorialManager] Activated GameObject: {objectName}");
                }
                else
                {
                    // Deep search for inactive objects (this won't work for deactivated objects)
                    GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
                    foreach (GameObject searchObj in allObjects)
                    {
                        if (searchObj.name == objectName)
                        {
                            searchObj.SetActive(true);
                            Debug.Log($"[TutorialManager] Activated GameObject (deep search): {objectName}");
                            break;
                        }
                    }
                }
            }
        }

        public TutorialEvent GetCurrentEvent()
        {
            if (currentEventIndex >= 0 && currentEventIndex < playlist.events.Length)
            {
                return playlist.events[currentEventIndex];
            }
            return null;
        }
    }
}
