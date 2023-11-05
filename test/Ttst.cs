using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Ttst : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<string> expression = new List<string>()
        {
            @"\{[^\}]+\}", @"\\frac"
        };
        List<DocumentBuilderParser.TokenType> tokenTypes  = new List<DocumentBuilderParser.TokenType>() {
             DocumentBuilderParser.TokenType.Hello, DocumentBuilderParser.TokenType.World
        };
        var tokens = DocumentBuilderParser.Tokenize("\\frac{1}{2}", expression, tokenTypes);
        foreach (var token in tokens)
        {
            Debug.Log(token.type.ToString());
            Debug.Log(token.value.ToString());
        }
        UIDocument uid = FindAnyObjectByType<UIDocument>();
        uid.rootVisualElement.Add(DocVisual.Create(DocFuncDisplay.CreateComponent(typeof(IStyleExtension).GetMethods().Where(obj =>
        {
            return obj.Name.Contains("SetIS");
        }))));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
