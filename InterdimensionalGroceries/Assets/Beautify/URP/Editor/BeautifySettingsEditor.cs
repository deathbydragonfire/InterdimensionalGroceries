using UnityEngine;
using UnityEditor;

namespace Beautify.Universal {

    [CustomEditor(typeof(BeautifySettings))]
    public class BeautifySettingsEditor : Editor {

        SerializedProperty sun;
        SerializedProperty depthOfFieldTarget;
        SerializedProperty depthOfFieldFocusPositionEnabled;
        SerializedProperty depthOfFieldFocusPosition;

        static GUIStyle sectionHeaderStyle;
        static GUIStyle boxStyle;
        static bool sunFoldout = true;
        static bool dofFoldout = true;

        void OnEnable() {
            sun = serializedObject.FindProperty("sun");
            depthOfFieldTarget = serializedObject.FindProperty("depthOfFieldTarget");
            depthOfFieldFocusPositionEnabled = serializedObject.FindProperty("depthOfFieldFocusPositionEnabled");
            depthOfFieldFocusPosition = serializedObject.FindProperty("depthOfFieldFocusPosition");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            SetupStyles();

            // Header
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Beautify Scene Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Configure scene-specific settings for Beautify effects. These settings override or complement the Volume profile settings.", MessageType.Info);
            EditorGUILayout.Space(10);

            // Sun Section
            sunFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(sunFoldout, "Sun & Lighting");
            if (sunFoldout) {
                EditorGUILayout.BeginVertical(boxStyle);
                EditorGUILayout.PropertyField(sun, new GUIContent("Sun Transform", "The directional light used for sun flares and other lighting effects"));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(5);

            // Depth of Field Section
            dofFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(dofFoldout, "Depth of Field");
            if (dofFoldout) {
                EditorGUILayout.BeginVertical(boxStyle);
                
                // Target Transform
                EditorGUILayout.LabelField("Follow Target Mode", EditorStyles.miniBoldLabel);
                EditorGUILayout.PropertyField(depthOfFieldTarget, new GUIContent("Target", "Transform to focus on when using FollowTarget focus mode"));
                
                EditorGUILayout.Space(8);
                
                // Focus Position
                EditorGUILayout.LabelField("Follow Position Mode", EditorStyles.miniBoldLabel);
                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(depthOfFieldFocusPositionEnabled, new GUIContent("Override Focus Position", "Enable to use this position instead of the Volume's Focus Position setting"));
                EditorGUILayout.EndHorizontal();

                EditorGUI.BeginDisabledGroup(!depthOfFieldFocusPositionEnabled.boolValue);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(depthOfFieldFocusPosition, new GUIContent("Focus Position", "World position to focus on when using FollowPosition focus mode"));
                EditorGUI.indentLevel--;
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(10);

            // Utility buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select Beautify Volume")) {
                SelectBeautifyVolume();
            }
            if (GUILayout.Button("Reset to Defaults")) {
                ResetToDefaults();
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        void SetupStyles() {
            if (sectionHeaderStyle == null) {
                sectionHeaderStyle = new GUIStyle(EditorStyles.foldoutHeader) {
                    fontStyle = FontStyle.Bold,
                    fontSize = 12
                };
            }

            if (boxStyle == null) {
                boxStyle = new GUIStyle("box") {
                    padding = new RectOffset(10, 10, 10, 10),
                    margin = new RectOffset(0, 0, 5, 5)
                };
            }
        }

        void SelectBeautifyVolume() {
#if UNITY_2023_1_OR_NEWER
            var volumes = FindObjectsByType<UnityEngine.Rendering.Volume>(FindObjectsSortMode.None);
#else
            var volumes = FindObjectsOfType<UnityEngine.Rendering.Volume>();
#endif
            foreach (var volume in volumes) {
                if (volume.sharedProfile != null && volume.sharedProfile.TryGet<Beautify>(out var beautify)) {
                    Selection.activeObject = volume.gameObject;
                    EditorGUIUtility.PingObject(volume.gameObject);
                    break;
                }
            }
        }

        void ResetToDefaults() {
            Undo.RecordObject(target, "Reset Beautify Settings");
            BeautifySettings settings = (BeautifySettings)target;
            settings.sun = null;
            settings.depthOfFieldTarget = null;
            settings.depthOfFieldFocusPositionEnabled = false;
            settings.depthOfFieldFocusPosition = Vector3.zero;
            EditorUtility.SetDirty(target);
        }
    }
}

