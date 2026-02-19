using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using InterdimensionalGroceries.ItemSystem;

namespace InterdimensionalGroceries.Editor
{
    public class UpdateItemData : EditorWindow
    {
        [MenuItem("Tools/Update Item Data")]
        public static void UpdateAllItems()
        {
            var scene = EditorSceneManager.GetActiveScene();
            var rootObjects = scene.GetRootGameObjects();
            
            System.Collections.Generic.Dictionary<string, (string name, string tagline, float price, ItemType type)> itemConfig =
                new System.Collections.Generic.Dictionary<string, (string, string, float, ItemType)>
            {
                { "Meat", ("Meat", "A vegans worst nightmare", 15f, ItemType.Meat) },
                { "Soda", ("Soda", "Carbonated chaos in a can", 8f, ItemType.Soda) },
                { "beans", ("Beans", "The musical fruit", 10f, ItemType.Beans) },
                { "eye", ("Eye", "The window to the soul", 20f, ItemType.Eye) },
                { "chicken", ("Meat", "The hunter's prize", 15f, ItemType.Meat) },
                { "ratinavat", ("Rat", "The sewer dweller", 12f, ItemType.Rat) },
                { "scarabonastick", ("Scarabs", "The sacred beetle", 18f, ItemType.Scarabs) },
                { "longpig", ("Long Pig", "The forbidden meat", 25f, ItemType.LongPig) },
                { "FIshPebbles", ("Pebbles", "Small stones from the shore", 5f, ItemType.Pebbles) },
                { "Cannedgrub", ("Grub", "Wriggly protein snacks", 14f, ItemType.Grub) },
                { "egg (1)", ("Eggs", "Fragile and oval", 12f, ItemType.Eggs) },
                { "Apple", ("Apples", "Sweet to the core", 8f, ItemType.Apples) }
            };
            
            int updatedCount = 0;
            
            foreach (var rootObj in rootObjects)
            {
                if (itemConfig.ContainsKey(rootObj.name))
                {
                    var pickable = rootObj.GetComponent<PickableItem>();
                    if (pickable != null)
                    {
                        var config = itemConfig[rootObj.name];
                        
                        SerializedObject so = new SerializedObject(pickable);
                        SerializedProperty itemDataProp = so.FindProperty("itemData");
                        
                        itemDataProp.FindPropertyRelative("itemName").stringValue = config.name;
                        itemDataProp.FindPropertyRelative("tagline").stringValue = config.tagline;
                        itemDataProp.FindPropertyRelative("price").floatValue = config.price;
                        itemDataProp.FindPropertyRelative("itemType").enumValueIndex = (int)config.type;
                        
                        so.ApplyModifiedProperties();
                        updatedCount++;
                        
                        Debug.Log($"Updated {rootObj.name} to {config.type}");
                    }
                }
            }
            
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log($"Successfully updated {updatedCount} items!");
        }
    }
}
