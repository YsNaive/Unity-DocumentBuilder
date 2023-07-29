using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocPageVisual : ScrollView
    {
        public SODocPage Target;
        List<DocVisual> visuals = new List<DocVisual>();
        int index;
        Action aniCallback;
        public bool IsPlayingAnimation = false;
        int playingCount = 0;
        public DocPageVisual(SODocPage page) {
            DocRuntime.ApplyStyle(this);
            this.Q("unity-content-container").style.marginRight = Length.Percent(3.5f);
            Target = page;
            Repaint();
        }
        public void Repaint()
        {
            Clear();
            foreach (var com in Target.Components)
            {
                DocVisual docVisual = (DocVisual)DocRuntime.CreateVisual(com);
                if (docVisual.VisualID == "1")
                {
                    docVisual.style.marginLeft = 20;
                    docVisual.style.marginTop = DocStyle.Current.ComponentSpace+10;
                }
                else
                {
                    docVisual.style.marginLeft = 40;
                    docVisual.style.marginTop = DocStyle.Current.ComponentSpace;
                }
                Add(docVisual);
                visuals.Add(docVisual);
            }
            if(childCount!=0)
                this[childCount - 1].style.marginBottom = 400;
        }
        public void PlayIntro(Action callback = null)
        {
            if (IsPlayingAnimation) return;
            IsPlayingAnimation = true;
            Clear();
            index = -1;
            switch (Target.IntroMode)
            {
                case SODocPage.DocPageAniMode.None:
                    {
                        foreach (var ve in visuals)
                            Add(ve);
                        startAniEndCheck();
                        break;
                    }
                case SODocPage.DocPageAniMode.Sametime:
                    {
                        foreach (var ve in visuals)
                        {
                            if (ve.IntroAnimation != null)
                            {
                                playingCount++;
                                ve.IntroAnimation(docPlayingEnd);
                            }
                            Add(ve);
                        }
                        startAniEndCheck();
                    }
                    break;
                case SODocPage.DocPageAniMode.OneByOne:
                    {
                        introOneByOne();
                    }
                    break;
                case SODocPage.DocPageAniMode.Flow:
                    {
                        introFlow();
                    }
                    break;
            }
            aniCallback = callback;
        }
        public void PlayOuttro(Action callback = null)
        {
            if (IsPlayingAnimation) return;
            IsPlayingAnimation = true;
            aniCallback = callback;
            index = -1;
            switch (Target.OuttroMode)
            {
                case SODocPage.DocPageAniMode.None:
                    {
                        startAniEndCheck();
                    }
                    break;
                case SODocPage.DocPageAniMode.Sametime:
                    {
                        {
                            foreach (var ve in visuals)
                            {
                                if (ve.OuttroAnimation != null)
                                {
                                    playingCount++;
                                    ve.OuttroAnimation(docPlayingEnd);
                                }
                            }
                        }
                        startAniEndCheck();
                    }
                    break;
                case SODocPage.DocPageAniMode.OneByOne:
                    {
                        outtroOneByOne();
                    }
                    break;
                case SODocPage.DocPageAniMode.Flow:
                    {
                        outtroFlow();
                    }
                    break;
            }
        }
        private void introOneByOne()
        {
            index++;
            if (index >= visuals.Count)
            {
                startAniEndCheck();
            }
            else
            {
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
        }
        private void outtroOneByOne()
        {
            index++;
            if (index >= visuals.Count)
            {
                startAniEndCheck();
                return;
            }
            else
            {
                if (visuals[index].OuttroAnimation != null)
                {
                    visuals[index].OuttroAnimation(outtroOneByOne);
                }
                else
                {
                    outtroOneByOne();
                }
            }
        }
        private void introFlow()
        {
            index++;
            if (index >= visuals.Count)
            {
                startAniEndCheck();
                return;
            }
            if (visuals[index].IntroAnimation != null)
            {
                playingCount++;
                visuals[index].IntroAnimation(docPlayingEnd);
            }
            var item = visuals[index].schedule.Execute(introFlow);
            item.ExecuteLater(Target.IntroDuration);
            Add(visuals[index]);
        }
        private void outtroFlow()
        {
            index++;
            if (index >= visuals.Count)
            {
                startAniEndCheck();
                return;
            }
            if (visuals[index].OuttroAnimation != null)
            {
                playingCount++;
                visuals[index].OuttroAnimation(docPlayingEnd);
            }
            var item = visuals[index].schedule.Execute(outtroFlow);
            item.ExecuteLater(Target.OuttroDuration);
        }
        void startAniEndCheck()
        {
            schedule.Execute(() => { }).Until(() =>
            {
                if (playingCount == 0)
                {
                    IsPlayingAnimation = false;
                    if(aniCallback != null)
                        schedule.Execute(aniCallback).ExecuteLater(0);
                }
                return !IsPlayingAnimation;
            });
        }
        void docPlayingEnd()
        {
            playingCount--;
        }
    }
}
