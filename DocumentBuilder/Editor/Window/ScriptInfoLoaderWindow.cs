using NaiveAPI_Editor.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NaiveAPI_UI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class ScriptInfoLoaderWindow : EditorWindow
    {
        [MenuItem("Tools/NaiveAPI/DocumentBuilder/Script Components Loader", priority = 3)]
        public static void ShowWindow()
        {
            GetWindow<ScriptInfoLoaderWindow>("Script Components Loader");
        }

        ScrollView show;
        ObjectField selectScript;
        DropdownField publicDrop, staticDrop;
        private void CreateGUI()
        {
            show = DocRuntime.NewScrollView();
            show.style.marginTop = 20;
            var root = rootVisualElement;
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            root.style.SetIS_Style(ISPadding.Pixel(10));
            selectScript = DocEditor.NewObjectField<MonoScript>("", (e) =>
            {
                if (e.newValue == null) return;
                repaint();
            });
            selectScript[0].style.minWidth = 50;
            root.Add(selectScript);
            publicDrop = DocRuntime.NewDropdownField("", new List<string> { "Public", "Private" }, e =>
            {
                repaint();
            });publicDrop.index = 0;
            staticDrop = DocRuntime.NewDropdownField("", new List<string> { "Instance", "Static"  }, e =>
            {
                repaint();
            });staticDrop.index = 0;
            root.Add(DocRuntime.NewHorizontalBar(1,selectScript, publicDrop, staticDrop));
            root.Add(show);
            repaint();
        }
        BindingFlags flag = BindingFlags.DeclaredOnly;
        void repaint()
        {
            flag = BindingFlags.DeclaredOnly;
            if (publicDrop.index == 0) flag |= BindingFlags.Public;
            else flag |= BindingFlags.NonPublic;
            if (staticDrop.index == 0) flag |= BindingFlags.Instance;
            else flag |= BindingFlags.Static;
            if (selectScript.value == null) return;
            show.Clear();
            Type targetType = ((MonoScript)selectScript.value).GetClass();
            List< DocFuncDisplay.Data > datas = new List< DocFuncDisplay.Data >();
            DocComponent com = new DocComponent();
            #region Methods
            foreach (var method in targetType.GetMethods(flag))
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
                    foreach(var param in data.Params)
                    {
                        if (datas[index].Params.FindIndex(0, m => { return (m.ParamName == param.ParamName && m.Type == param.Type); }) == -1)
                        {
                            datas[index].Params.Add(param);
                        }
                    }
                    foreach(var ret in data.ReturnTypes)
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
                var visual = DocEditor.CreateComponentField(com);
                var container = DocRuntime.NewEmpty();
                var btn = visual.CopyBtn();
                btn.style.position = Position.Absolute;
                btn.style.top = 4;
                visual.style.marginLeft = 28;
                visual.SetEnabled(false);
                container.Add(visual);
                container.Add(btn);
                show.Add(container);
            }
            if(datas.Count != 0)
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
                var visual = DocEditor.CreateComponentField(com);
                var container = DocRuntime.NewEmpty();
                var btn = visual.CopyBtn();
                btn.style.position = Position.Absolute;
                btn.style.top = 4;
                visual.style.marginLeft = 28;
                visual.SetEnabled(false);
                container.Add(visual);
                container.Add(btn);
                show.Add(container);
            }
            #endregion
            #region get set
            com = new DocComponent();
            com.VisualID = new DocMatrix().VisualID;
            var matrixData = new DocMatrix.Data();
            matrixData.ResizeContent(1, 5);
            matrixData.mode = DocMatrix.Mode.FixedText;
            matrixData.anchors = new TextAnchor[] {TextAnchor.MiddleLeft, TextAnchor.MiddleLeft, TextAnchor.MiddleCenter, TextAnchor.MiddleCenter, TextAnchor.MiddleLeft };

            com.TextData.Add("Type");
            com.TextData.Add("Name");
            com.TextData.Add("get");
            com.TextData.Add("set");
            com.TextData.Add("Usage");
            foreach (var prop in targetType.GetProperties(flag))
            {
                matrixData.row++;
                com.TextData.Add(DocumentBuilderParser.CalGenericTypeName(prop.PropertyType));
                com.TextData.Add(prop.Name);
                com.TextData.Add(prop.CanRead? "―" : "ー");
                com.TextData.Add(prop.CanWrite ? "―" : "ー");
                com.TextData.Add("");
            }
            com.JsonData = JsonUtility.ToJson(matrixData);
            if(matrixData.row > 1)
            {
                var visual = DocEditor.CreateComponentField(com);
                var container = DocRuntime.NewEmpty();
                var btn = visual.CopyBtn();
                btn.style.position = Position.Absolute;
                btn.style.top = 4;
                visual.style.marginLeft = 28;
                visual.SetEnabled(false);
                container.Add(visual);
                container.Add(btn);
                show.Add(container);
            }

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
            foreach (var field in targetType.GetFields(flag))
            {
                matrixData.row++;
                com.TextData.Add(DocumentBuilderParser.CalGenericTypeName(field.FieldType));
                com.TextData.Add(field.Name);
                com.TextData.Add("");
            }
            com.JsonData = JsonUtility.ToJson(matrixData);
            if (matrixData.row > 1)
            {
                var visual = DocEditor.CreateComponentField(com);
                var container = DocRuntime.NewEmpty();
                var btn = visual.CopyBtn();
                btn.style.position = Position.Absolute;
                btn.style.top = 4;
                visual.style.marginLeft = 28;
                visual.SetEnabled(false);
                container.Add(visual);
                container.Add(btn);
                show.Add(container);
            }
            #endregion
        }

    }
}
