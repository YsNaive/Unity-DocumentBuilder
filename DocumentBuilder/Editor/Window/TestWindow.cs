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
            rootVisualElement.Add(new ScrollView());
            foreach (var method in typeof(ISMargin).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance| BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                rootVisualElement[0].Add(DocRuntime.NewTextElement(method.Name + " " + method.IsConstructedGenericMethod));
            }
        }
    }
}
