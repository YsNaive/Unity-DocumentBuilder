using NaiveAPI_Editor.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        Toggle isPrivate, isPublic, isInstance, isStatic;
        ObjectField selectScript;
        DropdownField selectType;
        private void CreateGUI()
        {
            show = DocRuntime.NewScrollView();
            var root = rootVisualElement;
            root.style.backgroundColor = SODocStyle.Current.BackgroundColor;
            selectType = DocRuntime.NewDropdownField("Info to get", new List<string> { "Constructor", "Methods", "Fields" });
            selectType.RegisterValueChangedCallback(e => { repaint(); });
            selectType.index = 1;
            selectType[0].style.minWidth = 50;
            isPrivate = new Toggle("Private");
            isPrivate.value = false;
            isPrivate[0].style.minWidth = 50;
            isPrivate.RegisterValueChangedCallback(recalFlag);
            isPublic = new Toggle("Public");
            isPublic.value = true;
            isPublic[0].style.minWidth = 50;
            isPublic.RegisterValueChangedCallback(recalFlag);
            isInstance = new Toggle("Instance");
            isInstance.value = true;
            isInstance[0].style.minWidth = 50;
            isInstance.RegisterValueChangedCallback(recalFlag);
            isStatic = new Toggle("Static");
            isStatic.value = false;
            isStatic[0].style.minWidth = 50;
            isStatic.RegisterValueChangedCallback(recalFlag);
            selectScript = DocEditor.NewObjectField<MonoScript>("Target", (e) =>
            {
                if (e.newValue == null) return;
                repaint();
            });
            selectScript[0].style.minWidth = 50;
            root.Add(selectScript);
            root.Add(selectType);
            root.Add(DocRuntime.NewHorizontalBar(isPrivate, isPublic));
            root.Add(DocRuntime.NewHorizontalBar(isInstance, isStatic));
            root.Add(show);
        }
        BindingFlags flag = BindingFlags.DeclaredOnly;
        void recalFlag(ChangeEvent<bool> e)
        {
            flag = BindingFlags.DeclaredOnly;
            if (isPublic.value) flag |= BindingFlags.Public;
            if (isPrivate.value) flag |= BindingFlags.NonPublic;
            if (isInstance.value) flag |= BindingFlags.Instance;
            if (isStatic.value) flag |= BindingFlags.Static;
            repaint();
        }
        void repaint()
        {
            if (selectScript.value == null) return;
            show.Clear();
            Type targetType = ((MonoScript)selectScript.value).GetClass();
            if (selectType.value == "Constructor")
            {
                foreach (var cons in targetType.GetConstructors(flag))
                {
                    show.Add(DocRuntime.NewTextElement("Cons: " + cons.Name));
                }
            }
            if (selectType.value == "Methods")
            {
                foreach (var mods in targetType.GetMethods(flag))
                {
                    if (mods.Name.IndexOf("<") == 0) continue;
                    if (mods.Name.IndexOf("get_") == 0) continue;
                    if (mods.Name.IndexOf("set_") == 0) continue;
                    if (mods.Name.IndexOf("add_") == 0) continue;
                    if (mods.Name.IndexOf("remove_") == 0) continue;
                    show.Add(DocEditor.CreateComponentField(DocEditFuncDisplay.LoadMethod(mods)));
                }
            }
            if (selectType.value == "Fields")
            {
                DocComponent com = new DocComponent();
                com.VisualID = new DocMatrix().VisualID;
                DocMatrix.Data data = new DocMatrix.Data();
                var fields = targetType.GetFields(flag);
                data.row = fields.Length; 
                data.col = 5;
                List<TextAnchor> list = new List<TextAnchor>();
                for (int i = 0; i < 100; i++)
                    list.Add(TextAnchor.MiddleCenter);
                list[0] = TextAnchor.MiddleLeft;
                list[1] = TextAnchor.MiddleLeft;
                list[4] = TextAnchor.MiddleLeft;
                data.anchors = list.ToArray();
                data.mode = DocMatrix.Mode.FixedText;
                com.JsonData = JsonUtility.ToJson(data);
                for (int i = 0; i < 200; i++)
                    com.TextData.Add("");

                com.TextData[0] = "Type";
                com.TextData[1] = "Name";
                com.TextData[2] = "Get";
                com.TextData[3] = "Set";
                com.TextData[4] = "Usage";

                int n = 5;
                var func = targetType.GetMethods(flag);
                foreach(var fun in func)
                {
                    if (fun.Name.IndexOf("get_") != -1)
                    {
                        data.row++;
                        string str = fun.Name.Substring(4);
                        int i = com.TextData.IndexOf(str);
                        if (i == -1)
                        {
                            com.TextData[n] = fun.ReturnType.Name;
                            com.TextData[n + 1] = str;
                            com.TextData[n + 2] = "―";
                            com.TextData[n + 3] = "ー";
                            n += 5;
                        }
                        else
                        {
                            com.TextData[i + 2] = "―";
                        }
                    }
                    else if (fun.Name.IndexOf("set_") != -1)
                    {
                        data.row++;
                        string str = fun.Name.Substring(4);
                        int i = com.TextData.IndexOf(str);
                        if (i == -1)
                        {
                            com.TextData[n] = fun.ReturnType.Name;
                            com.TextData[n + 1] = str;
                            com.TextData[n + 2] = "ー";
                            com.TextData[n + 3] = "―";
                            n += 5;
                        }
                        else
                        {
                            com.TextData[i + 3] = "―";
                        }
                    }
                }

                foreach (var mems in fields)
                {
                    com.TextData[n] = mems.FieldType.Name;
                    com.TextData[n + 1] = mems.Name;
                    com.TextData[n + 2] = mems.IsPublic ? "―" : "ー";
                    com.TextData[n + 3] = mems.IsPublic ? "―" : "ー";
                    n +=5;
                }
                
                var container = DocRuntime.NewEmpty();
                if(com.TextData.Count !=0)
                    container.Add(DocEditor.CreateComponentField(com));
                show.Add(container);
            }
        }
    }
}
