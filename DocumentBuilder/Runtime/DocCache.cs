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
        private static DocCache instance;

        [SerializeField] public List<string> LanguageList = new List<string>();
        [SerializeField] public DocStyle CurrentStyle = new DocStyle();
        [SerializeField] public string OpeningBookHierarchy = "";
        [SerializeField] public float DocMenuWidth = 200f;

        public static DocCache Get() { return Get(Application.temporaryCachePath + "/DocumentBuilderSettings.json"); }
        public static DocCache Get(string path)
        {
            if (instance == null)
            {
                if (File.Exists(path))
                    Load(path);
                else
                {
                    instance = new DocCache();
                    instance.CurrentStyle = DocStyle.Dark;
                    Save(path);
                }
            }
            return instance;
        }
        public static void Save() { Save(Application.temporaryCachePath + "/DocumentBuilderSettings.json"); }
        public static void Save(string path)
        {
            File.WriteAllText(path, JsonUtility.ToJson(instance));
        }
        public static void Load() { Load(Application.temporaryCachePath + "/DocumentBuilderSettings.json"); }
        public static void Load(string path)
        {
            instance = JsonUtility.FromJson<DocCache>(File.ReadAllText(path));
        }

    }
}
