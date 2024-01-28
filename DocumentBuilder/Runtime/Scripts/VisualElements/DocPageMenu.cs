using System;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocPageMenu : VisualElement
    {
        public event Action<DocPageMenuItem> OnSelected;
        public SODocPage RootPage => m_RootPage;
        SODocPage m_RootPage;
        public DocPageMenuItem RootMenuItem => m_RootMenuItem;
        DocPageMenuItem m_RootMenuItem;
        public DocPageMenuItem DisplayingRootMenuItem => m_DisplayingRootMenuItem;
        DocPageMenuItem m_DisplayingRootMenuItem;
        public DocPageMenuItem Selecting
        {
            get => m_Selecting;
            set
            {
                if (value == null) return;
                if (RootMenuItem == null) return;
                if(trySelect(value.TargetPage))
                    OnSelected?.Invoke(Selecting);

            }
        }
        DocPageMenuItem m_Selecting;
        public DocPageMenuItem Hovering => m_Hovering;
        DocPageMenuItem m_Hovering;

        public ScrollView MenuItemContainer => m_MenuItemContainer;
        ScrollView m_MenuItemContainer;

        public bool EnableEmptySelecting { get => m_EnableEmptySelecting; set => m_EnableEmptySelecting = value; }
        public bool EnableDisplayingRootChange { get => m_EnableDisplayingRootChange; set => m_EnableDisplayingRootChange = value; }
        public bool EnableAutoHierarchySave { get => m_EnableAutoHierarchySave; set => m_EnableAutoHierarchySave = value; }
        public bool LockSelect { get => m_LockSelect; set => m_LockSelect = value; }
        public bool EnableSerachField
        {
            get => m_EnableSerachField;
            set
            {
                m_EnableSerachField = value;
            }
        }

        bool m_EnableEmptySelecting = false;
        bool m_EnableDisplayingRootChange = true;
        bool m_EnableSerachField = true;
        bool m_EnableAutoHierarchySave = false;
        bool m_LockSelect = false;

        public string ID => menuId;
        string menuId;
        public DocPageMenu(SODocPage page) : this(page, page.name) { }
        public DocPageMenu(SODocPage page, string id)
        {
            menuId = id;
            style.backgroundColor = DocStyle.Current.BackgroundColor;
            m_RootPage = page;

            m_MenuItemContainer = new DSScrollView();
            m_MenuItemContainer.mode = ScrollViewMode.VerticalAndHorizontal;
            m_MenuItemContainer.style.flexGrow = 1;

            RepaintMenuHierarchy();
            
            Add(m_MenuItemContainer);
            LoadStateHierarchy();
        }

        (DocPageMenuItem parent, int index) displayingRootOriginParentInfo = new (null, -1);
        public void SetDisplayingRoot(DocPageMenuItem item)
        {
            if (!m_EnableDisplayingRootChange) return;
            if (item == DisplayingRootMenuItem) return;

            if (displayingRootOriginParentInfo.parent != null)
                displayingRootOriginParentInfo.parent.SubItemContainer.Insert(displayingRootOriginParentInfo.index, DisplayingRootMenuItem);
            m_MenuItemContainer.Clear();
            foreach (var menuItem in item.ParentMenuItems())
            {
                var ptr = menuItem;
                var backBtn = new DocPageMenuItem(ptr.TargetPage, DocPageMenuItem.InitMode.Single);
                backBtn.style.opacity = 0.65f;
                var scale = .85f;
                backBtn.style.scale = new Scale(new Vector3(scale, scale, scale));
                backBtn.style.left = Length.Percent((scale - 1f) * 50f);
                backBtn.RegisterCallback<PointerEnterEvent>(e => { backBtn.style.opacity = 1f; });
                backBtn.RegisterCallback<PointerLeaveEvent>(e => { backBtn.style.opacity = 0.65f; });
                backBtn.RegisterCallback<PointerDownEvent>(e =>
                {
                    SetDisplayingRoot(ptr);
                });
                m_MenuItemContainer.Insert(0, backBtn);
            }
            if (m_MenuItemContainer.childCount != 0)
            {
                float indent = 0;
                float indentStep = DocStyle.Current.MainTextSize * 0.5f;
                foreach (var ve in m_MenuItemContainer.Children())
                {
                    ve.style.marginLeft = indent;
                    indent += indentStep;
                }
                m_MenuItemContainer[m_MenuItemContainer.childCount - 1].style.marginBottom = indentStep;
            }
            
            displayingRootOriginParentInfo.parent = item.ParentMenuItem;
            if(item.ParentMenuItem != null)
                displayingRootOriginParentInfo.index = item.parent.IndexOf(item);

            m_MenuItemContainer.Add(item);
            m_DisplayingRootMenuItem = item;
        }

        void selectMenuItem(DocPageMenuItem item)
        {
            if (m_LockSelect) return;
            var current = Selecting;
            while (current != null)
            {
                current.CurrentStyle = DocPageMenuItem.StyleType.None;
                current = current.ParentMenuItem;
            }
            m_Selecting = item;
            if (Selecting != null)
            {
                Selecting.CurrentStyle = DocPageMenuItem.StyleType.Selected;
                current = Selecting.ParentMenuItem;
                while (current != null)
                {
                    current.CurrentStyle = DocPageMenuItem.StyleType.ChildSelected;
                    current = current.ParentMenuItem;
                }
            }
        }
        public void RegisterMenuItemBeheavior(DocPageMenuItem item)
        {
            float lastClickTime = 0;
            item.OnStateChanged += () =>
            {
                if (EnableAutoHierarchySave)
                    SaveStateHierarchy();
            };
            item.TitleText.RegisterCallback<PointerDownEvent>(e =>
            {
                if(((Time.realtimeSinceStartup - lastClickTime) < 0.25) && !item.TargetPage.IsSubPageEmpty)
                    SetDisplayingRoot(item);
                lastClickTime = Time.realtimeSinceStartup;
                // execute switch open if not allow empty select && target is empty
                if (!m_EnableEmptySelecting && item.TargetPage.IsComponentsEmpty)
                {
                    item.InvertOpenState();
                    return;
                }

                // execute selecting
                selectMenuItem(item);
                OnSelected?.Invoke(Selecting);
            });

            item.TitleText.RegisterCallback<PointerEnterEvent>(e =>
            {
                m_Hovering = item;
                if (item.CurrentStyle == DocPageMenuItem.StyleType.None)
                    item.CurrentStyle = DocPageMenuItem.StyleType.Hover;
            });

            item.TitleText.RegisterCallback<PointerLeaveEvent>(e =>
            {
                if (item.CurrentStyle == DocPageMenuItem.StyleType.Hover)
                    item.CurrentStyle = DocPageMenuItem.StyleType.None;
            });
        }

        public void RepaintMenuHierarchy()
        {
            var lastSelect = Selecting;
            m_MenuItemContainer.Clear();
            m_RootMenuItem = new DocPageMenuItem(RootPage);
            m_DisplayingRootMenuItem = m_RootMenuItem;
            m_MenuItemContainer.Add(m_RootMenuItem);
            VisualElement space = new VisualElement();
            space.style.height = DocStyle.Current.LineHeight * 7f;
            m_MenuItemContainer.Add(space);
            foreach (var item in m_RootMenuItem.MenuItems())
                RegisterMenuItemBeheavior(item);

            if(lastSelect != null)
                trySelect(lastSelect.TargetPage);
            LoadStateHierarchy();
        }
        public string GetStateHierarchy()
        {
            StringBuilder sb = new();
            foreach(var pair in RootMenuItem.MenuItemsAndPath())
            {
                if(pair.item.IsOpen)
                    sb.Append(pair.path);
            }
            return sb.ToString();
        }
        public void LoadStateHierarchy()
        {
            foreach(var item in RootMenuItem.MenuItems())
                item.IsOpen = false;

            string stateHierarchy = DocCache.LoadData($"DocPageMenuHierarchy_{ID}");
            foreach (var path in stateHierarchy.Split('R'))
            {
                DocPageMenuItem current = RootMenuItem;
                foreach(var charIndex in path)
                {
                    int index = charIndex - '0';
                    if (current.SubItemContainer.childCount > index)
                        current = (DocPageMenuItem)current.SubItemContainer[index];
                    else
                        continue;
                }
                current.IsOpen = true;
            }
        }
        public void SaveStateHierarchy()
        {
            DocCache.SaveData($"DocPageMenuHierarchy_{ID}", GetStateHierarchy());
        }

        public bool TrySelect(SODocPage page)
        {
            if (trySelect(page))
            {
                OnSelected?.Invoke(Selecting);
                return true;
            }
            return false;
        }
        bool trySelect(SODocPage page)
        {
            foreach (var item in RootMenuItem.MenuItems())
            {
                if (item.TargetPage == page)
                {
                    selectMenuItem(item);
                    return true;
                }
            }
            return false;
        }
    }
}