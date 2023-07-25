using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocStyleEditWindow : EditorWindow
    {
        [MenuItem("Tools/NaiveAPI/DocumentBuilder/Edit Style")]
        public static void ShowWindow()
        {
            GetWindow<DocStyleEditWindow>("Style");
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

