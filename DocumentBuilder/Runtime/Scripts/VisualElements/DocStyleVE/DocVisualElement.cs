using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocVisualElement : VisualElement
    {
        public virtual DocStyleFlag StyleFlag => DocStyleFlag.None;
    }
}
