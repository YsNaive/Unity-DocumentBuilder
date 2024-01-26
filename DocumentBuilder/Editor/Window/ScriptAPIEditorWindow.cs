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
using Settings = NaiveAPI_Editor.DocumentBuilder.ScriptAPIWindow.Settings;
namespace NaiveAPI_Editor.DocumentBuilder
{
    public class ScriptAPIEditorWindow : EditorWindow
    {
        [MenuItem("Tools/NaiveAPI/Script API Editor", priority = 100)]
        public static void GetWindow()
        {
            GetWindow<ScriptAPIEditorWindow>("Script API Editor");
        }
        Settings settings;
        DSTypeField typeField;
        VisualElement LeftPanel, MidPanel;
        SplitView splitView;
        private void CreateGUI()
        {
            settings = JsonUtility.FromJson<Settings>(DocCache.LoadData("SciprtAPIWindowSettings.json"));
            settings ??= new();
            LeftPanel = new();
            MidPanel = new();
            typeField = new DSTypeField();
            var root = rootVisualElement;
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            var padding = ISPadding.Pixel(5);
            LeftPanel.style.SetIS_Style(padding);
            MidPanel.style.SetIS_Style(padding);
            splitView = new SplitView(FlexDirection.Row, 20);
            splitView.Add(LeftPanel);
            splitView.Add(MidPanel);
            splitView.SplitPercent = settings.SplitViewPercent;
            root.Add(splitView);
            typeField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == null) return;
                MidPanel.Clear();
                MidPanel.Add(new ScriptAPIField(evt.newValue));
            });
            LeftPanel.Add(typeField);
            typeField.value = settings.ActiveType;
            foreach (var type in DocRuntime.FindAllTypesWhere(t => { return t.IsSubclassOf(typeof(ScriptAPIMenuDefinition)); }))
                LeftPanel.Add(((ScriptAPIMenuDefinition)Activator.CreateInstance(type)).CreateFoldoutHierarchy(null, ve =>
                {
                    ve.RegisterCallback<PointerEnterEvent>(evt =>
                    {
                        ve.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
                    });
                    ve.RegisterCallback<PointerLeaveEvent>(evt =>
                    {
                        ve.style.backgroundColor = Color.clear;
                    });
                    ve.RegisterCallback<PointerDownEvent>(evt =>
                    {
                        MidPanel.Clear();
                        typeField.value = ve.TargetType;
                        MidPanel.Add(new ScriptAPIField(ve.TargetType));
                    });
                }));
        }
        private void OnDisable()
        {
            settings = JsonUtility.FromJson<Settings>(DocCache.LoadData("SciprtAPIWindowSettings.json"));
            settings ??= new();
            if (splitView != null)
                settings.SplitViewPercent = splitView.SplitPercent;
            if (typeField != null)
                settings.ActiveType = typeField.value;
            DocCache.SaveData("SciprtAPIWindowSettings.json", JsonUtility.ToJson(settings));
        }
    }

}