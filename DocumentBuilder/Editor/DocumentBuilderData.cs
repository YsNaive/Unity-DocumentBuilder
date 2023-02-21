using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DocumentBuilder
{
    public static class DocumentBuilderData
    {
        public static void SaveString(string folder,string name, string data, bool autoCreateDirectory)
        {
            if (autoCreateDirectory)
            {
                string[] assetsDir = folder.Split(new char[] { '\\', '/' });
                string pathNow = assetsDir[0];
                for (int i = 1; i < assetsDir.Length; i++)
                {
                    if (!AssetDatabase.IsValidFolder($"{pathNow}\\{assetsDir[i]}"))
                    {
                        AssetDatabase.CreateFolder(pathNow, assetsDir[i]);
                    }
                    pathNow += "\\" + assetsDir[i];
                }
            }
            SaveString(folder, name, data);
        }
        /// <summary>
        /// Path releated on Assets folder
        /// </summary>
        public static void SaveString(string folder, string name, string data)
        {
            if(AssetDatabase.IsValidFolder(folder))
            {
                File.WriteAllText(Path.ProjectRoot + '\\' + folder+'\\'+name, data);
                AssetDatabase.Refresh();
                Debug.Log("Save data at " + Path.ProjectRoot + '\\' + folder + '\\' + name);
            }
        }
        public static string LoadString(string folder,string name)
        {
            return File.ReadAllText(Path.ProjectRoot + '\\' + folder + '\\' + name);
        }
        /// <summary>
        /// All path is below Assets Folder
        /// </summary>
        public static class Path
        {
            public static string DocumentBuilderRoot 
            {
                get
                {
                    string fullPath = Directory.GetFiles(Application.dataPath, "SODocumentBuilderSetting.cs", SearchOption.AllDirectories)[0];
                    int index = fullPath.IndexOf("Assets");
                    return fullPath.Substring(index, 22);
                }
            }
            public static string ProjectRoot
            {
                get
                {
                    string path = Application.dataPath;
                    return path.Substring(0, path.Length-7);
                }
            }
            public static string ProjectSetting
            {
                get
                {
                    return DocumentBuilderRoot + "\\Editor\\Settings";
                }
            }
            public static string DocTemplate
            {
                get
                {
                    return DocumentBuilderRoot + "\\Editor\\Settings\\DocTemplate";
                }
            }
        }
        [System.Serializable]
        public class SerializableList<T> where T:new()
        {
            public List<T> list;
        }
    }
}
