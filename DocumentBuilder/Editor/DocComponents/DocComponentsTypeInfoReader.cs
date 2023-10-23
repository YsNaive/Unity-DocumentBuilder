using NaiveAPI.DocumentBuilder;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocComponentsTypeInfoReader
    {
        public List<DocComponent> MethodComponents = new();
        public DocComponent PropertiesMatrixComponents;
        public DocComponent FieldsMatrixComponents;
        public DocComponent MethodListComponent;
        public DocComponentsTypeInfoReader(Type type, BindingFlags bindingFlags)
        {
            List<DocFuncDisplay.Data> datas = new List<DocFuncDisplay.Data>();
            DocComponent com = new DocComponent();

            #region Methods
            foreach (var method in type.GetMethods(bindingFlags))
            {
                if (method.Name.IndexOf("<") == 0) continue;
                if (method.Name.IndexOf("get_") == 0) continue;
                if (method.Name.IndexOf("set_") == 0) continue;
                if (method.Name.IndexOf("add_") == 0) continue;
                if (method.Name.IndexOf("remove_") == 0) continue;
                com = DocEditFuncDisplay.LoadMethod(method);
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
                MethodComponents.Add(com);
            }

            if (datas.Count != 0)
            {
                com = new DocComponent();
                com.VisualID = new DocItems().VisualID;
                var itemData = new DocItems.Data();
                itemData.num = datas.Count;
                com.JsonData = JsonUtility.ToJson(itemData);
                com.ObjsData.Add(DocEditorData.Instance.BuildinIcon.Find((match) =>
                {
                    return match.name == "ItemElement";
                }));
                foreach (var data in datas)
                    com.TextData.Add(data.Name);
                MethodListComponent = com;
            }
            #endregion

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
                com.TextData.Add(DocumentBuilderParser.CalGenericTypeName(prop.PropertyType));
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
                com.TextData.Add(DocumentBuilderParser.CalGenericTypeName(field.FieldType));
                com.TextData.Add(field.Name);
                com.TextData.Add("");
            }
            com.JsonData = JsonUtility.ToJson(matrixData);
            if (matrixData.row > 1)
                FieldsMatrixComponents = com;
            #endregion
        }
    }
}