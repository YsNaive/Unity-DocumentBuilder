using NaiveAPI_UI;
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

        private static ISText funcNameTextStyle = new ISText() { FontStyle = FontStyle.Bold, Color = new Color(0.85f, 0.85f, 0.85f), FontSize = Current.LabelTextSize };
        private static ISText syntaxTextStyle = new ISText() { Color = Current.ArgsColor, FontSize = Current.MainTextSize };
        private static ISText paramTextStyle = new ISText() { FontStyle = FontStyle.BoldAndItalic, Color = Current.ArgsColor, FontSize = Current.MainTextSize };
        private static ISText typeTextStyle = new ISText() { Color = Current.TypeColor, FontSize = 12 };
        private static ISText labelTextStyle = new ISText() { Color = Current.SubFrontGroundColor, FontSize = 12 };
        private float tabGap = 2;

        protected override void OnCreateGUI()
        {
            Data data = JsonUtility.FromJson<Data>(Target.JsonData);
            if (data == null) return;
            this.style.paddingLeft = Length.Percent(tabGap);
            TextElement nameText = new TextElement();
            nameText.text = data.Name;
            nameText.style.SetIS_Style(funcNameTextStyle);
            nameText.style.ClearMarginPadding();
            this.Add(nameText);
            TextElement descriptionText = new TextElement();
            descriptionText.text = Target.TextData[0];
            descriptionText.style.SetIS_Style(DocStyle.Current.MainText);
            descriptionText.style.ClearMarginPadding();
            descriptionText.style.paddingLeft = Length.Percent(tabGap);
            this.Add(descriptionText);
            this.Add(generateSyntaxContainer(data));
            this.Add(generateParamContainer(data));
            this.Add(generateReturnTypeContainer(data));
        }

        private string parseSyntax(string synatx)
        {
            string[] strs = synatx.Split('(');
            strs[1] = strs[1].Substring(0, strs[1].LastIndexOf(")"));

            string[] prefixs = strs[0].Split(" ");
            StringBuilder stringBuilder = new StringBuilder();
            string subFrontGroundColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(Current.SubFrontGroundColor)}>";
            string prefixColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(Color.blue)}>";
            string funcColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(Current.FuncColor)}>";
            string typeColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(typeTextStyle.Color)}>";
            string paramColor = $"<color=#{ColorUtility.ToHtmlStringRGBA(paramTextStyle.Color)}>";
            string postfixColor = "</color>";
            foreach (string str in prefixs)
            {
                if (str == prefixs[^1] && str != "void")
                {
                    stringBuilder.Append(funcColor);
                }
                else if (str == prefixs[^2] && str != "void")
                {
                    stringBuilder.Append(typeColor);
                }
                else
                {
                    stringBuilder.Append(prefixColor);
                }
                stringBuilder.Append(str);
                stringBuilder.Append(postfixColor);
                stringBuilder.Append(" ");
            }

            stringBuilder.Append("(");
            foreach (string str in strs[1].Split(","))
            {
                string[] param = str.Split(" ");
                int index = 0;
                while (param[index] == "")
                    index++;
                if (param[index] == "ref" || param[index] == "out" || param[index] == "in")
                {
                    stringBuilder.Append(prefixColor);
                    stringBuilder.Append(param[index]);
                    stringBuilder.Append(postfixColor);
                    stringBuilder.Append(" ");
                    index++;
                }
                stringBuilder.Append(typeColor);
                stringBuilder.Append(param[index++]);
                stringBuilder.Append(postfixColor);
                while (param[index] == "")
                    index++;
                stringBuilder.Append(" ");
                stringBuilder.Append(paramColor);
                stringBuilder.Append(param[index++]);
                stringBuilder.Append(postfixColor);
                if (param.Length > index + 1)
                {
                    stringBuilder.Append(subFrontGroundColor);
                    while (index < param.Length)
                    {
                        stringBuilder.Append(" ");
                        stringBuilder.Append(param[index++]);
                    }
                    stringBuilder.Append(postfixColor);
                }
                stringBuilder.Append(", ");
            }
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            stringBuilder.Append(");");

            return stringBuilder.ToString();
        }

        private VisualElement generateSyntaxContainer(Data data)
        {
            VisualElement root = new VisualElement();

            TextElement syntaxLabel = new TextElement();
            syntaxLabel.text = "Syntaxs";
            syntaxLabel.style.SetIS_Style(labelTextStyle);
            syntaxLabel.style.ClearMarginPadding();
            syntaxLabel.style.marginTop = Current.MainTextSize / 2f;
            root.Add(syntaxLabel);

            for (int i = 0;i < data.Syntaxs.Count; i++)
            {
                TextElement syntaxText = new TextElement();
                syntaxText.text = parseSyntax(data.Syntaxs[i]);
                syntaxText.style.SetIS_Style(syntaxTextStyle);
                syntaxText.style.ClearMarginPadding();
                syntaxText.style.paddingLeft = Length.Percent(tabGap);
                root.Add(syntaxText);
            }

            return root;
        }

        private VisualElement generateParamVisual(ParamData data, string description) 
        {
            VisualElement root = new VisualElement();

            TextElement paramNameText = new TextElement();
            paramNameText.text = data.ParamName;
            paramNameText.style.SetIS_Style(paramTextStyle);
            paramNameText.style.ClearMarginPadding();
            paramNameText.style.paddingLeft = Length.Percent(tabGap);
            root.Add(paramNameText);
            TextElement paramTypeText = new TextElement();
            paramTypeText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(DocStyle.Current.MainText.Color)}>Type: </color>" + data.Type;
            paramTypeText.style.SetIS_Style(typeTextStyle);
            paramTypeText.style.ClearMarginPadding();
            paramTypeText.style.paddingLeft = Length.Percent(2 * tabGap);
            root.Add(paramTypeText);
            TextElement descriptionText = new TextElement();
            descriptionText.text = description;
            descriptionText.style.SetIS_Style(DocStyle.Current.MainText);
            descriptionText.style.ClearMarginPadding();
            descriptionText.style.paddingLeft = Length.Percent(2 * tabGap);
            root.Add(descriptionText);

            return root;
        }

        private VisualElement generateParamContainer(Data data)
        {
            VisualElement root = new VisualElement();

            TextElement paramText = new TextElement();
            paramText.text = "Parameters";
            paramText.style.SetIS_Style(labelTextStyle);
            paramText.style.ClearMarginPadding();
            paramText.style.marginTop = Current.MainTextSize / 2f;
            root.Add(paramText);

            for (int i = 0;i < data.Params.Count; i++)
            {
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
            TextElement descriptionText = new TextElement();
            descriptionText.text = description;
            descriptionText.style.SetIS_Style(DocStyle.Current.MainText);
            descriptionText.style.ClearMarginPadding();
            descriptionText.style.paddingLeft = Length.Percent(tabGap);
            root.Add(descriptionText);

            return root;
        }

        private VisualElement generateReturnTypeContainer(Data data)
        {
            VisualElement root = new VisualElement();

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

            public Data()
            {
                Syntaxs.Add("");
                ReturnTypes.Add("");
                Params.Add(new ParamData());
                ReturnTypesDescription.Add("");
                ParamsDescription.Add("");
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
        }
    }
}
