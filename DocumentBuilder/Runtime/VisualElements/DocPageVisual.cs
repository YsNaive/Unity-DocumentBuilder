using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocPageVisual : VisualElement
    {
        SODocPage target;
        public DocPageVisual(SODocPage page)
        {
            target = page;
            RegisterCallback<GeometryChangedEvent>(generateChild);
        }

        private void generateChild(GeometryChangedEvent e)
        {
            foreach (var doc in target.Components)
            {
                Add(doc.CreateViewGUI((int)e.newRect.width));
            }
            UnregisterCallback<GeometryChangedEvent>(generateChild);
        }
    }
}
