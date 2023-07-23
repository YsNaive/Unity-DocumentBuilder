using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [System.Serializable]
    public class DocSettings
    {

        public static void Save() { Save(Application.temporaryCachePath + "/DocSettings.json"); }
        public static void Save(string path)
        {
            
        }
    }
}
