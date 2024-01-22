using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class ScriptAPIEditorWindow : EditorWindow
    {
        [MenuItem("Tools/NaiveAPI/Script API Editor", priority = 99)]
        public static void GetWindow()
        {
            GetWindow<ScriptAPIEditorWindow>("Script API Editor");
        }
        Queue<Type> History = new();
        DSTypeField typeField;
        VisualElement LeftPanel, MidPanel;
        private void CreateGUI()
        {
            LeftPanel = new();
            MidPanel = new();
            typeField = new DSTypeField();
            var root = rootVisualElement;
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            var padding = ISPadding.Pixel(5);
            LeftPanel.style.SetIS_Style(padding);
            MidPanel.style.SetIS_Style(padding);
            var split1 = new SplitView(FlexDirection.Row, 20);
            split1.Add(LeftPanel);
            split1.Add(MidPanel);
            root.Add(split1);
            typeField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == null) return;
                History.Enqueue(evt.newValue);
                MidPanel.Clear();
                MidPanel.Add(new ScriptAPIField(evt.newValue));
            });
            LeftPanel.Add(typeField);
        }
    }

}