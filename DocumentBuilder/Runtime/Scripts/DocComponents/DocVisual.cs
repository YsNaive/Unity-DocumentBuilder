using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public abstract class DocVisual : VisualElement
    {
        public enum AniMode
        {
            None = 0,
            Fade = 1,
        }
        public DocVisual()
        {
            RegisterCallback<GeometryChangedEvent>(e =>
            {
                if (e.oldRect.width != e.newRect.width)
                    OnWidthChanged?.Invoke(e.newRect.width);
                if (e.oldRect.height != e.newRect.height)
                    OnHeightChanged?.Invoke(e.newRect.height);
            });
        }
        public abstract string VisualID { get; }
        public Action<float> OnHeightChanged;
        public Action<float> OnWidthChanged;
        public Action<Action> IntroAnimation;
        public Action<Action> OuttroAnimation;
        public DocComponent Target => m_target;

        protected DocComponent m_target;
        public virtual void SetTarget(DocComponent target)
        {
            m_target = target;
            Clear();
            OnCreateGUI();
            OnSelectIntroAni(Target.IntroType);
            OnSelectOuttroAni(Target.OuttroType);
        }
        protected abstract void OnCreateGUI();
        protected virtual void OnSelectIntroAni(int type)
        {
            if ((AniMode)type == AniMode.None)
                return;
            else if ((AniMode)type == AniMode.Fade)
                IntroAnimation = (callback) => { this.Fade(0, 1, Target.IntroTime, 20, callback); };
        }
        protected virtual void OnSelectOuttroAni(int type)
        {
            if ((AniMode)type == AniMode.None)
                return;
            else if ((AniMode)type == AniMode.Fade)
                OuttroAnimation = (callback) => { this.Fade(1, 0, Target.OuttroTime, 20, callback); };
        }

        public static Dictionary<string, Type> VisualID_Dict = new Dictionary<string, Type>();

        #region static

        static DocVisual()
        {
            VisualID_Dict.Clear();
            foreach (var type in DocRuntime.FindAllTypesWhere(t => { return (t.IsSubclassOf(typeof(DocVisual)) && !t.IsAbstract); }))
                VisualID_Dict.Add(((DocVisual)Activator.CreateInstance(type)).VisualID, type);
        }
        public static DocVisual Create(DocComponent docComponent)
        {
            Type t;
            if (!VisualID_Dict.TryGetValue(docComponent.VisualID, out t))
            {
                DocDescription textElement = new DocDescription();
                textElement.SetTarget(new DocComponent { VisualID = "2", TextData = new List<string>() { $"Not Fount View for ID \"{docComponent.VisualID}\"" } });
                return textElement;
            }
            DocVisual doc = (DocVisual)Activator.CreateInstance(t);
            doc.SetTarget(docComponent);
            return doc;
        }

        #endregion
    }
    public abstract class DocVisual<DType> : DocVisual
        where DType : new()
    {
        protected DType visualData;
        public override void SetTarget(DocComponent target)
        {
            m_target = target;
            LoadJsonData();
            Clear();
            OnCreateGUI();
            OnSelectIntroAni(Target.IntroType);
            OnSelectOuttroAni(Target.OuttroType);
        }
        /// <summary>
        /// Load data from Target
        /// </summary>
        protected virtual void LoadJsonData()
        {
            visualData = LoadJsonData(Target);
        }
        /// <summary>
        /// Save data to Target
        /// </summary>
        protected virtual void SaveJsonData()
        {
            SaveJsonData(Target, visualData);
        }
        public static DType LoadJsonData(DocComponent component)
        {
            var data = JsonUtility.FromJson<DType>(component.JsonData);
            data ??= new DType();
            return data;
        }
        public static void SaveJsonData(DocComponent component, DType data)
        {
            component.JsonData = JsonUtility.ToJson(data);
        }
    }
}