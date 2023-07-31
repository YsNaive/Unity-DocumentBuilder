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
        public List<int> AniSettings = new List<int>() { 1, 250, 1, 250 }; // {intro type, intro time, outtro type, outtro time}
        public DocComponent Copy()
        {
            DocComponent copy = new DocComponent();
            copy.VisualID = VisualID;
            copy.JsonData = JsonData;
            copy.TextData = new List<string>(TextData);
            copy.ObjsData = new List<Object>(ObjsData);
            return copy;
        } 
        public bool ContentsEqual(DocComponent other)
        {
            if(VisualID != other.VisualID) return false;
            if(JsonData != other.JsonData) return false;
            if(TextData.Count != other.TextData.Count) return false;
            int i = 0;
            foreach(int val in AniSettings) { 
                if(val != other.AniSettings[i])return false;
                i++;
            }
            i = 0;
            foreach(string text in TextData) { 
                if(text != other.TextData[i])return false;
                i++;
            }
            if(ObjsData.Count != other.ObjsData.Count) return false;
            i = 0;
            foreach(Object obj in ObjsData) { 
                if(obj != other.ObjsData[i])return false;
                i++;
            }
            return true;
        }

        #region get set        
        public int IntroType
        {
            get {
                if(AniSettings == null || AniSettings.Count == 0)
                {
                    AniSettings = new List<int> { 1, 1, 250, 250 };
                }
                return AniSettings[0];
            }
            set
            {
                if (AniSettings == null || AniSettings.Count == 0)
                {
                    AniSettings = new List<int> { 1, 1, 250, 250 };
                }
                AniSettings[0] = value;
            }
        }
        public int IntroTime
        {
            get => AniSettings[1];
            set => AniSettings[1] = value;
        }
        public int OuttroType
        {
            get => AniSettings[2];
            set => AniSettings[2] = value;
        }
        public int OuttroTime
        {
            get => AniSettings[3];
            set => AniSettings[3] = value;
        }
        #endregion
    }
}

