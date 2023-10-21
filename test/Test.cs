using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class Test : MonoBehaviour
{
    public Sprite icon;
    public SODocPage page;
    // Start is called before the first frame update
    void Start()
    {
        var root = FindAnyObjectByType<UIDocument>().rootVisualElement;
        var sc = new DocScrollView();
        sc.Add(new DocPageMenuItem(page));
        root.Add(sc);
    }
    //https://chart.apis.google.com/chart?cht=tx&chs=50&chf=bg,s,FFFFFF00&chl=
    // Update is called once per frame
    void Update()
    {
    }
}