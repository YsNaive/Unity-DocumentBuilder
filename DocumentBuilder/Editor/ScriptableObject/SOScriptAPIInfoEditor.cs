using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    //[CustomEditor(typeof(SOScriptAPIInfo))]
    public class SOScriptAPIInfoEditor : Editor
    {
        new SOScriptAPIInfo target { get => (SOScriptAPIInfo)base.target; set => base.target = value; }
        VisualElement root;
        DSTypeField typeField;
        public override VisualElement CreateInspectorGUI()
        {
            root = new VisualElement();
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            root.style.SetIS_Style(ISPadding.Pixel(7));

            typeField = new DSTypeField();

            if (target.TargetType != null)
                typeField.value = target.TargetType;
            typeField.RegisterValueChangedCallback(evt => { 
                target.TargetType = evt.newValue;
                repaint();
            });
            repaint();
            return root;
        }
        void repaint()
        {
            root.Clear();
            root.Add(typeField);
            if (target.TargetType == null) return;
            foreach (var field in target.TargetType.GetFields())
                root.Add(new FieldEdit(target, field));
        }
        public class FieldEdit:VisualElement
        {
            public FieldEdit(SOScriptAPIInfo target, FieldInfo field)
            {
                Add(new DSFieldInfoElement(field));
                var toggle = new DSToggle("Hide");
                Add(toggle);
                var tooltip = new DSTextField("Tooltips");
                Add(tooltip);
            }
        }
    }

}