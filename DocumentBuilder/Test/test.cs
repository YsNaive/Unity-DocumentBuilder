using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class test : MonoBehaviour
{
    public UIDocument document;
    void Start()
    {
        StartCoroutine(DownloadImage("https://hackmd.io/_uploads/SyZQPQ-6q.png"));
    }
    Texture2D texture;
    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            Debug.Log(request.error);
        else
            texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        VisualElement visualElement = new VisualElement();
        visualElement.style.backgroundImage = texture;
        visualElement.style.width = 300;
        visualElement.style.height = 600;
        document.rootVisualElement.Add(visualElement);
    }

}
