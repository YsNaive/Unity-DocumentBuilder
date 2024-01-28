using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [System.Serializable]
    public class DocCache
    {
        static DocCache()
        {
            if(!Directory.Exists(DirectoryRoot))
                Directory.CreateDirectory(DirectoryRoot);
        }
        public static string DirectoryRoot => $"{Application.temporaryCachePath}/DocumentBuilderCache";
        private static DocCache instance;

        [SerializeField] public string OpeningBookHierarchy = "";
        [SerializeField] public float DocMenuWidth = 0.25f;
        [SerializeField] public List<string> FavoriteDocVisualID = new List<string>();
        public static DocCache Get() { return Get($"{DirectoryRoot}/DocumentBuilderSettings.json"); }
        public static DocCache Get(string path)
        {
            if (instance == null)
            {
                if (File.Exists(path))
                    Load(path);
                else
                {
                    instance = new DocCache();
                    Save(path);
                }
            }
            return instance;
        }
        public static void Save() { Save($"{DirectoryRoot}/DocumentBuilderSettings.json"); }
        public static void Save(string path)
        {
            File.WriteAllText(path, JsonUtility.ToJson(instance));
        }        
        public static void SaveData(string name, string data)
        {
            File.WriteAllText($"{DirectoryRoot}/{name}", data);
        }
        public static void Load() { Load($"{DirectoryRoot}/DocumentBuilderSettings.json"); }
        public static void Load(string path)
        {
            instance = JsonUtility.FromJson<DocCache>(File.ReadAllText(path));
        }
        public static string LoadData(string name)
        {
            var path = $"{DirectoryRoot}/{name}";
            if (!File.Exists(path)) return "";
            return File.ReadAllText(path);
        }

    }
}
