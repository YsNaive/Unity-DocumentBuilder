using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomEditor(typeof(SODocPage))]
    public class SODocPageEditor : Editor
    {
        public static event Action<SODocPageEditor> OnCreateEditor;
        public static SODocPageEditor Current;
        
        private void OnEnable()
        {
            OnCreateEditor?.Invoke(this);
            Current = this;
            foreach(var asset in DocEditorData.Instance.BuildinIcon)
            {
                buildinIconList.Add(asset.name);
            }
            DocStyle.OnStyleChanged += RepaintStyle;
        }
        private void OnDestroy()
        {
            DocStyle.OnStyleChanged -= RepaintStyle;
        }
        void RepaintStyle(DocStyle style)
        {
            if (root == null) return;
            var parent = root.parent;
            if (parent == null) return;
            int i = parent.IndexOf(root);
            parent.Remove(root);
            parent.Add(CreateInspectorGUI());
        }
        public SODocPage Target;
        VisualElement root;
        VisualElement contents;
        VisualElement header;
        ObjectField icon;
        public ObjectField IconField => icon;
        List<string> buildinIconList = new List<string>(); 
        public override VisualElement CreateInspectorGUI()
        {
            var styleTemp = DocStyle.Current.Copy();
            Target = target as SODocPage;
            root = new VisualElement();
            
            #region mod bar
            root.style.SetIS_Style(ISPadding.Pixel(10));
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            header = new VisualElement();
            contents = new VisualElement();
            clickMask = new VisualElement();
            clickMask.style.SetIS_Style(ISSize.Percent(100, 100));
            clickMask.style.position = Position.Absolute;
            
            Button editMode = new DSButton("Edit", () =>
            {
                root.Insert(1, header);
                contents.Clear();
                contents.Add(createEdit());
            });
            Button viewMode = new DSButton("View", () =>
            {
                if (root.Contains(header)) { root.Remove(header); }
                contents.Clear();
                contents.Add(createView());
            });
            Button defuMode = new DSButton("Inspector", () =>
            {
                if (root.Contains(header)) { root.Remove(header); }
                contents.Clear();
                contents.Add(new IMGUIContainer(() => { DrawDefaultInspector(); }));
            });
            ObjectField curSOStyle = DocEditor.NewObjectField<SODocStyle>("", e =>
            {
                DocRuntimeData.Instance.CurrentStyle = (SODocStyle)e.newValue;
                DocStyle.Current = ((SODocStyle)e.newValue).Get();
            });
            curSOStyle.value = DocRuntimeData.Instance.CurrentStyle;
            curSOStyle.Q<Label>().style.SetIS_Style(DocStyle.Current.MainText);
            var hor = new DSHorizontal(1f, editMode, viewMode, defuMode, curSOStyle, null);
            root.Add(hor);

            #endregion

            #region header bar
            DocStyle.Current.BeginLabelWidth(ISLength.Pixel(96));
            icon = DocEditor.NewObjectField<Texture2D>("Menu icon", (value) =>
            {
                Target.Icon = (Texture2D)value.newValue;
                EditorUtility.SetDirty(target);
            });
            DocStyle.Current.EndLabelWidth();
            icon.value = Target.Icon;
            icon.style.ClearMarginPadding();
            icon[0].style.unityTextAlign = TextAnchor.MiddleCenter;
            icon.style.width = Length.Percent(50);
            var buildinIcon = new DSDropdown() { choices = buildinIconList };
            buildinIcon.RegisterValueChangedCallback(evt =>
            {
                icon.value = DocEditorData.Instance.BuildinIcon[buildinIconList.IndexOf(evt.newValue)];
            });
            buildinIcon.index = DocEditorData.Instance.BuildinIcon.IndexOf(Target.Icon);
            buildinIcon.style.width = Length.Percent(50);
            buildinIcon.style.ClearMarginPadding();
            buildinIcon[0].style.ClearMarginPadding();
            buildinIcon[0].style.marginLeft = 5;
            buildinIcon[0].style.paddingLeft = 5;

            header.style.marginBottom = 10;
            root.Add(header);
            #endregion

            contents.Add(createEdit());
            root.Add(contents);

            header.Add(new DSHorizontal(icon,buildinIcon));
            foreach (var ve in header.Children())
            {
                ve.style.marginTop = 2;
                ve.style.marginBottom = 0;
            }
            DocStyle.Current = styleTemp;
            return root;
        }

        public DocComponentsField EditRoot;
        VisualElement clickMask;
        VisualElement createEdit()
        {
            var so = new SerializedObject(target);
            EditRoot = new DocComponentsField(so.FindProperty("Components"));
            return EditRoot;
        }
        VisualElement createView()
        {
            return new DocPageVisual(Target);
        }
    }
}