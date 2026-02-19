using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InterdimensionalGroceries.PhaseManagement;

namespace InterdimensionalGroceries.UI
{
    public class CustomerScreenEyeAnimator : MonoBehaviour
    {
        [Header("Eye References")]
        [SerializeField] private SpriteRenderer eye1SpriteRenderer;
        [SerializeField] private SpriteRenderer eye2SpriteRenderer;
        [SerializeField] private GameObject eye1GameObject;
        [SerializeField] private GameObject eye2GameObject;

        [Header("Animation Settings")]
        [SerializeField] private float minAnimationInterval = 3f;
        [SerializeField] private float maxAnimationInterval = 5f;

        private Sprite[] eyeSprites;
        private Coroutine animationCoroutine;
        private const string EYE_SPRITES_PATH = "Assets/Art/Eyes/Eye Movement";

        private void Start()
        {
            LoadEyeSprites();
            SubscribeToPhaseEvents();
            
            if (GamePhaseManager.Instance != null && GamePhaseManager.Instance.CurrentPhase == GamePhase.InventoryPhase)
            {
                StartEyeAnimation();
            }
            else
            {
                SetEyeVisibility(false);
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromPhaseEvents();
        }

        private void LoadEyeSprites()
        {
#if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Sprite", new[] { EYE_SPRITES_PATH });
            eyeSprites = new Sprite[guids.Length];
            
            for (int i = 0; i < guids.Length; i++)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                eyeSprites[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
#else
            eyeSprites = new Sprite[0];
            Debug.LogWarning("CustomerScreenEyeAnimator: Eye sprites can only be loaded in the editor. Consider using Resources folder or serialized sprite array for builds.");
#endif
        }

        private void SubscribeToPhaseEvents()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnInventoryPhaseStarted += OnInventoryPhaseStarted;
                GamePhaseManager.Instance.OnDeliveryPhaseStarted += OnDeliveryPhaseStarted;
            }
        }

        private void UnsubscribeFromPhaseEvents()
        {
            if (GamePhaseManager.Instance != null)
            {
                GamePhaseManager.Instance.OnInventoryPhaseStarted -= OnInventoryPhaseStarted;
                GamePhaseManager.Instance.OnDeliveryPhaseStarted -= OnDeliveryPhaseStarted;
            }
        }

        private void OnInventoryPhaseStarted()
        {
            SetEyeVisibility(true);
            StartEyeAnimation();
        }

        private void OnDeliveryPhaseStarted()
        {
            StopEyeAnimation();
            SetEyeVisibility(false);
        }

        private void StartEyeAnimation()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }
            animationCoroutine = StartCoroutine(AnimateEyes());
        }

        private void StopEyeAnimation()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }
        }

        private IEnumerator AnimateEyes()
        {
            while (true)
            {
                float waitTime = Random.Range(minAnimationInterval, maxAnimationInterval);
                yield return new WaitForSeconds(waitTime);

                if (eyeSprites != null && eyeSprites.Length > 0)
                {
                    Sprite randomSprite = eyeSprites[Random.Range(0, eyeSprites.Length)];
                    
                    if (eye1SpriteRenderer != null)
                    {
                        eye1SpriteRenderer.sprite = randomSprite;
                    }
                    
                    if (eye2SpriteRenderer != null)
                    {
                        eye2SpriteRenderer.sprite = randomSprite;
                    }
                }
            }
        }

        private void SetEyeVisibility(bool visible)
        {
            if (eye1GameObject != null)
            {
                eye1GameObject.SetActive(visible);
            }
            
            if (eye2GameObject != null)
            {
                eye2GameObject.SetActive(visible);
            }
        }
    }
}
