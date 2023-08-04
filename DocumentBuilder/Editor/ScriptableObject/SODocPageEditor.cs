using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.EventSystems;
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
        VisualElement addAndDeleteBar;
        Button addPage;
        CheckButton deletePage;
        VisualElement introSetting;
        VisualElement outtroSetting;
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
            Target = target as SODocPage;
            root = new IMGUIContainer(OnInspectorGUI);
            #region mod bar
            root.style.SetIS_Style(ISPadding.Pixel(10));
            root.style.backgroundColor = SODocStyle.Current.BackgroundColor;
            header = DocRuntime.NewEmpty();
            contents = DocRuntime.NewEmpty();
            clickMask = DocRuntime.NewEmpty();
            clickMask.style.SetIS_Style(ISSize.Percent(100, 100));
            clickMask.style.position = Position.Absolute;
            
            Button editMode = DocRuntime.NewButton("Edit Layout", () =>
            {
                root.Insert(1, header);
                contents.Clear();
                contents.Add(createEdit());
            });
            Button viewMode = DocRuntime.NewButton("View Layout", () =>
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
            Button saveBtn = DocRuntime.NewButton("Save", SODocStyle.Current.SuccessColor, Save);
            saveBtn.style.SetIS_Style(new ISMargin(TextAnchor.MiddleRight));
            ObjectField curSOStyle = DocEditor.NewObjectField<SODocStyle>("", e =>
            {
                DocRuntimeData.Instance.CurrentStyle = (SODocStyle)e.newValue;
            });
            curSOStyle.value = SODocStyle.Current;
            root.Add(DocRuntime.NewHorizontalBar(1f,editMode,viewMode,defuMode,curSOStyle,null,saveBtn));

            #endregion

            #region header bar
            VisualElement iconHorBar = DocRuntime.NewEmptyHorizontal();
            icon = DocEditor.NewObjectField<Texture2D>("icon", (value) =>
            {
                Target.Icon = (Texture2D)value.newValue;
            });
            icon.value = Target.Icon;
            icon[0].style.minWidth = 95;
            icon.style.width = Length.Percent(80);
            var buildinIcon = DocRuntime.NewDropdownField("", buildinIconList, e =>
            {
                icon.value = DocEditorData.Instance.BuildinIcon[buildinIconList.IndexOf(e.newValue)];
            });
            buildinIcon.index = DocEditorData.Instance.BuildinIcon.IndexOf(Target.Icon);
            buildinIcon.style.width = Length.Percent(20);
            introSetting =DocRuntime.NewEmptyHorizontal();
            introSetting.style.height = 18;
            iconHorBar.Add(icon);
            iconHorBar.Add(buildinIcon);
            EnumField introMode = DocEditor.NewEnumField("Intro Mode", Target.IntroMode, value =>
            {
                Target.IntroMode = (DocPageAniMode)value.newValue;
            });
            introMode[0].style.minWidth = 96;
            introMode[0].style.unityTextAlign = TextAnchor.MiddleCenter;
            introMode.style.ClearMarginPadding();
            introMode.style.height = 18;
            introMode.style.width = Length.Percent(49);
            introSetting.Add(introMode);
            IntegerField durField = new IntegerField();
            durField.label = "Duration";
            durField.style.height = 18;
            durField[1].style.height = 18;
            durField.style.ClearMarginPadding();
            durField.style.width = Length.Percent(50);
            durField[0].style.minWidth = 60;
            durField.style.height = 18;
            durField[0].style.unityTextAlign = TextAnchor.UpperCenter;
            durField.value = Target.IntroDuration;
            durField.RegisterValueChangedCallback((value) =>
            {
                Target.IntroDuration = value.newValue;
            });
            introSetting.Add(durField);
            outtroSetting =DocRuntime.NewEmptyHorizontal();
            EnumField outroMode = DocEditor.NewEnumField("Outtro Mode", Target.OuttroMode, value =>
            {
                Target.OuttroMode = (DocPageAniMode)value.newValue;
            });
            outroMode[0].style.minWidth = 96;
            outroMode[0].style.unityTextAlign = TextAnchor.MiddleCenter;
            outroMode.style.height = 18;
            outroMode.style.ClearMarginPadding();
            outroMode.style.width = Length.Percent(49);
            outtroSetting.Add(outroMode);
            outtroSetting.style.height = 18;
            IntegerField outroDurField = new IntegerField();
            outroDurField.label = "Duration";
            outroDurField[0].style.unityTextAlign = TextAnchor.UpperCenter;
            outroDurField.style.ClearMarginPadding();
            outroDurField.style.width = Length.Percent(50);
            outroDurField[0].style.minWidth = 60;
            outroDurField.style.height = 18;
            outroDurField.value = Target.IntroDuration;
            outroDurField.RegisterValueChangedCallback((value) =>
            {
                Target.OuttroDuration = value.newValue;
            });
            outtroSetting.Add(outroDurField);
            addAndDeleteBar = DocRuntime.NewEmptyHorizontal();
            addPage = DocRuntime.NewButton("Add New Page", newPageBtn);
            addPage.style.width = Length.Percent(75);
            deletePage = DocRuntime.NewCheckButton("Delete Page",
                SODocStyle.Current.DangerColor, SODocStyle.Current.DangerColor, SODocStyle.Current.SuccessColor, () =>
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(Target));
                    AssetDatabase.Refresh();
                });
            deletePage.style.width = Length.Percent(24);
            deletePage.style.SetIS_Style(new ISMargin(TextAnchor.UpperRight));
            addAndDeleteBar.Add(addPage);
            addAndDeleteBar.Add(deletePage);

            header.style.marginBottom = 20;
            root.Add(header);
            #endregion

            root.schedule.Execute(Save).Every(250);
            contents.Add(createEdit());
            root.Add(contents);

            VisualElement loadAndSave = DocRuntime.NewEmptyHorizontal();
            Button loadFromTemplate = DocRuntime.NewButton("Load Template", () =>
            {
                header.Remove(loadAndSave);
                if (DocEditorData.Instance.DocTemplateFolder == null) return;
                VisualElement hor = DocRuntime.NewEmptyHorizontal();
                string path = AssetDatabase.GetAssetPath(DocEditorData.Instance.DocTemplateFolder);
                var select = DocRuntime.NewDropdownField("Template", findAllTemplateName());
                select[0].style.minWidth = 94;
                select.style.width = Length.Percent(70);
                Button load = DocRuntime.NewButton("Load", SODocStyle.Current.DangerColor, () =>
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
                Button cancel = DocRuntime.NewButton("Cancel", SODocStyle.Current.SuccessColor, () =>
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
            loadAndSave.Add(loadFromTemplate);
            var hint = DocRuntime.NewTextElement("Template name can not be empty.");
            Button saveAsTemplate = DocRuntime.NewButton("Save As Template", () =>
            {
                header.Remove(loadAndSave);
                VisualElement hor = DocRuntime.NewEmptyHorizontal();
                TextField name = DocRuntime.NewTextField("Name");
                name.style.width = Length.Percent(70);
                name[1].style.backgroundColor = SODocStyle.Current.DangerColor;
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
                        name[1].style.backgroundColor = SODocStyle.Current.DangerColor;
                        save.SetEnabled(false);
                        hint.text = "Target folder not valid.\nPlease check DocumentBuilder setting.";
                        return;
                    }
                    if (string.IsNullOrEmpty(val.newValue.Replace(" ","")))
                    {
                        name[1].style.backgroundColor = SODocStyle.Current.DangerColor;
                        hint.text = "Template name can not be empty.";
                        save.SetEnabled(false);
                        return;
                    }
                    if (templates.Contains(val.newValue))
                    {
                        name[1].style.backgroundColor = SODocStyle.Current.DangerColor;
                        hint.text = "Already exist a template with same name.";
                        save.SetEnabled(false);
                        return;
                    }
                    name[1].style.backgroundColor = SODocStyle.Current.SuccessColor;
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
            loadAndSave.Add(saveAsTemplate);

            header.Add(introSetting);
            header.Add(outtroSetting);
            header.Add(iconHorBar);
            header.Add(addAndDeleteBar);
            header.Add(new IMGUIContainer(() => {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SubPages"));
            }));
            header.Add(loadAndSave);
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
            Button create = DocRuntime.NewButton("Create", SODocStyle.Current.SuccessColor, () =>
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
            Button cancel = DocRuntime.NewButton("Cancel", SODocStyle.Current.DangerColor, () =>
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