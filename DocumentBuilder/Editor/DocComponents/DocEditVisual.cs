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
        public virtual string ToMarkdown() { return string.Empty; }
    }
}

public class TestEditVisual : DocEditVisual
{
    public override string DisplayName => "TextVisual";

    public override string VisualID => "Test";

    public override void OnCreateGUI()
    {
        var tx = new TextField();
        tx.value = Target.TextData;
        tx.RegisterValueChangedCallback((val) =>
        {
            Target.TextData = val.newValue;
        });
        Add(tx);
    }
}
