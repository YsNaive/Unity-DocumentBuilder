using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [System.Serializable]
    public class DocStyle
    {
        public static DocStyle Current => DocRuntimeData.Instance.CurrentStyle.Get();
        public int MainTextSize { get => MainText.FontSize; set => MainText.FontSize = value; }
        public int LabelTextSize { get => LabelText.FontSize; set => LabelText.FontSize = value; }
        public int ButtonTextSize { get => ButtonText.FontSize; set => ButtonText.FontSize = value; }

        public ISText MainText = new ISText();
        public ISText LabelText = new ISText();
        public ISText ButtonText = new ISText();

        public Color BackgroundColor = new Color(0, 0, 0, 1);
        public Color SubBackgroundColor = new Color(0, 0, 0, 1);
        public Color FrontgroundColor = new Color(0, 0, 0, 1);
        public Color SubFrontgroundColor = new Color(0, 0, 0, 1);

        public Color FuncColor = new Color(.89f, .79f, .35f);
        public Color ArgsColor = new Color(.65f, .85f, .95f);
        public Color TypeColor = new Color(.35f, .7f, .65f);
        public Color PrefixColor = new Color(.4f, .56f, .82f);
        public Color StringColor = new Color(.79f, .56f, .36f);
        public Color NumberColor = new Color(.6f, .8f, .6f);
        public Color ControlColor = new Color(.84f, .45f, .61f);
        public Color CommentsColor = new Color(.4f, .6f, .35f);
        public Color CodeBackgroundColor = new Color(.08f, .08f, .09f);

        public Color SuccessColor = new Color(0, 0, 0, 1);
        public Color WarningColor = new Color(0, 0, 0, 1);
        public Color DangerColor = new Color(0, 0, 0, 1);
        public Color HintColor = new Color(0, 0, 0, 1);
        public Color SuccessTextColor = new Color(0, 0, 0, 1);
        public Color WarningTextColor = new Color(0, 0, 0, 1);
        public Color DangerTextColor = new Color(0, 0, 0, 1);
        public Color HintTextColor = new Color(0, 0, 0, 1);

        public float MarginVer = 1;
        public float MarginHor = 0;
        public float LineHeight = 18;
        public float ScrollerWidth = 14;
        public float ComponentSpace = 10;
        public float GUIScale = 1;


        public DocStyle Copy()
        {
            DocStyle docStyle = new DocStyle();
            docStyle.MainText = this.MainText.Copy();
            docStyle.LabelText = this.LabelText.Copy();
            docStyle.ButtonText = this.ButtonText.Copy();
            docStyle.BackgroundColor = this.BackgroundColor;
            docStyle.SubBackgroundColor = this.SubBackgroundColor;
            docStyle.FrontgroundColor = this.FrontgroundColor;
            docStyle.SubFrontgroundColor = this.SubFrontgroundColor;

            docStyle.FuncColor = this.FuncColor;
            docStyle.ArgsColor = this.ArgsColor;
            docStyle.TypeColor = this.TypeColor;
            docStyle.PrefixColor = this.PrefixColor;
            docStyle.StringColor = this.StringColor;  
            docStyle.NumberColor = this.NumberColor;  
            docStyle.ControlColor = this.ControlColor;  
            docStyle.CommentsColor = this.CommentsColor;  
            docStyle.CodeBackgroundColor = this.CodeBackgroundColor;  

            docStyle.SuccessColor = this.SuccessColor;  
            docStyle.WarningColor = this.WarningColor;  
            docStyle.DangerColor = this.DangerColor;  
            docStyle.HintColor = this.HintColor;  
            docStyle.SuccessTextColor = this.SuccessTextColor;  
            docStyle.WarningTextColor = this.WarningTextColor;  
            docStyle.DangerTextColor = this.DangerTextColor;  
            docStyle.HintTextColor = this.HintTextColor;

            docStyle.MarginVer = this.MarginVer;
            docStyle.MarginHor = this.MarginHor;
            docStyle.LineHeight = this.LineHeight;
            docStyle.ScrollerWidth = this.ScrollerWidth;
            docStyle.ComponentSpace = this.ComponentSpace;
            docStyle.GUIScale = this.GUIScale;

            return docStyle;
        }
    }

}