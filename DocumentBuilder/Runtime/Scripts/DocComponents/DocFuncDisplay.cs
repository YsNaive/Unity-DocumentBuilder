using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using static NaiveAPI.DocumentBuilder.DocStyle;

namespace NaiveAPI.DocumentBuilder
{
    public class DocFuncDisplay : DocVisual
    {
        public override string VisualID => "4";

        private static ISText funcNameTextStyle = new ISText() { FontStyle = FontStyle.Bold, Color = Current.FuncColor, FontSize = Current.MainTextSize };
        private static ISText paramTextStyle = new ISText() { FontStyle = FontStyle.BoldAndItalic, Color = Current.ArgsColor, FontSize = Current.MainTextSize };
        private static ISText typeTextStyle = new ISText() { Color = Current.TypeColor, FontSize = Current.MainTextSize };
        private static ISText labelTextStyle = new ISText() { Color = Current.SubFrontgroundColor, FontSize = Current.MainTextSize };
        private float tabGap = 2;
        private VisualElement veFoldOut;

        public float test
        {
            get; set;
        }

        protected override void OnCreateGUI()
        {
            Data data = JsonUtility.FromJson<Data>(Target.JsonData);
            if (data == null) return;
            TextElement nameText = DocRuntime.NewTextElement(data.Name);
            nameText.style.SetIS_Style(funcNameTextStyle);
            veFoldOut = new VisualElement();
            veFoldOut.style.ClearMarginPadding();
            veFoldOut.style.paddingLeft = 5;
            this.Add(nameText);
            if (Target.TextData[0] != "")
            {
                TextElement descriptionText = DocRuntime.NewTextElement(Target.TextData[0]);
                this.Add(descriptionText);
            }
            veFoldOut.Add(generateSyntaxContainer(data));
            veFoldOut.Add(generateParamContainer(data));
            veFoldOut.Add(generateReturnTypeContainer(data));
            this.Add(veFoldOut);
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
            syntaxLabel.style.marginTop = Current.MainTextSize / 2f;
            root.Add(syntaxLabel);

            for (int i = 0;i < data.Syntaxs.Count; i++)
            {
                TextElement syntaxText = new TextElement();
                syntaxText.text = DocumentBuilderParser.FunctionParser(data.Syntaxs[i]);
                syntaxText.style.ClearMarginPadding();
                syntaxText.style.paddingLeft = Length.Percent(tabGap);
                syntaxText.style.SetIS_Style(DocStyle.Current.MainText);
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
            paramText.style.marginTop = Current.MainTextSize / 2f;
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
            returnTypeText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(DocStyle.Current.MainText.Color)}>Type: </color>" + returnType;
            returnTypeText.style.SetIS_Style(typeTextStyle); 
            returnTypeText.style.ClearMarginPadding();
            returnTypeText.style.paddingLeft = Length.Percent(tabGap);
            root.Add(returnTypeText);
            if (description != "")
            {
                TextElement descriptionText = new TextElement();
                descriptionText.text = description;
                descriptionText.style.SetIS_Style(DocStyle.Current.MainText);
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
            returnText.style.marginTop = Current.MainTextSize / 2f;
            root.Add(returnText);
            for (int i = 0; i < data.ReturnTypes.Count; i++)
            {
                root.Add(generateReturnTypeVisual(data.ReturnTypes[i], Target.TextData[1 + data.Params.Count + i]));
            }

            return root;
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

            public void AddNewParams()
            {
                Params.Add(new ParamData());
                ParamsDescription.Add("");
            }

            public void AddNewParams(ParamData paramData)
            {
                Params.Add(paramData);
                ParamsDescription.Add("");
            }

            public void RemoveLastParams()
            {
                Params.RemoveAt(Params.Count - 1);
                ParamsDescription.RemoveAt(ParamsDescription.Count - 1);
            }

            public void RemoveLastReturnType()
            {
                ReturnTypes.RemoveAt(ReturnTypes.Count - 1);
                ReturnTypesDescription.RemoveAt(ReturnTypesDescription.Count - 1);
            }

            public void AddNewReturnType()
            {
                ReturnTypes.Add("");
                ReturnTypesDescription.Add("");
            }

            public void AddNewReturnType(string returnType)
            {
                ReturnTypes.Add(returnType);
                ReturnTypesDescription.Add("");
            }

            public List<string> getTexts()
            {
                List<string> texts = new List<string>();
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
