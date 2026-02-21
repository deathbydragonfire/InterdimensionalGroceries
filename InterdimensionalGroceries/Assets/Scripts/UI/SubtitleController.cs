using UnityEngine;
using TMPro;
using System.Collections;

namespace InterdimensionalGroceries.UI
{
    public class SubtitleController : MonoBehaviour
    {
        public static SubtitleController Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject subtitlePanel;
        [SerializeField] private TextMeshProUGUI subtitleText;

        [Header("Glitch Characters")]
        [SerializeField] private string glitchCharacters = "!@#$%^&*()_+-=[]{}|;:,.<>?/~`";

        private Coroutine currentSubtitleCoroutine;
        private TMP_MeshInfo[] cachedMeshInfo;
        private bool isShaking = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (subtitlePanel != null)
            {
                subtitlePanel.SetActive(false);
            }
        }

        public void ShowSubtitle(SubtitleData data, float audioDuration = 0f)
        {
            if (data == null)
            {
                Debug.LogWarning("[SubtitleController] Subtitle data is null");
                return;
            }

            if (currentSubtitleCoroutine != null)
            {
                StopCoroutine(currentSubtitleCoroutine);
            }

            float duration = data.DisplayDuration > 0 ? data.DisplayDuration : audioDuration;

            switch (data.DisplayMode)
            {
                case SubtitleDisplayMode.Typewriter:
                    currentSubtitleCoroutine = StartCoroutine(TypewriterEffect(data.SubtitleText, data.CharacterDelay, duration));
                    break;
                case SubtitleDisplayMode.SuperGlitchy:
                    currentSubtitleCoroutine = StartCoroutine(SuperGlitchEffect(data.SubtitleText, data.GlitchIntensity, duration));
                    break;
                case SubtitleDisplayMode.ShakingGlitchy:
                    currentSubtitleCoroutine = StartCoroutine(ShakingGlitchEffect(data.SubtitleText, data.GlitchIntensity, duration));
                    break;
            }
        }

        public void HideSubtitle()
        {
            if (currentSubtitleCoroutine != null)
            {
                StopCoroutine(currentSubtitleCoroutine);
                currentSubtitleCoroutine = null;
            }

            isShaking = false;

            if (subtitlePanel != null)
            {
                subtitlePanel.SetActive(false);
            }

            if (subtitleText != null)
            {
                subtitleText.text = "";
            }
        }

        private IEnumerator TypewriterEffect(string text, float characterDelay, float totalDuration)
        {
            if (subtitlePanel != null)
            {
                subtitlePanel.SetActive(true);
            }

            if (subtitleText == null)
            {
                yield break;
            }

            subtitleText.text = "";

            for (int i = 0; i < text.Length; i++)
            {
                subtitleText.text = text.Substring(0, i + 1);
                yield return new WaitForSeconds(characterDelay);
            }

            float displayTime = text.Length * characterDelay;
            if (totalDuration > displayTime)
            {
                yield return new WaitForSeconds(totalDuration - displayTime);
            }

            HideSubtitle();
        }

        private IEnumerator SuperGlitchEffect(string text, float intensity, float duration)
        {
            if (subtitlePanel != null)
            {
                subtitlePanel.SetActive(true);
            }

            if (subtitleText == null)
            {
                yield break;
            }

            float elapsed = 0f;

            while (elapsed < duration)
            {
                string glitchedText = ApplySuperGlitch(text, intensity);
                subtitleText.text = glitchedText;

                Color randomColor = new Color(
                    Random.Range(0.8f, 1f),
                    Random.Range(0.7f, 1f),
                    Random.Range(0.8f, 1f)
                );
                subtitleText.color = randomColor;

                yield return new WaitForSeconds(0.05f);
                elapsed += 0.05f;
            }

            HideSubtitle();
        }

        private IEnumerator ShakingGlitchEffect(string text, float intensity, float duration)
        {
            if (subtitlePanel != null)
            {
                subtitlePanel.SetActive(true);
            }

            if (subtitleText == null)
            {
                yield break;
            }

            subtitleText.text = text;
            subtitleText.ForceMeshUpdate();

            isShaking = true;
            float elapsed = 0f;

            while (elapsed < duration && isShaking)
            {
                subtitleText.ForceMeshUpdate();
                TMP_TextInfo textInfo = subtitleText.textInfo;

                if (textInfo.characterCount == 0)
                {
                    yield return null;
                    continue;
                }

                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                    if (!charInfo.isVisible)
                        continue;

                    int materialIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;

                    Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;

                    float shakeAmount = intensity * 3f;
                    Vector3 offset = new Vector3(
                        Random.Range(-shakeAmount, shakeAmount),
                        Random.Range(-shakeAmount, shakeAmount),
                        0f
                    );

                    sourceVertices[vertexIndex + 0] += offset;
                    sourceVertices[vertexIndex + 1] += offset;
                    sourceVertices[vertexIndex + 2] += offset;
                    sourceVertices[vertexIndex + 3] += offset;

                    if (Random.value < intensity * 0.1f)
                    {
                        Color32[] colors = textInfo.meshInfo[materialIndex].colors32;
                        Color32 glitchColor = new Color32(
                            (byte)Random.Range(200, 255),
                            (byte)Random.Range(200, 255),
                            (byte)Random.Range(200, 255),
                            255
                        );

                        colors[vertexIndex + 0] = glitchColor;
                        colors[vertexIndex + 1] = glitchColor;
                        colors[vertexIndex + 2] = glitchColor;
                        colors[vertexIndex + 3] = glitchColor;
                    }
                }

                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                    subtitleText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                }

                yield return new WaitForSeconds(0.033f);
                elapsed += 0.033f;
            }

            isShaking = false;
            HideSubtitle();
        }

        private string ApplySuperGlitch(string text, float intensity)
        {
            char[] chars = text.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (Random.value < intensity * 0.5f)
                {
                    if (chars[i] != ' ')
                    {
                        chars[i] = glitchCharacters[Random.Range(0, glitchCharacters.Length)];
                    }
                }
            }

            return new string(chars);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
