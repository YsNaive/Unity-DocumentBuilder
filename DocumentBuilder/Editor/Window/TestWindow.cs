using NaiveAPI_Editor.DocumentBuilder;
using NaiveAPI_Editor.window;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class TestWindow : EditorWindow
    {
        static TestWindow window;
        [MenuItem("Tools/NaiveAPI/Test Window")]
        public static void ShowWindow()
        {
            if (window != null)
            {
                window.Close();
                window = null;
            }
            window = CreateWindow<TestWindow>("Test Window");
        }
        private void CreateGUI()
        {
            Button btn = null;
            btn = DocRuntime.NewButton("Button", () =>
            {
                btn.Highlight(50);
            });
            rootVisualElement.Add(btn);
            rootVisualElement.Add(DocRuntime.NewScrollView());
            rootVisualElement[1].style.height = 300;
            rootVisualElement[1].style.width = 300;
            rootVisualElement[1].Add(DocRuntime.NewEmpty());
            rootVisualElement[1][0].style.width = 900;
            rootVisualElement[1][0].style.height = 900;
        }
    }
}
