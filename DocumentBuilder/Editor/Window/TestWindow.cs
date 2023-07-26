using NaiveAPI_Editor.DocumentBuilder;
using NaiveAPI_Editor.window;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
            var btn = new CheckButton();
            btn.text = "Test Check Btn";
            btn.Confirm += () =>
            {
                Debug.Log("YES");
                btn.style.opacity = 1;
                btn.Fade(0, 1000, 50, () => { btn.Fade(1, 1000); });
            };
            btn.Cancel += () =>
            {
                Type t = typeof(DocComponent);
                foreach(var info in t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    Debug.Log(info.Name);
                }
            };
            rootVisualElement.Add(btn);
        }
    }
}
