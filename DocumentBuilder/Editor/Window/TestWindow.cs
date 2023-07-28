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
            rootVisualElement.Add(new SearchDropdown("", new List<string>() { "AAA","BBB","Ccc","ABC"}));
            var btn = new CheckButton();
            btn.text = "Test Check Btn";
            btn.Confirm += () =>
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("QQQQQQQQ");
                int i = 0;
                i+=stringBuilder.UnityRTF(i+2,3, Color.green);
                i+=stringBuilder.UnityRTF(i+5,2, FontStyle.BoldAndItalic);
                Debug.Log(stringBuilder.ToString());
            };
            btn.Cancel += () =>
            {
            };
            rootVisualElement.Add(btn);
        }
    }
}
