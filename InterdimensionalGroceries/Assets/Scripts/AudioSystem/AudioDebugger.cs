using UnityEngine;
using UnityEngine.InputSystem;

namespace InterdimensionalGroceries.AudioSystem
{
    public class AudioDebugger : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool enableTestKeys = true;
        [SerializeField] private Vector3 testSoundOffset = new Vector3(0f, 1f, 2f);

        [Header("Visual Debugging")]
        [SerializeField] private bool showAudioSourceGizmos = true;
        [SerializeField] private Color activeSourceColor = Color.cyan;
        [SerializeField] private float gizmoSphereSize = 0.3f;

        private void Update()
        {
            if (!enableTestKeys || AudioManager.Instance == null || Keyboard.current == null)
                return;

            Vector3 testPosition = Camera.main != null 
                ? Camera.main.transform.position + Camera.main.transform.forward * 2f 
                : transform.position + testSoundOffset;

            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                AudioManager.Instance.PlaySound(AudioEventType.Pickup, testPosition);
                Debug.Log("[AudioDebugger] Testing Pickup sound");
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                AudioManager.Instance.PlaySound(AudioEventType.Throw, testPosition);
                Debug.Log("[AudioDebugger] Testing Throw sound");
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                AudioManager.Instance.PlaySound(AudioEventType.Place, testPosition);
                Debug.Log("[AudioDebugger] Testing Place sound");
            }
            else if (Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                AudioManager.Instance.PlaySound(AudioEventType.Scan, testPosition);
                Debug.Log("[AudioDebugger] Testing Scan sound");
            }
            else if (Keyboard.current.digit5Key.wasPressedThisFrame)
            {
                AudioManager.Instance.PlaySound(AudioEventType.Acceptance, testPosition);
                Debug.Log("[AudioDebugger] Testing Acceptance sound");
            }
            else if (Keyboard.current.digit6Key.wasPressedThisFrame)
            {
                AudioManager.Instance.PlaySound(AudioEventType.Rejection, testPosition);
                Debug.Log("[AudioDebugger] Testing Rejection sound");
            }
            else if (Keyboard.current.digit7Key.wasPressedThisFrame)
            {
                AudioManager.Instance.PlaySound(AudioEventType.MoneyGained, testPosition);
                Debug.Log("[AudioDebugger] Testing MoneyGained sound");
            }
            else if (Keyboard.current.digit8Key.wasPressedThisFrame)
            {
                AudioManager.Instance.PlaySound(AudioEventType.BuildModePlace, testPosition);
                Debug.Log("[AudioDebugger] Testing BuildModePlace sound");
            }
            else if (Keyboard.current.digit9Key.wasPressedThisFrame)
            {
                TestAllSurfaceImpacts(testPosition);
            }
        }

        private void TestAllSurfaceImpacts(Vector3 position)
        {
            Debug.Log("[AudioDebugger] Testing all surface impact sounds");
            
            SurfaceType[] surfaceTypes = (SurfaceType[])System.Enum.GetValues(typeof(SurfaceType));
            
            for (int i = 0; i < surfaceTypes.Length; i++)
            {
                Vector3 offset = position + Vector3.right * i * 0.5f;
                AudioManager.Instance.PlayImpactSound(surfaceTypes[i], offset, 0.8f);
                Debug.Log($"[AudioDebugger] Testing {surfaceTypes[i]} impact");
            }
        }

        private void OnGUI()
        {
            if (!enableTestKeys)
                return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 400));
            GUILayout.Label("Audio System Debugger", new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold });
            GUILayout.Space(10);
            GUILayout.Label("Press number keys to test sounds:");
            GUILayout.Label("1 - Pickup");
            GUILayout.Label("2 - Throw");
            GUILayout.Label("3 - Place");
            GUILayout.Label("4 - Scan");
            GUILayout.Label("5 - Acceptance");
            GUILayout.Label("6 - Rejection");
            GUILayout.Label("7 - Money Gained");
            GUILayout.Label("8 - Build Mode Place");
            GUILayout.Label("9 - All Surface Impacts");
            GUILayout.Space(10);
            GUILayout.Label("F10 - Show pool stats in console");
            GUILayout.EndArea();
        }
    }
}
