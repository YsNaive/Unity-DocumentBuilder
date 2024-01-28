using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocComponentProperty
    {
        public event Action<string> OnPropertyChanged;
        public bool IsSerializedProperty => m_IsSerializedProperty;
        private bool m_IsSerializedProperty;
        private DocComponent m_TargetComponent;
        private SerializedProperty m_TargetPreperty;
        private SerializedProperty m_VisualVersion;
        private SerializedProperty m_VisualID;
        private SerializedProperty m_JsonData;
        private TextDataProperty m_TextData;
        private ObjsDataProperty m_ObjsData;
        public bool IsChanged
        {
            get
            {
                var result = (m_IsChanged | m_TextData.IsChanged | m_ObjsData.IsChanged);
                m_IsChanged = false;
                return result;
            }
        }
        private bool m_IsChanged;
        public SerializedProperty SerializedProperty => m_TargetPreperty;
        public ushort VisualVersion
        {
            get
            {
                if (m_IsSerializedProperty)
                    return (ushort)m_VisualVersion.intValue;
                else
                    return m_TargetComponent.VisualVersion;
            }
            set
            {
                if (m_IsSerializedProperty)
                    m_VisualVersion.intValue = value;
                else
                    m_TargetComponent.VisualVersion = value;
                m_IsChanged = true;
                OnPropertyChanged?.Invoke("VisualVersion");
            }
        }
        public string VisualID
        {
            get
            {
                if (m_IsSerializedProperty)
                    return m_VisualID.stringValue;
                else
                    return m_TargetComponent.VisualID;
            }
            set
            {
                if(m_IsSerializedProperty)
                    m_VisualID.stringValue = value;
                else
                    m_TargetComponent.VisualID = value;
                m_IsChanged = true;
                OnPropertyChanged?.Invoke("VisualID");
            }
        }
        public string JsonData
        {
            get
            {
                if(m_IsSerializedProperty)
                    return m_JsonData.stringValue;
                else
                    return m_TargetComponent.JsonData;
            }
            set
            {
                if (m_IsSerializedProperty)
                    m_JsonData.stringValue = value;
                else
                    m_TargetComponent.JsonData = value;
                m_IsChanged = true;
                OnPropertyChanged?.Invoke("JsonData");
            }
        }
        public TextDataProperty TextData => m_TextData;
        public ObjsDataProperty ObjsData => m_ObjsData;
        public DocComponentProperty(DocComponent docComponent)
        {
            m_TargetComponent = docComponent;
            m_TextData = new TextDataProperty(docComponent.TextData);
            m_ObjsData = new ObjsDataProperty(docComponent.ObjsData);
            m_TextData.OnPropertyChanged += invokePropertyChanged;
            m_ObjsData.OnPropertyChanged += invokePropertyChanged;
            m_IsSerializedProperty = false;
        }
        public DocComponentProperty(SerializedProperty serializedProperty)
        {
            m_TargetPreperty = serializedProperty;
            m_VisualVersion = serializedProperty.FindPropertyRelative("VisualVersion");
            m_VisualID = serializedProperty.FindPropertyRelative("VisualID");
            m_JsonData = serializedProperty.FindPropertyRelative("JsonData");
            m_TextData = new TextDataProperty(serializedProperty.FindPropertyRelative("TextData"));
            m_ObjsData = new ObjsDataProperty(serializedProperty.FindPropertyRelative("ObjsData"));
            m_TextData.OnPropertyChanged += invokePropertyChanged;
            m_ObjsData.OnPropertyChanged += invokePropertyChanged;
            m_IsSerializedProperty = true;
        }
        void invokePropertyChanged(string info)
        {
            OnPropertyChanged?.Invoke(info);
        }
        public void Clear()
        {
            VisualID = string.Empty;
            JsonData = string.Empty;
            TextData.Clear();
            ObjsData.Clear();
            VisualVersion = 0;
            m_IsChanged = true;
            OnPropertyChanged?.Invoke("Clear");
        }
        public DocComponent ToDocComponent()
        {
            return new DocComponent()
            {
                VisualID = VisualID,
                JsonData = JsonData,
                VisualVersion = VisualVersion,
                TextData = TextData.ToList(),
                ObjsData = ObjsData.ToList(),
            };
        }
        public void FromDocComponent(DocComponent docComponent)
        {
            if (m_IsSerializedProperty)
            {
                VisualID = docComponent.VisualID;
                JsonData = docComponent.JsonData;
                VisualVersion = docComponent.VisualVersion;
                TextData.FromList(docComponent.TextData);
                ObjsData.FromList(docComponent.ObjsData);
            }
            else
            {
                m_TargetComponent = docComponent.Copy();
            }
            m_IsChanged = true;
            OnPropertyChanged?.Invoke("FromDocComponent");
        }
        public class TextDataProperty : IEnumerable<string>
        {
            public event Action<string> OnPropertyChanged;
            private bool m_IsSerializedProperty;
            SerializedProperty textDataProperty;
            List<string> textDataList;
            public bool IsChanged
            {
                get
                {
                    var result = m_IsChanged;
                    m_IsChanged = false;
                    return result;
                }
            }
            private bool m_IsChanged;
            public TextDataProperty(SerializedProperty serializedProperty)
            {
                m_IsSerializedProperty = true;
                textDataProperty = serializedProperty;
            }
            public TextDataProperty(List<string> property)
            {
                m_IsSerializedProperty = false;
                textDataList = property;
            }
            public int Count
            {
                get
                {
                    if (m_IsSerializedProperty)
                        return textDataProperty.arraySize;
                    else
                        return textDataList.Count;
                }
            }
            public string this[int index]
            {
                get
                {
                    if (m_IsSerializedProperty)
                        return textDataProperty.GetArrayElementAtIndex(index).stringValue;
                    else
                        return textDataList[index];
                }
                set
                {
                    if(m_IsSerializedProperty)
                        textDataProperty.GetArrayElementAtIndex(index).stringValue = value;
                    else
                        textDataList[index] = value;
                    m_IsChanged = true;
                    OnPropertyChanged?.Invoke($"TextData{index}");
                }
            }
            public void Insert(string value, int index)
            {
                if (m_IsSerializedProperty)
                {
                    textDataProperty.InsertArrayElementAtIndex(index);
                    textDataProperty.GetArrayElementAtIndex(index).stringValue = value;
                }
                else
                {
                    textDataList.Insert(index, value);
                }
                m_IsChanged = true;
                OnPropertyChanged?.Invoke($"TextData_Insert{index}");
            }
            public void Add(string value)
            {
                Insert(value, Count);
            }
            public void Clear()
            {
                if (m_IsSerializedProperty)
                    textDataProperty.ClearArray();
                else
                    textDataList.Clear();
                m_IsChanged = true;
                OnPropertyChanged?.Invoke("TextData_Clear");
            }
            public void RemoveAt(int index)
            {
                if (m_IsSerializedProperty)
                    textDataProperty.DeleteArrayElementAtIndex(index);
                else
                    textDataList.RemoveAt(index);
                m_IsChanged = true;
                OnPropertyChanged?.Invoke($"TextData_RemoveAt{index}");
            }
            public List<string> ToList()
            {
                if (m_IsSerializedProperty)
                {
                    List<string> result = new();
                    for(int i = 0; i < Count; i++)
                    {
                        result.Add(textDataProperty.GetArrayElementAtIndex(i).stringValue);
                    }
                    return result;
                }
                else
                    return new List<string>(textDataList);
            }

            public void FromList(List<string> list)
            {
                Clear();
                foreach (string item in list)
                    Add(item);
                m_IsChanged = true;
                OnPropertyChanged?.Invoke($"TextData_FromList");
            }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
            public IEnumerator<string> GetEnumerator()
            {
                if (m_IsSerializedProperty)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        yield return textDataProperty.GetArrayElementAtIndex(i).stringValue;
                    }
                }
                else
                {
                    foreach(var str in textDataList)
                        yield return str;
                }
            }

        }
        public class ObjsDataProperty : IEnumerable<Object>
        {
            public event Action<string> OnPropertyChanged;
            private bool m_IsSerializedProperty;
            SerializedProperty objsDataProperty;
            List<Object> objsDataList;
            public bool IsChanged
            {
                get
                {
                    var result = m_IsChanged;
                    m_IsChanged = false;
                    return result;
                }
            }
            private bool m_IsChanged;
            public ObjsDataProperty(SerializedProperty serializedProperty)
            {
                m_IsSerializedProperty = true;
                objsDataProperty = serializedProperty;
            }
            public ObjsDataProperty(List<Object> property)
            {
                m_IsSerializedProperty = false;
                objsDataList = property;
            }
            public int Count
            {
                get
                {
                    if (m_IsSerializedProperty)
                        return objsDataProperty.arraySize;
                    else
                        return objsDataList.Count;
                }
            }
            public Object this[int index]
            {
                get
                {
                    if (m_IsSerializedProperty)
                        return objsDataProperty.GetArrayElementAtIndex(index).objectReferenceValue;
                    else
                        return objsDataList[index];
                }
                set
                {
                    if (m_IsSerializedProperty)
                        objsDataProperty.GetArrayElementAtIndex(index).objectReferenceValue = value;
                    else
                        objsDataList[index] = value;
                    m_IsChanged = true;
                    OnPropertyChanged?.Invoke($"ObjsData{index}");
                }
            }
            public void Insert(Object value, int index)
            {
                if (m_IsSerializedProperty)
                {
                    objsDataProperty.InsertArrayElementAtIndex(index);
                    objsDataProperty.GetArrayElementAtIndex(index).objectReferenceValue = value;
                }
                else
                {
                    objsDataList.Insert(index, value);
                }
                m_IsChanged = true;
                OnPropertyChanged?.Invoke($"ObjsData_Insert{index}");
            }
            public void Add(Object value)
            {
                Insert(value, Count);
            }
            public void Clear()
            {
                if (m_IsSerializedProperty)
                    objsDataProperty.ClearArray();
                else
                    objsDataList.Clear();
                m_IsChanged = true;
                OnPropertyChanged?.Invoke($"ObjsData_Clear");
            }
            public void RemoveAt(int index)
            {
                if (m_IsSerializedProperty)
                    objsDataProperty.DeleteArrayElementAtIndex(index);
                else
                    objsDataList.RemoveAt(index);
                m_IsChanged = true;
                OnPropertyChanged?.Invoke($"ObjsData_RemoveAt{index}");
            }
            public List<Object> ToList()
            {
                if (m_IsSerializedProperty)
                {
                    List<Object> result = new();
                    for (int i = 0; i < Count; i++)
                    {
                        result.Add(objsDataProperty.GetArrayElementAtIndex(i).objectReferenceValue);
                    }
                    return result;
                }
                else
                    return new List<Object>(objsDataList);
            }
            public void FromList(List<Object> list)
            {
                Clear();
                foreach (Object item in list)
                    Add(item);
                m_IsChanged = true;
                OnPropertyChanged?.Invoke("ObjsData_FromList");
            }
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
            public IEnumerator<Object> GetEnumerator()
            {
                if (m_IsSerializedProperty)
                {
                    for (int i = 0; i < Count; i++)
                    {
                        yield return objsDataProperty.GetArrayElementAtIndex(i).objectReferenceValue;
                    }
                }
                else
                {
                    foreach (var str in objsDataList)
                        yield return str;
                }
            }
        }
        public static List<DocComponentProperty> LoadArrayProperty(SerializedProperty serializedProperty)
        {
            List<DocComponentProperty> result = new();
            for(int i = 0, imax = serializedProperty.arraySize; i < imax; i++)
                result.Add(new DocComponentProperty(serializedProperty.GetArrayElementAtIndex(i)));
            return result;
        }
    }
}