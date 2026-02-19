using UnityEngine;
using UnityEditor;
using InterdimensionalGroceries.ItemSystem;
using System.IO;

namespace InterdimensionalGroceries.Editor
{
    public class ItemReplacementTool : EditorWindow
    {
        private GameObject sourceModel;
        private string targetPrefabPath = "Assets/Prefabs/Items/";
        private string itemName = "New Item";
        private string itemTagline = "A mysterious object";
        private float itemPrice = 1f;
        private ItemType itemType = ItemType.Unknown;
        private ColliderType colliderType = ColliderType.MeshColliderConvex;
        private Vector3 scaleAdjustment = new Vector3(0.1f, 0.1f, 0.1f);
        private GameObject templatePrefab;
        
        private enum ColliderType
        {
            MeshColliderConvex,
            BoxCollider,
            SphereCollider,
            CapsuleCollider
        }
        
        [MenuItem("Tools/Item Replacement Tool")]
        public static void ShowWindow()
        {
            GetWindow<ItemReplacementTool>("Item Replacement Tool");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Item Model Replacement Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("Source Configuration", EditorStyles.boldLabel);
            sourceModel = EditorGUILayout.ObjectField("Source Model (FBX/Prefab)", sourceModel, typeof(GameObject), false) as GameObject;
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Target Configuration", EditorStyles.boldLabel);
            targetPrefabPath = EditorGUILayout.TextField("Target Prefab Path", targetPrefabPath);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Item Data", EditorStyles.boldLabel);
            itemName = EditorGUILayout.TextField("Item Name", itemName);
            itemTagline = EditorGUILayout.TextField("Tagline", itemTagline);
            itemPrice = EditorGUILayout.FloatField("Price", itemPrice);
            itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", itemType);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Physics Configuration", EditorStyles.boldLabel);
            colliderType = (ColliderType)EditorGUILayout.EnumPopup("Collider Type", colliderType);
            scaleAdjustment = EditorGUILayout.Vector3Field("Scale Adjustment", scaleAdjustment);
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Template (Optional)", EditorStyles.boldLabel);
            templatePrefab = EditorGUILayout.ObjectField("Template Prefab", templatePrefab, typeof(GameObject), false) as GameObject;
            EditorGUILayout.HelpBox("If provided, all components will be copied from this template prefab.", MessageType.Info);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Create Item Prefab", GUILayout.Height(30)))
            {
                CreateItemPrefab();
            }
            
            EditorGUILayout.Space();
            DisplayValidationWarnings();
        }
        
        private void DisplayValidationWarnings()
        {
            if (sourceModel == null)
            {
                EditorGUILayout.HelpBox("Please assign a source model (FBX or prefab).", MessageType.Warning);
            }
            
            if (string.IsNullOrEmpty(targetPrefabPath))
            {
                EditorGUILayout.HelpBox("Please specify a target prefab path.", MessageType.Warning);
            }
            
            if (!targetPrefabPath.EndsWith(".prefab"))
            {
                EditorGUILayout.HelpBox("Target path should end with .prefab", MessageType.Warning);
            }
        }
        
