using NaiveAPI_Editor.DocumentBuilder;
using NaiveAPI_Editor.window;
using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

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
            rootVisualElement.Add(DocRuntime.NewButton("btn", () =>
            {
                Debug.Log(DocRuntimeData.Instance);
            }));
            Button b1 = DocRuntime.NewButton("btn1");
            Button b2 = DocRuntime.NewButton("btn2");
            Button b3 = DocRuntime.NewButton("btn3");
            Button b4 = DocRuntime.NewButton("btn4");
            rootVisualElement.Add(DocRuntime.NewHorizontalBar(b1, b2));
            rootVisualElement.Add(DocRuntime.NewHorizontalBar(b3, null, null, b4));
        }
    }
}
