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
        [MenuItem("Tools/NaiveAPI/DocumentBuilder/Script DocLoader")]
        public static void ShowWindow()
        {
            GetWindow<ScriptInfoLoaderWindow>("Script DocLoader");
        }

        ScrollView show;
        ObjectField selectScript;
        DropdownField publicDrop, staticDrop;
        private void CreateGUI()
        {
            show = DocRuntime.NewScrollView();
            var root = rootVisualElement;
            root.style.backgroundColor = SODocStyle.Current.BackgroundColor;
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
                var data = JsonUtility.FromJson<DocFuncDisplay.Data>(com.JsonData);
                int index = datas.FindIndex(0, m => { return m.Name == data.Name; });
                if (index != -1)
                {
                    datas[index].Syntaxs.Add(data.Syntaxs[0]);
                }
                else
                {
                    datas.Add(data);
                }
            }
            for (int i = 0; i < 100; i++)
                com.TextData.Add("");
            foreach (var data in datas)
            {
                com = com.Copy();
                com.JsonData = JsonUtility.ToJson(data);
                show.Add(DocEditor.CreateComponentField(com));
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
                com.TextData.Add(prop.PropertyType.Name);
                com.TextData.Add(prop.Name);
                com.TextData.Add(prop.CanRead? "―" : "ー");
                com.TextData.Add(prop.CanWrite ? "―" : "ー");
                com.TextData.Add("");
            }
            com.JsonData = JsonUtility.ToJson(matrixData);
            if(matrixData.row > 1)
                show.Add(DocEditor.CreateComponentField(com));

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
                com.TextData.Add(field.FieldType.Name);
                com.TextData.Add(field.Name);
                com.TextData.Add("");
            }
            com.JsonData = JsonUtility.ToJson(matrixData);
            if (matrixData.row > 1)
                show.Add(DocEditor.CreateComponentField(com));
            #endregion
        }

    }
}
