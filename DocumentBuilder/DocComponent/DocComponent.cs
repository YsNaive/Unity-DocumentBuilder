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
        public List<GameObject> ObjData = new List<GameObject>();
        public abstract VisualElement CreateRuntimeGUI();
        public abstract VisualElement CreateEditorGUI();
    }
}
