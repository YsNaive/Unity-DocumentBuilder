using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
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
        public SODocPage Target;
        VisualElement root;
        bool isDraging = false;
        VisualElement dragingTarget;
        ISPosition dragPosition;
        VisualElement contents;
        VisualElement header;
        Button addComponent;
        VisualElement addAndDeleteBar;
        Button addPage;
        CheckButton deletePage;
        VisualElement introSetting;
        VisualElement outtroSetting;
        ObjectField icon;
        List<string> buildinIconList = new List<string>();
        public override VisualElement CreateInspectorGUI()
        {
            Target = target as SODocPage;
            root = DocRuntime.NewEmpty();
            #region mod bar
            root.style.SetIS_Style(ISPadding.Pixel(10));
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            root.RegisterCallback<GeometryChangedEvent>(e =>
            {
                float sum = 0;
                foreach (var ve in root.Children()) { sum += ve.layout.height; }
                if(!isDraging&&(root.style.height.value.value != sum + 400))
                {
                    root.style.height = sum + 800;
                }
            });
            header = DocRuntime.NewEmpty();
            contents = DocRuntime.NewEmpty();
            clickMask = DocRuntime.NewEmpty();
            clickMask.style.SetIS_Style(ISSize.Percent(100, 100));
            clickMask.style.position = Position.Absolute;

            VisualElement bar = DocRuntime.NewEmptyHorizontal();
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
            EnumField outroMode = DocEditor.NewEnumField("Outtro Mode", Target.IntroMode, value =>
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
                DocStyle.Current.DangerColor, DocStyle.Current.DangerColor, DocStyle.Current.SuccessColor, () =>
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

            header.Add(introSetting);
            header.Add(outtroSetting);
            header.Add(iconHorBar);
            header.Add(addAndDeleteBar);
            header.Add(new IMGUIContainer(() => {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SubPages"));
            }));
            header.Add(loadAndSave);
            root.schedule.Execute(() =>
            {
                root.panel.visualTree.RegisterCallback<KeyDownEvent>(hotkey);
            }).ExecuteLater(500);
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
            EditRoot.style.backgroundColor = DocStyle.Current.BackgroundColor;
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
            List<DocComponent> newComponents = new List<DocComponent>();
            if (EditRoot == null) return;
            foreach (unit v in EditRoot.Children())
            {
                newComponents.Add(v.docComponent);
            }
            Target.Components = newComponents;
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
        }

        class unit : VisualElement
        {
            public VisualElement toolBar;
            public DocEditField editView;
            public DocComponent docComponent;
            public void ViewMode()
            {
                Clear();
                var ve = DocRuntime.CreateVisual(docComponent);
                ve.style.opacity = 0.7f;
                ve.style.marginTop = 7;
                Add(ve);
                this[0].RegisterCallback<PointerDownEvent>(e => { EditMode(); });
            }
            public void EditMode()
            {
                foreach (unit u in parent.Children())
                {
                    u.ViewMode();
                }
                Clear();
                Add(toolBar);
                Add(editView);
            }
        }
        unit createUnit(DocComponent doc)
        {
            unit unit = new unit();
            unit.docComponent = doc;
            unit.style.ClearMarginPadding();
            unit.toolBar = DocRuntime.NewEmptyHorizontal();
            VisualElement view = DocRuntime.CreateVisual(doc);
            unit.editView = DocEditor.CreateEditVisual(doc);
            unit.toolBar.Add(insertBtn(unit));
            unit.toolBar.Add(dragBtn(unit));
            unit.toolBar.Add(copyBtn(unit));
            unit.toolBar.Add(pasteBtn(unit));
            unit.toolBar.Add(dupBtn(unit));
            unit.toolBar.Add(deleteBtn(unit));
            unit.style.borderTopColor = DocStyle.Current.FrontGroundColor;
            unit.style.borderBottomColor = DocStyle.Current.SubBackgroundColor;
            unit.style.borderBottomWidth = 2;
            unit.toolBar.style.marginTop = 7;
            unit.style.paddingBottom = 7;
            unit.ViewMode();
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
            button.style.SetIS_Style(new ISMargin(TextAnchor.UpperRight));
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
                EditRoot.Insert(i+1, createUnit(((DocEditField)unit[1]).Target.Copy()));
            });
            button.style.width = 40;
            return button;
        }
        static DocComponent copyBuffer = null;
        Button copyBtn(unit unit)
        {
            Button button =null;
            button = DocRuntime.NewButton("Copy", () =>
            {
                copyBuffer = unit.docComponent.Copy();
                button.text = "Copied !";
                button.schedule.Execute(() =>
                {
                    button.text = "Copy";
                }).ExecuteLater(1000);
            });
            button.style.width = 60;
            return button;
        }
        Button pasteBtn(unit unit)
        {
            Button button = DocRuntime.NewButton("Paste", () =>
            {
                if (copyBuffer != null)
                {
                    unit.docComponent = copyBuffer.Copy();
                    unit.editView.SelectVisualType.value = DocEditor.ID2Name[unit.docComponent.VisualID];
                }
            });
            button.style.width = 60;
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
            float pos = dragPosition.Top.Value.Value;
            foreach (var ve in EditRoot.Children())
            {
                if (ve.layout.center.y > pos)
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

        void hotkey(KeyDownEvent e)
        {
            if (e.ctrlKey && e.keyCode == KeyCode.S)
            {
                Save();
                foreach (unit u in EditRoot.Children())
                    u.ViewMode();
            }
        }

    }
}