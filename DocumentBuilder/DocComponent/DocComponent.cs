using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DocumentBuilder
{
    public abstract class DocComponent
    {
        public const bool AllowRuntime = true;
        public List<string> StrData = new List<string>();
        public List<Object> ObjData = new List<Object>();
        public List<ISStyle> StyleData = new List<ISStyle>();
        public abstract VisualElement CreateRuntimeGUI();
        public abstract VisualElement CreateEditorGUI();
    }
}
