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
        public ushort VisualVersion = 0;
        public DocComponent Copy()
        {
            DocComponent copy = new DocComponent();
            copy.VisualID = VisualID;
            copy.JsonData = JsonData;
            copy.TextData = new List<string>(TextData);
            copy.ObjsData = new List<Object>(ObjsData);
            return copy;
        } 
        public void Clear()
        {
            VisualID = string.Empty;
            JsonData = string.Empty;
            TextData = new();
            ObjsData = new();
            VisualVersion = 0;
        }
        public bool ContentsEqual(DocComponent other)
        {
            if(VisualID != other.VisualID) return false;
            if(JsonData != other.JsonData) return false;
            if(TextData.Count != other.TextData.Count) return false;
            if (ObjsData.Count != other.ObjsData.Count) return false;
            int i = 0;
            foreach(string text in TextData) { 
                if(text != other.TextData[i])return false;
                i++;
            }
            i = 0;
            foreach(Object obj in ObjsData) { 
                if(obj != other.ObjsData[i])return false;
                i++;
            }
            return true;
        }
        public bool Contains(string value)
        {
            foreach (var text in TextData)
                if (text.Contains(value)) return true;
            return false;
        }
    }
}

