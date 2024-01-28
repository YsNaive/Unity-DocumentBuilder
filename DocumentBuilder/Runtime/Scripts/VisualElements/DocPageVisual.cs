using System.Collections.Generic;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocPageVisual : DSScrollView
    {
        public SODocPage Target;
        List<DocVisual> visuals = new List<DocVisual>();
        public bool IsPlayingAnimation = false;
        public DocPageVisual(SODocPage page) : base() {
            verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
            this.Q("unity-content-container").style.marginRight = Length.Percent(3.5f);
            Target = page;
            Repaint();
        }
        public void Repaint()
        {
            var boundBefore = contentContainer.localBound;
            Clear();
            if (Target == null) return;
            foreach (var com in Target.Components)
            {
                DocVisual docVisual = DocRuntime.CreateDocVisual(com);
                if (docVisual.VisualID != "1")
                    docVisual.style.marginLeft = DocStyle.Current.MainTextSize * 3f;
                else
                    docVisual.style.marginLeft = DocStyle.Current.MainTextSize;
                docVisual.style.marginTop = DocStyle.Current.MainTextSize;
                Add(docVisual);
                visuals.Add(docVisual);
            }
            if(!Target.IsComponentsEmpty)
                visuals[^1].style.marginBottom = DocStyle.Current.MainTextSize*7f;
            contentContainer.style.minHeight = boundBefore.height;
        } 
    }
}
