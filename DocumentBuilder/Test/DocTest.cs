using NaiveAPI_Editor.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class DocSample : DocVisual
//{
//    public enum MyAniMode
//    {
//        None = 0,
//        Fade = 1,
//        MyAnimation = 101,
//        MyAnimation2 = 102,
//    }
//    public override string VisualID => "Sample";
    
//    // this will invoke after the DocVisual's value set-up
//    protected override void OnCreateGUI()
//    {
//        // Add some visual you want
//    }
//    protected override void OnSelectIntroAni(int type)
//    {
//        // Apply default animation first
//        base.OnSelectIntroAni(type);
        
//        // If not using defaul, Apply extends
//        if(IntroAnimation == null)
//        {
//            if((MyAniMode)type == MyAniMode.MyAnimation)
//            {
//                IntroAnimation = (callback) =>
//                {
//                    // MyAnimation 1

//                    // Invoke callback when animation is done
//                    callback?.Invoke();
//                };
//            }
//            else if ((MyAniMode)type == MyAniMode.MyAnimation2)
//            {
//                IntroAnimation = (callback) =>
//                {
//                    // MyAnimation 2 ...
//                };
//            }
//        }
//    }
//    protected override void OnSelectOuttroAni(int type)
//    {
//        // Same as SelectIntro ...
//    }
//}
//public class DocEditSample : DocEditVisual
//{
//    public override string DisplayName => "Sample";
//    public override string VisualID => "Your Visual ID"; // same ID as in DocVisual
//    protected override Enum InitAniType
//            => DocSample.MyAniMode.None;

//    protected override void OnCreateGUI()
//    {
//        // Add some visual to modify contents
//    }
//}
