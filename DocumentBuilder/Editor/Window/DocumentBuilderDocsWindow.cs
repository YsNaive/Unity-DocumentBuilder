using NaiveAPI_Editor.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    public class DocumentBuilderDocsWindow : EditorWindow
    {
        // Create MenuItem static function
        [MenuItem("Tools/NaiveAPI/Documentation/Document Builder")]
        public static void ShowWindow()
        {
            GetWindow<DocumentBuilderDocsWindow>("DocumentBuilder docs");
        }

        // Create and Add BookVisual
        private void CreateGUI()
        {
            SODocPage rootPage = DocEditorData.Instance.DocumentBuilderDocsRoot;
            rootVisualElement.Add(new DocBookVisual(rootPage));
        }
    }
}
