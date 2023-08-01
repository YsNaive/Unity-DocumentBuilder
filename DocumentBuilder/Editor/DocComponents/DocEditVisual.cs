using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_Editor.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static DocVisual;

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
            OnCreateAniGUI(InitAniType);
            OnCreateGUI();
        }
        /// <summary>
        /// Call after Target is set
        /// </summary>
        protected abstract void OnCreateGUI();
        protected virtual Enum InitAniType => AniMode.Fade;
        protected virtual void OnCreateAniGUI(Enum initType)
        {
            var bar = DocRuntime.NewEmptyHorizontal();
            var introType = DocEditor.NewEnumField("in", initType, (e) =>
            {
                Target.IntroType = Convert.ToInt32(e.newValue);
            });introType.value = (Enum)Enum.ToObject(initType.GetType(), (byte)Target.IntroType);
            introType.style.width = Length.Percent(30);
            introType[0].style.minWidth = 30;
            var introTime = DocEditor.NewIntField("", e =>
            {
                Target.IntroTime = e.newValue;
            }); introTime.value = Target.IntroTime;
            introTime.style.width = Length.Percent(20);
            var outtroType = DocEditor.NewEnumField("out", initType, (e) =>
            {
                Target.OuttroType = Convert.ToInt32(e.newValue);
            }); outtroType.value = (Enum)Enum.ToObject(initType.GetType(), (byte)Target.OuttroType);
            outtroType.style.width = Length.Percent(30);
            outtroType[0].style.minWidth = 30;
            var outtroTime = DocEditor.NewIntField("", e =>
            {
                Target.OuttroTime = e.newValue;
            }); outtroTime.value = Target.OuttroTime;
            outtroTime.style.width = Length.Percent(20);
            bar.Add(introType);
            bar.Add(introTime);
            bar.Add(outtroType);
            bar.Add(outtroTime);
            Add(bar);
        }
        public virtual string ToMarkdown(string dstPath) { return string.Empty; }
    }
}