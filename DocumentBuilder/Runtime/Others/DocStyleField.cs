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
            defaultStyle = style;  
            colorWidth = colorFieldMaxWidth;
            Target = defaultStyle.Copy();
            
            createVisual(); 
            Button reset = new Button();
            reset.style.SetIS_Style(new ISMargin(TextAnchor.LowerCenter));
            reset.text = "Reset to DarkTheme";
            reset.clicked += () =>
            {
                Target = DocStyle.Dark.Copy();
                scrollView.Clear();
                createVisual();
            };

            Add(scrollView);
            Add(reset);
        }
        void createVisual()
        {
            ColorField colorField;
            colorField = new ColorField(Target.BackgroundColor, "Background", colorWidth);
            colorField.OnValueChange += val => { Target.BackgroundColor = val; }; scrollView.Add(colorField);
            colorField = new ColorField(Target.SubFrontGroundColor, "Sub Background", colorWidth);
            colorField.OnValueChange += val => { Target.SubFrontGroundColor = val; }; scrollView.Add(colorField);
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


        }
    }

}