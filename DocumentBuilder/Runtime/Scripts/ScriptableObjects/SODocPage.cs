using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [CreateAssetMenu(menuName ="Naive API/Doc Page", order = 2)]
    public class SODocPage : ScriptableObject, ISerializationCallbackReceiver
    {
        public Texture2D Icon;
        public List<SODocPage> SubPages = new List<SODocPage>();
        public List<DocComponent> Components = new List<DocComponent>();

        public bool IsComponentsEmpty => Components.Count == 0;
        public bool IsSubPageEmpty => SubPages.Count == 0;

        public IEnumerable<SODocPage> Pages()
        {
            Queue<SODocPage> queue = new();
            queue.Enqueue(this);
            SODocPage current;
            while (queue.TryDequeue(out current))
            {
                if (current == null) continue;
                yield return current;
                foreach(var page in current.SubPages)
                    queue.Enqueue(page);
            }
        }

        public void OnBeforeSerialize()
        {
            for(int i=0; i < SubPages.Count; i++)
            {
                if (SubPages[i] == null)
                {
                    SubPages.RemoveAt(i);
                    i--;
                }
            }
        }

        public void OnAfterDeserialize()
        {
            for (int i = 0; i < SubPages.Count; i++)
            {
                if (SubPages[i] == null)
                {
                    SubPages.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
