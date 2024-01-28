using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static NaiveAPI_Editor.DocumentBuilder.DocComponentField;
using static NaiveAPI_Editor.DocumentBuilder.DocEditVisual;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocComponentsField : VisualElement
    {
        public VisualElement ComponentsVisualRoot;
        public event Action<DocComponentField, string> OnPropertyChanged;
        // OperateType, SelfIndex, AdditionInfo
        public event Action<FieldOperate, int, int> OnFieldOperate;
        public bool IsDraging
        {
            get
            {
                foreach(var doc in ComponentsVisualRoot.Children())
                {
                    if(doc is DocComponentField)
                    if(((DocComponentField)doc).IsDraging)return true;
                }return false;
            }
        }
        private bool m_IsSerializedProperty = false;
        public bool IsSerializedProperty => m_IsSerializedProperty;
        private SerializedProperty SerializedComponents;
        public DocComponentsField(IEnumerable<DocComponent> initComponents)
            : this(initComponents.Select(component => { return new DocComponentProperty(component); })) { }
        public DocComponentsField(SerializedProperty serializedProperty, bool autoSave = true)
            : this(DocComponentProperty.LoadArrayProperty(serializedProperty))
        {
            SerializedComponents = serializedProperty;
            m_IsSerializedProperty = true;
            if (autoSave)
            {
                OnPropertyChanged += (_, _) =>
                { serializedProperty.serializedObject.ApplyModifiedProperties(); };
                OnFieldOperate += (operate, index, additionInfo) =>
                {
                    switch (operate)
                    {
                        case FieldOperate.Insert:
                            serializedProperty.InsertArrayElementAtIndex(index);
                            var prop = new DocComponentProperty(serializedProperty.GetArrayElementAtIndex(index));
                            var field = (ComponentsVisualRoot[index] as DocComponentField);
                            regEvent(field);
                            prop.FromDocComponent(field.Target.ToDocComponent());
                            field.Target = prop;
                            serializedProperty.serializedObject.ApplyModifiedProperties();
                            for (int i = 0, imax = ComponentsVisualRoot.childCount; i < imax; i++)
                                (ComponentsVisualRoot[i] as DocComponentField).Target = new DocComponentProperty(serializedProperty.GetArrayElementAtIndex(i));
                            break;
                        case FieldOperate.Delete:
                            serializedProperty.DeleteArrayElementAtIndex(index);
                            serializedProperty.serializedObject.ApplyModifiedProperties();
                            break;
                        case FieldOperate.MoveToIndex:
                            if (index == additionInfo) return;
                            serializedProperty.MoveArrayElement(index, additionInfo);
                            serializedProperty.serializedObject.ApplyModifiedProperties();
                            for (int i = 0, imax = ComponentsVisualRoot.childCount; i < imax; i++)
                                (ComponentsVisualRoot[i] as DocComponentField).Target = new DocComponentProperty(serializedProperty.GetArrayElementAtIndex(i));
                            break;
                        default:
                            break;
                    }
                };
            }
            Undo.undoRedoPerformed += rePaintUndoRedoComponents;
        }
        public DocComponentsField(IEnumerable<DocComponentProperty> initComponents)
        {
            style.backgroundColor = DocStyle.Current.BackgroundColor;
            style.SetIS_Style(ISPadding.Pixel(5));
            ComponentsVisualRoot = new VisualElement();
            Repaint(initComponents);
            Add(ComponentsVisualRoot);
            var addBtn = new DSButton("Add Component", () =>
            {
                addNewComponent("");
            });
            var addChioce = new DSDropdown();
            addChioce.choices = Dict.NameList;
            addChioce.RegisterValueChangedCallback(evt =>
            {
                addChioce.SetValueWithoutNotify(""); 
                if (evt.newValue == "None") return;
                addNewComponent(Dict.Name2ID[evt.newValue]);
            });
            Add(new DSHorizontal(addBtn,addChioce));
        }
        ~DocComponentsField()
        {
            Undo.undoRedoPerformed -= rePaintUndoRedoComponents;
        }
        void rePaintUndoRedoComponents()
        {
            if (panel == null) return;
            SerializedComponents.serializedObject.Update();
            if (SerializedComponents.arraySize != ComponentsVisualRoot.childCount)
            {
                Repaint(DocComponentProperty.LoadArrayProperty(SerializedComponents));
            }
        }
        public void Repaint(IEnumerable<DocComponentProperty> components)
        {
            ComponentsVisualRoot.Clear();
            if (components != null)
            {
                foreach (var component in components)
                {
                    var field = new DocComponentField(component);
                    regEvent(field);
                    ComponentsVisualRoot.Add(field);
                }
            }
        }
        void regEvent(DocComponentField field)
        {
            field.OnPropertyChanged += info => { OnPropertyChanged?.Invoke(field, info); };
            field.OnFieldOperate += (a1, a2, a3) => { OnFieldOperate?.Invoke(a1, a2, a3); };
        }
        void addNewComponent(string id)
        {
            DocComponentProperty prop = null;
            if (IsSerializedProperty)
            {
                int i = SerializedComponents.arraySize;
                SerializedComponents.InsertArrayElementAtIndex(i);
                SerializedComponents.serializedObject.ApplyModifiedProperties();
                prop = new DocComponentProperty(SerializedComponents.GetArrayElementAtIndex(i));
                prop.Clear();
            }
            else
            {
                prop = new DocComponentProperty(new DocComponent());
            }
            prop.VisualID = id;
            var field = new DocComponentField(prop);
            regEvent(field);
            ComponentsVisualRoot.Add(field);
            field.SetStatus(true);
        }
        public List<DocComponent> ToComponentsList()
        {
            var output = new List<DocComponent>();
            foreach (DocComponentField ve in ComponentsVisualRoot.Children())
            {
                output.Add(ve.Target.ToDocComponent());
            }
            return output;
        }
    }
    public class DocComponentField : VisualElement
    {
        public enum FieldOperate
        {
            Insert,
            Delete,
            MoveToIndex,
        }
        public event Action<string> OnPropertyChanged;
        // OperateType, SelfIndex, AdditionInfo
        public event Action<FieldOperate,int,int> OnFieldOperate;
        public DocComponentProperty Target
        {
            get => m_Target;
            set
            {
                m_Target.OnPropertyChanged -= invokePropertyChange;
                m_Target = value;
                m_Target.OnPropertyChanged += invokePropertyChange;
            }
        }
        private DocComponentProperty m_Target;
        public DSDropdown SelectVisualType;
        public VisualElement ToolBar;
        public VisualElement EditView;
        public DocEditVisual DocEditVisual;
        public int Index
        {
            get
            {
                if (parent == null) return -1;
                return parent.IndexOf(this);
            }
        }
        public bool IsEditing = false;
        public bool IsDraging = false;
        public bool SingleMode { get { return m_singleMode; } }
        private bool m_singleMode;
        private static DocComponent copyBuffer;
        private DocComponent startEditingStatus;
        public void SetStatus(bool isEditing)
        {
            if (IsEditing == isEditing) return;
            IsEditing = isEditing;
            if (IsEditing)
                closeOther();
            Repaint();
        }
        VisualElement createPreview()
        {
            style.borderLeftColor = DocStyle.Current.HintColor;
            RegisterCallback<PointerDownEvent>(enableEditMode);
            VisualElement ve = DocRuntime.CreateDocVisual(m_Target.ToDocComponent());
            foreach (var child in ve.Children())
                child.SetEnabled(false);
            return ve;
        }
        VisualElement createEdit()
        {
            style.borderLeftColor = DocStyle.Current.SuccessColor;
            UnregisterCallback<PointerDownEvent>(enableEditMode);
            EditView = new VisualElement();
            ToolBar = new DSHorizontal();
            ToolBar.style.flexGrow = 1;
            if (!m_singleMode)
                ToolBar.Add(insertBtn());
            createDropfield();
            ToolBar.Add(SelectVisualType);
            ToolBar.Add(CopyBtn());
            ToolBar.Add(PasetBtn());
            if (!m_singleMode)
            {
                ToolBar.Add(duplicateBtn());
                ToolBar.Add(dragBtn());
            }
            ToolBar.Add(closeBtn());
            if (!m_singleMode)
                ToolBar.Add(deleteBtn());
            EditView.Add(ToolBar);
            Type docType = null;
            if (Dict.ID2Type.TryGetValue(m_Target.VisualID, out docType))
            {
                DocEditVisual = (DocEditVisual)Activator.CreateInstance(docType);
                DocEditVisual.SetTarget(m_Target);
                EditView.Add(DocEditVisual);
            }
            else
            {
                VisualElement visualElement = new DSHorizontal();
                visualElement.style.marginTop = 5;
                visualElement.style.alignItems = Align.Center;
                visualElement.style.flexWrap = Wrap.Wrap;
                Button editFavorite = new DSButton("", () =>
                {
                    if (editFavoriteRoot != null)
                    {
                        EditView.Remove(editFavoriteRoot);
                        editFavoriteRoot = null;
                    }
                    else
                    {
                        createEditFavorite();
                        EditView.Add(editFavoriteRoot);
                    }
                });
                editFavorite.style.backgroundImage = DocEditorData.Icon.StarIcon;
                editFavorite.style.unityBackgroundImageTintColor = new Color(.8f, .6f, .2f);
                editFavorite.style.width = 18;
                editFavorite.style.height = 18;
                visualElement.Add(editFavorite);
                foreach (var id in DocCache.Get().FavoriteDocVisualID)
                {
                    string name;
                    if (Dict.ID2Name.TryGetValue(id, out name))
                    {
                        if (name == "None") continue;

                        string displayName;
                        int i = name.LastIndexOf('/');
                        if (i != -1)
                            displayName = name.Substring(i + 1);
                        else displayName = name;
                        Button button = new DSButton(displayName, () =>
                        {
                            SelectVisualType.value = name;
                        });
                        button.style.marginLeft = 5;
                        visualElement.Add(button);
                    }
                }
                EditView.Add(visualElement);
                if (editFavoriteRoot != null)
                    EditView.Add(editFavoriteRoot);
            }
            return EditView;
        }
        public DocComponentField(SerializedProperty serializedProperty, bool singleMode = false)
            : this(new DocComponentProperty(serializedProperty), singleMode) { }
        public DocComponentField(DocComponent docComponent, bool singleMode = false)
            : this(new DocComponentProperty(docComponent), singleMode) { }
        public DocComponentField(DocComponentProperty docComponent, bool singleMode = false)
        {
            m_singleMode = singleMode;
            style.borderTopWidth = 6;
            style.borderLeftWidth = 6;
            style.borderBottomWidth = 6;
            style.paddingLeft = 6;
            style.borderLeftColor = DocStyle.Current.HintColor;
            style.marginBottom = 1;
            m_Target = docComponent;
            m_Target.OnPropertyChanged += invokePropertyChange;
            bool isInit = true;
            RegisterCallback<GeometryChangedEvent>(e =>
            {
                if (isInit)
                {
                    isInit = false;
                    return;
                }
                if (e.oldRect.y == 0) return;
                Vector2 orgpos = transform.position;
                Vector2 toPos = orgpos;
                toPos.y += e.oldRect.y - e.newRect.y;
                transform.position = toPos;
                this.GoToPosition(Vector2.zero);
            });
            Color highlight = DocStyle.Current.HintColor;
            highlight *= 1.3f;
            RegisterCallback<PointerEnterEvent>(e =>
            {
                if (IsDraging || IsEditing) return;
                style.borderLeftColor = highlight;
            });
            RegisterCallback<PointerLeaveEvent>(e =>
            {
                if (IsDraging || IsEditing) return;
                style.borderLeftColor = DocStyle.Current.HintColor;
            });
            Repaint();
            if (m_Target.IsSerializedProperty)
            {
                Undo.undoRedoPerformed += UndoRedoRepaint;
            }
        }
        ~DocComponentField()
        {
            if (m_Target.IsSerializedProperty)
            {
                Undo.undoRedoPerformed -= UndoRedoRepaint;
            }
        }
        public void UndoRedoRepaint()
        {
            if (panel == null) return;
            m_Target?.SerializedProperty?.serializedObject?.Update();
            Repaint();
        }
        private void createDropfield()
        {
            SelectVisualType = new DSDropdown();
            SelectVisualType[0].style.ClearMarginPadding();
            SelectVisualType.choices = Dict.NameList;
            string tName = string.Empty;
            Dict.ID2Name.TryGetValue(m_Target.VisualID, out tName);
            SelectVisualType.index = Dict.NameList.FindIndex(0, (str) => { return str == tName; });
            if (SelectVisualType.index == -1) SelectVisualType.index = 0;
            SelectVisualType.RegisterValueChangedCallback((val) =>
            {
                m_Target.Clear();
                EditView.RemoveAt(1);
                if (val.newValue == "None")
                    m_Target.VisualID = string.Empty;
                else
                    m_Target.VisualID = Dict.Name2ID[val.newValue];
                OnPropertyChanged?.Invoke("VisualType");
                Repaint();
            });
            SelectVisualType.value = Dict.NameList[SelectVisualType.index];
            SelectVisualType[0].style.paddingLeft = 5;
            SelectVisualType.style.height = 20;
            SelectVisualType.style.width = 160;
        }
        VisualElement editFavoriteRoot;
        public void Repaint()
        {
            Clear();
            if (IsEditing)
                Add(createEdit());
            else
                Add(createPreview());
        }
        void createEditFavorite()
        {
            editFavoriteRoot = new VisualElement();
            foreach (var name in Dict.NameList)
            {
                if (name == "None") continue;
                Toggle toggle = new Toggle();
                toggle.style.marginLeft = 10;
                toggle.text = name;
                toggle.value = false;
                foreach (var id in DocCache.Get().FavoriteDocVisualID)
                {
                    string tName;
                    if (Dict.ID2Name.TryGetValue(id, out tName))
                    {
                        if (tName == name)
                            toggle.value = true;
                    }
                }
                toggle.RegisterValueChangedCallback(e =>
                {
                    string t;
                    if (e.newValue)
                    {
                        if (Dict.Name2ID.TryGetValue(name, out t))
                        {
                            DocCache.Get().FavoriteDocVisualID.Add(t);
                            DocCache.Save();
                        }
                    }
                    else
                    {
                        if (Dict.Name2ID.TryGetValue(name, out t))
                        {

                            DocCache.Get().FavoriteDocVisualID.Remove(t);
                            DocCache.Save();
                        }
                    }
                    EditorUtility.SetDirty(DocEditorData.Instance);
                    Clear();
                    Repaint();
                });
                editFavoriteRoot.Add(toggle);
            }
        }

        Button closeBtn()
        {
            Button button = null;
            button = new DSButton("-", DocStyle.Current.HintColor, () =>
            {
                SetStatus(false);
            });
            button.style.width = 20;
            button.style.height = 20;
            button.style.marginLeft = 5;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            return button;
        }
        Button insertBtn()
        {
            Button button = null;
            button = new DSButton("Insert", () =>
            {
                DocComponentField doc = new DocComponentField(new DocComponentProperty(new DocComponent()));
                var index = Index;
                parent.Insert(index, doc);
                doc.SetStatus(true);
                doc.OnPropertyChanged += this.OnPropertyChanged;
                OnFieldOperate?.Invoke(FieldOperate.Insert, index, -1);
            });
            button.style.width = 41;
            button.style.height = 20;
            return button;
        }
        public Button CopyBtn()
        {
            Button button = null;
            button = new DSButton("", () =>
            {
                copyBuffer = m_Target.ToDocComponent();
                button.style.unityBackgroundImageTintColor = DocStyle.Current.SuccessTextColor;
                button.schedule.Execute(() =>
                {
                    button.style.unityBackgroundImageTintColor = Color.white;
                }).ExecuteLater(1000);
            });
            button.style.backgroundImage = DocEditor.Icon.Copy;
            button.style.height = 20;
            button.style.width = 20;
            button.style.marginLeft = 5;
            return button;
        }
        public Button PasetBtn()
        {
            Button button = null;
            button = new DSButton("", () =>
            {
                if (copyBuffer == null)
                {
                    button.style.unityBackgroundImageTintColor = DocStyle.Current.DangerTextColor;
                    button.schedule.Execute(() =>
                    {
                        button.style.unityBackgroundImageTintColor = Color.white;
                    }).ExecuteLater(1000);
                }
                else
                {
                    m_Target.FromDocComponent(copyBuffer);
                    OnPropertyChanged?.Invoke("Paset");
                    Repaint();
                }
            });
            button.style.backgroundImage = DocEditor.Icon.Paste;
            button.style.height = 20;
            button.style.width = 20;
            button.style.marginLeft = 5;
            return button;
        }
        Button dragBtn()
        {
            Button button = null;
            button = new DSButton("☰", () =>
            {
                int begIndex = Index;
                IsDraging = true;
                SetStatus(false);
                style.borderLeftColor = DocStyle.Current.WarningColor;
                VisualElement dragMask = new VisualElement();
                dragMask.style.width = parent.resolvedStyle.width;
                dragMask.style.height = parent.resolvedStyle.height;
                dragMask.style.position = Position.Absolute;
                dragMask.RegisterCallback<PointerMoveEvent>(whileDraging);
                dragMask.RegisterCallback<PointerDownEvent>(e =>
                {
                    parent.Remove(dragMask);
                    style.borderLeftColor = DocStyle.Current.HintColor;
                    IsDraging = false;
                    SetStatus(true);
                    OnFieldOperate?.Invoke(FieldOperate.MoveToIndex, begIndex, Index);
                });
                parent.Add(dragMask);
            });
            button.style.height = 20;
            button.style.width = 20;
            button.style.marginLeft = 5;
            return button;
        }
        Button duplicateBtn()
        {
            Button button = null;
            button = new DSButton("", () =>
            {
                DocComponentField doc = new DocComponentField(new DocComponentProperty(m_Target.ToDocComponent()));
                var index = Index + 1;
                parent.Insert(index, doc);
                doc.SetStatus(true);
                doc.OnPropertyChanged += this.OnPropertyChanged;
                OnFieldOperate?.Invoke(FieldOperate.Insert, index, -1);
            });
            button.style.backgroundImage = DocEditor.Icon.Duplicate;
            button.style.height = 20;
            button.style.width = 20;
            button.style.marginLeft = 5;
            return button;
        }
        Button deleteBtn()
        {
            Button button = null;
            button = new DSButton("", () =>
            {
                var index = Index;
                parent.Remove(this);
                OnFieldOperate?.Invoke(FieldOperate.Delete, index, -1);
            });
            button.style.backgroundImage = DocEditor.Icon.Delete;
            button.style.height = 20;
            button.style.width = 20;
            button.style.marginLeft = StyleKeyword.Auto;
            button.style.unityBackgroundImageTintColor = DocStyle.Current.DangerTextColor;
            return button;
        }
        void closeOther()
        {
            if (parent == null) return;
            foreach (var ve in parent.Children())
            {
                if(ve is DocComponentField)
                {
                    if (ve != this) ((DocComponentField)ve).SetStatus(false);
                }
            }
        }
        void whileDraging(PointerMoveEvent e)
        {
            int i = calDragingIndex(e.localPosition.y);
            if (Index != i)
            {
                parent.Insert(i, this);
            }
        }
        int calDragingIndex(float yPos)
        {
            float sum = 0;
            int i = 0;
            foreach(var ve in parent.Children())
            {
                sum += ve.resolvedStyle.height;
                if (sum- ve.resolvedStyle.height/2 > yPos ) return i;
                i++;
            }
            return i;
        }
        void enableEditMode(PointerDownEvent e)
        {
            SetStatus(true);
        }

        void invokePropertyChange(string info)
        {
            OnPropertyChanged?.Invoke(info);
        }
    }
}
