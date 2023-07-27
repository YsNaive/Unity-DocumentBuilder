using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomEditor(typeof(SODocComponents))]
    public class SODocComponentsEditor : Editor
    {
        IMGUIContainer imgui;
        VisualElement root;
        SODocComponents Target;
        public override VisualElement CreateInspectorGUI()
        {
            Target = target as SODocComponents;
            root = new VisualElement();
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            root.style.ClearMarginPadding();
            root.style.SetIS_Style(ISPadding.Pixel(10));
            imgui = new IMGUIContainer(() =>
            {
                EditorGUI.BeginChangeCheck();
                base.OnInspectorGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    repaint();
                }
            });
            root.Add(imgui);
            repaint();
            return root;
        }
        private void repaint()
        {
            root.Clear();
            root.Add(imgui);
            foreach(var doc in Target.Components)
            {
                var ve = DocEditor.CreateEditVisual(doc);
                ve.style.marginBottom = 15;
                root.Add(ve);
            }
        }
        private void OnDisable()
        {
            EditorUtility.SetDirty(target);
        }
    }
}
