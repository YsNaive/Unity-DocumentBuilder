using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocPageVisual : ScrollView
    {
        public DocPageVisual(SODocPage page) {
            foreach(var com in page.Components)
            {
                DocVisual docVisual = (DocVisual)DocRuntime.CreateVisual(com);
                if(docVisual.VisualID == "1")
                {
                    docVisual.style.marginLeft = 20;
                    docVisual.style.marginTop = 10;
                }
                else
                {
                    docVisual.style.marginLeft = 40;
                    docVisual.style.marginTop = 5;
                }
                Add(docVisual);
            }
        }
    }
}
