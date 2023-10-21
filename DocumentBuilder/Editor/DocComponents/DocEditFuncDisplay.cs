using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomDocEditVisual("Advance/FuncDisplay")]
    public class DocEditFuncDisplay : DocEditVisual
    {
        [Obsolete] public override string DisplayName => "FuncDisplay";

        public override string VisualID => "4";

        private VisualElement syntaxContainer, paramsContainer, returnTypeContainer;
        private TextField nameTextField;
        private List<DocumentBuilderParser.FuncData> funcDatas = new List<DocumentBuilderParser.FuncData>();

        protected override void OnCreateGUI()
        {
            this.style.backgroundColor = DocStyle.Current.BackgroundColor;
            this.style.width = -1;
            DocFuncDisplay.Data data = setData(Target.JsonData, Target.TextData);
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(20));
            nameTextField = DocRuntime.NewTextField("Name", value =>
            {
                data.Name = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            nameTextField.label = "Name";
            nameTextField.value = data.Name + "";
            nameTextField.style.paddingLeft = Length.Percent(1);
            nameTextField.style.paddingRight = Length.Percent(1);
            TextField descriptionTextField = DocRuntime.NewTextField("Description", value =>
            {
                Target.TextData[0] = value.newValue;
            });
            descriptionTextField.label = "Description";
            descriptionTextField.multiline = true;
            descriptionTextField.value = Target.TextData[0] + "";
            descriptionTextField.style.paddingLeft = Length.Percent(1);
            descriptionTextField.style.paddingRight = Length.Percent(1);
            this.Add(nameTextField);
            this.Add(descriptionTextField);
            DocStyle.Current.EndLabelWidth();
            syntaxContainer = generateSyntaxContainer(data);
            this.Add(syntaxContainer);
            paramsContainer = generateParamsContainer(data);
            this.Add(paramsContainer);
            returnTypeContainer = generateReturnTypeContainer(data);
            this.Add(returnTypeContainer);
        }

        public override string ToMarkdown(string dstPath)
        {
            DocFuncDisplay.Data data = JsonUtility.FromJson<DocFuncDisplay.Data>(Target.JsonData);
            StringBuilder stringBuilder = new StringBuilder();
            string strName = $"<font color=#{ColorUtility.ToHtmlStringRGB(DocStyle.Current.FuncColor)}>{data.Name}</font>";
            stringBuilder.Append(strName).AppendLine("<br>");
            stringBuilder.Append(Target.TextData[0]).AppendLine("<br>");
            stringBuilder.Append("Syntaxs").AppendLine("<br>");
            foreach (string str in data.Syntaxs)
                stringBuilder.Append("&emsp;").Append(DocumentBuilderParser.FunctionParser(str, true)).AppendLine("<br>");
            if (data.Params.Count > 0)
                stringBuilder.AppendLine("Parameters<br>");
            for (int i = 0;i < data.Params.Count; i++)
            {
                string strParam = 
                    $"<font color=#{ColorUtility.ToHtmlStringRGB(DocStyle.Current.TypeColor)}>{data.Params[i].Type}</font>&emsp;{data.Params[i].ParamName}";
                stringBuilder.Append("&emsp;").Append(strParam).AppendLine("<br>");
                stringBuilder.Append("&emsp;&emsp;").Append(Target.TextData[1 + i]).AppendLine("<br>");
            }
            if (data.ReturnTypes.Count > 0)
                stringBuilder.Append("Return Values").AppendLine("<br>");
            for (int i = 0; i < data.ReturnTypes.Count; i++)
            {
                string strReturnType = $"<font color=#{ColorUtility.ToHtmlStringRGB(DocStyle.Current.TypeColor)}>{data.ReturnTypes[i]}</font>";
                stringBuilder.Append("&emsp;Type : ").Append(strReturnType).AppendLine("<br>");
                if (i == data.ReturnTypes.Count - 1)
                    stringBuilder.Append("&emsp;&emsp;").Append(Target.TextData[1 + data.Params.Count + i]);
                else
                    stringBuilder.Append("&emsp;&emsp;").Append(Target.TextData[1 + data.Params.Count + i]).AppendLine("<br>");
            }

            return stringBuilder.ToString();
        }

        public static DocComponent LoadMethod(MethodInfo methodInfo)
        {
            DocComponent doc = new DocComponent();
            doc.VisualID = "4";
            DocFuncDisplay.Data data = new DocFuncDisplay.Data();
            List<string> texts = new List<string>();
            data.Name = methodInfo.Name;
            texts.Add("");
            data.Syntaxs[0] = GetSignature(methodInfo);
            string typeName = DocumentBuilderParser.CalGenericTypeName(methodInfo.ReturnType);
            if (typeName != "void")
            {
                data.ReturnTypes.Add(DocumentBuilderParser.CalGenericTypeName(methodInfo.ReturnType));
                texts.Add("");
            }
            else
            {
                data.ReturnTypes.Clear();
            }
            data.Params = new List<DocFuncDisplay.ParamData>();
            ParameterInfo[] parameters = methodInfo.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                DocFuncDisplay.ParamData param = new DocFuncDisplay.ParamData();
                param.ParamName = parameters[i].Name;
                param.Type = DocumentBuilderParser.CalGenericTypeName(parameters[i].ParameterType);
                data.Params.Add(param);
                texts.Add("");
            }

            doc.JsonData = JsonUtility.ToJson(data);
            doc.TextData = texts;

            return doc;
        }

        public static string GetSignature(MethodInfo methodInfo)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(getAccessLevel(methodInfo));
            if (methodInfo.IsStatic)
                stringBuilder.Append(" static");
            stringBuilder.Append(" ");
            stringBuilder.Append(DocumentBuilderParser.CalGenericTypeName(methodInfo.ReturnType));
            stringBuilder.Append(" ");
            stringBuilder.Append(methodInfo.Name);
            stringBuilder.Append('(');

            ParameterInfo[] parameters = methodInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                stringBuilder.Append(DocumentBuilderParser.CalGenericTypeName(parameters[i].ParameterType));
                stringBuilder.Append(" ");
                stringBuilder.Append(parameters[i].Name);
                if (i != parameters.Length - 1)
                    stringBuilder.Append(", ");
            }
            stringBuilder.Append(")");

            return stringBuilder.ToString();
        }

        private static string getAccessLevel(MethodInfo methodInfo)
        {
            if (methodInfo.IsPublic)
            {
                return "public";
            }
            else if (methodInfo.IsFamily)
            {
                return "protected";
            }
            else if (methodInfo.IsPrivate)
            {
                return "private";
            }
            else if (methodInfo.IsAssembly)
            {
                return "internal";
            }
            else if (methodInfo.IsFamilyAndAssembly)
            {
                return "protected internal";
            }
            else if (methodInfo.IsFamilyOrAssembly)
            {
                return "protected internal";
            }

            return "";
        }

        private DocFuncDisplay.Data setData(string jsonData, List<string> texts)
        {
            DocFuncDisplay.Data data = JsonUtility.FromJson<DocFuncDisplay.Data>(jsonData);
            if (data == null)
            {
                data = new DocFuncDisplay.Data();
                Target.TextData.Clear();
                Target.TextData.Add("");
                Target.TextData.AddRange(data.getTexts());
            }
            else
            {
                data.SetParamsDescription(texts, 1, data.Params.Count + 1);
                data.SetReturnTypesDescription(texts, 1 + data.Params.Count, 1 + data.Params.Count + data.ReturnTypes.Count);
            }
            for (int i = 0; i < data.Syntaxs.Count; i++)
                funcDatas.Add(new DocumentBuilderParser.FuncData());
            Target.JsonData = JsonUtility.ToJson(data);
            return data;
        }

        private void setFuncData(DocFuncDisplay.Data data)
        {
            List<string> paramNames = new List<string>();
            List<string> returnNames = new List<string>();
            for (int i = 0; i < data.Params.Count; i++)
                paramNames.Add(data.Params[i].ParamName);
            for (int i = 0; i < data.ReturnTypes.Count; i++)
                returnNames.Add(data.ReturnTypes[i]);
            string description = Target.TextData[0];
            Target.TextData.Clear();
            Target.TextData.Add(description);
            data.Params.Clear();
            data.ReturnTypes.Clear();
            if (funcDatas.Count > 0)
            {
                data.Name = funcDatas[0].Name;
                nameTextField.value = funcDatas[0].Name;
            }
            for (int i = 0; i < funcDatas.Count; i++)
            {
                DocFuncDisplay.ParamData paramData;
                for (int j = 0; j < funcDatas[i].paramsName.Count; j++)
                {
                    paramData = new DocFuncDisplay.ParamData(funcDatas[i].paramsName[j], funcDatas[i].paramsType[j]);
                    if (data.Params.Contains(paramData))
                    {
                        continue;
                    }
                    if (paramNames.Contains(paramData.ParamName))
                    {
                        data.Params.Add(paramData);
                        Target.TextData.Add(data.ParamsDescription[paramNames.IndexOf(paramData.ParamName)]);
                    }
                    else
                    {
                        data.Params.Add(paramData);
                        Target.TextData.Add("");
                    }
                }
            }
            for (int i = 0; i < funcDatas.Count; i++)
            {
                if (funcDatas[i].ReturnType == "" || funcDatas[i].ReturnType == "void" || 
                    data.ReturnTypes.Contains(funcDatas[i].ReturnType))
                    continue;
                if (returnNames.Contains(funcDatas[i].ReturnType))
                {
                    data.ReturnTypes.Add(funcDatas[i].ReturnType);
                    Target.TextData.Add(data.ReturnTypesDescription[returnNames.IndexOf(funcDatas[i].ReturnType)]);
                }
                else
                {
                    data.ReturnTypes.Add(funcDatas[i].ReturnType);
                    Target.TextData.Add("");
                }
            }
            data.ParamsDescription.Clear();
            for (int i = 0; i < data.Params.Count; i++)
            {
                data.ParamsDescription.Add(Target.TextData[1 + i]);
            }
            data.ReturnTypesDescription.Clear();
            for (int i = 0; i < data.ReturnTypes.Count; i++)
            {
                data.ReturnTypesDescription.Add(Target.TextData[1 + data.Params.Count + i]);
            }
        }

        private VisualElement generateAddDeleteButton()
        {
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);

            Button addButton = DocRuntime.NewButton("+", DocStyle.Current.SuccessColor);
            addButton.style.width = Length.Percent(10);
            addButton.style.ClearMarginPadding();
            addButton.style.SetIS_Style(new ISMargin(TextAnchor.MiddleRight));

            Button deleteButton = DocRuntime.NewButton("-", DocStyle.Current.DangerColor);
            deleteButton.style.width = Length.Percent(10);
            deleteButton.style.marginRight = Length.Percent(1);

            root.Add(addButton);
            root.Add(deleteButton);

            return root;
        }

        private VisualElement generateSyntaxVisual(DocFuncDisplay.Data data, int index)
        {
            TextField syntaxField = DocRuntime.NewTextField("", value =>
            {
                data.Syntaxs[index] = value.newValue;
                DocumentBuilderParser.FuncData funcData = new DocumentBuilderParser.FuncData(value.newValue);
                funcDatas[index] = funcData;
                setFuncData(data);
                if (this.Contains(paramsContainer))
                    this.Remove(paramsContainer);
                if (this.Contains(returnTypeContainer))
                    this.Remove(returnTypeContainer);
                paramsContainer = generateParamsContainer(data);
                this.Add(paramsContainer);
                returnTypeContainer = generateReturnTypeContainer(data);
                this.Add(returnTypeContainer);
                Target.JsonData = JsonUtility.ToJson(data);
            });
            syntaxField.value = data.Syntaxs[index];
            DocumentBuilderParser.FuncData funcData = new DocumentBuilderParser.FuncData(data.Syntaxs[index]);
            funcDatas[index] = funcData;
            syntaxField.style.paddingLeft = Length.Percent(1);
            syntaxField.style.paddingRight = Length.Percent(1);

            return syntaxField;
        }

        private VisualElement generateSyntax(DocFuncDisplay.Data data)
        {
            VisualElement root = new VisualElement();

            for (int i = 0; i < data.Syntaxs.Count; i++)
            {
                VisualElement syntaxField = generateSyntaxVisual(data, i);
                root.Add(syntaxField);
            }

            setFuncData(data);

            return root;
        }

        private VisualElement generateSyntaxContainer(DocFuncDisplay.Data data)
        {
            VisualElement root = new VisualElement();
            VisualElement veSyntax = generateSyntax(data);

            Label label = DocRuntime.NewLabel("Syntaxs");
            label.style.paddingLeft = Length.Percent(1);
            label.style.paddingRight = Length.Percent(1);
            root.Add(label);

            VisualElement veAddDelete = generateAddDeleteButton();

            ((Button)veAddDelete[0]).clicked += () =>
            {
                data.Syntaxs.Add("");
                funcDatas.Add(new DocumentBuilderParser.FuncData());
                VisualElement ve = generateSyntaxVisual(data, data.Syntaxs.Count - 1);
                veSyntax.Add(ve);
                Target.JsonData = JsonUtility.ToJson(data);
            };
            ((Button)veAddDelete[1]).clicked += () =>
            {
                if (data.Syntaxs.Count == 0)
                    return;
                data.Syntaxs.RemoveAt(data.Syntaxs.Count - 1);
                veSyntax.RemoveAt(veSyntax.childCount - 1);
                Target.JsonData = JsonUtility.ToJson(data);
            };


            root.Add(veSyntax);
            root.Add(veAddDelete);

            return root;
        }

        private VisualElement generateParamsVisual(DocFuncDisplay.Data data, int index)
        {
            VisualElement root = new VisualElement();
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(20));
            TextField typeField = DocRuntime.NewTextField("Type", value =>
            {
                data.Params[index].Type = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            typeField.value = data.Params[index].Type + "";
            typeField.focusable = false;
            typeField.style.paddingLeft = Length.Percent(1);
            TextField nameField = DocRuntime.NewTextField("Name", value =>
            {
                data.Params[index].ParamName = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            nameField.value = data.Params[index].ParamName + "";
            nameField.focusable = false;
            nameField.style.paddingLeft = Length.Percent(1);
            nameField.style.paddingRight = Length.Percent(1);
            root.Add(DocRuntime.NewHorizontalBar(typeField, nameField));

            TextField descriptionField = DocRuntime.NewTextField("", value =>
            {
                data.ParamsDescription[index] = value.newValue;
                Target.TextData[1 + index] = value.newValue;
            });
            descriptionField.value = data.ParamsDescription[index] + "";
            descriptionField.multiline = true;
            descriptionField.style.paddingLeft = Length.Percent(1);
            descriptionField.style.paddingRight = Length.Percent(1);
            root.Add(descriptionField);
            return root;
        }

        private VisualElement generateParams(DocFuncDisplay.Data data)
        {
            VisualElement root = new VisualElement();

            for (int i = 0; i < data.Params.Count; i++)
            {
                VisualElement ve = generateParamsVisual(data, i);
                root.Add(ve);
            }

            return root;
        }

        private VisualElement generateParamsContainer(DocFuncDisplay.Data data)
        {
            VisualElement root = new VisualElement();

            if (data.Params.Count == 0)
                return root;

            VisualElement veParams = generateParams(data);

            Label label = DocRuntime.NewLabel("Params");
            label.style.paddingLeft = Length.Percent(1);
            label.style.paddingRight = Length.Percent(1);
            root.Add(label);

            root.Add(veParams);

            return root;
        }

        private VisualElement generateReturnTypeVisual(DocFuncDisplay.Data data, int index)
        {
            VisualElement root = new VisualElement();
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(20));
            TextField typeField = DocRuntime.NewTextField("ReturnType", value =>
            {
                data.ReturnTypes[index] = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            typeField.style.height = 20;
            typeField.value = data.ReturnTypes[index] + "";
            typeField.focusable = false;
            typeField.style.paddingLeft = Length.Percent(1);
            typeField.style.paddingRight = Length.Percent(1);
            TextField descriptionField = DocRuntime.NewTextField("", value =>
            {
                data.ReturnTypesDescription[index] = value.newValue;
                Target.TextData[1 + data.Params.Count + index] = value.newValue;
            });
            descriptionField.value = data.ReturnTypesDescription[index] + "";
            descriptionField.multiline = true;
            descriptionField.style.paddingLeft = Length.Percent(1);
            descriptionField.style.paddingRight = Length.Percent(1);

            root.Add(typeField);
            root.Add(descriptionField);

            return root;
        }

        private VisualElement generateReturnType(DocFuncDisplay.Data data)
        {
            VisualElement root = new VisualElement();

            for (int i = 0; i < data.ReturnTypes.Count; i++)
            {
                VisualElement ve = generateReturnTypeVisual(data, i);
                root.Add(ve);
            }

            return root;
        }

        private VisualElement generateReturnTypeContainer(DocFuncDisplay.Data data)
        {
            VisualElement root = new VisualElement();

            if (data.ReturnTypes.Count == 0)
                return root;
            Label label = DocRuntime.NewLabel("ReturnTypes");
            label.style.paddingLeft = Length.Percent(1);
            label.style.paddingRight = Length.Percent(1);
            root.Add(label);

            VisualElement veReturnTypes = generateReturnType(data);

            root.Add(veReturnTypes);

            return root;
        }
    }
}
