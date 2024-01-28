using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocFuncDisplay : DocVisual
    {
        static DocFuncDisplay()
        {
            DocStyle.OnStyleChanged += (newStyle) =>
            {
                funcNameTextStyle = new ISText() { FontStyle = FontStyle.Bold, Color = DocStyle.Current.FuncColor, FontSize = DocStyle.Current.MainTextSize };
                paramTextStyle = new ISText() { FontStyle = FontStyle.BoldAndItalic, Color = DocStyle.Current.ArgsColor, FontSize = DocStyle.Current.MainTextSize };
                typeTextStyle = new ISText() { Color = DocStyle.Current.TypeColor, FontSize = DocStyle.Current.MainTextSize };
                labelTextStyle = new ISText() { Color = DocStyle.Current.SubFrontgroundColor, FontSize = DocStyle.Current.MainTextSize };
            };
        }
        public override string VisualID => "4";

        private static ISText funcNameTextStyle = new ISText() { FontStyle = FontStyle.Bold, Color = DocStyle.Current.FuncColor, FontSize = DocStyle.Current.MainTextSize };
        private static ISText paramTextStyle = new ISText() { FontStyle = FontStyle.BoldAndItalic, Color = DocStyle.Current.ArgsColor, FontSize = DocStyle.Current.MainTextSize };
        private static ISText typeTextStyle = new ISText() { Color = DocStyle.Current.TypeColor, FontSize = DocStyle.Current.MainTextSize };
        private static ISText labelTextStyle = new ISText() { Color = DocStyle.Current.SubFrontgroundColor, FontSize = DocStyle.Current.MainTextSize };
        private float tabGap = 2;

        protected override void OnCreateGUI()
        {
            Data data = JsonUtility.FromJson<Data>(Target.JsonData);
            if (data == null) return;
            if (Target.TextData.Count == 0)
            {
                Add(Create(DocDescription.CreateComponent("DocFuncDisplay wrong format", DocDescription.DescriptionType.Warning)));
                return;
            }
            TextElement nameText = new DSTextElement(data.Name);
            nameText.style.SetIS_Style(funcNameTextStyle);
            VisualElement child = new VisualElement();
            this.Add(nameText);
            if (Target.TextData[0] != "")
            {
                TextElement descriptionText = new DSTextElement(Target.TextData[0]);
                descriptionText.style.SetIS_Style(DocStyle.Current.MainText);
                descriptionText.style.color = DocStyle.Current.CodeTextColor;
                this.Add(descriptionText);
            }
            child.Add(generateSyntaxContainer(data));
            child.Add(generateParamContainer(data));
            child.Add(generateReturnTypeContainer(data));
            this.style.SetIS_Style(ISPadding.Pixel(DocStyle.Current.MainTextSize));
            this.style.backgroundColor = DocStyle.Current.CodeBackgroundColor;
            this.Add(child);
        }

        private VisualElement generateSyntaxContainer(Data data)
        {
            VisualElement root = new VisualElement();

            if (data.Syntaxs.Count == 0)
                return root;

            TextElement syntaxLabel = new TextElement();
            syntaxLabel.text = "Syntaxs";
            syntaxLabel.style.SetIS_Style(labelTextStyle);
            syntaxLabel.style.ClearMarginPadding();
            syntaxLabel.style.marginTop = DocStyle.Current.MainTextSize / 2f;
            root.Add(syntaxLabel);

            for (int i = 0;i < data.Syntaxs.Count; i++)
            {
                TextElement syntaxText = new TextElement();
                syntaxText.text = DocumentBuilderParser.ParseMethodSyntax(data.Syntaxs[i], false);
                syntaxText.style.ClearMarginPadding();
                syntaxText.style.paddingLeft = Length.Percent(tabGap);
                syntaxText.style.SetIS_Style(DocStyle.Current.MainText);
                syntaxText.style.color = DocStyle.Current.CodeTextColor;
                root.Add(syntaxText);
            }

            return root;
        }

        private VisualElement generateParamVisual(ParamData data, string description) 
        {
            VisualElement root = new VisualElement();

            TextElement paramNameText = new TextElement();
            paramNameText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(DocStyle.Current.TypeColor)}>{data.Type + " "}</color>" + data.ParamName;
            paramNameText.style.SetIS_Style(paramTextStyle);
            paramNameText.style.ClearMarginPadding();
            paramNameText.style.paddingLeft = Length.Percent(tabGap);
            root.Add(paramNameText);
            if (description != "")
            {
                TextElement descriptionText = new TextElement();
                descriptionText.text = description;
                descriptionText.style.SetIS_Style(DocStyle.Current.MainText);
                descriptionText.style.color = DocStyle.Current.CodeTextColor;
                descriptionText.style.ClearMarginPadding();
                descriptionText.style.paddingLeft = Length.Percent(2 * tabGap);
                root.Add(descriptionText);
            }

            return root;
        }

        private VisualElement generateParamContainer(Data data)
        {
            VisualElement root = new VisualElement();

            if (data.Params.Count == 0)
                return root;

            TextElement paramText = new TextElement();
            paramText.text = "Parameters";
            paramText.style.SetIS_Style(labelTextStyle);
            paramText.style.ClearMarginPadding();
            paramText.style.marginTop = DocStyle.Current.MainTextSize / 2f;
            root.Add(paramText);

            for (int i = 0;i < data.Params.Count; i++)
            {
                while (Target.TextData.Count < (2 + i)) Target.TextData.Add("");
                root.Add(generateParamVisual(data.Params[i], Target.TextData[1 + i]));
            }

            return root;
        }

        private VisualElement generateReturnTypeVisual(string returnType, string description)
        {
            VisualElement root = new VisualElement();
            TextElement returnTypeText = new TextElement();
            returnTypeText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(DocStyle.Current.CodeTextColor)}>Type: </color>" + returnType;
            returnTypeText.style.SetIS_Style(typeTextStyle); 
            returnTypeText.style.ClearMarginPadding();
            returnTypeText.style.paddingLeft = Length.Percent(tabGap);
            root.Add(returnTypeText);
            if (description != "")
            {
                TextElement descriptionText = new TextElement();
                descriptionText.text = description;
                descriptionText.style.SetIS_Style(DocStyle.Current.MainText);
                descriptionText.style.color = DocStyle.Current.CodeTextColor;
                descriptionText.style.ClearMarginPadding();
                descriptionText.style.paddingLeft = Length.Percent(tabGap);
                root.Add(descriptionText);
            }

            return root;
        }

        private VisualElement generateReturnTypeContainer(Data data)
        {
            VisualElement root = new VisualElement();

            if (data.ReturnTypes.Count == 0)
                return root;

            TextElement returnText = new TextElement();
            returnText.text = "Return Values";
            returnText.style.SetIS_Style(labelTextStyle);
            returnText.style.ClearMarginPadding();
            returnText.style.marginTop = DocStyle.Current.MainTextSize / 2f;
            root.Add(returnText);
            for (int i = 0; i < data.ReturnTypes.Count; i++)
            {
                root.Add(generateReturnTypeVisual(data.ReturnTypes[i], Target.TextData[1 + data.Params.Count + i]));
            }

            return root;
        }
        public static DocComponent CreateComponent(MethodBase methodInfo)
        {
            DocComponent doc = new DocComponent();
            doc.VisualID = "4";
            Data data = new Data();
            List<string> texts = new List<string>();
            data.Name = methodInfo.Name;
            texts.Add("");
            data.Syntaxs[0] = TypeReader.GetSignature(methodInfo);
            string typeName = "";
            if (methodInfo is MethodInfo)
            {
                typeName = TypeReader.GetName(((MethodInfo)methodInfo).ReturnType);
            }
            else if (methodInfo is ConstructorInfo)
            {
                typeName = TypeReader.GetName(((ConstructorInfo)methodInfo).DeclaringType);
            }

            if (typeName != "void")
            {
                data.ReturnTypes.Add(typeName);
                texts.Add("");
            }
            else
            {
                data.ReturnTypes.Clear();
            }
            data.Params = new List<ParamData>();
            ParameterInfo[] parameters = methodInfo.GetParameters();

            for (int i = 0; i < parameters.Length; i++)
            {
                ParamData param = new ParamData();
                param.ParamName = parameters[i].Name;
                param.Type = TypeReader.GetName(parameters[i].ParameterType);
                data.Params.Add(param);
                texts.Add("");
            }

            doc.JsonData = JsonUtility.ToJson(data);
            doc.TextData = texts;

            return doc;
        }

        public static DocComponent CreateComponent(IEnumerable<MethodBase> methodInfos)
        {
            DocComponent doc = new DocComponent();
            doc.VisualID = "4";
            Data data = new Data();
            data.Syntaxs.Clear();
            bool first = true;
            foreach (MethodBase methodBase in methodInfos)
            {
                if (first)
                {
                    data.Name = methodBase.Name;
                    first = false;
                }
                else if (methodBase.Name != data.Name)
                {
                    Debug.LogException(new Exception("DocEditFuncDisplay: Cannot create docComponent with different named method."));
                    return DocDescription.CreateComponent("DocEditFuncDisplay: Cannot create docComponent with different named method.", DocDescription.DescriptionType.Danger);
                }
                data.Syntaxs.Add(TypeReader.GetSignature(methodBase));
                string typeName = "";
                if (methodBase is MethodInfo)
                {
                    typeName = TypeReader.GetName(((MethodInfo)methodBase).ReturnType);
                }
                else if (methodBase is ConstructorInfo)
                {
                    typeName = TypeReader.GetName(((ConstructorInfo)methodBase).DeclaringType);
                }

                if (typeName != "void")
                {
                    data.AddNewReturnType(typeName);
                }
                ParameterInfo[] parameters = methodBase.GetParameters();
                foreach (var parameter in parameters)
                {
                    ParamData param = new ParamData();
                    param.ParamName = parameter.Name;
                    param.Type = TypeReader.GetName(parameter.ParameterType);
                    data.AddNewParams(param);
                }
            }


            doc.JsonData = JsonUtility.ToJson(data);
            doc.TextData = data.GetTexts();

            return doc;
        }


        public class Data
        {
            public string Name = "";
            public List<string> Syntaxs = new List<string>();
            public List<string> ReturnTypes = new List<string>();
            public List<ParamData> Params = new List<ParamData>();
            [System.NonSerialized]
            public List<string> ReturnTypesDescription = new List<string>();
            [System.NonSerialized]
            public List<string> ParamsDescription = new List<string>();
            public bool isOn;

            public Data()
            {
                Syntaxs.Add("");
            }

            public void SetParamsDescription(List<string> descriptions, int start, int end)
            {
                ParamsDescription.Clear();
                for (int i = start; i < end; i++)
                {
                    ParamsDescription.Add(descriptions[i]);
                }
            }

            public void SetReturnTypesDescription(List<string> descriptions, int start, int end)
            {
                ReturnTypesDescription.Clear();
                for (int i = start; i < end; i++)
                {
                    ReturnTypesDescription.Add(descriptions[i]);
                }
            }

            public void AddNewParams(ParamData paramData)
            {
                if (Params.Contains(paramData)) return;
                Params.Add(paramData);
                ParamsDescription.Add("");
            }

            public void RemoveLastReturnType()
            {
                ReturnTypes.RemoveAt(ReturnTypes.Count - 1);
                ReturnTypesDescription.RemoveAt(ReturnTypesDescription.Count - 1);
            }

            public void AddNewReturnType(string returnType)
            {
                if (ReturnTypes.Contains(returnType)) return;
                ReturnTypes.Add(returnType);
                ReturnTypesDescription.Add("");
            }

            public List<string> GetTexts()
            {
                List<string> texts = new List<string> { "" };
                for (int i = 0;i < ParamsDescription.Count; i++)
                {
                    texts.Add(ParamsDescription[i]);
                }
                for (int i = 0;i < ReturnTypesDescription.Count;i++)
                {
                    texts.Add(ReturnTypesDescription[i]);
                }

                return texts;
            }
        }

        [System.Serializable]
        public class ParamData
        {
            public string ParamName = "";
            public string Type = "";

            public ParamData() { }

            public ParamData(string paramName, string type) 
            { 
                this.ParamName = paramName;
                this.Type = type;
            }

            public override bool Equals(object obj)
            {
                return this.ParamName == ((ParamData)obj).ParamName &&
                       this.Type == ((ParamData)obj).Type;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override string ToString()
            {
                return base.ToString();
            }
        }
    }
}