        private void CreateItemPrefab()
        {
            if (sourceModel == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a source model.", "OK");
                return;
            }
            
            if (string.IsNullOrEmpty(targetPrefabPath) || !targetPrefabPath.EndsWith(".prefab"))
            {
                EditorUtility.DisplayDialog("Error", "Please specify a valid target prefab path ending with .prefab", "OK");
                return;
            }
            
            string directory = Path.GetDirectoryName(targetPrefabPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                AssetDatabase.Refresh();
            }
            
            string fullPrefabPath = targetPrefabPath;
            if (!fullPrefabPath.StartsWith("Assets/"))
            {
                fullPrefabPath = "Assets/" + fullPrefabPath;
            }
            
            GameObject instance = null;
            
            try
            {
                instance = Instantiate(sourceModel);
                instance.name = itemName;
                
                RemoveUnnecessaryComponents(instance);
                
                instance.transform.localScale = scaleAdjustment;
                instance.layer = LayerMask.NameToLayer("Interactable");
                
                GameObject targetObject = DetermineComponentTarget(instance);
                
                AddCollider(targetObject);
                AddRigidbody(targetObject);
                
                if (templatePrefab != null)
                {
                    CopyComponentsFromTemplate(targetObject);
                }
                else
                {
                    AddItemSystemComponents(targetObject);
                }
                
                bool success;
                PrefabUtility.SaveAsPrefabAsset(instance, fullPrefabPath, out success);
                
                if (success)
                {
                    EditorUtility.DisplayDialog("Success", $"Item prefab created successfully at:\n{fullPrefabPath}", "OK");
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(fullPrefabPath);
                }
                else
                {
                    EditorUtility.DisplayDialog("Error", "Failed to save prefab asset.", "OK");
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to create item prefab:\n{e.Message}", "OK");
                Debug.LogError($"ItemReplacementTool error: {e}");
            }
            finally
            {
                if (instance != null)
                {
                    DestroyImmediate(instance);
                }
            }
            
            AssetDatabase.Refresh();
        }
        
        private void RemoveUnnecessaryComponents(GameObject instance)
        {
            Camera[] cameras = instance.GetComponentsInChildren<Camera>();
            foreach (Camera cam in cameras)
            {
                DestroyImmediate(cam.gameObject);
            }
            
            Light[] lights = instance.GetComponentsInChildren<Light>();
            foreach (Light light in lights)
            {
                DestroyImmediate(light.gameObject);
            }
        }
        
        private GameObject DetermineComponentTarget(GameObject instance)
        {
            MeshFilter meshFilter = instance.GetComponentInChildren<MeshFilter>();
            
            if (meshFilter != null && meshFilter.gameObject != instance)
            {
                meshFilter.gameObject.layer = LayerMask.NameToLayer("Interactable");
                return meshFilter.gameObject;
            }
            
            return instance;
        }
        
        private void AddCollider(GameObject targetObject)
        {
            switch (colliderType)
            {
                case ColliderType.MeshColliderConvex:
                    MeshCollider meshCollider = targetObject.GetComponent<MeshCollider>();
                    if (meshCollider == null)
                    {
                        meshCollider = targetObject.AddComponent<MeshCollider>();
                    }
                    meshCollider.convex = true;
                    
                    MeshFilter meshFilter = targetObject.GetComponent<MeshFilter>();
                    if (meshFilter != null && meshFilter.sharedMesh != null)
                    {
                        meshCollider.sharedMesh = meshFilter.sharedMesh;
                    }
                    break;
                    
                case ColliderType.BoxCollider:
                    if (targetObject.GetComponent<BoxCollider>() == null)
                    {
                        targetObject.AddComponent<BoxCollider>();
                    }
                    break;
                    
                case ColliderType.SphereCollider:
                    if (targetObject.GetComponent<SphereCollider>() == null)
                    {
                        targetObject.AddComponent<SphereCollider>();
                    }
                    break;
                    
                case ColliderType.CapsuleCollider:
                    if (targetObject.GetComponent<CapsuleCollider>() == null)
                    {
                        targetObject.AddComponent<CapsuleCollider>();
                    }
                    break;
            }
        }
        
        private void AddRigidbody(GameObject targetObject)
        {
            Rigidbody rb = targetObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = targetObject.AddComponent<Rigidbody>();
            }
            
            rb.mass = 1f;
            rb.linearDamping = 0.5f;
            rb.angularDamping = 0.5f;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.None;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        }
        
