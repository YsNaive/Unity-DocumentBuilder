using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace NaiveAPI.DocumentBuilder
{
    public class DocPageVisual : ScrollView
    {
        SODocPage target;
        List<DocVisual> visuals = new List<DocVisual>();
        int index;
        Action introCallback, outtroCallback;
        public DocPageVisual(SODocPage page) {
            target = page;
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
                visuals.Add(docVisual);
            }
        }
        public void PlayIntro(Action callback = null)
        {
            Clear();
            introCallback = callback;
            index = -1;
            switch (target.IntroMode)
            {
                case SODocPage.DocPageAniMode.None:
                    foreach (var ve in visuals)
                        Add(ve);
                    callback();
                    break;
                case SODocPage.DocPageAniMode.Sametime:
                    Action<Action> last = null;
                    foreach (var ve in visuals)
                    {
                        if (ve.IntroAnimation != null)
                        {
                            if (last != null)
                                last(null);
                            last = ve.IntroAnimation;
                        }
                        Add(ve);
                    }
                    if (last != null) last(callback);
                    else callback?.Invoke();
                    break;
                case SODocPage.DocPageAniMode.OneByOne:
                    introOneByOne();
                    break;
                case SODocPage.DocPageAniMode.Flow:
                    introFlow();
                    break;
            }
        }
        public void PlayOuttro(Action callback = null)
        {
            outtroCallback = callback;
            index = -1;
            switch (target.OuttroMode)
            {
                case SODocPage.DocPageAniMode.None:
                    callback();
                    break;
                case SODocPage.DocPageAniMode.Sametime:
                    Action<Action> last = null;
                    foreach (var ve in visuals)
                    {
                        if(ve.OuttroAnimation != null)
                        {
                            if (last != null)
                                last(null);
                            last = ve.OuttroAnimation;
                        }
                    }
                    if (last != null) last(callback);
                    else callback?.Invoke();
                    break;
                case SODocPage.DocPageAniMode.OneByOne:
                    outtroOneByOne();
                    break;
                case SODocPage.DocPageAniMode.Flow:
                    outtroFlow();
                    break;
            }
        }

        private void introOneByOne()
        {
            index++;
            if (index >= visuals.Count)
            {
                introCallback?.Invoke();
                return;
            }
            Add(visuals[index]);
            if (visuals[index].IntroAnimation != null)
            {
                visuals[index].IntroAnimation(introOneByOne);
            }
            else
            {
                introOneByOne();
            }
        }
        private void outtroOneByOne()
        {
            index++;
            if (index >= visuals.Count)
            {
                outtroCallback?.Invoke();
                return;
            }
            if (visuals[index].OuttroAnimation != null)
            {
                visuals[index].OuttroAnimation(outtroOneByOne);
            }
            else
            {
                outtroOneByOne();
            }
        }
        Action<Action> inLast = null;
        private void introFlow()
        {
            index++;
            if (index == visuals.Count)
            {
                if (inLast != null)
                    inLast(introCallback);
                else
                    introCallback?.Invoke();
                return;
            }
            int t = 0;
            if (inLast != null)
            {
                t = target.IntroDuration;
                inLast(null);
            }
            Add(visuals[index]);
            inLast = visuals[index].IntroAnimation;
            visuals[index].schedule.Execute(() => {
                introFlow();
            }).ExecuteLater(t);
        }
        Action<Action> outLast = null;
        private void outtroFlow()
        {
            index++;
            if (index == visuals.Count)
            {
                if (outLast != null)
                    outLast(outtroCallback);
                else
                outtroCallback?.Invoke();
                return;
            }
            int t = 0;
            if (outLast != null)
            {
                t = target.OuttroDuration;
                outLast(null);
            }
            outLast = visuals[index].OuttroAnimation;
            visuals[index].schedule.Execute(outtroFlow).ExecuteLater(t);
        }
    }
}
