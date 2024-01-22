using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class URLImage : TextElement
    {
        static URLImage()
        {
            string path = Application.temporaryCachePath + "/imgCache";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        public Texture2D LoadedTexture;
        public string URL;
        private UnityWebRequest request;
        private Action<Texture2D> onTextureLoaded;
        private UnityWebRequestAsyncOperation asyncOperation;
        public bool IsLoadFromCache = false;
        private static Dictionary<string,Texture2D> ramCache = new Dictionary<string,Texture2D>();
        string cachePath;
        public URLImage(string url, Action<Texture2D> onTextureLoaded)
        {
            if (string.IsNullOrEmpty(url)) return;
            style.SetIS_Style(DocStyle.Current.LabelText);
            style.backgroundColor = DocStyle.Current.BackgroundColor;
            style.unityTextAlign = TextAnchor.MiddleCenter;
            string path = Application.temporaryCachePath + "/imgCache";
            Hash128 hash128 = new();
            
            hash128.Append(url);
            cachePath = hash128.ToString();

            cachePath = path + "/" + cachePath;
            if (File.Exists(cachePath))
            {
                IsLoadFromCache = true;
                url = cachePath;
            }

            URL = url;
            this.onTextureLoaded = onTextureLoaded; 
            ramCache.TryGetValue(cachePath, out LoadedTexture);
            if (LoadedTexture != null)
            {
                style.backgroundImage = LoadedTexture;
                onTextureLoaded?.Invoke(LoadedTexture);
            }
            else
            {
                request = UnityWebRequestTexture.GetTexture(url);
                asyncOperation = request.SendWebRequest();
                text = "Loading Img...";
                loadTexture();
            }
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
                    text = "";
                    style.backgroundImage = LoadedTexture;
                    onTextureLoaded?.Invoke(LoadedTexture);
                    if (!IsLoadFromCache)
                    { 
                        File.WriteAllBytes(cachePath, LoadedTexture.EncodeToPNG());
                    }
                    ramCache.TryAdd(cachePath, LoadedTexture);
                }
            }
            else
            {
                schedule.Execute(loadTexture).ExecuteLater(50);
            }
        }
    }
}
