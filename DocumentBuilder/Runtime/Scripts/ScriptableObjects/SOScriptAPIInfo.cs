using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
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
                ClearAll();
            }
        }
        private Type m_TargetType;
        [SerializeField] private string s_type = "";
        public List<DocComponent> Tooltip = new();
        public List<DocComponent> Description = new();
        public List<DocComponent> Tutorial = new();
        public List<string> s_memberInfoKeys = new();
        public List<ScriptAPIMemberInfo> s_memberInfos = new();
        public Dictionary<string, ScriptAPIMemberInfo> MemberInfos = new();
        
        public void MakeValid()
        {
            if (TargetType == null)
                return;
            foreach (var field in TargetType.GetFields(TypeReader.DeclaredPublicInstance))
            {
                var id = GetMemberID(field);
                if (!MemberInfos.ContainsKey(id))
                    MemberInfos.Add(id, new ScriptAPIMemberInfo() { IsDisplay = true });
            }
            foreach (var field in TargetType.GetFields(TypeReader.DeclaredPrivateInstance))
            {
                var id = GetMemberID(field);
                if (!MemberInfos.ContainsKey(id))
                    MemberInfos.Add(id, new ScriptAPIMemberInfo() { IsDisplay = false });
            }
            foreach (var prop in TargetType.GetProperties(TypeReader.DeclaredPublicInstance))
            {
                var id = GetMemberID(prop);
                if (!MemberInfos.ContainsKey(id))
                    MemberInfos.Add(id, new ScriptAPIMemberInfo() { IsDisplay = true });
            }
            foreach (var prop in TargetType.GetProperties(TypeReader.DeclaredPrivateInstance))
            {
                var id = GetMemberID(prop);
                if (!MemberInfos.ContainsKey(id))
                    MemberInfos.Add(id, new ScriptAPIMemberInfo() { IsDisplay = false });
            }
            foreach (var method in TargetType.GetMethods(TypeReader.DeclaredPublicInstance))
            {
                var id = GetMemberID(method);
                var display = true;
                if      (id.StartsWith("Equals("))      display = false;
                else if (id.StartsWith("GetHashCode(")) display = false;
                else if (id.StartsWith("ToString("))    display = false;
                if (!MemberInfos.ContainsKey(id))
                    MemberInfos.Add(id, new ScriptAPIMemberInfo() { IsDisplay = display });
            }
            foreach (var method in TargetType.GetMethods(TypeReader.DeclaredPrivateInstance))
            {
                var id = GetMemberID(method);
                if (!MemberInfos.ContainsKey(id))
                    MemberInfos.Add(id, new ScriptAPIMemberInfo() { IsDisplay = false });
            }
            foreach (var ctor in TargetType.GetConstructors(TypeReader.DeclaredPublicInstance))
            {
                var id = GetMemberID(ctor);
                if (!MemberInfos.ContainsKey(id))
                    MemberInfos.Add(id, new ScriptAPIMemberInfo() { IsDisplay = true });
            }
            foreach (var ctor in TargetType.GetConstructors(TypeReader.DeclaredPrivateInstance))
            {
                var id = GetMemberID(ctor);
                if (!MemberInfos.ContainsKey(id))
                    MemberInfos.Add(id, new ScriptAPIMemberInfo() { IsDisplay = false });
            }
        }
        public void ClearNonUsing()
        {
            SDictionary<string, ScriptAPIMemberInfo> newDict = new();
            foreach(var pair in MemberInfos)
            {
                if(!pair.Value.IsEmpty)
                    newDict.Add(pair.Key, pair.Value);
            }
            MemberInfos = newDict;
        }
        public void ClearAll()
        {
            MemberInfos.Clear();
            Description = new();
            Tutorial = new();
        }
        public ScriptAPIMemberInfo GetMemberInfo(ParameterInfo info) { return GetMemberInfo(GetMemberID(info)); }
        public ScriptAPIMemberInfo GetMemberInfo(MemberInfo info) { return GetMemberInfo(GetMemberID(info)); }
        public ScriptAPIMemberInfo GetMemberInfo(string id)
        {
            if (MemberInfos.ContainsKey(id))
                return MemberInfos[id];
            ScriptAPIMemberInfo newInfo = new();
            MemberInfos.Add(id, newInfo);
            return newInfo;
        }

        public void OnAfterDeserialize()
        {
            MemberInfos.Clear();
            var s_keys = s_memberInfoKeys;
            var s_values = s_memberInfos;
            for (int i = 0, imax = s_keys.Count; i < imax; i++)
                MemberInfos.Add(s_keys[i], s_values[i]);

            s_keys = null;
            s_values = null;
            TargetType = Type.GetType(s_type);
            s_type = null;
        }
        public void OnBeforeSerialize()
        {
            var s_keys = s_memberInfoKeys;
            var s_values = s_memberInfos;
            s_keys = new ();
            s_values = new ();
            foreach (var pair in MemberInfos)
            {
                s_keys.Add(pair.Key);
                s_values.Add(pair.Value);
            }
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
                result += ((i == 0) ? "" : ",") + DocumentBuilderParser.CalGenericTypeName(parameters[i].ParameterType);
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
