using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class SerializedDocComponentEditWindow : EditorWindow
    {
        private void OnEnable()
        {
            maxSize = new Vector2 (300, 300);
        }
        public static SerializedDocComponentEditWindow Create(SerializedProperty property)
        {
            var window = CreateWindow<SerializedDocComponentEditWindow>("Document Component");
            var editView = new SerializedDocEditVisual(property);
            window.rootVisualElement.Add(editView);
            window.rootVisualElement.style.backgroundColor = DocStyle.Current.BackgroundColor;
            Button save = new Button();
            save.text = "Save";
            save.style.SetIS_Style(new ISMargin(TextAnchor.LowerCenter));
            save.style.width = Length.Percent(75);
            save.clicked += () =>
            {
                window.Close();
                editView.ApplyChange();
            };
            window.rootVisualElement.Add(save);
            return window;
        }
    }
}
