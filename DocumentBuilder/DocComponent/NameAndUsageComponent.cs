using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DocumentBuilder
{
    [Serializable]
    public class NameAndUsageComponent : DocComponent
    {
        public ISStyle ISStyle = new ISStyle(1152);
        public override VisualElement CreateEditorGUI()
        {
            throw new System.NotImplementedException();
        }

        public override VisualElement CreateRuntimeGUI()
        {
            NameAndUsageVisual output = new NameAndUsageVisual(this);
            return output;
        }
    }

    public class NameAndUsageVisual : VisualElement
    {
        protected VisualElement nameBlock, usageBlock;
        public NameAndUsageVisual(NameAndUsageComponent data)
        {
            this.style.SetIS_Style(ISFlex.Horizontal);
            this.style.SetIS_Style(new ISBorder(Color.white, 2));
            nameBlock = new VisualElement();
            usageBlock = new VisualElement();
            nameBlock.style.SetIS_Style(data.ISStyle);
            usageBlock.style.SetIS_Style(data.ISStyle);
            nameBlock.style.width = new Length(20, LengthUnit.Percent);
            usageBlock.style.width = new Length(80, LengthUnit.Percent);
            for (int i = 0;i < data.StrData.Count;i++)
            {
                string[] temp = data.StrData[i].Split("%SPACE%");
                this.AddData(temp[0], temp[1]);
            }
            this.Add(nameBlock);
            this.Add(usageBlock);
        }

        ISBorder elementBorder = new ISBorder { BottomColor = Color.white, Bottom = 2 };
        public void AddData(string name, string usage)
        {
            TextElement nameElement = new TextElement();
            TextElement usageElement = new TextElement();
            nameElement.text = name;
            usageElement.text = usage;
            nameElement.style.SetIS_Style(elementBorder);
            usageElement.style.SetIS_Style(elementBorder);
            usageElement.style.width = new Length(100, LengthUnit.Percent);
            nameBlock.Add(nameElement);
            usageBlock.Add(usageElement);
        }
    }

    public class NameAndUsageEditVisual
    {

    }
}
