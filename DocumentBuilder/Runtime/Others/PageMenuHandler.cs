using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    public class PageMenuHandler
    {
        public SODocPage Root;
        public DocPageMenuVisual RootVisual;
        const int menuContentsChildCount = 2;
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
                DocPageMenuVisual cur = RootVisual;
                foreach (char i in cmd[0]) {
                    int index = i - '0' + 2;
                    if (index >= cur.childCount) continue;
                    cur = (DocPageMenuVisual)cur[index];
                }
                cur.IsOpen = (cmd[1] == "1") ? true : false; 
            }
        }
        void calStateRec(DocPageMenuVisual current,string path, StringBuilder buffer)
        {
            buffer.Append(path).Append(':').Append((current.childCount > menuContentsChildCount) ?'1':'0').Append('\n');
            int i = 0;
            foreach (var subPage in current.SubMenuVisual)
                calStateRec((DocPageMenuVisual)subPage, path + (i++).ToString(), buffer);
        }
    }
}
