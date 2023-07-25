using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

namespace NaiveAPI.DocumentBuilder
{
    public class DocImage : DocVisual
    {
        public override string VisualID => "5";

        public override void OnCreateGUI()
        {
            style.height = 300;
            LoadImgFromURL(Target.TextData[0], (tex, req) =>
            {
                style.backgroundImage = tex;
                Debug.Log(req.result);
            });
        }

        private static UnityWebRequest requestToURL = null;
        private static Texture2D textureToLoad = null;
        public static void LoadImgFromURL(string url, Action<Texture2D, UnityWebRequest> onImageLoad)
        {
            requestToURL = UnityWebRequestTexture.GetTexture(url);
            
        }
        private static bool urlRequest()
        {
            if(requestToURL.SendWebRequest().isDone)
            {
                textureToLoad = ((DownloadHandlerTexture)requestToURL.downloadHandler).texture;
                return true;
            }
            return false;
        }
    }

}