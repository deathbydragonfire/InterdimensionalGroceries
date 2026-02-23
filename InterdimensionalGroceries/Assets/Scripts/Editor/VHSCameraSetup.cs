using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using InterdimensionalGroceries.Rendering;

namespace InterdimensionalGroceries.Editor
{
    public class VHSCameraSetup : EditorWindow
    {
        private Material vhsMaterial;
        private VHSPreset defaultPreset = VHSPreset.Classic;
        
        [MenuItem("Tools/Interdimensional Groceries/VHS Camera Setup")]
        public static void ShowWindow()
        {
            GetWindow<VHSCameraSetup>("VHS Camera Setup");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("VHS Effect Setup Utility", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            vhsMaterial = (Material)EditorGUILayout.ObjectField("VHS Material", vhsMaterial, typeof(Material), false);
            defaultPreset = (VHSPreset)EditorGUILayout.EnumPopup("Default Preset", defaultPreset);
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Add VHS Effect to All Cameras in Current Scene"))
            {
                AddVHSToAllCamerasInScene();
            }
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("Add VHS Effect to Selected Camera"))
            {
                AddVHSToSelectedCamera();
            }
            
            GUILayout.Space(5);
            
            if (GUILayout.Button("Remove VHS Effect from All Cameras"))
            {
                RemoveVHSFromAllCameras();
            }
            
            GUILayout.Space(20);
            GUILayout.Label("Auto-Load Material", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Load Default VHS Material"))
            {
                LoadDefaultMaterial();
            }
        }
        
        private void AddVHSToAllCamerasInScene()
        {
            if (vhsMaterial == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a VHS Material first!", "OK");
                return;
            }
            
            Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            int addedCount = 0;
            
            foreach (Camera cam in cameras)
            {
                VHSEffectController controller = cam.GetComponent<VHSEffectController>();
                
                if (controller == null)
                {
                    controller = cam.gameObject.AddComponent<VHSEffectController>();
                    addedCount++;
                }
                
                SerializedObject so = new SerializedObject(controller);
                so.FindProperty("vhsMaterial").objectReferenceValue = vhsMaterial;
                so.ApplyModifiedProperties();
                
                controller.SetPreset(defaultPreset);
                
                EditorUtility.SetDirty(cam.gameObject);
            }
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"VHS Camera Setup: Added VHS effect to {addedCount} cameras (total: {cameras.Length})");
        }
        
        private void AddVHSToSelectedCamera()
        {
            if (vhsMaterial == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a VHS Material first!", "OK");
                return;
            }
            
            GameObject selected = Selection.activeGameObject;
            
            if (selected == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a GameObject with a Camera component!", "OK");
                return;
            }
            
            Camera cam = selected.GetComponent<Camera>();
            
            if (cam == null)
            {
                EditorUtility.DisplayDialog("Error", "Selected GameObject does not have a Camera component!", "OK");
                return;
            }
            
            VHSEffectController controller = cam.GetComponent<VHSEffectController>();
            
            if (controller == null)
            {
                controller = cam.gameObject.AddComponent<VHSEffectController>();
            }
            
            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("vhsMaterial").objectReferenceValue = vhsMaterial;
            so.ApplyModifiedProperties();
            
            controller.SetPreset(defaultPreset);
            
            EditorUtility.SetDirty(cam.gameObject);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            Debug.Log($"VHS Camera Setup: Added VHS effect to {cam.name}");
        }
        
        private void RemoveVHSFromAllCameras()
        {
            Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            int removedCount = 0;
            
            foreach (Camera cam in cameras)
            {
                VHSEffectController controller = cam.GetComponent<VHSEffectController>();
                
                if (controller != null)
                {
                    DestroyImmediate(controller);
                    removedCount++;
                    EditorUtility.SetDirty(cam.gameObject);
                }
            }
            
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"VHS Camera Setup: Removed VHS effect from {removedCount} cameras");
        }
        
        private void LoadDefaultMaterial()
        {
            string[] guids = AssetDatabase.FindAssets("VHSEffectMaterial t:Material");
            
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                vhsMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
                Debug.Log($"VHS Camera Setup: Loaded material from {path}");
            }
            else
            {
                EditorUtility.DisplayDialog("Not Found", "Could not find VHSEffectMaterial.mat in the project!", "OK");
            }
        }
    }
}
