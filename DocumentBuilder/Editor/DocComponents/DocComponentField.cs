using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static NaiveAPI_Editor.DocumentBuilder.DocEditVisual;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocComponentsField : VisualElement
    {
        public VisualElement ComponentsVisualRoot;
        public event Action<DocComponentField, string> OnPropertyChanged;
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
            OnPropertyChanged += (_, info) =>
            {
                if (info == "Drag" ||
                    info == "Insert" ||
                    info == "Remove" ||
                    info == "Add")
                { reSerializeComponents(); }
            };
            if (autoSave)
                OnPropertyChanged += (_, _) =>
                { serializedProperty.serializedObject.ApplyModifiedProperties(); };
            Undo.undoRedoPerformed += rePaintUndoRedoComponents;
        }
        public DocComponentsField(IEnumerable<DocComponentProperty> initComponents)
        {
            style.backgroundColor = DocStyle.Current.BackgroundColor;
            style.SetIS_Style(ISPadding.Pixel(5));
            ComponentsVisualRoot = new VisualElement();
            Repaint(initComponents);
            Add(ComponentsVisualRoot);
            Add(new DSButton("Add Component", () =>
            {
                var doc = new DocComponentField(new DocComponentProperty(new DocComponent()));
                ComponentsVisualRoot.Add(doc);
                doc.SetStatus(true);
                doc.OnPropertyChanged += info => { OnPropertyChanged?.Invoke(doc, info); };
                OnPropertyChanged?.Invoke(doc, "Add");
            }));
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
        void reSerializeComponents()
        {
            var coms = ToComponentsList();
            SerializedComponents.ClearArray();
            int i = 0;
            List<DocComponentProperty> props = new();
            foreach (var component in coms)
            {
                SerializedComponents.InsertArrayElementAtIndex(i);
                var prop = new DocComponentProperty(SerializedComponents.GetArrayElementAtIndex(i));
                prop.FromDocComponent(component);
                props.Add(prop);
                i++;
            };
            SerializedComponents.serializedObject.ApplyModifiedProperties();
            Repaint(props);
        }
        public void Repaint(IEnumerable<DocComponentProperty> components)
        {
            ComponentsVisualRoot.Clear();
            if (components != null)
            {
                foreach (var component in components)
                {
                    var doc = new DocComponentField(component);
                    ComponentsVisualRoot.Add(doc);
                    doc.OnPropertyChanged += info => { OnPropertyChanged?.Invoke(doc, info); };
                }
            }
        }
        public void CtrlHotKeyAction(KeyCode keycode)
        {
            if (keycode == KeyCode.S)
            {
                foreach (DocComponentField doc in ComponentsVisualRoot.Children())
                    doc.SetStatus(false);
            }
            else if(keycode == KeyCode.UpArrow)
            {
                for(int i = 0; i < ComponentsVisualRoot.childCount; i++)
                {
                    if (((DocComponentField)ComponentsVisualRoot[i]).IsEditing)
                    {
                        if (i != 0)
                        {
                            ComponentsVisualRoot.Insert(i - 1, ComponentsVisualRoot[i]);
                            OnPropertyChanged?.Invoke((DocComponentField)ComponentsVisualRoot[i-1], "MoveUp");
                        }
                        break;
                    }
                }
            }
            else if (keycode == KeyCode.DownArrow)
            {
                for (int i = 0; i < ComponentsVisualRoot.childCount; i++)
                {
                    if (((DocComponentField)ComponentsVisualRoot[i]).IsEditing)
                    {
                        if (i != ComponentsVisualRoot.childCount - 1)
                        {
                            ComponentsVisualRoot.Insert(i + 1, ComponentsVisualRoot[i]);
                            OnPropertyChanged?.Invoke((DocComponentField)ComponentsVisualRoot[i + 1], "MoveDown");
                        }
                        break;
                    }
                }
            }
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
        public event Action<string> OnPropertyChanged;
        public DocComponentProperty Target;
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
            VisualElement ve = DocRuntime.CreateDocVisual(Target.ToDocComponent());
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
                ToolBar.Add(deleteBtn());
            }
            ToolBar.Add(closeBtn());
            EditView.Add(ToolBar);
            Type docType = null;
            if (Dict.ID2Type.TryGetValue(Target.VisualID, out docType))
            {
                DocEditVisual = (DocEditVisual)Activator.CreateInstance(docType);
                DocEditVisual.SetTarget(Target);
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
            Target = docComponent;
            Target.OnPropertyChanged += (info) => { OnPropertyChanged?.Invoke(info); };
            RegisterCallback<GeometryChangedEvent>(e =>
            {
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
            if (Target.IsSerializedProperty)
            {
                Undo.undoRedoPerformed += undoRedoRepaint;
            }
        }
        ~DocComponentField()
        {
            if (Target.IsSerializedProperty)
            {
                Undo.undoRedoPerformed -= undoRedoRepaint;
            }
        }
        void undoRedoRepaint()
        {
            if (panel == null) return;
            Target?.SerializedProperty?.serializedObject?.Update();
            Repaint();
        }
        private void createDropfield()
        {
            SelectVisualType = new DSDropdown();
            SelectVisualType[0].style.ClearMarginPadding();
            SelectVisualType.choices = Dict.NameList;
            string tName = string.Empty;
            Dict.ID2Name.TryGetValue(Target.VisualID, out tName);
            SelectVisualType.index = Dict.NameList.FindIndex(0, (str) => { return str == tName; });
            if (SelectVisualType.index == -1) SelectVisualType.index = 0;
            SelectVisualType.RegisterValueChangedCallback((val) =>
            {
                Target.Clear();
                EditView.RemoveAt(1);
                if (val.newValue == "None")
                    Target.VisualID = string.Empty;
                else
                    Target.VisualID = Dict.Name2ID[val.newValue];
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
            button.style.marginLeft = StyleKeyword.Auto;
            button.style.unityFontStyleAndWeight = FontStyle.Bold;
            return button;
        }
        Button insertBtn()
        {
            Button button = null;
            button = new DSButton("Insert", () =>
            {
                DocComponentField doc = new DocComponentField(new DocComponentProperty(new DocComponent()));
                parent.Insert(Index, doc);
                doc.SetStatus(true);
                doc.OnPropertyChanged += this.OnPropertyChanged;
                OnPropertyChanged?.Invoke("Insert");
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
                copyBuffer = Target.ToDocComponent();
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
                    Target.FromDocComponent(copyBuffer);
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
                    OnPropertyChanged?.Invoke("Drag");
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
                DocComponentField doc = new DocComponentField(new DocComponentProperty(Target.ToDocComponent()));
                parent.Insert(Index + 1, doc);
                doc.SetStatus(true);
                doc.OnPropertyChanged += this.OnPropertyChanged;
                OnPropertyChanged?.Invoke("Duplicate");
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
                parent?.Remove(this);
                OnPropertyChanged?.Invoke("Remove");
            });
            button.style.backgroundImage = DocEditor.Icon.Delete;
            button.style.height = 20;
            button.style.width = 20;
            button.style.marginLeft = 5;
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
    }
}
