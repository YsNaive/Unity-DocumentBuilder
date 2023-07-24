using NaiveAPI.DocumentBuilder;
using NaiveAPI_Editor.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public abstract class DocEditVisual : VisualElement
    {
        public abstract string DisplayName { get; }
        public abstract string VisualID { get; }
        public float Width = -1.0f;
        public DocComponent Target
        {
            get => m_target;
            set
            {
                m_target = value;
                Repaint();
            }
        }

        private DocComponent m_target;
        /// <summary>
        /// Call after Target is set
        /// </summary>
        public abstract void Repaint();
        public virtual string ToMarkdown() { return string.Empty; }
    }
}

public class TestEditVisual : DocEditVisual
{
    public TestEditVisual()
    {
        Add(new TextField());
    }
    public override string DisplayName => "TextVisual";

    public override string VisualID => "Test";

    public override void Repaint()
    {
        ((TextField)this[0]).value = "Test";
    }
}
