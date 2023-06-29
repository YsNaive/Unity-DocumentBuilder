using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
namespace DocumentBuilder
{
    public class DoucmentWindow : VisualElement
    {
        public VisualElement Toolbar;
        public DoucmentWindow()
        {
            DocumentBuilderSetting.Save();
            DocumentBuilderSetting.Load();
            style.SetIS_Style(ISSize.Percent(100, 100));
            style.backgroundColor = DocumentBuilderSetting.DocStyle.BackgroundColor;

            Toolbar = new VisualElement();
            Toolbar.style.backgroundColor = DocumentBuilderSetting.DocStyle.MainColor;
            Toolbar.style.SetIS_Style(new ISSize { Width = ISStyleLength.Percent(96), 
                                                   Height = ISStyleLength.Pixel(80 * DocumentBuilderSetting.GUIScale)});
            Toolbar.style.SetIS_Style(ISMargin.Pixel((int)(5 * DocumentBuilderSetting.GUIScale)));
            Toolbar.style.alignSelf = Align.Center;

            this.Add(Toolbar);
        }

        [UnityEngine.Scripting.Preserve]
        public new class UxmlFactory : UxmlFactory<DoucmentWindow, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as DoucmentWindow;

            }
        }
    }

}