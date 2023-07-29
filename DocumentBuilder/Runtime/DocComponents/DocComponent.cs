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

        public DocComponent Copy()
        {
            DocComponent copy = new DocComponent();
            copy.VisualID = VisualID;
            copy.JsonData = JsonData;
            copy.TextData = new List<string>(TextData);
            copy.ObjsData = new List<Object>(ObjsData);
            return copy;
        } 
    }
}

