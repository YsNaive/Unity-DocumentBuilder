using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
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
        public virtual ushort Version => 0;
        [Obsolete("\nThis is no longer needed after version 2.0.2, define it on Attribute instead")]
        public virtual string DisplayName { get => "N/A"; }
        public abstract string VisualID { get; }
        public Action<float> OnHeightChanged;
        public Action<float> OnWidthChanged;
        public DocComponent Target => m_target;
        protected DocComponent m_target;
        public virtual void SetTarget(DocComponent target)
        {
            m_target = target;
            if (target.VisualVersion != Version)
                VersionConflict();
            OnCreateAniGUI(InitAniType);
            OnCreateGUI();
        }
        /// <summary>
        /// Call after Target is set
        /// </summary>
        protected abstract void OnCreateGUI();
        protected virtual Enum InitAniType => DocVisual.AniMode.Fade;
        protected virtual void OnCreateAniGUI(Enum initType)
        {
            Length typeWidth = Length.Percent(35);
            Length timeWidth = Length.Percent(15);
            var bar = DocRuntime.NewEmptyHorizontal();
            var introType = DocEditor.NewEnumField("in", initType, (e) =>
            {
                Target.IntroType = Convert.ToInt32(e.newValue);
            });introType.value = (Enum)Enum.ToObject(initType.GetType(), (byte)Target.IntroType);
            introType.style.width = typeWidth;
            introType.style.ClearMarginPadding();
            introType[0].style.minWidth = 45;
            introType[1].style.paddingLeft = 4;
            introType[0].style.unityTextAlign = TextAnchor.MiddleCenter;
            var introTime = DocEditor.NewIntField("", e =>
            {
                Target.IntroTime = e.newValue;
            }); introTime.value = Target.IntroTime;
            introTime.style.width = timeWidth;
            introTime.style.ClearMarginPadding();
            introTime.style.paddingLeft = 4;
            var outtroType = DocEditor.NewEnumField("out", initType, (e) =>
            {
                Target.OuttroType = Convert.ToInt32(e.newValue);
            }); outtroType.value = (Enum)Enum.ToObject(initType.GetType(), (byte)Target.OuttroType);
            outtroType.style.width = typeWidth;
            outtroType.style.ClearMarginPadding();
            outtroType[0].style.minWidth = 45;
            outtroType[1].style.paddingLeft = 4;
            outtroType[0].style.unityTextAlign = TextAnchor.MiddleCenter;
            var outtroTime = DocEditor.NewIntField("", e =>
            {
                Target.OuttroTime = e.newValue;
            }); outtroTime.value = Target.OuttroTime;
            outtroTime.style.width = timeWidth;
            outtroTime.style.ClearMarginPadding();
            outtroTime.style.paddingLeft = 4;
            bar.Add(introType);
            bar.Add(introTime);
            bar.Add(outtroType);
            bar.Add(outtroTime);
            Add(bar);
        }
        protected virtual void VersionConflict()
        {
            Debug.LogWarning($"DocEditVisual: VersionConflict NOT Implement in type [{GetType()}]");
        }
        public virtual string ToMarkdown(string dstPath) { return string.Empty; }
    }
    public abstract class DocEditVisual<DType> : DocEditVisual
    where DType : new()
    {
        protected DType visualData;
        public override void SetTarget(DocComponent target)
        {
            m_target = target;
            LoadDataFromTarget();
            base.SetTarget(target);
        }
        protected void LoadDataFromTarget()
        {
            if(!string.IsNullOrEmpty(Target.JsonData))
                visualData = JsonUtility.FromJson<DType>(Target.JsonData);
            visualData ??= new DType();
        }
        protected void SaveDataToTarget()
        {
            Target.JsonData = JsonUtility.ToJson(visualData);
            Target.VisualVersion = Version;
        }
    }
}