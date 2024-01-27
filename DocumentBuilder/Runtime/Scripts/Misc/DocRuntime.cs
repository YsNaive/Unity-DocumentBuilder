using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public static class DocRuntime
    {
        public static Dictionary<string, Type> VisualID_Dict => DocVisual.VisualID_Dict;
        public static DocVisual CreateDocVisual(DocComponent docComponent)
        {
            return DocVisual.Create(docComponent);
        }

    }

}