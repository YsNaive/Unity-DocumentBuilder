using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class ScriptAPIEditorWindow : EditorWindow
    {
        [Serializable]
        public class Settings : ISerializationCallbackReceiver
        {
            public float SplitViewPercent = 25;
            public Type ActiveType;
            [SerializeField] string s_ActiveType;

            public void OnAfterDeserialize()
            {
                ActiveType = Type.GetType(s_ActiveType);
            }

            public void OnBeforeSerialize()
            {
                if (ActiveType == null)
                    s_ActiveType = "";
                else
                    s_ActiveType = ActiveType.AssemblyQualifiedName;
            }
        }
        Settings settings;
        DSTypeField typeField;
        VisualElement LeftPanel, MidPanel;
        SplitView splitView;
        private void CreateGUI()
        {
            settings = JsonUtility.FromJson<Settings>(DocCache.LoadData("SciprtAPIWindowEditorSettings.json"));
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
                MidPanel.Clear();
                if (evt.newValue == null) return;
                MidPanel.Add(new ScriptAPIField(evt.newValue));
            });
            LeftPanel.Add(typeField);
            typeField.value = settings.ActiveType;
            DSScrollView leftScrollView = new DSScrollView( );
            leftScrollView.mode = ScrollViewMode.VerticalAndHorizontal;
            foreach (var type in TypeReader.FindAllTypesWhere(t => { return t.IsSubclassOf(typeof(ScriptAPIMenuDefinition)); }))
                leftScrollView.Add(((ScriptAPIMenuDefinition)Activator.CreateInstance(type)).CreateFoldoutHierarchy(null, ve =>
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
            LeftPanel.Add(leftScrollView);
        }
        private void OnDisable()
        {
            settings = JsonUtility.FromJson<Settings>(DocCache.LoadData("SciprtAPIWindowEditorSettings.json"));
            settings ??= new();
            if (splitView != null)
                settings.SplitViewPercent = splitView.SplitPercent;
            if (typeField != null)
                settings.ActiveType = typeField.value;
            DocCache.SaveData("SciprtAPIWindowEditorSettings.json", JsonUtility.ToJson(settings));
        }
    }

    [CustomEditor(typeof(SOScriptAPIInfo))]
    class SOScriptAPIInfoEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();
            root.style.SetIS_Style(ISPadding.Pixel(5));
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            root.Add(DocVisual.Create(DocDescription.CreateComponent("Edit it on ScriptAPI Editor", DocDescription.DescriptionType.Hint)));
            root.Add(new DSButton("Open", () => { DocumentBuilderMenuItem.GetScriptAPIEditor(); }));
            return root;
        }
    }
}