using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomDocEditVisual("Advance/FuncDisplay")]
    public class DocEditFuncDisplay : DocEditVisual<DocFuncDisplay.Data>
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

            init();
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(20));
            nameTextField = new DSTextField("Name", value =>
            {
                visualData.Name = value.newValue;
                SaveDataToTarget();
            });
            nameTextField.value = visualData.Name;
            nameTextField.style.paddingLeft = Length.Percent(1);
            nameTextField.style.paddingRight = Length.Percent(1);
            TextField descriptionTextField = new DSTextField("Description", value =>
            {
                Target.TextData[0] = value.newValue;
            });
            descriptionTextField.multiline = true;
            descriptionTextField.value = Target.TextData[0];
            descriptionTextField.style.paddingLeft = Length.Percent(1);
            descriptionTextField.style.paddingRight = Length.Percent(1);
            this.Add(nameTextField);
            this.Add(descriptionTextField);
            DocStyle.Current.EndLabelWidth();
            syntaxContainer = generateSyntaxContainer();
            this.Add(syntaxContainer);
            paramsContainer = generateParamsContainer();
            this.Add(paramsContainer);
            returnTypeContainer = generateReturnTypeContainer();
            this.Add(returnTypeContainer);
        }

        public override string ToMarkdown(string dstPath)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string strName = $"<font color=#{ColorUtility.ToHtmlStringRGB(DocStyle.Current.FuncColor)}>{visualData.Name}</font>";
            stringBuilder.Append(strName).AppendLine("<br>");
            stringBuilder.Append(Target.TextData[0]).AppendLine("<br>");
            stringBuilder.Append("Syntaxs").AppendLine("<br>");
            foreach (string str in visualData.Syntaxs)
                stringBuilder.Append("&emsp;").Append(DocumentBuilderParser.ParseMethodSyntax(str, true)).AppendLine("<br>");
            if (visualData.Params.Count > 0)
                stringBuilder.AppendLine("Parameters<br>");
            for (int i = 0;i < visualData.Params.Count; i++)
            {
                string strParam = 
                    $"<font color=#{ColorUtility.ToHtmlStringRGB(DocStyle.Current.TypeColor)}>{visualData.Params[i].Type}</font>&emsp;{visualData.Params[i].ParamName}";
                stringBuilder.Append("&emsp;").Append(strParam).AppendLine("<br>");
                stringBuilder.Append("&emsp;&emsp;").Append(Target.TextData[1 + i]).AppendLine("<br>");
            }
            if (visualData.ReturnTypes.Count > 0)
                stringBuilder.Append("Return Values").AppendLine("<br>");
            for (int i = 0; i < visualData.ReturnTypes.Count; i++)
            {
                string strReturnType = $"<font color=#{ColorUtility.ToHtmlStringRGB(DocStyle.Current.TypeColor)}>{visualData.ReturnTypes[i]}</font>";
                stringBuilder.Append("&emsp;Type : ").Append(strReturnType).AppendLine("<br>");
                if (i == visualData.ReturnTypes.Count - 1)
                    stringBuilder.Append("&emsp;&emsp;").Append(Target.TextData[1 + visualData.Params.Count + i]);
                else
                    stringBuilder.Append("&emsp;&emsp;").Append(Target.TextData[1 + visualData.Params.Count + i]).AppendLine("<br>");
            }

            return stringBuilder.ToString();
        }

        private void init()
        {
            if (visualData.Syntaxs.Count == 0 || visualData.Syntaxs[0] == "")
            {
                Target.TextData.FromList(visualData.GetTexts());
            }
            else
            {
                var list = Target.TextData.ToList();
                visualData.SetParamsDescription(list, 1, visualData.Params.Count + 1);
                visualData.SetReturnTypesDescription(list, 1 + visualData.Params.Count, 1 + visualData.Params.Count + visualData.ReturnTypes.Count);
            }
            for (int i = 0; i < visualData.Syntaxs.Count; i++)
                funcDatas.Add(new DocumentBuilderParser.FuncData());
            SaveDataToTarget();
        }

        private void setFuncData()
        {
            List<string> paramNames = new List<string>();
            List<string> returnNames = new List<string>();
            for (int i = 0; i < visualData.Params.Count; i++)
                paramNames.Add(visualData.Params[i].ParamName);
            for (int i = 0; i < visualData.ReturnTypes.Count; i++)
                returnNames.Add(visualData.ReturnTypes[i]);
            string description = Target.TextData[0];
            Target.TextData.Clear();
            Target.TextData.Add(description);
            visualData.Params.Clear();
            visualData.ReturnTypes.Clear();
            if (funcDatas.Count > 0)
            {
                visualData.Name = funcDatas[0].Name;
                nameTextField.value = funcDatas[0].Name;
            }
            for (int i = 0; i < funcDatas.Count; i++)
            {
                DocFuncDisplay.ParamData paramData;
                for (int j = 0; j < funcDatas[i].paramsName.Count; j++)
                {
                    paramData = new DocFuncDisplay.ParamData(funcDatas[i].paramsName[j], funcDatas[i].paramsType[j]);
                    if (visualData.Params.Contains(paramData))
                    {
                        continue;
                    }
                    if (paramNames.Contains(paramData.ParamName))
                    {
                        visualData.Params.Add(paramData);
                        Target.TextData.Add(visualData.ParamsDescription[paramNames.IndexOf(paramData.ParamName)]);
                    }
                    else
                    {
                        visualData.Params.Add(paramData);
                        Target.TextData.Add("");
                    }
                }
            }
            for (int i = 0; i < funcDatas.Count; i++)
            {
                if (funcDatas[i].ReturnType == "" || funcDatas[i].ReturnType == "void" ||
                    visualData.ReturnTypes.Contains(funcDatas[i].ReturnType))
                    continue;
                if (returnNames.Contains(funcDatas[i].ReturnType))
                {
                    visualData.ReturnTypes.Add(funcDatas[i].ReturnType);
                    Target.TextData.Add(visualData.ReturnTypesDescription[returnNames.IndexOf(funcDatas[i].ReturnType)]);
                }
                else
                {
                    visualData.ReturnTypes.Add(funcDatas[i].ReturnType);
                    Target.TextData.Add("");
                }
            }
            visualData.ParamsDescription.Clear();
            for (int i = 0; i < visualData.Params.Count; i++)
            {
                visualData.ParamsDescription.Add(Target.TextData[1 + i]);
            }
            visualData.ReturnTypesDescription.Clear();
            for (int i = 0; i < visualData.ReturnTypes.Count; i++)
            {
                visualData.ReturnTypesDescription.Add(Target.TextData[1 + visualData.Params.Count + i]);
            }
        }

        private VisualElement generateAddDeleteButton()
        {
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);

            Button addButton = new DSButton("+", DocStyle.Current.SuccessColor);
            addButton.style.width = Length.Percent(10);
            addButton.style.ClearMarginPadding();
            addButton.style.SetIS_Style(new ISMargin(TextAnchor.MiddleRight));

            Button deleteButton = new DSButton("-", DocStyle.Current.DangerColor);
            deleteButton.style.width = Length.Percent(10);
            deleteButton.style.marginRight = Length.Percent(1);

            root.Add(addButton);
            root.Add(deleteButton);

            return root;
        }

        private VisualElement generateSyntaxVisual(int index)
        {
            TextField syntaxField = new DSTextField("", value =>
            {
                visualData.Syntaxs[index] = value.newValue;
                DocumentBuilderParser.FuncData funcData = new DocumentBuilderParser.FuncData(value.newValue);
                funcDatas[index] = funcData;
                setFuncData();
                if (this.Contains(paramsContainer))
                    this.Remove(paramsContainer);
                if (this.Contains(returnTypeContainer))
                    this.Remove(returnTypeContainer);
                paramsContainer = generateParamsContainer();
                this.Add(paramsContainer);
                returnTypeContainer = generateReturnTypeContainer();
                this.Add(returnTypeContainer);
                SaveDataToTarget();
            });
            syntaxField.value = visualData.Syntaxs[index];
            DocumentBuilderParser.FuncData funcData = new DocumentBuilderParser.FuncData(visualData.Syntaxs[index]);
            funcDatas[index] = funcData;
            syntaxField.style.paddingLeft = Length.Percent(1);
            syntaxField.style.paddingRight = Length.Percent(1);

            return syntaxField;
        }

        private VisualElement generateSyntax()
        {
            VisualElement root = new VisualElement();

            for (int i = 0; i < visualData.Syntaxs.Count; i++)
            {
                VisualElement syntaxField = generateSyntaxVisual(i);
                root.Add(syntaxField);
            }

            setFuncData();

            return root;
        }

        private VisualElement generateSyntaxContainer()
        {
            VisualElement root = new VisualElement();
            VisualElement veSyntax = generateSyntax();

            DSTextElement label = new DSTextElement("Syntaxs");
            label.style.paddingLeft = Length.Percent(1);
            label.style.paddingRight = Length.Percent(1);
            root.Add(label);

            VisualElement veAddDelete = generateAddDeleteButton();

            ((Button)veAddDelete[0]).clicked += () =>
            {
                visualData.Syntaxs.Add("");
                funcDatas.Add(new DocumentBuilderParser.FuncData());
                VisualElement ve = generateSyntaxVisual(visualData.Syntaxs.Count - 1);
                veSyntax.Add(ve);
                SaveDataToTarget();
            };
            ((Button)veAddDelete[1]).clicked += () =>
            {
                if (visualData.Syntaxs.Count == 0)
                    return;
                visualData.Syntaxs.RemoveAt(visualData.Syntaxs.Count - 1);
                veSyntax.RemoveAt(veSyntax.childCount - 1);
                SaveDataToTarget();
            };


            root.Add(veSyntax);
            root.Add(veAddDelete);

            return root;
        }

        private VisualElement generateParamsVisual(int index)
        {
            VisualElement root = new VisualElement();
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(20));
            TextField typeField = new DSTextField("Type", value =>
            {
                visualData.Params[index].Type = value.newValue;
                SaveDataToTarget();
            });
            typeField.value = visualData.Params[index].Type;
            typeField.focusable = false;
            typeField.style.paddingLeft = Length.Percent(1);
            TextField nameField = new DSTextField("Name", value =>
            {
                visualData.Params[index].ParamName = value.newValue;
                SaveDataToTarget();
            });
            nameField.value = visualData.Params[index].ParamName;
            nameField.focusable = false;
            nameField.style.paddingLeft = Length.Percent(1);
            nameField.style.paddingRight = Length.Percent(1);
            root.Add(new DSHorizontal(typeField, nameField));

            TextField descriptionField = new DSTextField("", value =>
            {
                visualData.ParamsDescription[index] = value.newValue;
                Target.TextData[1 + index] = value.newValue;
            });
            descriptionField.value = visualData.ParamsDescription[index];
            descriptionField.multiline = true;
            descriptionField.style.paddingLeft = Length.Percent(1);
            descriptionField.style.paddingRight = Length.Percent(1);
            root.Add(descriptionField);
            return root;
        }

        private VisualElement generateParams()
        {
            VisualElement root = new VisualElement();

            for (int i = 0; i < visualData.Params.Count; i++)
            {
                VisualElement ve = generateParamsVisual(i);
                root.Add(ve);
            }

            return root;
        }

        private VisualElement generateParamsContainer()
        {
            VisualElement root = new VisualElement();

            if (visualData.Params.Count == 0)
                return root;

            VisualElement veParams = generateParams();

            Label label = new DSLabel("Params");
            label.style.paddingLeft = Length.Percent(1);
            label.style.paddingRight = Length.Percent(1);
            root.Add(label);

            root.Add(veParams);

            return root;
        }

        private VisualElement generateReturnTypeVisual(int index)
        {
            VisualElement root = new VisualElement();
            DocStyle.Current.BeginLabelWidth(ISLength.Percent(20));
            TextField typeField = new DSTextField("ReturnType", value =>
            {
                visualData.ReturnTypes[index] = value.newValue;
                SaveDataToTarget();
            });
            typeField.style.height = 20;
            typeField.value = visualData.ReturnTypes[index];
            typeField.focusable = false;
            typeField.style.paddingLeft = Length.Percent(1);
            typeField.style.paddingRight = Length.Percent(1);
            TextField descriptionField = new DSTextField("", value =>
            {
                visualData.ReturnTypesDescription[index] = value.newValue;
                Target.TextData[1 + visualData.Params.Count + index] = value.newValue;
            });
            descriptionField.value = visualData.ReturnTypesDescription[index];
            descriptionField.multiline = true;
            descriptionField.style.paddingLeft = Length.Percent(1);
            descriptionField.style.paddingRight = Length.Percent(1);

            root.Add(typeField);
            root.Add(descriptionField);

            return root;
        }

        private VisualElement generateReturnType()
        {
            VisualElement root = new VisualElement();

            for (int i = 0; i < visualData.ReturnTypes.Count; i++)
            {
                VisualElement ve = generateReturnTypeVisual(i);
                root.Add(ve);
            }

            return root;
        }

        private VisualElement generateReturnTypeContainer()
        {
            VisualElement root = new VisualElement();

            if (visualData.ReturnTypes.Count == 0)
                return root;
            Label label = new DSLabel("ReturnTypes");
            label.style.paddingLeft = Length.Percent(1);
            label.style.paddingRight = Length.Percent(1);
            root.Add(label);

            VisualElement veReturnTypes = generateReturnType();

            root.Add(veReturnTypes);

            return root;
        }
    }
}
