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
        // �N�C���ܾ����ڤ����K�[��UI���O��
        var uiRoot = GetComponent<UIDocument>().rootVisualElement;
        uiRoot.Add(new ColorField(Color.cyan));

    }

}
