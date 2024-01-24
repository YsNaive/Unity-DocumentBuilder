using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class ScriptAPIWindow : EditorWindow
    {
        [MenuItem("Tools/NaiveAPI/Script API", priority = 99)]
        public static void GetWindow()
        {
            GetWindow<ScriptAPIWindow>("Script API");
        }
        Queue<Type> History = new();
        DSTypeField typeField;
        DSScrollView LeftPanel, MidPanel;
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
                MidPanel.Add(new TypeElement(evt.newValue));
            });
            LeftPanel.Add(typeField);
        }
    }

}