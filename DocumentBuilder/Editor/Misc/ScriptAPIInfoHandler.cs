using NaiveAPI.DocumentBuilder;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [InitializeOnLoad]
    public static class ScriptAPIInfoHandler
    {
        static ScriptAPIInfoHandler()
        {
            //var info = ScriptableObject.CreateInstance<SOScriptAPIInfo>();
            //info.TargetType = typeof(DocComponent);
            //info.Hide.Add("VisualID");
            //info.Tooltip.Add("JsonData", "Save data as json.");
            //AssetDatabase.CreateAsset(info, "Assets/test/DocComAPI.asset");
            LoadAllInfos();

        }

        public static Dictionary<Type, SOScriptAPIInfo> LoadAllInfos()
        {
            ScriptAPIInfoHolder.Infos.Clear();
            foreach(var asset in AssetDatabase.FindAssets("t:SOScriptAPIInfo"))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(asset);
                var info = AssetDatabase.LoadAssetAtPath<SOScriptAPIInfo>(assetPath);
                if (info.TargetType != null)
                    ScriptAPIInfoHolder.Infos.TryAdd(info.TargetType, info);
            }
            return ScriptAPIInfoHolder.Infos;
        }
    }

}