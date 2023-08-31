using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI_UI.Editor
{
    public class UIElementExtensionDocsWindow : EditorWindow
    {
        // Create MenuItem static function
        [MenuItem("Tools/NaiveAPI/Documentation/UIElement Extension", priority = 2)]
        public static void ShowWindow()
        {
            GetWindow<UIElementExtensionDocsWindow>("UIElement Extension docs");
        }

        // Create and Add BookVisual
        private void CreateGUI()
        {
            SODocPage rootPage = AssetDatabase.LoadAssetAtPath<SODocPage>(AssetDatabase.GUIDToAssetPath("f2a6482daabdbea42be5b25908194f97"));
            rootVisualElement.Add(new DocBookVisual(rootPage));
        }
    }
}
