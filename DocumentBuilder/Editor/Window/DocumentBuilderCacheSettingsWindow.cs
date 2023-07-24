using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocumentBuilderCacheSettingsWindow : EditorWindow
    {
        [MenuItem("Tools/NaiveAPI/DocBuilder/Settings")]
        public static void ShowWindow()
        {
            GetWindow<DocumentBuilderCacheSettingsWindow>("Settings");
        }
        DocStyle style;
        private void CreateGUI()
        {
            style = DocCache.CurrentStyle;

        }
        private void addColorField(string name, ref Color target)
        {
            ColorField colorField = new ColorField();
            colorField.label = name;
            colorField.value = target;
            colorField.RegisterValueChangedCallback((val) =>
            {
                
            });
            rootVisualElement.Add(colorField);
        }
        private void saveColor(ref Color target) 
        { 

        }
    }
}
