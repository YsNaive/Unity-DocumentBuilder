using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static NaiveAPI.DocumentBuilder.SODocPage;

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
        public override void OnInspectorGUI()
        {
            if(Event.current.type == EventType.KeyDown)
            {
                if (Event.current.control)
                    EditRoot.CtrlHotKeyAction(Event.current.keyCode);
            }
        }
        public SODocPage Target;
        VisualElement root;
        VisualElement contents;
        VisualElement header;
        ObjectField icon;
        public ObjectField IconField => icon;
        List<string> buildinIconList = new List<string>(); 
        [SerializeField] private List<DocComponent> undoBuffer;
        void reCalHeigth()
        {
            float sum = 0;
            foreach (var ve in root.Children()) { sum += ve.layout.height; }
            if ((root.style.height.value.value <= sum + 500))
            {
                root.style.height = sum + 800;
            }
        }
        public override VisualElement CreateInspectorGUI()
        {
            var styleTemp = DocStyle.Current.Copy();
            Target = target as SODocPage;
            root = new IMGUIContainer(OnInspectorGUI);
            
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
            Button saveBtn = new DSButton("Save", DocStyle.Current.SuccessColor, Save);
            saveBtn.style.SetIS_Style(new ISMargin(TextAnchor.MiddleRight));
            ObjectField curSOStyle = DocEditor.NewObjectField<SODocStyle>("", e =>
            {
                DocRuntimeData.Instance.CurrentStyle = (SODocStyle)e.newValue;
                DocStyle.Current = ((SODocStyle)e.newValue).Get();
            });
            curSOStyle.value = DocRuntimeData.Instance.CurrentStyle;
            curSOStyle.Q<Label>().style.SetIS_Style(DocStyle.Current.MainText);
            var hor = new DSHorizontal(1f, editMode, viewMode, defuMode, curSOStyle, null, saveBtn);
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
            var introMode = new DSEnumField<DocPageAniMode>("Intro Mode", Target.IntroMode, value =>
            {
                Target.IntroMode = value.newValue;
            });
            introMode.labelElement.style.minWidth = 96;
            introMode.labelElement.style.width = 96;
            introMode.labelElement.style.unityTextAlign = TextAnchor.MiddleCenter;
            introMode.style.ClearMarginPadding();
            introMode.style.height = 20;
            introMode.style.width = Length.Percent(49);
            IntegerField introDurField = DocEditor.NewIntField(" Duration", (value) =>
            {
                Target.IntroDuration = value.newValue;
            });
            introDurField.style.height = 20;
            introDurField.style.width = Length.Percent(50);
            introDurField[0].style.minWidth = 70;
            introDurField.value = Target.IntroDuration;
            var outroMode = new DSEnumField<DocPageAniMode>("Outtro Mode",Target.OuttroMode, value =>
            {
                Target.OuttroMode = value.newValue;
            });
            outroMode.labelElement.style.minWidth = 96;
            outroMode.labelElement.style.width = 96;
            outroMode.labelElement.style.unityTextAlign = TextAnchor.MiddleCenter;
            outroMode.style.height = 20;
            outroMode.style.ClearMarginPadding();
            outroMode.style.width = Length.Percent(49);
            IntegerField outroDurField = DocEditor.NewIntField(" Duration", (value) =>
            {
                Target.OuttroDuration = value.newValue;
            });
            outroDurField.style.width = Length.Percent(50);
            outroDurField[0].style.minWidth = 70;
            outroDurField.style.height = 20;
            outroDurField.value = Target.OuttroDuration;

            header.style.marginBottom = 10;
            root.Add(header);
            #endregion

            root.schedule.Execute(Save).Every(1000);
            contents.Add(createEdit());
            root.Add(contents);


            header.Add(new DSHorizontal(introMode,introDurField));
            header.Add(new DSHorizontal(outroMode,outroDurField));
            header.Add(new DSHorizontal(icon,buildinIcon));
            foreach (var ve in header.Children())
            {
                ve.style.marginTop = 2;
                ve.style.marginBottom = 0;
            }
            DocStyle.Current = styleTemp;
            return root;
        }
        private void OnDisable() { Save(); }

        public DocComponentsField EditRoot;
        VisualElement clickMask;
        VisualElement createEdit()
        {
            EditRoot = new DocComponentsField(Target.Components);
            Undo.IncrementCurrentGroup();
            undoBuffer = EditRoot.ToComponentsList();
            Undo.RegisterCompleteObjectUndo(this, "DocComponentsFieldBeging");
            EditRoot.OnModify += (doc) => {
                Undo.IncrementCurrentGroup();
                Undo.RegisterCompleteObjectUndo(this, "DocComponentsField");
                undoBuffer = EditRoot.ToComponentsList();
                reCalHeigth();
            };
            Undo.undoRedoPerformed += () => { EditRoot.Repaint(undoBuffer); };
            return EditRoot;
        }
        VisualElement createView()
        {
            return new DocPageVisual(Target);
        }

        public void Save()
        {
            if (Target == null ) return;
            if (EditRoot == null) return;
            if (EditRoot.IsDraging) return;
            if (Target.SubPages != null)
            {
                for (int i = 0; i < Target.SubPages.Count; i++)
                {
                    if (Target.SubPages[i] == null)
                    {
                        Target.SubPages.RemoveAt(i);
                        i--;
                    }
                }
            }
            Target.Components = EditRoot.ToComponentsList();
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
    }
}