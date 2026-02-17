using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InterdimensionalGroceries.Editor
{
    public class UIFontChangerWindow : EditorWindow
    {
        private Font targetFont;
        private Vector2 scrollPosition;
        private List<string> foundUSSFiles = new List<string>();

        [MenuItem("Tools/UI Font Changer")]
        public static void ShowWindow()
        {
            var window = GetWindow<UIFontChangerWindow>("UI Font Changer");
            window.minSize = new Vector2(400, 300);
        }

        private void OnEnable()
        {
            FindAllUSSFiles();
        }

        private void OnGUI()
        {
            GUILayout.Label("UI Toolkit Font Changer", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            EditorGUILayout.HelpBox("Select a font and it will be added to ALL USS files in your project as a global Label style.", MessageType.Info);
            EditorGUILayout.Space(10);

            targetFont = EditorGUILayout.ObjectField("Target Font", targetFont, typeof(Font), false) as Font;

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Refresh USS Files", GUILayout.Height(25)))
            {
                FindAllUSSFiles();
            }

            if (foundUSSFiles.Count > 0)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField($"Found {foundUSSFiles.Count} USS Files:", EditorStyles.boldLabel);
                
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(100));
                foreach (var file in foundUSSFiles)
                {
                    EditorGUILayout.LabelField($"• {Path.GetFileName(file)}");
                }
                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space(10);

                EditorGUI.BeginDisabledGroup(targetFont == null);
                if (GUILayout.Button("Apply Font to ALL USS Files", GUILayout.Height(40)))
                {
                    ApplyFontToAllUSS();
                }
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.HelpBox("No USS files found in the project.", MessageType.Warning);
            }

            if (targetFont == null)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.HelpBox("Select a font to apply.", MessageType.Warning);
            }
        }

        private void FindAllUSSFiles()
        {
            foundUSSFiles.Clear();
            string[] guids = AssetDatabase.FindAssets("t:StyleSheet");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(".uss"))
                {
                    foundUSSFiles.Add(path);
                }
            }
            Debug.Log($"Found {foundUSSFiles.Count} USS files in project");
        }

        private void ApplyFontToAllUSS()
        {
            if (targetFont == null)
            {
                Debug.LogError("No font selected!");
                return;
            }

            string fontPath = AssetDatabase.GetAssetPath(targetFont);
            string fontUrl = $"url('project://database/{fontPath.Replace("\\", "/")}')";

            int filesModified = 0;

            foreach (string ussPath in foundUSSFiles)
            {
                string content = File.ReadAllText(ussPath);
                
                string fontRule = $"\nLabel {{\n    -unity-font: {fontUrl};\n}}\n";
                
                if (!content.Contains("-unity-font"))
                {
                    content += fontRule;
                    File.WriteAllText(ussPath, content);
                    filesModified++;
                }
                else if (!content.Contains(fontUrl))
                {
                    content += $"\n/* Updated font */\n{fontRule}";
                    File.WriteAllText(ussPath, content);
                    filesModified++;
                }
            }

            AssetDatabase.Refresh();

            Debug.Log($"Applied font '{targetFont.name}' to {filesModified} USS files");
            EditorUtility.DisplayDialog("Font Applied", $"Successfully applied '{targetFont.name}' to {filesModified} USS files!\n\nThe font will now be used by all Labels in those stylesheets.", "OK");
        }
    }
}
