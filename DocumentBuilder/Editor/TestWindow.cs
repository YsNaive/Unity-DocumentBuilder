using DocumentBuilder;
using NaiveAPI_Editor.drawer;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace NaiveAPI_Editor.window
{
    public class TestWindow : EditorWindow
    {
        [MenuItem("Tools/NaiveAPI/TestWindow")]
        public static void ShowWindow()
        {
            GetWindow<TestWindow>("TestWindow");
        }
        private void OnEnable()
        {
            minSize = new Vector2(200, 300);

        }
        public void CreateGUI()
        {
            rootVisualElement.Add(new DoucmentWindow());
        }
    }
}

