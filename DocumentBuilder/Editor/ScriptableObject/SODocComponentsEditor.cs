using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomEditor(typeof(SODocComponents))]
    public class SODocComponentsEditor : Editor
    {
        DocComponentsField editField;
        public override void OnInspectorGUI()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.control)
                    editField.CtrlHotKeyAction(Event.current.keyCode);
            }
        }
        public override VisualElement CreateInspectorGUI()
        {
            var so = new SerializedObject(target);
            VisualElement root = new VisualElement();
            editField = new DocComponentsField(so.FindProperty("Components"));
            root.Add(new IMGUIContainer(OnInspectorGUI));
            root.Add(editField);
            return root;
        }
    }
}
