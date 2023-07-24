using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class test : MonoBehaviour
{
    void Start()
    {        
        // 將顏色選擇器的根元素添加到UI面板中
        var uiRoot = GetComponent<UIDocument>().rootVisualElement;
        uiRoot.Add(new ColorField(Color.cyan));

    }

}
