using NaiveAPI_Editor.window;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
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
        DocStyleField styleField;
        private void CreateGUI()
        {
            styleField = new DocStyleField(DocStyle.Current);
            Button save = new Button();
            save.text = "Save";
            save.clicked += () =>
            {
                DocCache.Get().CurrentStyle = styleField.Target;
                DocCache.Save();
            };
            rootVisualElement.Add(save);
            rootVisualElement.Add(styleField);
        }
    }
}
