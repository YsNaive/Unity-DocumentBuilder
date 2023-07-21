using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocStyleVisual : VisualElement
    {
        DocStyle DS = DocStyle.Current;
        public DocStyleVisual()
        {
            style.SetIS_Style(ISSize.Percent(90, 90));
            style.backgroundColor = DS.BackgroundColor;

            VisualElement subBackGround = new VisualElement();
            subBackGround.style.SetIS_Style(ISSize.Percent(90, 90));
            subBackGround.style.SetIS_Style(ISPadding.Percent(5));
            subBackGround.style.SetIS_Style(ISMargin.Percent(5));
            subBackGround.style.backgroundColor = DS.SubBackgroundColor;
            Add(subBackGround);

            VisualElement funcDisplay = new VisualElement();
            funcDisplay.style.SetIS_Style(ISFlex.Horizontal);
            TextElement type = new TextElement();
            type.text = "MyClass";
            type.style.color = DS.TypeColor;
            TextElement Ttype = new TextElement();
            Ttype.text = " T";
            Ttype.style.color = DS.TypeColor;
            TextElement func = new TextElement();
            func.text = " Function";
            func.style.color = DS.FuncColor;
            TextElement args = new TextElement();
            args.text = " args";
            args.style.color = DS.ArgsColor;
            funcDisplay.Add(type);
            funcDisplay.Add(func);
            funcDisplay.Add(Ttype);
            funcDisplay.Add(args);

            subBackGround.Add(funcDisplay);

            TextElement label = new TextElement();
            label.style.SetIS_Style(DS.LabelText);
            label.text = "Label";

            subBackGround.Add(label);

            TextElement description = new TextElement();
            description.style.SetIS_Style(DS.MainText);
            description.text = "Some Description !\nAlso next LINE.";

            subBackGround.Add(description);


            VisualElement frontGround = new VisualElement();
            frontGround.style.SetIS_Style(ISSize.Percent(90, 90));
            frontGround.style.SetIS_Style(ISPadding.Percent(5));
            frontGround.style.SetIS_Style(ISMargin.Percent(5));
            frontGround.style.SetIS_Style(new ISBorder(DS.FrontGroundColor, 5));

            VisualElement subFrontGround = new VisualElement();
            subFrontGround.style.SetIS_Style(ISSize.Percent(90, 90));
            subFrontGround.style.SetIS_Style(ISPadding.Percent(5));
            subFrontGround.style.SetIS_Style(ISMargin.Percent(5));
            subFrontGround.style.SetIS_Style(new ISBorder(DS.SubFrontGroundColor, 5));

            frontGround.Add(subFrontGround);
            subBackGround.Add(frontGround);

        }
    }
}
