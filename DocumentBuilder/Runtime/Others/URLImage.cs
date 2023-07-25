using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NaiveAPI.DocumentBuilder
{
    public class URLImage
    {
        public URLImage(string url, Action<Texture2D, UnityWebRequest> onImageLoad)
        {
            URL = UnityWebRequestTexture.GetTexture(url);

        }
        public UnityWebRequest URL = null;
        public Texture2D Texture = null;
        
        //private static bool urlRequest()
        //{
        //    if (requestToURL.SendWebRequest().isDone)
        //    {
        //        textureToLoad = ((DownloadHandlerTexture)requestToURL.downloadHandler).texture;
        //        return true;
        //    }
        //    return false;
        //}
    }
}
