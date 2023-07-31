using NaiveAPI_Editor.DocumentBuilder;
using NaiveAPI_Editor.window;
using NaiveAPI_UI;
using System;
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
            var root = rootVisualElement;
            root.Add(DocRuntime.NewScrollView());
            root = root[0];
            Type targetType = typeof(DocVisual);
            root.Add(DocRuntime.NewTextElement("Type: " + targetType.Name));
            foreach (var cons in targetType.GetConstructors())
            {
                root.Add(DocRuntime.NewTextElement("Cons: " + cons.Name));
            }
            foreach (var mods in targetType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                root.Add(DocRuntime.NewTextElement("Mods: " + mods.Name));
            }
            foreach (var mems in targetType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            {
                root.Add(DocRuntime.NewTextElement("Mems: " + mems.Name));
            }
        }
    }
}
