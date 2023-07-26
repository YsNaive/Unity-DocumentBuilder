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
        public static event Action<SODocPage> OnCreateNewPage;
        private void OnEnable()
        {
            OnCreateEditor?.Invoke(this);
        }
        SODocPage Target;
        VisualElement root;
        bool isEditMode = true;
        bool isDraging = false;
        VisualElement dragingTarget;
        ISPosition dragPosition;
        VisualElement contents;
        VisualElement header;
        Button addComponent;
        VisualElement addAndDeleteBar;
        Button addPage;
        CheckButton deletePage;
        VisualElement aniSetting;
        ObjectField icon;
        public override VisualElement CreateInspectorGUI()
        {
            Target = target as SODocPage;
            #region mod bar
            root = DocRuntime.NewEmpty();
            root.style.SetIS_Style(ISPadding.Pixel(10));
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            header = DocRuntime.NewEmpty();
            contents = DocRuntime.NewEmpty();
            clickMask = DocRuntime.NewEmpty();
            clickMask.style.SetIS_Style(ISSize.Percent(100, 100));
            clickMask.style.position = Position.Absolute;

            VisualElement bar = DocRuntime.NewEmptyHorizontal();
            Button editMode = DocRuntime.NewButton("Edit Layout", () =>
            {
                Save();
                isEditMode = true;
                root.Insert(1, header);
                contents.Clear();
                contents.Add(createEdit());
            });
            Button viewMode = DocRuntime.NewButton("View Layout", () =>
            {
                Save();
                isEditMode = false;
                if (root.Contains(header)) { root.Remove(header); }
                contents.Clear();
                contents.Add(createView());
            });
            Button defuMode = DocRuntime.NewButton("Inspector", () =>
            {
                Save();
                isEditMode = false;
                if (root.Contains(header)) { root.Remove(header); }
                contents.Clear();
                contents.Add(new IMGUIContainer(() => { DrawDefaultInspector(); }));
            });
            Button saveBtn = DocRuntime.NewButton("Save", DocStyle.Current.SuccessColor, Save);
            saveBtn.style.SetIS_Style(new ISMargin(TextAnchor.MiddleRight));
            viewMode.style.marginLeft = 7;
            defuMode.style.marginLeft = 7;
            bar.style.marginBottom = 10;
            bar.Add(editMode);
            bar.Add(viewMode);
            bar.Add(defuMode);
            bar.Add(saveBtn);
            bar.style.height = 20;
            root.Add(bar);

            #endregion

            #region header bar
            icon = DocEditor.NewObjectField<Texture2D>("icon", (value) =>
            {
                Target.Icon = (Texture2D)value.newValue;
                DocEditorWindow.RepaintMenu();
            });
            icon.value = Target.Icon;
            icon[0].style.minWidth = 95;

            aniSetting =DocRuntime.NewEmptyHorizontal();
            EnumField aniModeField = DocEditor.NewEnumField("Animation Mode", Target.AnimationMode, value =>
            {
                Target.AnimationMode = (DocPageAniMode)value.newValue;
            });
            aniModeField[0].style.minWidth = 96;
            aniModeField.style.width = Length.Percent(49);
            aniSetting.Add(aniModeField);
            IntegerField durField = new IntegerField();
            durField.label = "Duration";
            durField.style.ClearMarginPadding();
            durField.style.width = Length.Percent(50);
            durField[0].style.minWidth = 60;
            durField.value = Target.AnimationDuration;
            durField.RegisterValueChangedCallback((value) =>
            {
                Target.AnimationDuration = value.newValue;
            });
            aniSetting.Add(durField);
            addAndDeleteBar = DocRuntime.NewEmptyHorizontal();
            addPage = DocRuntime.NewButton("Add New Page", newPageBtn);
            addPage.style.width = Length.Percent(75);
            deletePage = DocRuntime.NewCheckButton("Delete Page",
                DocStyle.Current.DangerColor, DocStyle.Current.DangerColor, DocStyle.Current.SuccessColor, () =>
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(Target));
                    AssetDatabase.Refresh();
                    if (DocEditorWindow.Instance != null)
                        DocEditorWindow.RepaintMenu();
                });
            deletePage.style.width = Length.Percent(24);
            addAndDeleteBar.Add(addPage);
            addAndDeleteBar.Add(deletePage);

            header.style.marginBottom = 20;
            root.Add(header);
            #endregion
            root.RegisterCallback<KeyDownEvent>(e =>
            {
                if (e.ctrlKey && e.keyCode == KeyCode.S) { Save(); }
            });

            contents.Add(createEdit());
            addComponent = new Button();
            addComponent.text = "Add";
            addComponent.clicked += () => { EditRoot.Add(createUnit(new DocComponent())); };
            addComponent.style.marginTop = 10;
            contents.Add(addComponent);
            root.Add(contents);

            VisualElement loadAndSave = DocRuntime.NewEmptyHorizontal();
            Button loadFromTemplate = DocRuntime.NewButton("Load SO DocComponents", () =>
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
                        EditRoot.Clear();
                        foreach (var com in AssetDatabase.LoadAssetAtPath<SODocComponents>(path + '/'+select.value+".asset").Components)
                        {
                            EditRoot.Add(createUnit(com));
                        }
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
            loadAndSave.Add(loadFromTemplate);
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
            loadAndSave.Add(saveAsTemplate);

            header.Add(aniSetting);
            header.Add(icon);
            header.Add(addAndDeleteBar);
            header.Add(new IMGUIContainer(() => {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SubPages"));
                if (EditorGUI.EndChangeCheck()) { OnSubPagesChange?.Invoke(); serializedObject.ApplyModifiedProperties(); }
            }));
            header.Add(loadAndSave);
            return root;
        }
        private void OnDisable() { Save(); }

        public VisualElement EditRoot;
        VisualElement clickMask;
        VisualElement createEdit()
        {

            dragPosition = new ISPosition() { Left = ISStyleLength.Pixel(0), Top = ISStyleLength.Pixel(0) };
            dragPosition.Position = Position.Absolute;
            EditRoot = new VisualElement();
            EditRoot.style.height = Length.Percent(100);
            foreach (var doc in Target.Components)
            {
                EditRoot.Add(createUnit(doc));
            }
            clickMask.RegisterCallback<MouseMoveEvent>((e) =>
            {
                if (isDraging)
                {
                    int i = calDragingIndex();
                    for (int j = 0; j < EditRoot.childCount; j++)
                    {
                        if (i == j)
                            EditRoot[j].style.borderTopWidth = 5;
                        else
                            EditRoot[j].style.borderTopWidth = 0;
                    }
                    dragPosition.Top.Value.Value = e.localMousePosition.y + 1 - header.layout.yMax;
                    dragPosition.Left.Value.Value = e.localMousePosition.x + 1;
                    dragingTarget.style.SetIS_Style(dragPosition);
                }
            });
            return EditRoot;
        }
        VisualElement createView()
        {
            return new DocPageVisual(Target);
        }

        public event Action<SODocPage> OnSaveData;
        public event Action OnSubPagesChange;
        public void Save()
        {
            if (Target == null) return;

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
            if (isEditMode)
            {
                List<DocComponent> newComponents = new List<DocComponent>();
                if (EditRoot == null) return;
                foreach (var visual in EditRoot.Children())
                {
                    var edit = visual.Q<DocEditVisual>();
                    if (edit == null) continue;
                    if (edit.Target == null) continue;
                    newComponents.Add(edit.Target);
                }
                Target.Components = newComponents;
            }
            EditorUtility.SetDirty(target);
            OnSaveData?.Invoke(Target);
        }

        VisualElement createUnit(DocComponent doc)
        {
            VisualElement unit = DocRuntime.NewEmpty();
            VisualElement toolBar = DocRuntime.NewEmptyHorizontal();
            var docEdit = DocEditor.CreateEditVisual(doc);
            toolBar.Add(insertBtn(unit));
            toolBar.Add(dragBtn(unit));
            toolBar.Add(dupBtn(unit));
            toolBar.Add(deleteBtn(unit));
            unit.Add(toolBar);
            unit.Add(docEdit);
            unit.style.borderBottomWidth = 3;
            unit.style.borderBottomColor = DocStyle.Current.SubBackgroundColor;
            unit.style.borderTopColor = DocStyle.Current.SuccessColor;
            toolBar.style.marginTop = 7;
            unit.style.paddingBottom = 7;
            return unit;
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
                Target.SubPages.Add(asset);
                header.Remove(root);

                OnCreateNewPage?.Invoke(asset);
                OnSubPagesChange?.Invoke();
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
        Button insertBtn(VisualElement unit)
        {
            Button button = DocRuntime.NewButton("> Insert", () =>
            {
                int index = EditRoot.IndexOf(unit);
                EditRoot.Insert(index, createUnit(new DocComponent()));
            });
            button.style.width = 60;
            return button;
        }
        CheckButton deleteBtn(VisualElement unit)
        {
            CheckButton button = DocRuntime.NewCheckButton("Delete", () => { EditRoot.RemoveAt(EditRoot.IndexOf(unit)); });
            button.style.SetIS_Style(new ISMargin(TextAnchor.MiddleRight));
            button.MainBtn.style.width = 80;
            return button;
        }
        Button dragBtn(VisualElement unit)
        {
            Button button = DocRuntime.NewButton("Drag", () =>
            {
                isDraging = true;
                dragingTarget = unit;
                EditRoot.Add(unit);
                root.Add(clickMask);
                clickMask.RegisterCallback<MouseDownEvent>(endDraging);
                float sumHeight = 0;
                foreach (var ve in EditRoot.Children()) { sumHeight += ve.layout.height; }
                sumHeight += 400;
                root.style.height = sumHeight;
                EditRoot.style.height = sumHeight;
            });
            button.style.width = 40;
            return button;
        }        
        Button dupBtn(VisualElement unit)
        {
            Button button = DocRuntime.NewButton("Dup", () =>
            {
                int i = EditRoot.IndexOf(unit);
                EditRoot.Insert(i, createUnit(((DocEditField)unit[1]).Target.Copy()));
            });
            button.style.width = 40;
            return button;
        }
        void endDraging(MouseDownEvent e)
        {
            isDraging = false;
            root.style.height = Length.Percent(100);
            EditRoot.style.height = Length.Percent(100);
            dragingTarget.style.SetIS_Style(new ISPosition());
            EditRoot.Insert(calDragingIndex(), dragingTarget);
            dragingTarget = null;
            for (int j = 0; j < EditRoot.childCount; j++)
            {
                EditRoot[j].style.borderTopWidth = 0;
            }
            root.Remove(clickMask);
            clickMask.UnregisterCallback<MouseDownEvent>(endDraging);
            Save();
        }
        int calDragingIndex()
        {
            int i = 0;
            float pos = dragPosition.Top.Value.Value - header.layout.yMax;
            foreach (var ve in EditRoot.Children())
            {
                if (ve.layout.y > pos)
                    break;
                i++;
            }
            return i;
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