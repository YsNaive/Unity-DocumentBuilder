using NaiveAPI;
using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Sample : MonoBehaviour
{
    // UID pre-set in inspector
    [SerializeField] UIDocument UIDocument;

    // document pre-set in inspector
    [SerializeField] private DocComponent component;
    public SODocPage root;
    void Start()
    {
        //var visual = DocRuntime.CreateVisual(component);
        //UIDocument.rootVisualElement.Add(visual);
        //visual.IntroAnimation(() =>
        //{ // after intro animation is done
        //    visual.schedule.Execute(() =>
        //    { // wait 2 sec
        //        visual.OuttroAnimation(() =>
        //        { // after outtro animation is done
        //            UIDocument.rootVisualElement.Remove(visual);
        //        });
        //    }).ExecuteLater(2000); 
        //});

        UIDocument.rootVisualElement.Add(new DocBookVisual(root));
    }
}
