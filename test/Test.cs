using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
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
        var list = new List<string>() { "AAA/A1", "AAA/A2","AAA", "BB", "CCCC","L1/L2/L3/L4/L5" };
        var menu = DSStringMenu.CreatePopupMenu(list, val => { Debug.Log(val); });
        menu.Open(root);
        var ani = new VisualElementAnimation.GotoTransformPositionBackAndForth(menu, 1.5f, new Vector2(500, 200), () => { Debug.Log("END Animation"); });
        root.Add(new DSHorizontal(
            new DSButton("START", () => { ani.Start(); }),
            new DSButton("PAUSE", () => { ani.Pause(); }),
            new DSButton("CONTI", () => { ani.Continue(); })));
    }

    //https://chart.apis.google.com/chart?cht=tx&chs=50&chf=bg,s,FFFFFF00&chl=
    // Update is called once per frame
    void Update()
    {

    }
}