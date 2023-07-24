using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [System.Serializable]
    public class DocComponent
    {
        public string VisualID = string.Empty;
        public string JsonData = string.Empty;
        public List<string> TextData = new List<string>();
        public List<Object> ObjsData = new List<Object>();
    }
}

