using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class SerializedDocComponentEditWindow : EditorWindow
    {
        private void OnEnable()
        {
            minSize = new Vector2 (400, 300);
        }
        public static SerializedDocComponentEditWindow Create(SerializedProperty property, string name = "Document Component" )
        {
            var window = CreateWindow<SerializedDocComponentEditWindow>(name);
            var editView = new SerializedDocEditVisual(property);
            window.rootVisualElement.Add(editView);
            window.rootVisualElement.style.paddingRight = 10;
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
            ((DocComponentField)editView[0]).SetStatus(true);
            window.rootVisualElement.Add(save);
            return window;
        }
    }
}
