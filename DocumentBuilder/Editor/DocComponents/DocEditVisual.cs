using NaiveAPI.DocumentBuilder;
using NaiveAPI_Editor.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public abstract class DocEditVisual : VisualElement
    {
        public DocEditVisual()
        {
            RegisterCallback<GeometryChangedEvent>(e =>
            {
                if (e.oldRect.width != e.newRect.width)
                    OnWidthChanged?.Invoke(e.newRect.width);
                if (e.oldRect.height != e.newRect.height)
                    OnHeightChanged?.Invoke(e.newRect.height);
            });
        }
        public abstract string DisplayName { get; }
        public abstract string VisualID { get; }
        public Action<float> OnHeightChanged;
        public Action<float> OnWidthChanged;
        public DocComponent Target => m_target;

        private DocComponent m_target;
        public void SetTarget(DocComponent target)
        {
            m_target = target;
            OnCreateGUI();
        }
        /// <summary>
        /// Call after Target is set
        /// </summary>
        public abstract void OnCreateGUI();
        public virtual string ToMarkdown(string dstPath) { return string.Empty; }
    }
}