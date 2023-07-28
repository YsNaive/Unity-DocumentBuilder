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

        private static ISText funcNameTextStyle = new ISText() { FontStyle = FontStyle.Bold, Color = new Color(0.85f, 0.85f, 0.85f), FontSize = Current.MainTextSize };
        private static ISText syntaxTextStyle = new ISText() { Color = Current.ArgsColor, FontSize = Current.MainTextSize };
        private static ISText paramTextStyle = new ISText() { FontStyle = FontStyle.BoldAndItalic, Color = Current.ArgsColor, FontSize = Current.MainTextSize };
        private static ISText typeTextStyle = new ISText() { Color = Current.TypeColor, FontSize = Current.MainTextSize };
        private static ISText labelTextStyle = new ISText() { Color = Current.SubFrontGroundColor, FontSize = Current.MainTextSize };
        private float tabGap = 2;
        private VisualElement veFoldOut;

        protected override void OnCreateGUI()
        {
            Data data = JsonUtility.FromJson<Data>(Target.JsonData);
            if (data == null) return;
            switch (data.IntroAniMode)
            {
                case AniMode.None:
                    break;
                case AniMode.Fade:
                    IntroAnimation = (callBack) => { this.Fade(0, 1, data.IntroDuration, 50, callBack); };
                    break;
                case AniMode.TextFade:
                    break;
            }
            switch (data.OuttroAniMode)
            {
                case AniMode.None:
                    break;
                case AniMode.Fade:
                    OuttroAnimation = (callBack) => { this.Fade(1, 0, data.OuttroDuration, 50, callBack); };
                    break;
                case AniMode.TextFade:
                    break;
            }
            Foldout foldout = new Foldout();
            foldout.text = data.Name;
            foldout.value = data.isOn;
            foldout.style.SetIS_Style(funcNameTextStyle);
            foldout.Q<Toggle>().style.ClearMarginPadding();
            foldout.Q("unity-checkmark").style.position = Position.Absolute;
            foldout.Q("unity-checkmark").visible = false;
            foldout.style.paddingLeft = 5;
            foldout.RegisterValueChangedCallback(value =>
            {
                if (value.newValue)
                    this.Add(veFoldOut);
                else
                    this.Remove(veFoldOut);
                data.isOn = value.newValue;
                Target.JsonData = JsonUtility.ToJson(data);
            });
            this.Add(foldout);
            veFoldOut = new VisualElement();
            veFoldOut.style.ClearMarginPadding();
            veFoldOut.style.paddingLeft = 5;
            if (Target.TextData[0] != "")
            {
                TextElement descriptionText = new TextElement();
                descriptionText.text = Target.TextData[0];
                descriptionText.style.SetIS_Style(DocStyle.Current.MainText);
                descriptionText.style.ClearMarginPadding();
                descriptionText.style.paddingLeft = 1f * Current.MainTextSize;
                this.Add(descriptionText);
            }
            veFoldOut.Add(generateSyntaxContainer(data));
            veFoldOut.Add(generateParamContainer(data));
            veFoldOut.Add(generateReturnTypeContainer(data));
            if (data.isOn)
                this.Add(veFoldOut);
        }

        private string parseSyntax(string synatx)
        {
            try
            {
                if (string.IsNullOrEmpty(synatx)) return "";
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
                    if (str != prefixs[^1])
                        stringBuilder.Append(" ");
                }

                stringBuilder.Append("(");
                foreach (string str in strs[1].Split(","))
                {
                    if (str == "")
                    {
                        stringBuilder.Append("  ");
                        break;
                    }
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
            } catch
            {
                Debug.LogWarning("DocVisual : DocFuncDisplay Syntax Error");
                return synatx;
            }
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
            public AniMode IntroAniMode = AniMode.None, OuttroAniMode = AniMode.None;
            public int IntroDuration = 0, OuttroDuration = 0;
            public bool isOn;

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

        public enum AniMode
        {
            None,
            Fade,
            TextFade,
        }
    }
}