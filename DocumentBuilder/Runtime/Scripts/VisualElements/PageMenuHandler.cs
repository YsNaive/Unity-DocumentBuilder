using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class PageMenuHandler
    {
        /// <summary>
        /// <old, new>
        /// </summary>
        public event Action<PageMenuVisual, PageMenuVisual> OnChangeSelect;
        public SODocPage Root;
        public List<SODocPage> AddedPages = new List<SODocPage>(); 
        public List<PageMenuVisual> AddedVisual = new List<PageMenuVisual>(); 
        public PageMenuVisual RootVisual;
        public bool LockSelect = false;
        public PageMenuVisual Selecting
        {
            get => m_selecting;
            set
            {
                if (LockSelect) return;
                if(value!=null)
                {
                    OnChangeSelect?.Invoke(m_selecting, value);
                };
                m_selecting = value;
            }
        }
        private PageMenuVisual m_selecting;
        const int menuContentsChildCount = 1;
        public void Repaint()
        {
            DocCache.Get().OpeningBookHierarchy = GetState();
            AddedPages.Clear();
            AddedVisual.Clear();
            if (RootVisual == null) return;
            RootVisual.Clear();
            var parent = RootVisual.parent;
            int i = parent.IndexOf(RootVisual);
            parent.RemoveAt(i);
            if(Root != null)
            {
                RootVisual = new PageMenuVisual(Root, this);
                RootVisual.style.marginTop = 10;
                parent.Insert(i, RootVisual);
            }
            if(m_selecting != null)
                Selecting = AddedVisual.Find(m => { return m.Target == m_selecting.Target; });
            SetState(DocCache.Get().OpeningBookHierarchy);
        }
        public string GetState()
        {
            StringBuilder sb = new StringBuilder();
            calStateRec(RootVisual, "", sb);
            return sb.ToString();
        }
        public void SetState(string state)
        {
            state = state.Replace("\r\n", "\n").Replace("\r", "\n");
            foreach (var path in state.Split('\n'))
            {
                if(string.IsNullOrEmpty(path)) continue;
                var cmd = path.Split(':');
                PageMenuVisual cur = RootVisual;
                bool skip = false;
                foreach (char i in cmd[0]) {
                    int index = i - '0' + menuContentsChildCount;
                    if (index >= cur.childCount)
                    {
                        skip = true;
                        continue;
                    }
                    cur = (PageMenuVisual)cur[index];
                }
                if(!skip)
                    cur.IsOpen = (cmd[1] == "1");
            }
        }
        void calStateRec(PageMenuVisual current,string path, StringBuilder buffer)
        {
            if (current == null) return;
            buffer.Append(path).Append(':').Append(current.IsOpen ?'1':'0').Append('\n');
            int i = 0;
            foreach (PageMenuVisual subPage in current.SubMenuVisual)
                    calStateRec(subPage, path + (i++).ToString(), buffer);
        }
    }
}
