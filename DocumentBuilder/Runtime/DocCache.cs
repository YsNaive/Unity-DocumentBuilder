using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [System.Serializable]
    public class DocCache
    {
        static string path = Application.temporaryCachePath + "/DocumentBuilderSettings.json";
        private static DocCache instance;
        public static DocCache Get()
        {
            instance = null;
            if (instance == null)
            {
                if (File.Exists(path))
                    instance = JsonUtility.FromJson<DocCache>(File.ReadAllText(path));
                else
                {
                    instance = new DocCache();
                    instance.currentStyle = DocStyle.Dark;
                    Save();
                }
            }
            return instance;
        }
        public static void Save()
        {
            File.WriteAllText(path, JsonUtility.ToJson(instance));
        }
        public static List<string> LanguageList => Get().languageList;
        public static DocStyle CurrentStyle => Get().currentStyle;

        [SerializeField] private List<string> languageList = new List<string>();
        [SerializeField] private DocStyle currentStyle = new DocStyle();
    }
}
