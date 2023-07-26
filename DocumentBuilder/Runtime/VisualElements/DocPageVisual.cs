using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocPageVisual : ScrollView
    {
        public DocPageVisual(SODocPage page) {
            List<DocVisual> visuals = new List<DocVisual>();
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
                visuals.Add(docVisual);
            }
            int i = -1;
            Action exe = null;
            switch (page.AnimationMode)
            {
                case SODocPage.DocPageAniMode.None:
                    foreach (var ve in visuals)
                        Add(ve);
                    break;
                case SODocPage.DocPageAniMode.Sametime:
                    foreach (var ve in visuals)
                    {
                        Add(ve);
                        ve.IntroAnimation?.Invoke(null);
                    }
                    break;
                case SODocPage.DocPageAniMode.OneByOne:
                    exe = () =>
                    {
                        i++;
                        if (i >= visuals.Count) return;
                        Add(visuals[i]);
                        if (visuals[i].IntroAnimation != null)
                        {
                            visuals[i].IntroAnimation(exe);
                        }
                        else
                        {
                            exe();
                        }
                    };
                    exe();
                    break;
                case SODocPage.DocPageAniMode.Flow:
                    exe = () =>
                    {
                        i++;
                        if (i >= visuals.Count) return;
                        Add(visuals[i]);
                        visuals[i].IntroAnimation?.Invoke(null);
                        visuals[i].schedule.Execute(exe).ExecuteLater(page.AnimationDuration);
                    };
                    exe();
                    break;
            }
        }
    }
}
