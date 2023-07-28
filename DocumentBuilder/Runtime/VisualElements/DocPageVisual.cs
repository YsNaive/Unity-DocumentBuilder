using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocPageVisual : ScrollView
    {
        SODocPage target;
        List<DocVisual> visuals = new List<DocVisual>();
        int index;
        Action aniCallback;
        public bool IsPlayingAnimation = false;
        int playingCount = 0;
        public DocPageVisual(SODocPage page) {
            style.paddingRight = Length.Percent(3);
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
        class itemCounter
        {
            public itemCounter(Action onClear)
            {
                callback = onClear;
            }
            public void Add()
            {
                i++;
                Debug.Log("ADD");
            }
            public void End()
            {
                i--;
            }
            public bool Check()
            {
                if(i == 0)
                {
                    callback?.Invoke();
                    return true;
                }return false;
            }
            Action callback = null;
            int i = 0;
        }
        public void PlayIntro(Action callback = null)
        {
            if (IsPlayingAnimation) return;
            IsPlayingAnimation = true;
            Clear();
            aniCallback = callback;
            index = -1;
            switch (target.IntroMode)
            {
                case SODocPage.DocPageAniMode.None:
                    foreach (var ve in visuals)
                        Add(ve);
                    startAniEndCheck();
                    break;
                case SODocPage.DocPageAniMode.Sametime:
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
            if (IsPlayingAnimation) return;
            IsPlayingAnimation = true;
            aniCallback = callback;
            index = -1;
            switch (target.OuttroMode)
            {
                case SODocPage.DocPageAniMode.None:
                    startAniEndCheck();
                    break;
                case SODocPage.DocPageAniMode.Sametime:
                    foreach (var ve in visuals)
                    {
                        if(ve.OuttroAnimation != null)
                        {
                            playingCount++;
                            ve.OuttroAnimation(docPlayingEnd);
                        }
                    }
                    startAniEndCheck();
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
                aniCallback?.Invoke();
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
                aniCallback?.Invoke();
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
            item.ExecuteLater(target.IntroDuration);
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
            item.ExecuteLater(target.OuttroDuration);
        }

        void startAniEndCheck()
        {
            schedule.Execute(() => { }).Until(() =>
            {
                if (playingCount == 0)
                {
                    aniCallback?.Invoke();
                    IsPlayingAnimation = false;
                    return true;
                }
                return false;
            }).Every(50);
        }
        void docPlayingEnd()
        {
            playingCount--;
        }
    }
}
