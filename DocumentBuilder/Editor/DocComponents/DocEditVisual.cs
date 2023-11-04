using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var bar = new DSHorizontal();
            var introType = new DSEnumField("in", initType, (e) =>
            {
                Target.IntroType = Convert.ToInt32(e.newValue);
            });introType.value = (Enum)Enum.ToObject(initType.GetType(), (byte)Target.IntroType);
            introType.style.width = typeWidth;
            introType.style.ClearMarginPadding();
            introType.labelElement.style.minWidth = 45;
            introType.labelElement.style.width = 45;
            introType.labelElement.style.unityTextAlign = TextAnchor.MiddleCenter;
            var introTime = DocEditor.NewIntField("", e =>
            {
                Target.IntroTime = e.newValue;
            }); introTime.value = Target.IntroTime;
            introTime.style.width = timeWidth;
            introTime.style.ClearMarginPadding();
            introTime.style.paddingLeft = 4;
            var outtroType = new DSEnumField("out", initType, (e) =>
            {
                Target.OuttroType = Convert.ToInt32(e.newValue);
            }); outtroType.value = (Enum)Enum.ToObject(initType.GetType(), (byte)Target.OuttroType);
            outtroType.style.width = typeWidth;
            outtroType.style.ClearMarginPadding();
            outtroType.labelElement.style.minWidth = 45;
            outtroType.labelElement.style.width = 45;
            outtroType.labelElement.style.unityTextAlign = TextAnchor.MiddleCenter;
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

        #region static

        public static class Dict
        {
            public static List<string> NameList = new();
            public static Dictionary<string, string> Name2ID = new();
            public static Dictionary<string, string> Name2Path = new();
            public static Dictionary<string, string> ID2Name = new();
            public static Dictionary<string, Type> ID2Type = new();
            static Dict()
            {
                ID2Type.Clear();
                Name2ID.Clear();
                Name2Path.Clear();
                ID2Name.Clear();
                NameList.Clear();
                NameList.Add("None");
                Dictionary<string, List<(CustomDocEditVisualAttribute attr, Type docType)>> dir2Types = new() { { "", new() } };
                foreach (var type in DocRuntime.FindAllTypesWhere((t) =>
                {
                    if (t.IsAbstract)
                        return false;
                    if (!t.IsSubclassOf(typeof(DocEditVisual)))
                        return false;
                    if (!t.IsDefined(typeof(CustomDocEditVisualAttribute)))
                        return false;
                    return true;
                }))
                {
                    var attr = type.GetCustomAttribute<CustomDocEditVisualAttribute>();
                    var path = attr.MenuPath;
                    int index = Math.Max(path.LastIndexOf('\\'), path.LastIndexOf('/'));
                    var dir = (index == -1) ? "" : path.Substring(0, index);
                    var name = path.Substring(index + 1);
                    if (!dir2Types.ContainsKey(dir))
                        dir2Types.Add(dir, new());
                    dir2Types[dir].Add((attr, type));
                }
                foreach (var list in dir2Types.Values)
                {
                    foreach (var pair in list.OrderBy((obj) => { return obj.attr.Priority; }))
                    {
                        DocEditVisual doc = (DocEditVisual)Activator.CreateInstance(pair.docType);
                        NameList.Add(pair.attr.MenuPath);
                        Name2ID.Add(pair.attr.MenuPath, doc.VisualID);
                        ID2Type.Add(doc.VisualID, pair.docType);
                        ID2Name.Add(doc.VisualID, pair.attr.MenuPath);
                    }
                }
            }
        }

        #endregion
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