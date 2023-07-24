using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    public class DocSettings : ScriptableObject
    {
        private static DocSettings instance;
        public static DocSettings Get()
        {
            if (instance == null)
                instance = Resources.Load<DocSettings>("DocSettings.asset");
            return instance;
        }

        public List<string> LanguageList = new List<string>();
    }
}
