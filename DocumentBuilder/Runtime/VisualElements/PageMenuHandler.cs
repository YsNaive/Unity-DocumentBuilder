using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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
        public PageMenuVisual Selecting
        {
            get => m_selecting;
            set
            {
                OnChangeSelect?.Invoke(m_selecting, value);
                m_selecting = value;
            }
        }
        private PageMenuVisual m_selecting;
        const int menuContentsChildCount = 2;
        public void Repaint()
        {
            AddedPages.Clear();
            AddedVisual.Clear();
            RootVisual.Clear();
            var parent = RootVisual.parent;
            int i = parent.IndexOf(RootVisual);
            parent.RemoveAt(i);
            RootVisual = new PageMenuVisual(Root, this);
            parent.Insert(i, RootVisual);
            Selecting = AddedVisual.Find(m => { return m.Target == m_selecting.Target; });
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
                foreach (char i in cmd[0]) {
                    int index = i - '0' + 2;
                    if (index >= cur.childCount) continue;
                    cur = (PageMenuVisual)cur[index];
                }
                cur.IsOpen = (cmd[1] == "1") ? true : false; 
            }
        }
        void calStateRec(PageMenuVisual current,string path, StringBuilder buffer)
        {
            buffer.Append(path).Append(':').Append((current.childCount > menuContentsChildCount) ?'1':'0').Append('\n');
            int i = 0;
            foreach (var subPage in current.SubMenuVisual)
                calStateRec((PageMenuVisual)subPage, path + (i++).ToString(), buffer);
        }
    }
}
