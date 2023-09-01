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
        Button addPage;
        CheckButton deletePage;
        ObjectField icon;
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
            DocStyle.Current = DocRuntimeData.Instance.CurrentStyle.Get(false);
            Target = target as SODocPage;
            root = new IMGUIContainer(OnInspectorGUI);
            #region mod bar
            root.style.SetIS_Style(ISPadding.Pixel(10));
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            header = DocRuntime.NewEmpty();
            contents = DocRuntime.NewEmpty();
            clickMask = DocRuntime.NewEmpty();
            clickMask.style.SetIS_Style(ISSize.Percent(100, 100));
            clickMask.style.position = Position.Absolute;
            
            Button editMode = DocRuntime.NewButton("Edit", () =>
            {
                root.Insert(1, header);
                contents.Clear();
                contents.Add(createEdit());
            });
            Button viewMode = DocRuntime.NewButton("View", () =>
            {
                if (root.Contains(header)) { root.Remove(header); }
                contents.Clear();
                contents.Add(createView());
            });
            Button defuMode = DocRuntime.NewButton("Inspector", () =>
            {
                if (root.Contains(header)) { root.Remove(header); }
                contents.Clear();
                contents.Add(new IMGUIContainer(() => { DrawDefaultInspector(); }));
            });
            Button saveBtn = DocRuntime.NewButton("Save", DocStyle.Current.SuccessColor, Save);
            saveBtn.style.SetIS_Style(new ISMargin(TextAnchor.MiddleRight));
            ObjectField curSOStyle = DocEditor.NewObjectField<SODocStyle>("", e =>
            {
                DocRuntimeData.Instance.CurrentStyle = (SODocStyle)e.newValue;
                DocStyle.Current = ((SODocStyle)e.newValue).Get();
            });
            curSOStyle.value = DocRuntimeData.Instance.CurrentStyle;
            curSOStyle.Q<Label>().style.SetIS_Style(DocStyle.Current.MainText);
            var hor = DocRuntime.NewHorizontalBar(1f, editMode, viewMode, defuMode, curSOStyle, null, saveBtn);
            root.Add(hor);

            #endregion

            #region header bar
            icon = DocEditor.NewObjectField<Texture2D>("Menu icon", (value) =>
            {
                Target.Icon = (Texture2D)value.newValue;
                EditorUtility.SetDirty(target);
            });
            icon.value = Target.Icon;
            icon.style.ClearMarginPadding();
            icon[0].style.minWidth = 98;
            icon[0].style.unityTextAlign = TextAnchor.MiddleCenter;
            icon.style.width = Length.Percent(50);
            var buildinIcon = DocRuntime.NewDropdownField("", buildinIconList, e =>
            {
                icon.value = DocEditorData.Instance.BuildinIcon[buildinIconList.IndexOf(e.newValue)];
            });
            buildinIcon.index = DocEditorData.Instance.BuildinIcon.IndexOf(Target.Icon);
            buildinIcon.style.width = Length.Percent(50);
            buildinIcon.style.ClearMarginPadding();
            buildinIcon[0].style.ClearMarginPadding();
            buildinIcon[0].style.marginLeft = 5;
            buildinIcon[0].style.paddingLeft = 5;
            EnumField introMode = DocEditor.NewEnumField("Intro Mode", Target.IntroMode, value =>
            {
                Target.IntroMode = (DocPageAniMode)value.newValue;
            });
            introMode[0].style.minWidth = 96;
            introMode[0].style.unityTextAlign = TextAnchor.MiddleCenter;
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
            EnumField outroMode = DocEditor.NewEnumField("Outtro Mode", Target.OuttroMode, value =>
            {
                Target.OuttroMode = (DocPageAniMode)value.newValue;
            });
            outroMode[0].style.minWidth = 96;
            outroMode[0].style.unityTextAlign = TextAnchor.MiddleCenter;
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
            addPage = DocRuntime.NewButton("Add New Page", newPageBtn);
            addPage.style.width = Length.Percent(75);
            deletePage = DocRuntime.NewCheckButton("Delete Page",
                DocStyle.Current.DangerColor, DocStyle.Current.DangerColor, DocStyle.Current.SuccessColor, () =>
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(Target));
                    AssetDatabase.Refresh();
                });
            deletePage.style.SetIS_Style(new ISMargin(TextAnchor.UpperRight));

            header.style.marginBottom = 10;
            root.Add(header);
            #endregion

            root.schedule.Execute(Save).Every(250);
            contents.Add(createEdit());
            root.Add(contents);

            VisualElement loadAndSave = null;
            Button loadFromTemplate = DocRuntime.NewButton("Load Template", () =>
            {
                header.Remove(loadAndSave);
                if (DocEditorData.Instance.DocTemplateFolder == null) return;
                VisualElement hor = DocRuntime.NewEmptyHorizontal();
                string path = AssetDatabase.GetAssetPath(DocEditorData.Instance.DocTemplateFolder);
                var select = DocRuntime.NewDropdownField("Template", findAllTemplateName());
                select[0].style.minWidth = 94;
                select.style.width = Length.Percent(70);
                Button load = DocRuntime.NewButton("Load", DocStyle.Current.DangerColor, () =>
                {
                    if (select.value != null)
                    {
                        List<DocComponent> copied = new List<DocComponent>(); ;
                        foreach (var c in AssetDatabase.LoadAssetAtPath<SODocComponents>(path + '/' + select.value + ".asset").Components)
                            copied.Add(c.Copy());  
                        EditRoot.Repaint(copied);
                        Save();
                    }
                    header.Remove(hor);
                    header.Add(loadAndSave);
                });
                load.style.width = Length.Percent(15);
                Button cancel = DocRuntime.NewButton("Cancel", DocStyle.Current.SuccessColor, () =>
                {
                    header.Remove(hor);
                    header.Add(loadAndSave);
                });
                cancel.style.width = Length.Percent(15);
                hor.Add(select);
                hor.Add(load);
                hor.Add(cancel);

                header.Add(hor);
            });
            loadFromTemplate.style.width = Length.Percent(50);
            var hint = DocRuntime.NewTextElement("Template name can not be empty.");
            Button saveAsTemplate = DocRuntime.NewButton("Save As Template", () =>
            {
                header.Remove(loadAndSave);
                VisualElement hor = DocRuntime.NewEmptyHorizontal();
                TextField name = DocRuntime.NewTextField("Name");
                name.style.width = Length.Percent(70);
                name[1].style.backgroundColor = DocStyle.Current.DangerColor;
                List<string> templates = findAllTemplateName();
                Button save = DocRuntime.NewButton("Save", () =>
                {
                    string path = AssetDatabase.GetAssetPath(DocEditorData.Instance.DocTemplateFolder);
                    var asset = CreateInstance<SODocComponents>();
                    Save();
                    foreach(var c in Target.Components)
                        asset.Components.Add(c);
                    asset.name = name.value;
                    AssetDatabase.CreateAsset(asset, path+'/'+ name.value+".asset");
                    AssetDatabase.Refresh();
                    header.Remove(hor);
                    header.Remove(hint);
                    header.Add(loadAndSave);
                });
                name.RegisterValueChangedCallback(val =>
                {
                    string path = AssetDatabase.GetAssetPath(DocEditorData.Instance.DocTemplateFolder);
                    if (!AssetDatabase.IsValidFolder(path))
                    {
                        name[1].style.backgroundColor = DocStyle.Current.DangerColor;
                        save.SetEnabled(false);
                        hint.text = "Target folder not valid.\nPlease check DocumentBuilder setting.";
                        return;
                    }
                    if (string.IsNullOrEmpty(val.newValue.Replace(" ","")))
                    {
                        name[1].style.backgroundColor = DocStyle.Current.DangerColor;
                        hint.text = "Template name can not be empty.";
                        save.SetEnabled(false);
                        return;
                    }
                    if (templates.Contains(val.newValue))
                    {
                        name[1].style.backgroundColor = DocStyle.Current.DangerColor;
                        hint.text = "Already exist a template with same name.";
                        save.SetEnabled(false);
                        return;
                    }
                    name[1].style.backgroundColor = DocStyle.Current.SuccessColor;
                    hint.text = "";
                    save.SetEnabled(true);
                });
                name[0].style.minWidth = 60;
                hint.style.marginLeft = 65;
                save.style.width = Length.Percent(15);
                save.SetEnabled(false);
                Button cancel = DocRuntime.NewButton("Cancel", () =>
                {
                    header.Remove(hor);
                    header.Remove(hint);
                    header.Add(loadAndSave);
                });
                cancel.style.width = Length.Percent(15);
                hor.Add(name);
                hor.Add(save);
                hor.Add(cancel);

                header.Add(hor);
                header.Add(hint);
            });
            saveAsTemplate.style.width = Length.Percent(50);

            var imgui = new IMGUIContainer(() =>
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SubPages"));
            });

            imgui.style.backgroundColor = new Color(.24f, .24f, .24f);
            header.Add(DocRuntime.NewHorizontalBar(introMode,introDurField));
            header.Add(DocRuntime.NewHorizontalBar(outroMode,outroDurField));
            header.Add(DocRuntime.NewHorizontalBar(icon,buildinIcon));
            header.Add(DocRuntime.NewHorizontalBar(0.5f,addPage,null,null,null,null,deletePage));
            header.Add(imgui);
            loadAndSave = DocRuntime.NewHorizontalBar(1f,loadFromTemplate, saveAsTemplate);
            header.Add(loadAndSave);
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
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }
        void newPageBtn()
        {
            VisualElement root = DocRuntime.NewEmptyHorizontal();
            var addPageParent = addPage.parent.parent;
            var bar = addPage.parent;
            int addPageIndex = addPageParent.IndexOf(bar);
            addPageParent.RemoveAt(addPageIndex);
            TextField inputName = DocRuntime.NewTextField("Page Name");
            Button create = DocRuntime.NewButton("Create", DocStyle.Current.SuccessColor, () =>
            {
                string path = AssetDatabase.GetAssetPath(Target);
                path = path.Substring(0, path.LastIndexOf('/'));
                string folder = $"{Target.name}SubPages";
                if (!AssetDatabase.IsValidFolder(path + '/' + folder))
                    AssetDatabase.CreateFolder(path, folder);
                var asset = CreateInstance<SODocPage>();
                asset.name = inputName.value;
                AssetDatabase.CreateAsset(asset, path + '/' + folder + $"/{asset.name}.asset");
                AssetDatabase.Refresh();
                var sp = serializedObject.FindProperty("SubPages");
                sp.InsertArrayElementAtIndex(sp.arraySize);
                sp = sp.GetArrayElementAtIndex(sp.arraySize - 1);
                sp.objectReferenceValue = asset;
                serializedObject.ApplyModifiedProperties();
                header.Remove(root);
                addPageParent.Insert(addPageIndex, bar);
            });
            Button cancel = DocRuntime.NewButton("Cancel", DocStyle.Current.DangerColor, () =>
            {
                header.Remove(root);
                addPageParent.Insert(addPageIndex, bar);
            });
            inputName.style.width = Length.Percent(69);
            inputName[0].style.minWidth = 50;
            create.style.width = Length.Percent(15);
            cancel.style.width = Length.Percent(15);

            root.Add(inputName);
            root.Add(create);
            root.Add(cancel);
            root.style.marginBottom = 7;
            header.Insert(addPageIndex,root);
        }
        List<string> findAllTemplateName()
        {
            List<string> choice = new List<string>();
            string path = AssetDatabase.GetAssetPath(DocEditorData.Instance.DocTemplateFolder);
            string assetRoot = Application.dataPath;
            assetRoot = assetRoot.Substring(0, assetRoot.Length - 7);
            string[] filePaths = Directory.GetFiles(assetRoot + '/' + path);

            if (filePaths != null && filePaths.Length > 0)
            {
                foreach (string p in filePaths)
                {
                    string releatedPath = p.Substring(assetRoot.Length + 1);
                    UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<SODocComponents>(releatedPath);
                    if (obj == null) continue;
                    if (obj.GetType() == typeof(SODocComponents))
                    {
                        choice.Add(obj.name);
                    }
                }
            }
            foreach (var temp in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                Debug.Log(temp.name);
                if (temp.GetType() == typeof(SODocComponents))
                    choice.Add(temp.name);
            }
            return choice;
        }
    }
}