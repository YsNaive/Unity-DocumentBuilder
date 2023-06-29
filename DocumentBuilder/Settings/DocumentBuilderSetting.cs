using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace DocumentBuilder
{
    [System.Serializable]
    public class DocumentBuilderSetting
    {
        public static DocumentStyle DocStyle = DocumentStyle.DarkTheme;

        public static float GUIScale = 1.0f;



        public static void Save() { Save(Application.temporaryCachePath); }
        public static void Save(string path, string fileName = "DocumentBuilderSetting.txt")
        {
            if (!Directory.Exists(path)) { Debug.LogError("Folder not exist: " + path + '/' + fileName); return; }
            using (StreamWriter writer = new StreamWriter(path+ '/' + fileName))
            {
                // Write the text to the file
                writer.Write(JsonUtility.ToJson(DocStyle));
                writer.Write('\n');
                writer.Write(GUIScale.ToString());
            }
        }

        public static void Load() { Load(Application.temporaryCachePath); }
        public static void Load(string path, string fileName = "DocumentBuilderSetting.txt")
        {
            if (!Directory.Exists(path)) { Debug.LogError("Folder not exist: " + path + '/' + fileName); return; }
            using (StreamReader reader = new StreamReader(path + '/' + fileName))
            {
                DocStyle = JsonUtility.FromJson<DocumentStyle>(reader.ReadLine());
                GUIScale = float.Parse(reader.ReadLine());
            }
        }
    }
}
