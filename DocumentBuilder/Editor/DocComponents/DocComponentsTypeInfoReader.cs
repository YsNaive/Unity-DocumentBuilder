using NaiveAPI.DocumentBuilder;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocComponentsTypeInfoReader
    {
        public Type TargetType => m_TargetType;
        Type m_TargetType;
        public List<(string name, DocComponent component)> MethodComponents = new();
        public DocComponent PropertiesMatrixComponents;
        public DocComponent FieldsMatrixComponents;
        public DocComponent MethodListComponent;
        public List<MethodInfo> Methods = new();
        public List<ConstructorInfo> Constructors = new();
        public DocComponent ConstructorComponent;
        public List<(string name, MethodInfo getter, MethodInfo setter)> GetSetMethods = new();
        public List<(string name, DocComponent getter, DocComponent setter)> GetSetComponents = new();
        public DocComponentsTypeInfoReader(Type type, BindingFlags bindingFlags)
        {
            m_TargetType = type;
            readMethod(bindingFlags);

            methods2Components();
            getset2Conponents();
            constructor2Components();

            DocComponent com = new DocComponent();
            #region get set
            com = new DocComponent();
            com.VisualID = new DocMatrix().VisualID;
            var matrixData = new DocMatrix.Data();
            matrixData.ResizeContent(1, 5);
            matrixData.mode = DocMatrix.Mode.FixedText;
            matrixData.anchors = new TextAnchor[] { TextAnchor.MiddleLeft, TextAnchor.MiddleLeft, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleLeft };

            com.TextData.Add("Type");
            com.TextData.Add("Name");
            com.TextData.Add("get");
            com.TextData.Add("set");
            com.TextData.Add("Usage");
            foreach (var prop in type.GetProperties(bindingFlags))
            {
                matrixData.row++;
                com.TextData.Add(TypeReader.GetName(prop.PropertyType));
                com.TextData.Add(prop.Name);
                com.TextData.Add(prop.CanRead ? "―" : "ー");
                com.TextData.Add(prop.CanWrite ? "―" : "ー");
                com.TextData.Add("");
            }
            com.JsonData = JsonUtility.ToJson(matrixData);
            if (matrixData.row > 1)
                PropertiesMatrixComponents = com;

            #endregion
            #region fields
            com = new DocComponent();
            com.VisualID = new DocMatrix().VisualID;
            matrixData = new DocMatrix.Data();
            matrixData.ResizeContent(1, 3);
            matrixData.mode = DocMatrix.Mode.FixedText;
            matrixData.anchors = new TextAnchor[] { TextAnchor.MiddleLeft, TextAnchor.MiddleLeft, TextAnchor.MiddleLeft };

            com.TextData.Add("Type");
            com.TextData.Add("Name");
            com.TextData.Add("Usage");
            foreach (var field in type.GetFields(bindingFlags))
            {
                matrixData.row++;
                com.TextData.Add(TypeReader.GetName(field.FieldType));
                com.TextData.Add(field.Name);
                com.TextData.Add("");
            }
            com.JsonData = JsonUtility.ToJson(matrixData);
            if (matrixData.row > 1)
                FieldsMatrixComponents = com;
            #endregion
        }
        private void readMethod(BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
            foreach (var method in TargetType.GetMethods(bindingFlags))
            {
                bool isGet = method.Name.StartsWith("get_");
                bool isSet = method.Name.StartsWith("set_");
                if (isGet || isSet)
                {
                    var name = method.Name.Substring(4);
                    (string name, MethodInfo getter, MethodInfo setter) curr = ("", null, null);
                    foreach (var item in GetSetMethods)
                    {
                        if (item.name == name)
                        {
                            curr = item;
                            break;
                        }
                    }
                    if (curr.name == "")
                    {
                        curr = (name, null, null);
                        GetSetMethods.Add(curr);
                    }
                    if (isGet) curr.getter = method;
                    else curr.setter = method;
                }
                else
                {
                    Methods.Add(method);
                }
            }
            foreach (var constructor in TargetType.GetConstructors(bindingFlags))
            {
                Constructors.Add(constructor);
            }
        }
        private void methods2Components()
        {
            List<DocFuncDisplay.Data> datas = new List<DocFuncDisplay.Data>();
            DocComponent com = new DocComponent();
            foreach (var method in Methods)
            {
                com = DocFuncDisplay.CreateComponent(method);
                com.TextData.Add("");
                var data = JsonUtility.FromJson<DocFuncDisplay.Data>(com.JsonData);
                int index = datas.FindIndex(0, m => { return m.Name == data.Name; });
                if (index != -1)
                {
                    datas[index].Syntaxs.Add(data.Syntaxs[0]);
                    foreach (var param in data.Params)
                    {
                        if (datas[index].Params.FindIndex(0, m => { return (m.ParamName == param.ParamName && m.Type == param.Type); }) == -1)
                        {
                            datas[index].Params.Add(param);
                        }
                    }
                    foreach (var ret in data.ReturnTypes)
                    {
                        if (!datas[index].ReturnTypes.Contains(ret))
                        {
                            datas[index].ReturnTypes.Add(ret);
                        }
                    }
                }
                else
                {
                    datas.Add(data);
                }
            }

            foreach (var data in datas)
            {
                com = com.Copy();
                com.TextData.Clear();
                com.TextData.Add("");
                foreach (var param in data.Params)
                    com.TextData.Add("");
                foreach (var param in data.ReturnTypes)
                    com.TextData.Add("");
                com.JsonData = JsonUtility.ToJson(data);
                MethodComponents.Add((data.Name, com));
            }

            if (datas.Count != 0)
            {
                com = new DocComponent();
                com.VisualID = new DocItems().VisualID;
                var itemData = new DocItems.Data();
                com.JsonData = JsonUtility.ToJson(itemData);
                com.ObjsData.Add(DocEditorData.Instance.BuildinIcon.Find((match) =>
                {
                    return match.name == "ItemElement";
                }));
                foreach (var data in datas)
                    com.TextData.Add(data.Name);
                MethodListComponent = com;
            }
        }
        private void getset2Conponents()
        {
            foreach(var item in GetSetMethods)
            {
                GetSetComponents.Add((item.name,
                    (item.getter != null) ? DocFuncDisplay.CreateComponent(item.setter) : null,
                    (item.setter != null) ? DocFuncDisplay.CreateComponent(item.getter) : null));
            }
        }
        private void constructor2Components()
        {
            if (Constructors.Count == 0) return;
            DocFuncDisplay.Data datas = null;
            DocComponent com = new DocComponent();
            foreach (var method in Constructors)
            {
                com = DocFuncDisplay.CreateComponent(method);
                com.TextData.Add("");
                com.TextData.Add("");
                var data = JsonUtility.FromJson<DocFuncDisplay.Data>(com.JsonData);
                if (datas != null)
                {
                    datas.Syntaxs.Add(data.Syntaxs[0]);
                    foreach (var param in data.Params)
                    {
                        if (datas.Params.FindIndex(0, m => { return (m.ParamName == param.ParamName && m.Type == param.Type); }) == -1)
                        {
                            datas.Params.Add(param);
                            com.TextData.Add("");
                        }
                    }
                }
                else
                {
                    datas = data;
                }
            }
            com.JsonData = JsonUtility.ToJson(datas);
            ConstructorComponent = com;
        }
    }
}