using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    public class SOScriptAPIInfo : ScriptableObject, ISerializationCallbackReceiver
    {
        public Type TargetType
        {
            get => m_TargetType;
            set
            {
                m_TargetType = value;
            }
        }
        private Type m_TargetType;
        [SerializeField] private string s_type = "";
        public List<DocComponent> Tooltip = new();
        public List<DocComponent> Description = new();
        [SerializeField] private List<string> m_MemberInfoKeys = new();
        [SerializeField] private List<ScriptAPIMemberInfo> m_MemberInfos = new();
        public void ClearAll()
        {
            m_MemberInfoKeys.Clear();
            m_MemberInfos.Clear();
            Description = new();
        }
        public ScriptAPIMemberInfo GetMemberInfo(ICustomAttributeProvider info)
        {
            var id = GetMemberID(info);
            var i = m_MemberInfoKeys.IndexOf(id);
            if (i != -1)
                return m_MemberInfos[i];
            ScriptAPIMemberInfo newInfo = new(info);
            m_MemberInfoKeys.Add(id);
            m_MemberInfos.Add(newInfo);
            return newInfo;
        }
#if UNITY_EDITOR
        /// <summary>
        /// <b>This Method Only Callable in Editor</b>
        /// </summary>
        /// <returns></returns>
        public static SerializedProperty GetMemberInfo(SerializedObject so, ICustomAttributeProvider info)
        {
            var id = GetMemberID(info);
            var keys = so.FindProperty("m_MemberInfoKeys");
            var vals = so.FindProperty("m_MemberInfos");
            for (int i = 0, imax = keys.arraySize; i < imax; i++)
            {
                if (keys.GetArrayElementAtIndex(i).stringValue == id)
                {
                    return vals.GetArrayElementAtIndex(i);
                }
            }
            int j = keys.arraySize;
            keys.InsertArrayElementAtIndex(j);
            keys.GetArrayElementAtIndex(j).stringValue = id;
            vals.InsertArrayElementAtIndex(j);
            vals.GetArrayElementAtIndex(j).FindPropertyRelative("IsDisplay").boolValue = new ScriptAPIMemberInfo(info).IsDisplay;
            return vals.GetArrayElementAtIndex(j);
        }
#endif
        public void OnAfterDeserialize()
        {
            TargetType = Type.GetType(s_type);
            s_type = null;
        }
        public void OnBeforeSerialize()
        {
            if (TargetType == null)
                s_type = "";
            else
                s_type = TargetType.AssemblyQualifiedName;
        }

        public static string GetMemberID(ICustomAttributeProvider info)
        {
            return info switch
            {
                FieldInfo     f => f.Name,
                PropertyInfo  p => p.Name,
                ParameterInfo p => $"{p.Member.Name}.{p.ParameterType.Name}.{p.Name}",
                MethodBase    m => getMethodBaseID(m),
                _ => ""
            };
        }
        private static string getMethodBaseID(MethodBase info)
        {
            var parameters = info.GetParameters();
            var result = $"{info.Name}(";
            for (int i = 0, imax = parameters.Length; i < imax; i++)
                result += ((i == 0) ? "" : ",") + TypeReader.GetName(parameters[i].ParameterType);
            result += ")";
            return result;
        }
    }
    [Serializable]
    public class ScriptAPIMemberInfo
    {
        public ScriptAPIMemberInfo() { }
        public ScriptAPIMemberInfo(ICustomAttributeProvider info) 
        {
            if (info.IsDefined(typeof(ObsoleteAttribute), false))
                IsDisplay = false;
            else
            {
                IsDisplay = info switch
                {
                    ParameterInfo p => false,
                    FieldInfo f => f.IsPublic,
                    MethodBase m => m.IsPublic,
                    PropertyInfo p => p.GetAccessors().Any(m => { return m.IsPublic; }),
                    _ => false
                };
            }
        }
        public bool IsDisplay;
        public List<DocComponent> Tooltip = new();
        public List<DocComponent> Description = new();
    }
}
