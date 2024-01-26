using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEditor;
using UnityEngine;


namespace NaiveAPI.DocumentBuilder
{
    [CreateAssetMenu(menuName = "Naive API/DocumentBuilder/new ScriptAPI Info")]
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
        
        public void MakeValid()
        {
            if (TargetType == null)
                return;
            foreach (var field in TargetType.GetFields(TypeReader.DeclaredPublicInstance))
            {
                var id = GetMemberID(field);
                if (!m_MemberInfoKeys.Contains(id))
                {
                    m_MemberInfoKeys.Add(id);
                    m_MemberInfos.Add(new ScriptAPIMemberInfo() { IsDisplay = true });
                }
            }
            foreach (var field in TargetType.GetFields(TypeReader.DeclaredPrivateInstance))
            {
                var id = GetMemberID(field);
                if (!m_MemberInfoKeys.Contains(id))
                {
                    m_MemberInfoKeys.Add(id);
                    m_MemberInfos.Add(new ScriptAPIMemberInfo() { IsDisplay = false });
                }
            }
            foreach (var prop in TargetType.GetProperties(TypeReader.DeclaredPublicInstance))
            {
                var id = GetMemberID(prop);
                if (!m_MemberInfoKeys.Contains(id))
                {
                    m_MemberInfoKeys.Add(id);
                    m_MemberInfos.Add(new ScriptAPIMemberInfo() { IsDisplay = true });
                }
            }
            foreach (var prop in TargetType.GetProperties(TypeReader.DeclaredPrivateInstance))
            {
                var id = GetMemberID(prop);
                if (!m_MemberInfoKeys.Contains(id))
                {
                    m_MemberInfoKeys.Add(id);
                    m_MemberInfos.Add(new ScriptAPIMemberInfo() { IsDisplay = false });
                }
            }
            foreach (var method in TargetType.GetMethods(TypeReader.DeclaredPublicInstance))
            {
                var id = GetMemberID(method);
                var display = true;
                if      (id.StartsWith("Equals("))      display = false;
                else if (id.StartsWith("GetHashCode(")) display = false;
                else if (id.StartsWith("ToString("))    display = false;
                if (!m_MemberInfoKeys.Contains(id))
                {
                    m_MemberInfoKeys.Add(id);
                    m_MemberInfos.Add(new ScriptAPIMemberInfo() { IsDisplay = display });
                }
            }
            foreach (var method in TargetType.GetMethods(TypeReader.DeclaredPrivateInstance))
            {
                var id = GetMemberID(method);
                if (!m_MemberInfoKeys.Contains(id))
                {
                    m_MemberInfoKeys.Add(id);
                    m_MemberInfos.Add(new ScriptAPIMemberInfo() { IsDisplay = false });
                }
            }
            foreach (var ctor in TargetType.GetConstructors(TypeReader.DeclaredPublicInstance))
            {
                var id = GetMemberID(ctor);
                if (!m_MemberInfoKeys.Contains(id))
                {
                    m_MemberInfoKeys.Add(id);
                    m_MemberInfos.Add(new ScriptAPIMemberInfo() { IsDisplay = true });
                }
            }
            foreach (var ctor in TargetType.GetConstructors(TypeReader.DeclaredPrivateInstance))
            {
                var id = GetMemberID(ctor);
                if (!m_MemberInfoKeys.Contains(id))
                {
                    m_MemberInfoKeys.Add(id);
                    m_MemberInfos.Add(new ScriptAPIMemberInfo() { IsDisplay = false });
                }
            }
        }
        public void ClearNonUsing()
        {
            List<int> deleteIndex = new();
            for(int i = 0; i < m_MemberInfos.Count; i++)
            {
                if (m_MemberInfos[i].IsEmpty)
                    deleteIndex.Add(i);
            }
            deleteIndex.Reverse();
            foreach (var i in deleteIndex)
            {
                m_MemberInfoKeys.RemoveAt(i);
                m_MemberInfos.RemoveAt(i);
            }
        }
        public void ClearAll()
        {
            m_MemberInfoKeys.Clear();
            m_MemberInfos.Clear();
            Description = new();
        }
        public ScriptAPIMemberInfo GetMemberInfo(ICustomAttributeProvider info) { 
            if(typeof(ParameterInfo).IsAssignableFrom(info.GetType()))
                return GetMemberInfo(GetMemberID(info as ParameterInfo));
            else
                return GetMemberInfo(GetMemberID(info as MemberInfo));
        }
        public ScriptAPIMemberInfo GetMemberInfo(ParameterInfo info) { return GetMemberInfo(GetMemberID(info)); }
        public ScriptAPIMemberInfo GetMemberInfo(MemberInfo info) { return GetMemberInfo(GetMemberID(info)); }
        public ScriptAPIMemberInfo GetMemberInfo(string id)
        {
            var i = m_MemberInfoKeys.IndexOf(id);
            if (i != -1)
                return m_MemberInfos[i];
            ScriptAPIMemberInfo newInfo = new();
            m_MemberInfoKeys.Add(id);
            m_MemberInfos.Add(newInfo);
            return newInfo;
        }
        public static SerializedProperty GetMemberInfo(SerializedObject so, string id)
        {
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
            return vals.GetArrayElementAtIndex(j);
        }

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

        public static string GetMemberID(MemberInfo info)
        {
            if (typeof(FieldInfo).IsAssignableFrom(info.GetType()))
                return GetMemberID((FieldInfo)info);
            else if (typeof(PropertyInfo).IsAssignableFrom(info.GetType()))
                return GetMemberID((PropertyInfo)info);
            else if (typeof(MethodBase).IsAssignableFrom(info.GetType()))
                return GetMemberID((MethodBase)info);
            return "";
        }
        public static string GetMemberID(FieldInfo info)
        {
            return info.Name;
        }
        public static string GetMemberID(PropertyInfo info)
        {
            return info.Name;
        }
        public static string GetMemberID(MethodBase info)
        {
            var parameters = info.GetParameters();
            var result = $"{info.Name}(";
            for (int i = 0, imax = parameters.Length; i < imax; i++)
                result += ((i == 0) ? "" : ",") + TypeReader.GetName(parameters[i].ParameterType);
            result += ")";
            return result;
        }
        public static string GetMemberID(ParameterInfo info)
        {
            return $"{info.Member.Name}.{info.ParameterType.Name}.{info.Name}";
        }
    }
    [Serializable]
    public class ScriptAPIMemberInfo
    {
        public bool IsEmpty
        {
            get => Tooltip.Count == 0 && Description.Count == 0;
        }
        public bool IsDisplay;
        public List<DocComponent> Tooltip = new();
        public List<DocComponent> Description = new();
    }
}