        private void CopyComponentsFromTemplate(GameObject targetObject)
        {
            PickableItem templatePickable = templatePrefab.GetComponent<PickableItem>();
            if (templatePickable == null)
            {
                templatePickable = templatePrefab.GetComponentInChildren<PickableItem>();
            }
            
            if (templatePickable != null)
            {
                PickableItem newPickable = targetObject.GetComponent<PickableItem>();
                if (newPickable == null)
                {
                    newPickable = targetObject.AddComponent<PickableItem>();
                }
                EditorUtility.CopySerialized(templatePickable, newPickable);
            }
            
            TrailRenderer templateTrail = templatePrefab.GetComponent<TrailRenderer>();
            if (templateTrail == null)
            {
                templateTrail = templatePrefab.GetComponentInChildren<TrailRenderer>();
            }
            
            if (templateTrail != null)
            {
                TrailRenderer newTrail = targetObject.GetComponent<TrailRenderer>();
                if (newTrail == null)
                {
                    newTrail = targetObject.AddComponent<TrailRenderer>();
                }
                EditorUtility.CopySerialized(templateTrail, newTrail);
            }
            
            ThrowableTrail templateThrowable = templatePrefab.GetComponent<ThrowableTrail>();
            if (templateThrowable == null)
            {
                templateThrowable = templatePrefab.GetComponentInChildren<ThrowableTrail>();
            }
            
            if (templateThrowable != null)
            {
                ThrowableTrail newThrowable = targetObject.GetComponent<ThrowableTrail>();
                if (newThrowable == null)
                {
                    newThrowable = targetObject.AddComponent<ThrowableTrail>();
                }
                EditorUtility.CopySerialized(templateThrowable, newThrowable);
            }
            
            ItemVisualFeedback templateFeedback = templatePrefab.GetComponent<ItemVisualFeedback>();
            if (templateFeedback == null)
            {
                templateFeedback = templatePrefab.GetComponentInChildren<ItemVisualFeedback>();
            }
            
            if (templateFeedback != null)
            {
                ItemVisualFeedback newFeedback = targetObject.GetComponent<ItemVisualFeedback>();
                if (newFeedback == null)
                {
                    newFeedback = targetObject.AddComponent<ItemVisualFeedback>();
                }
                EditorUtility.CopySerialized(templateFeedback, newFeedback);
            }
            
            ItemImpactAudio templateAudio = templatePrefab.GetComponent<ItemImpactAudio>();
            if (templateAudio == null)
            {
                templateAudio = templatePrefab.GetComponentInChildren<ItemImpactAudio>();
            }
            
            if (templateAudio != null)
            {
                ItemImpactAudio newAudio = targetObject.GetComponent<ItemImpactAudio>();
                if (newAudio == null)
                {
                    newAudio = targetObject.AddComponent<ItemImpactAudio>();
                }
                EditorUtility.CopySerialized(templateAudio, newAudio);
            }
        }
        
        private void AddItemSystemComponents(GameObject targetObject)
        {
            if (targetObject.GetComponent<PickableItem>() == null)
            {
                targetObject.AddComponent<PickableItem>();
            }
            
            if (targetObject.GetComponent<TrailRenderer>() == null)
            {
                TrailRenderer trail = targetObject.AddComponent<TrailRenderer>();
                ConfigureTrailRenderer(trail);
            }
            
            if (targetObject.GetComponent<ThrowableTrail>() == null)
            {
                targetObject.AddComponent<ThrowableTrail>();
            }
            
            if (targetObject.GetComponent<ItemVisualFeedback>() == null)
            {
                targetObject.AddComponent<ItemVisualFeedback>();
            }
            
            if (targetObject.GetComponent<ItemImpactAudio>() == null)
            {
                targetObject.AddComponent<ItemImpactAudio>();
            }
        }
        
        private void ConfigureTrailRenderer(TrailRenderer trail)
        {
            trail.time = 0.5f;
            trail.minVertexDistance = 0.1f;
            trail.autodestruct = false;
            trail.emitting = false;
            
            AnimationCurve widthCurve = new AnimationCurve();
            widthCurve.AddKey(0f, 0.3f);
            widthCurve.AddKey(1f, 0.05f);
            trail.widthCurve = widthCurve;
            
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(new Color(0.3f, 0.8f, 1f), 0f),
                    new GradientColorKey(new Color(0.1f, 0.5f, 0.8f), 1f)
                },
                new GradientAlphaKey[] { 
                    new GradientAlphaKey(0.7f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            trail.colorGradient = gradient;
            
            Material trailMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/ItemTrail.mat");
            if (trailMaterial != null)
            {
                trail.material = trailMaterial;
            }
        }
    }
}
