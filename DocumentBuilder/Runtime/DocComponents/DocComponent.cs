using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [System.Serializable]
    public class DocComponent
    {
        public string VisualID = string.Empty;
        public string TextData = string.Empty;
        public string JsonData = string.Empty;
        public List<Object> ObjsData = new List<Object>();
    }
}

