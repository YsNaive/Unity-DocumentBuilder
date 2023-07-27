using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocEditFuncDisplay : DocEditVisual
    {
        public override string DisplayName => "FuncDisplay";

        public override string VisualID => "4";

        private VisualElement syntaxContainer, paramsContainer, returnTypeContainer;

        protected override void OnCreateGUI()
        {
            this.style.backgroundColor = DocStyle.Current.BackgroundColor;
            this.style.width = -1;
            DocFuncDisplay.Data data = setData(Target.JsonData, Target.TextData);
            this.Add(generateAnimVisual(data));
            TextField nameTextField = new TextField();
            nameTextField.label = "Name";
            nameTextField[0].style.minWidth = new Length(20, LengthUnit.Percent);
            nameTextField.value = data.Name + "";
            nameTextField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            nameTextField.style.SetIS_Style(DocStyle.Current.MainText);
            nameTextField.style.ClearMarginPadding();
            nameTextField.style.paddingLeft = Length.Percent(1);
            nameTextField.style.paddingRight = Length.Percent(1);
            nameTextField.RegisterValueChangedCallback(value =>
            {
                data.Name = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            TextField descriptionTextField = new TextField();
            descriptionTextField.label = "Description";
            descriptionTextField[0].style.minWidth = new Length(20, LengthUnit.Percent);
            descriptionTextField.value = Target.TextData[0] + "";
            descriptionTextField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            descriptionTextField.style.SetIS_Style(DocStyle.Current.MainText);
            descriptionTextField.style.ClearMarginPadding();
            descriptionTextField.style.paddingLeft = Length.Percent(1);
            descriptionTextField.style.paddingRight = Length.Percent(1);
            descriptionTextField.RegisterValueChangedCallback(value =>
            {
                data.ParamsDescription[0] = value.newValue;
                Target.TextData[0] = value.newValue;
            });
            this.Add(nameTextField);
            this.Add(descriptionTextField);
            syntaxContainer = generateSyntaxContainer(data);
            this.Add(syntaxContainer);
            paramsContainer = generateParamsContainer(data);
            this.Add(paramsContainer);
            returnTypeContainer = generateReturnTypeContainer(data);
            this.Add(returnTypeContainer);
        }

        public static DocComponent LoadMethod(MethodInfo methodInfo)
        {
            DocComponent doc = new DocComponent();
            doc.VisualID = "4";
            DocFuncDisplay.Data data = new DocFuncDisplay.Data();
            data.IntroAniMode = DocFuncDisplay.AniMode.Fade;
            data.OuttroAniMode = DocFuncDisplay.AniMode.Fade;
            data.IntroDuration = 300;
            data.OuttroDuration = 300;
            List<string> texts = new List<string>();
            data.Name = methodInfo.Name;
            texts.Add("");
            data.Syntaxs[0] = GetSignature(methodInfo);
            string typeName = getTypeName(methodInfo.ReturnType.Name);
            if (typeName != "void")
            {
                data.ReturnTypes[0] = getTypeName(methodInfo.ReturnType.Name);
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
                param.Type = getTypeName(parameters[i].ParameterType.Name);
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
            stringBuilder.Append(getTypeName(methodInfo.ReturnType.Name));
            stringBuilder.Append(" ");
            stringBuilder.Append(methodInfo.Name);
            stringBuilder.Append('(');

            ParameterInfo[] parameters = methodInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                stringBuilder.Append(getTypeName(parameters[i].ParameterType.Name));
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

        private static string getTypeName(string typeName)
        {
            switch (typeName)
            {
                case "Void":
                    return "void";
                case "Int32":
                    return "int";
                case "String":
                    return "string";
            }

            return typeName;
        }

        private VisualElement generateAnimVisual(DocFuncDisplay.Data data)
        {
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);
            root.style.paddingLeft = Length.Percent(1);
            root.style.paddingRight = Length.Percent(1);

            EnumField introField = new EnumField();
            introField.Init(DocFuncDisplay.AniMode.None);
            introField.value = data.IntroAniMode;
            introField.style.width = Length.Percent(25);
            introField.label = "Intro Mode";
            introField[0].style.minWidth = Length.Percent(50);
            introField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            introField.style.ClearMarginPadding();
            root.Add(introField);
            TextField introDurationField = new TextField();
            introDurationField.label = "IntroDuration";
            introDurationField.value = data.IntroDuration.ToString();
            introDurationField.style.width = Length.Percent(25);
            introDurationField[0].style.minWidth = Length.Percent(50);
            introDurationField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            introDurationField.visible = data.IntroAniMode != DocFuncDisplay.AniMode.None;
            introDurationField.style.ClearMarginPadding();
            root.Add(introDurationField);
            introField.RegisterValueChangedCallback(value =>
            {
                data.IntroAniMode = (DocFuncDisplay.AniMode)value.newValue;
                introDurationField.visible = data.IntroAniMode != DocFuncDisplay.AniMode.None;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            introDurationField.RegisterValueChangedCallback(value =>
            {
                if (int.TryParse(value.newValue, out int duration))
                {
                    data.IntroDuration = duration;
                    Target.JsonData = JsonUtility.ToJson(data);
                }
            });
            EnumField outtroField = new EnumField();
            outtroField.Init(DocFuncDisplay.AniMode.None);
            outtroField.label = "Outtro Mode";
            outtroField.value = data.IntroAniMode;
            outtroField.style.width = Length.Percent(25);
            outtroField[0].style.minWidth = Length.Percent(50);
            outtroField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            outtroField.style.ClearMarginPadding();
            root.Add(outtroField);
            TextField outtroDurationField = new TextField();
            outtroDurationField.label = "OuttroDuration";
            outtroDurationField.value = data.OuttroDuration.ToString();
            outtroDurationField.style.width = Length.Percent(25);
            outtroDurationField[0].style.minWidth = Length.Percent(50);
            outtroDurationField[1].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            outtroDurationField.visible = data.OuttroAniMode != DocFuncDisplay.AniMode.None;
            outtroDurationField.style.ClearMarginPadding();
            root.Add(outtroDurationField);
            outtroField.RegisterValueChangedCallback(value =>
            {
                data.OuttroAniMode = (DocFuncDisplay.AniMode)value.newValue;
                outtroDurationField.visible = data.OuttroAniMode != DocFuncDisplay.AniMode.None;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            outtroDurationField.RegisterValueChangedCallback(value =>
            {
                if (int.TryParse(value.newValue, out int duration))
                {
                    data.OuttroDuration = duration;
                    Target.JsonData = JsonUtility.ToJson(data);
                }
            });

            return root;
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
            Target.JsonData = JsonUtility.ToJson(data);
            return data;
        }

        private VisualElement generateAddDeleteButton()
        {
            VisualElement root = new VisualElement();
            root.style.SetIS_Style(ISFlex.Horizontal);

            Button addButton = new Button();
            addButton.text = "+";
            addButton.style.backgroundColor = DocStyle.Current.SuccessColor;
            addButton.style.width = Length.Percent(10);
            addButton.style.ClearMarginPadding();
            addButton.style.SetIS_Style(new ISMargin(TextAnchor.MiddleRight));
            addButton.style.SetIS_Style(DocStyle.Current.MainText);
            addButton.style.unityTextAlign = TextAnchor.MiddleCenter;

            Button deleteButton = new Button();
            deleteButton.text = "-";
            deleteButton.style.backgroundColor = DocStyle.Current.DangerColor;
            deleteButton.style.width = Length.Percent(10);
            deleteButton.style.ClearMarginPadding();
            deleteButton.style.marginRight = Length.Percent(1);
            deleteButton.style.SetIS_Style(DocStyle.Current.MainText);
            deleteButton.style.unityTextAlign = TextAnchor.MiddleCenter;

            root.Add(addButton);
            root.Add(deleteButton);

            return root;
        }

        private VisualElement generateSyntaxVisual(DocFuncDisplay.Data data, int index)
        { 
            TextField syntaxField = new TextField();
            syntaxField[0].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            syntaxField.value = data.Syntaxs[index];
            syntaxField.RegisterValueChangedCallback(value =>
            {
                data.Syntaxs[index] = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            syntaxField.style.SetIS_Style(DocStyle.Current.MainText);
            syntaxField.style.ClearMarginPadding();
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

            return root;
        }

        private VisualElement generateSyntaxContainer(DocFuncDisplay.Data data)
        {
            VisualElement root = new VisualElement();
            VisualElement veSyntax = generateSyntax(data);

            Label label = new Label();
            label.text = "Syntaxs";
            label.style.SetIS_Style(DocStyle.Current.MainText);
            label.style.ClearMarginPadding();
            label.style.paddingLeft = Length.Percent(1);
            label.style.paddingRight = Length.Percent(1);
            root.Add(label);

            VisualElement veAddDelete = generateAddDeleteButton();

            ((Button)veAddDelete[0]).clicked += () =>
            {
                data.Syntaxs.Add("");
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
            VisualElement ve = new VisualElement();
            ve.style.SetIS_Style(ISFlex.Horizontal);
            ve.style.height = 20;
            float percent = 50;
            TextField typeField = new TextField();
            typeField.label = "Type";
            typeField[0].style.minWidth = new Length(20, LengthUnit.Percent);
            typeField.style.width = Length.Percent(percent);
            typeField.value = data.Params[index].Type + "";
            typeField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            typeField.style.SetIS_Style(DocStyle.Current.MainText);
            typeField.style.ClearMarginPadding();
            typeField.style.paddingLeft = Length.Percent(1);
            typeField.RegisterValueChangedCallback(value =>
            {
                data.Params[index].Type = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            TextField nameField = new TextField();
            nameField.label = "Name";
            nameField[0].style.minWidth = new Length(20, LengthUnit.Percent);
            nameField.style.width = Length.Percent(percent);
            nameField.value = data.Params[index].ParamName + "";
            nameField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            nameField.style.SetIS_Style(DocStyle.Current.MainText);
            nameField.style.ClearMarginPadding();
            nameField.style.paddingLeft = Length.Percent(1);
            nameField.style.paddingRight = Length.Percent(1);
            nameField.RegisterValueChangedCallback(value =>
            {
                data.Params[index].ParamName = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            ve.Add(typeField);
            ve.Add(nameField);
            root.Add(ve);

            TextField descriptionField = new TextField();
            descriptionField.value = data.ParamsDescription[index] + "";
            descriptionField.multiline = true;
            descriptionField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            descriptionField.style.SetIS_Style(DocStyle.Current.MainText);
            descriptionField.style.ClearMarginPadding();
            descriptionField.style.paddingLeft = Length.Percent(1);
            descriptionField.style.paddingRight = Length.Percent(1);
            descriptionField.RegisterValueChangedCallback(value =>
            {
                data.ParamsDescription[index] = value.newValue;
                Target.TextData[1 + index] = value.newValue;
            });
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

            VisualElement veParams = generateParams(data);

            Label label = new Label();
            label.text = "Params";
            label.style.SetIS_Style(DocStyle.Current.MainText);
            label.style.ClearMarginPadding();
            label.style.paddingLeft = Length.Percent(1);
            label.style.paddingRight = Length.Percent(1);
            root.Add(label);

            VisualElement veAddDelete = generateAddDeleteButton();

            ((Button)veAddDelete[0]).clicked += () =>
            {
                Target.TextData.Insert(1 + data.Params.Count, "");
                data.AddNewParams();
                VisualElement ve = generateParamsVisual(data, data.Params.Count - 1);
                veParams.Add(ve);
                Target.JsonData = JsonUtility.ToJson(data);
            };
            ((Button)veAddDelete[1]).clicked += () =>
            {
                if (data.Params.Count == 0)
                    return;
                Target.TextData.RemoveAt(data.Params.Count);
                veParams.RemoveAt(veParams.childCount - 1);
                data.RemoveLastParams();
                Target.JsonData = JsonUtility.ToJson(data);
            };

            root.Add(veParams);
            root.Add(veAddDelete);

            return root;
        }

        private VisualElement generateReturnTypeVisual(DocFuncDisplay.Data data, int index)
        {
            VisualElement root = new VisualElement();
            TextField typeField = new TextField();
            typeField.label = "ReturnType";
            typeField[0].style.minWidth = new Length(20, LengthUnit.Percent);
            typeField.style.height = 20;
            typeField.value = data.ReturnTypes[index] + "";
            typeField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            typeField.style.SetIS_Style(DocStyle.Current.MainText);
            typeField.style.ClearMarginPadding();
            typeField.style.paddingLeft = Length.Percent(1);
            typeField.style.paddingRight = Length.Percent(1);
            typeField.RegisterValueChangedCallback(value =>
            {
                data.ReturnTypes[index] = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            TextField descriptionField = new TextField();
            descriptionField.value = data.ReturnTypesDescription[index] + "";
            descriptionField.multiline = true;
            descriptionField.Q("unity-text-input").style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            descriptionField.style.SetIS_Style(DocStyle.Current.MainText);
            descriptionField.style.ClearMarginPadding();
            descriptionField.style.paddingLeft = Length.Percent(1);
            descriptionField.style.paddingRight = Length.Percent(1);
            descriptionField.RegisterValueChangedCallback(value =>
            {
                data.ReturnTypesDescription[index] = value.newValue;
                Target.TextData[1 + data.Params.Count + index] = value.newValue;
            });

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

            Label label = new Label();
            label.text = "ReturnTypes";
            label.style.SetIS_Style(DocStyle.Current.MainText);
            label.style.ClearMarginPadding();
            label.style.paddingLeft = Length.Percent(1);
            label.style.paddingRight = Length.Percent(1);
            root.Add(label);

            VisualElement veReturnTypes = generateReturnType(data);

            VisualElement veAddDelete = generateAddDeleteButton();

            ((Button)veAddDelete[0]).clicked += () =>
            {
                Target.TextData.Insert(1 + data.Params.Count + data.ReturnTypes.Count, "");
                data.AddNewReturnType();
                VisualElement ve = generateReturnTypeVisual(data, data.ReturnTypes.Count - 1);
                veReturnTypes.Add(ve);
                Target.JsonData = JsonUtility.ToJson(data);
            };
            ((Button)veAddDelete[1]).clicked += () =>
            {
                if (data.ReturnTypes.Count == 0)
                    return;
                Target.TextData.RemoveAt(data.Params.Count + data.ReturnTypes.Count);
                veReturnTypes.RemoveAt(veReturnTypes.childCount - 1);
                data.RemoveLastReturnType();
                Target.JsonData = JsonUtility.ToJson(data);
            };

            root.Add(veReturnTypes);
            root.Add(veAddDelete);

            return root;
        }
    }
}
