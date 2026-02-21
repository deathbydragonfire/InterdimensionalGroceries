using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Beautify.Universal {

    [CustomEditor(typeof(BeautifyRendererFeature))]
    public class BeautifyRenderFeatureEditor : Editor {

        SerializedProperty renderPassEvent, ignorePostProcessingOption;
#if ENABLE_VR && ENABLE_XR_MODULE
        SerializedProperty clearXRColorBuffer;
#endif
        SerializedProperty cameraLayerMask;
        SerializedProperty stripSettings;

        Editor internalStripSettingsEditor;

        void OnEnable () {
            renderPassEvent = serializedObject.FindProperty("renderPassEvent");
            ignorePostProcessingOption = serializedObject.FindProperty("ignorePostProcessingOption");
#if ENABLE_VR && ENABLE_XR_MODULE
            clearXRColorBuffer = serializedObject.FindProperty("clearXRColorBuffer");
#endif
            cameraLayerMask = serializedObject.FindProperty("cameraLayerMask");
            stripSettings = serializedObject.FindProperty("stripSettings");
        }

        public override void OnInspectorGUI () {

            serializedObject.Update();

            EditorGUILayout.PropertyField(renderPassEvent);
            EditorGUILayout.PropertyField(ignorePostProcessingOption);
#if ENABLE_VR && ENABLE_XR_MODULE
            EditorGUILayout.PropertyField(clearXRColorBuffer);
#endif
            EditorGUILayout.PropertyField(cameraLayerMask);

            BeautifyRendererFeature feature = (BeautifyRendererFeature)target;
            if (stripSettings.objectReferenceValue == null) {
                feature.UpdateInternalStripSettings();
            }
            BeautifyStripSettings currentSettings = feature.settings;

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Beautify Shader Features Stripping", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Optimize shader compilation time by stripping unused Beautify features. Select the features you wish to exclude from the build.", MessageType.Info);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(stripSettings);
            if (GUILayout.Button("Create Config Asset", GUILayout.Width(150))) {
                CreateConfigAsset(feature, currentSettings);
                GUIUtility.ExitGUI();
                return;
            }
            EditorGUILayout.EndHorizontal();

            if (currentSettings != null) {
                Editor.CreateCachedEditor(currentSettings, typeof(BeautifyStripSettingsEditor), ref internalStripSettingsEditor);

                EditorGUI.BeginChangeCheck();
                internalStripSettingsEditor.OnInspectorGUI();
                if (EditorGUI.EndChangeCheck()) {
                    if (stripSettings.objectReferenceValue == null) {
                        Undo.RecordObject(feature, "Change Strip Settings");
                        feature.SyncInternalToLegacy();
                        EditorUtility.SetDirty(feature);
                    }
                }
            }

            EditorGUILayout.Separator();

            if (GUILayout.Button("Select Beautify Volume >")) {
#if UNITY_2023_1_OR_NEWER
                var volumes = FindObjectsByType<Volume>(FindObjectsSortMode.None);
#else           
                var volumes = FindObjectsOfType<Volume>();
#endif

                foreach (var volume in volumes) {
                    if (volume.sharedProfile != null && volume.sharedProfile.TryGet<Beautify>(out var beautify)) {
                        Selection.activeObject = volume.gameObject;
                        EditorGUIUtility.PingObject(volume.gameObject);
                        break;
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        void CreateConfigAsset (BeautifyRendererFeature feature, BeautifyStripSettings source) {
            string path = EditorUtility.SaveFilePanelInProject("Create Strip Settings", "BeautifyStripSettings", "asset", "Please enter a file name to save the strip settings to");
            if (string.IsNullOrEmpty(path)) return;

            BeautifyStripSettings newSettings = ScriptableObject.CreateInstance<BeautifyStripSettings>();
            // Copy values
            EditorUtility.CopySerialized(source, newSettings);

            AssetDatabase.CreateAsset(newSettings, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            feature.stripSettings = newSettings;
            EditorUtility.SetDirty(feature);
            serializedObject.Update();
        }
    }
}
