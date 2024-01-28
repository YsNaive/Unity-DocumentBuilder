using System;
using System.Collections.Generic;

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