using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class URLImage : TextElement
    {
        public Texture2D LoadedTexture;
        public string URL;
        private UnityWebRequest request;
        private Action<Texture2D> onTextureLoaded;
        private UnityWebRequestAsyncOperation asyncOperation;
        public URLImage(string url, Action<Texture2D> onTextureLoaded)
        {
            style.SetIS_Style(DocStyle.Current.LabelText);
            style.backgroundColor = DocStyle.Current.BackgroundColor;
            style.unityTextAlign = TextAnchor.MiddleCenter;

            URL = url;
            request = UnityWebRequestTexture.GetTexture(url);
            asyncOperation = request.SendWebRequest();
            this.onTextureLoaded = onTextureLoaded;
            loadTexture();
        }
        private void loadTexture()
        {
            if (asyncOperation.isDone)
            {
                if(request.result == UnityWebRequest.Result.ConnectionError)
                    text = "Connection Error";
                else if(request.result == UnityWebRequest.Result.ProtocolError)
                    text = "Protocol Error";
                else
                    LoadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;

                if (LoadedTexture == null)
                    text = "Image Not Found";
                else
                {
                    style.backgroundImage = LoadedTexture;
                    onTextureLoaded?.Invoke(LoadedTexture);
                }
            }
            else
            {
                schedule.Execute(loadTexture).ExecuteLater(25);
            }
        }
    }
}
