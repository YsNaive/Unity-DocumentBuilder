using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocComponentsField : VisualElement
    {
        public VisualElement ComponentsVisualRoot;
        private event Action<DocComponentField> m_onModify;
        public event Action<DocComponentField> OnModify
        {
            add
            {
                m_onModify += value;
                if (ComponentsVisualRoot != null)
                    foreach (DocComponentField doc in ComponentsVisualRoot.Children())
                        doc.OnModify += value;
            }
            remove
            {
                m_onModify -= value;
                if (ComponentsVisualRoot != null)
                    foreach (DocComponentField doc in ComponentsVisualRoot.Children())
                        doc.OnModify -= value;
            }
        }
        public bool IsDraging
        {
            get
            {
                foreach(DocComponentField doc in ComponentsVisualRoot.Children())
                {
                    if(doc.IsDraging)return true;
                }return false;
            }
        }
        public DocComponentsField(List<DocComponent> initComponents)
        {
            style.backgroundColor = SODocStyle.Current.BackgroundColor;
            style.SetIS_Style(ISPadding.Pixel(5));
            ComponentsVisualRoot = DocRuntime.NewEmpty();
            ComponentsVisualRoot.style.marginBottom = 15;
            Repaint(initComponents);
            Add(ComponentsVisualRoot);
            Add(DocRuntime.NewButton("Add Component", () =>
            {
                var doc = new DocComponentField(new DocComponent());
                ComponentsVisualRoot.Add(doc);
                doc.SetStatus(true);
                doc.OnModify += m_onModify;
                m_onModify?.Invoke(doc);
            }));
        }
        public void Repaint(List<DocComponent> components)
        {
            ComponentsVisualRoot.Clear();
            if (components != null)
            {
                if(components.Count != 0)
                {
                    foreach (var component in components)
                    {
                        var doc = new DocComponentField(component);
                        ComponentsVisualRoot.Add(doc);
                        doc.OnModify += m_onModify;
                    }
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
                            m_onModify?.Invoke((DocComponentField)ComponentsVisualRoot[i-1]);
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
                            m_onModify?.Invoke((DocComponentField)ComponentsVisualRoot[i + 1]);
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
                output.Add(ve.Target);
            }
            return output;
        }
    }
    public class DocComponentField : VisualElement
    {
        public event Action<DocComponentField> OnModify;
        public DocComponent Target;
        public DropdownField SelectVisualType;
        public VisualElement ToolBar;
        public VisualElement EditView;
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
        public static List<DocComponent> HistoryBuffer = new List<DocComponent>();
        private static DocComponent copyBuffer;
        private DocComponent startEditingStatus;
        public static void ClearHistory() { HistoryBuffer.Clear(); }
        public void SetStatus(bool isEditing)
        {
            if (IsEditing == isEditing) return;
            IsEditing = isEditing;
            if (IsEditing)
            {
                closeOther();
                startEditingStatus = Target.Copy();
            }
            else
            {
                if (!startEditingStatus.ContentsEqual(Target))
                {
                    OnModify?.Invoke(this);
                }
            }
            Repaint();
        }
        VisualElement createPreview()
        {
            style.borderLeftColor = SODocStyle.Current.HintColor;
            RegisterCallback<PointerDownEvent>(enableEditMode);
            VisualElement ve = DocRuntime.CreateVisual(Target);
            foreach (var child in ve.Children())
                child.SetEnabled(false);
            return ve;
        }
        VisualElement createEdit()
        {
            style.borderLeftColor = SODocStyle.Current.SuccessColor;
            UnregisterCallback<PointerDownEvent>(enableEditMode);
            EditView = DocRuntime.NewEmpty();
            ToolBar = DocRuntime.NewEmptyHorizontal();
            if (!m_singleMode)
                ToolBar.Add(insertBtn());
            createDropfield();
            ToolBar.Add(SelectVisualType);
            ToolBar.Add(copyBtn());
            ToolBar.Add(pasetBtn());
            if (!m_singleMode)
            {
                ToolBar.Add(duplicateBtn());
                ToolBar.Add(dragBtn());
                ToolBar.Add(deleteBtn());
            }
            EditView.Add(ToolBar);
            Type docType = null;
            if (DocEditor.ID2Type.TryGetValue(Target.VisualID, out docType))
            {
                DocEditVisual doc = (DocEditVisual)Activator.CreateInstance(docType);
                doc.SetTarget(Target);
                EditView.Add(doc);
            }
            else
            {
                VisualElement visualElement = DocRuntime.NewEmptyHorizontal();
                visualElement.style.marginTop = 5;
                visualElement.style.alignItems = Align.Center;
                visualElement.style.flexWrap = Wrap.Wrap;
                Button editFavorite = DocRuntime.NewButton("", () =>
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
                editFavorite.style.backgroundImage = DocEditorData.Instance.WhiteStar;
                editFavorite.style.unityBackgroundImageTintColor = new Color(.8f, .6f, .2f);
                editFavorite.style.width = 18;
                editFavorite.style.height = 18;
                visualElement.Add(editFavorite);
                foreach (var id in DocCache.Get().FavoriteDocVisualID)
                {
                    string name;
                    if (DocEditor.ID2Name.TryGetValue(id, out name))
                    {
                        if (name == "None") continue;

                        string displayName;
                        int i = name.LastIndexOf('/');
                        if (i != -1)
                            displayName = name.Substring(i + 1);
                        else displayName = name;
                        Button button = DocRuntime.NewButton(displayName, () =>
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
        public DocComponentField(DocComponent docComponent, bool singleMode = false)
        {
            m_singleMode = singleMode;
            style.borderTopWidth = 6;
            style.borderLeftWidth = 6;
            style.borderBottomWidth = 6;
            style.paddingLeft = 6;
            style.borderLeftColor = SODocStyle.Current.HintColor;
            style.marginBottom = 1;
            Target = docComponent;
            RegisterCallback<GeometryChangedEvent>(e =>
            {
                if (e.oldRect.y == 0) return;
                Vector2 orgpos = transform.position;
                Vector2 toPos = orgpos;
                toPos.y += e.oldRect.y - e.newRect.y;
                transform.position = toPos;
                this.GoToPosition(Vector2.zero);
            });
            Color highlight = SODocStyle.Current.HintColor;
            highlight *= 1.3f;
            RegisterCallback<PointerEnterEvent>(e =>
            {
                if (IsDraging || IsEditing) return;
                style.borderLeftColor = highlight;
            });
            RegisterCallback<PointerLeaveEvent>(e =>
            {
                if (IsDraging || IsEditing) return;
                style.borderLeftColor = SODocStyle.Current.HintColor;
            });
            Repaint();
        }
        private void createDropfield()
        {
            SelectVisualType = DocRuntime.NewDropdownField("", null, null);
            SelectVisualType.choices = DocEditor.NameList;
            string tName = string.Empty;
            DocEditor.ID2Name.TryGetValue(Target.VisualID, out tName);
            SelectVisualType.index = DocEditor.NameList.FindIndex(0, (str) => { return str == tName; });
            if (SelectVisualType.index == -1) SelectVisualType.index = 0;
            SelectVisualType.RegisterValueChangedCallback((val) =>
            {
                Target.TextData.Clear();
                Target.JsonData = string.Empty;
                Target.ObjsData.Clear();
                EditView.RemoveAt(1);
                if (val.newValue == "None")
                    Target.VisualID = string.Empty;
                else
                    Target.VisualID = DocEditor.Name2ID[val.newValue];
                OnModify?.Invoke(this);
                Repaint();
            });
            SelectVisualType.value = DocEditor.NameList[SelectVisualType.index];
            SelectVisualType.style.marginLeft = 5;
            SelectVisualType.style.height = 20;
            SelectVisualType.style.width = 100;
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
            editFavoriteRoot = DocRuntime.NewEmpty();
            foreach (var name in DocEditor.NameList)
            {
                if (name == "None") continue;
                Toggle toggle = new Toggle();
                toggle.style.marginLeft = 10;
                toggle.text = name;
                toggle.value = false;
                foreach (var id in DocCache.Get().FavoriteDocVisualID)
                {
                    string tName;
                    if (DocEditor.ID2Name.TryGetValue(id, out tName))
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
                        if (DocEditor.Name2ID.TryGetValue(name, out t))
                        {
                            DocCache.Get().FavoriteDocVisualID.Add(t);
                            DocCache.Save();
                        }
                    }
                    else
                    {
                        if (DocEditor.Name2ID.TryGetValue(name, out t))
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

        Button insertBtn()
        {
            Button button = null;
            button = DocRuntime.NewButton("Insert", () =>
            {
                DocComponentField doc = new DocComponentField(new DocComponent());
                parent.Insert(Index, doc);
                doc.SetStatus(true);
                doc.OnModify += this.OnModify;
            });
            button.style.height = 20;
            return button;
        }
        Button copyBtn()
        {
            Button button = null;
            button = DocRuntime.NewButton("", () =>
            {
                copyBuffer = Target.Copy();
                button.style.unityBackgroundImageTintColor = SODocStyle.Current.SuccessTextColor;
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
        Button pasetBtn()
        {
            Button button = null;
            button = DocRuntime.NewButton("", () =>
            {
                if (copyBuffer == null)
                {
                    button.style.unityBackgroundImageTintColor = SODocStyle.Current.DangerTextColor;
                    button.schedule.Execute(() =>
                    {
                        button.style.unityBackgroundImageTintColor = Color.white;
                    }).ExecuteLater(1000);
                }
                else
                {
                    Target = copyBuffer.Copy();
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
            button = DocRuntime.NewButton("☰", () =>
            {
                IsDraging = true;
                SetStatus(false);
                style.borderLeftColor = SODocStyle.Current.WarningColor;
                VisualElement dragMask = DocRuntime.NewEmpty();
                dragMask.style.width = parent.resolvedStyle.width;
                dragMask.style.height = parent.resolvedStyle.height;
                dragMask.style.position = Position.Absolute;
                dragMask.RegisterCallback<PointerMoveEvent>(whileDraging);
                dragMask.RegisterCallback<PointerDownEvent>(e =>
                {
                    parent.Remove(dragMask);
                    style.borderLeftColor = SODocStyle.Current.HintColor;
                    IsDraging = false;
                    SetStatus(true);
                    OnModify?.Invoke(this);
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
            button = DocRuntime.NewButton("", () =>
            {
                DocComponentField doc = new DocComponentField(Target.Copy());
                parent.Insert(Index + 1, doc);
                doc.SetStatus(true);
                doc.OnModify += this.OnModify;
                OnModify?.Invoke(this);
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
            button = DocRuntime.NewButton("", () =>
            {
                OnModify?.Invoke(this);
                parent.Remove(this);
            });
            button.style.backgroundImage = DocEditor.Icon.Delete;
            button.style.height = 20;
            button.style.width = 20;
            button.style.marginLeft = 5;
            button.style.unityBackgroundImageTintColor = SODocStyle.Current.DangerTextColor;
            return button;
        }
        void closeOther()
        {
            foreach (DocComponentField ve in parent.Children())
                if(ve != this)ve.SetStatus(false);
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
