using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using UnityEngine.XR;

namespace NaiveAPI.DocumentBuilder
{
    public class URLImage : TextElement
    {
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
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            cachePath =url;
            foreach (char c in Path.GetInvalidPathChars())
                cachePath = cachePath.Replace(c, '_');
            foreach (char c in "/\\?!:.")
                cachePath = cachePath.Replace(c, '_');
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
