using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocStyleField : VisualElement
    {
        public DocStyle Target;
        private DocStyle defaultStyle;
        ScrollView scrollView = new ScrollView();
        int colorWidth;
        public DocStyleField(DocStyle style, int colorFieldMaxWidth = 200)
        {
            this.style.backgroundColor = DocStyle.Dark.BackgroundColor;
            defaultStyle = style;  
            colorWidth = colorFieldMaxWidth;
            Target = defaultStyle.Copy();

            create();
            Button reset = new Button();
            reset.style.SetIS_Style(new ISMargin(TextAnchor.LowerCenter));
            reset.text = "Reset to DarkTheme";
            reset.clicked += () =>
            {
                Target = DocStyle.Dark.Copy();
                scrollView.Clear();
                create();
            };

            Add(scrollView);
            Add(reset);
        }
        void create()
        {
            createColor();
            createText(Target.MainText,"Main text");
            createText(Target.LabelText, "Label text");
            createText(Target.ButtonText, "Button text");
        }
        void createColor()
        {
            ColorField colorField;
            colorField = new ColorField(Target.BackgroundColor, "Background", colorWidth);
            colorField.OnValueChange += val => { Target.BackgroundColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.SubBackgroundColor, "Sub Background", colorWidth);
            colorField.OnValueChange += val => { Target.SubBackgroundColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.FrontGroundColor, "Frontground", colorWidth);
            colorField.OnValueChange += val => { Target.FrontGroundColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.SubFrontGroundColor, "Sub Frontground", colorWidth);
            colorField.OnValueChange += val => { Target.SubFrontGroundColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.FuncColor, "Func", colorWidth);
            colorField.OnValueChange += val => { Target.FuncColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.ArgsColor, "Args", colorWidth);
            colorField.OnValueChange += val => { Target.ArgsColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.TypeColor, "Type", colorWidth);
            colorField.OnValueChange += val => { Target.TypeColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.PrefixColor, "Prefix", colorWidth);
            colorField.OnValueChange += val => { Target.PrefixColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.StringColor, "String", colorWidth);
            colorField.OnValueChange += val => { Target.StringColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.NumberColor, "Number", colorWidth);
            colorField.OnValueChange += val => { Target.NumberColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.ControlColor, "Control", colorWidth);
            colorField.OnValueChange += val => { Target.ControlColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.CommentsColor, "Comments", colorWidth);
            colorField.OnValueChange += val => { Target.CommentsColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.CodeBackgroundColor, "CodeBackground", colorWidth);
            colorField.OnValueChange += val => { Target.CodeBackgroundColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.SuccessColor, "Success", colorWidth);
            colorField.OnValueChange += val => { Target.SuccessColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.WarningColor, "Warning", colorWidth);
            colorField.OnValueChange += val => { Target.WarningColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.DangerColor, "Danger", colorWidth);
            colorField.OnValueChange += val => { Target.DangerColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.HintColor, "Hint", colorWidth);
            colorField.OnValueChange += val => { Target.HintColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.SuccessTextColor, "SuccessText", colorWidth);
            colorField.OnValueChange += val => { Target.SuccessTextColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.WarningTextColor, "WarningText", colorWidth);
            colorField.OnValueChange += val => { Target.WarningTextColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.DangerTextColor, "DangerText", colorWidth);
            colorField.OnValueChange += val => { Target.DangerTextColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.HintTextColor, "HintText", colorWidth);
            colorField.OnValueChange += val => { Target.HintTextColor = val; }; scrollView.Add(colorField);
        }
        void createText(ISText text, string label)
        {
            Foldout foldout = new Foldout();
            foldout.style.marginLeft = 5;
            foldout.Q("unity-content").style.marginLeft = 40;
            foldout.value = false;
            foldout.text = label;
            TextField size = new TextField();
            size.value = text.FontSize.ToString();
            size.label = "text size";
            size.RegisterValueChangedCallback(val =>
            {
                int newVal;
                if(int.TryParse(val.newValue,out newVal))
                    text.FontSize = newVal;
            });
            foldout.Add(size);
            ColorField colorField = new ColorField(text.Color,"text color", colorWidth);
            colorField.OnValueChange += val => { text.Color = val; };
            foldout.Add(colorField);
            scrollView.Add(foldout);
        }
    }

}