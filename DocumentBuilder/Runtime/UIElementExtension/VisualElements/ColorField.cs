using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    public class ColorField : VisualElement
    {
        private static Texture2D renderHimg(float pos)
        {
            Texture2D img = new Texture2D(1, 256);
            for (int i = 0; i < 256; i++)
            {
                float now = i / 256f;
                if (Math.Abs(now - pos) < 0.005f)
                    img.SetPixel(0, i, Color.white);
                else
                    img.SetPixel(0, i, Color.HSVToRGB(now, 1f, .75f));
            }
            img.Apply();
            return img;
        }
        private static Texture2D renderSVimg(float h, float posX,float posY)
        {
            Texture2D img = new Texture2D(100, 100);
            for(int i=0; i<100; i++)
            {
                for(int j=0; j<100; j++)
                {
                    if (Math.Abs(posX - i / 100f) < 0.015f && Math.Abs(posY - j / 100f) < 0.015f)
                        img.SetPixel(i, j, Color.white);
                    else
                        img.SetPixel(i, j, Color.HSVToRGB(h, i / 100f, j / 100f));
                }
            }
            img.Apply();
            return img;
        }

        VisualElement svImg = new VisualElement();
        VisualElement hImg = new VisualElement();
        VisualElement previewImg = new VisualElement();
        VisualElement previewImg2 = new VisualElement();
        VisualElement previewImg3 = new VisualElement();
        Foldout foldout = new Foldout();
        TextField htmlColor = new TextField();
        Slider a = new Slider("A",0,1);
        float h = 0.5f;
        float s = 0.5f, v = 0.5f;
        float height;
        float foldoutHeight;
        public Color value
        {
            get => m_color;
            private set
            {
                if (m_color != value)
                {
                    m_color = value;
                    OnValueChange?.Invoke(m_color);
                }
            }
        }
        private Color m_color;
        public event Action<Color> OnValueChange;
        public ColorField(Color initVal, string label = "", int maxWidth = -1) {
            foldout.text = label;
            foldout.value = false;
            value = initVal;
            foldout.style.SetIS_Style(DocStyle.Current.MainText);
            if(maxWidth != -1)foldout.Q("unity-content").style.width = maxWidth;
            previewImg3.style.backgroundColor = value;
            previewImg3.style.width = Length.Percent(33);
            previewImg3.style.position = Position.Absolute;
            Add(previewImg3);
            foldout.RegisterCallback<GeometryChangedEvent>(e =>
            {
                if(e.newRect.height != foldoutHeight && !foldout.value)
                {
                    foldoutHeight = e.newRect.height;
                    previewImg3.style.height = foldoutHeight;
                    previewImg3.style.top = e.newRect.y;
                }
                previewImg3.style.left = (e.newRect.xMax / 3f)*2f;
            });
            foldout.RegisterValueChangedCallback(val =>
            {
                previewImg3.visible = !foldout.value;
            });
            Add(foldout);
            Color.RGBToHSV(initVal, out h, out s, out v);
            style.backgroundColor = DocStyle.Current.BackgroundColor;
            style.SetIS_Style(ISPadding.Percent(5));

            VisualElement colorHor = new VisualElement();
            colorHor.style.SetIS_Style(ISFlex.Horizontal);
            previewImg.style.backgroundColor = value;
            previewImg.style.width = Length.Percent(46);
            previewImg2.style.backgroundColor = value;
            previewImg2.style.width = Length.Percent(47);
            previewImg2.style.marginRight = Length.Percent(5);
            colorHor.Add(previewImg2);
            colorHor.Add(previewImg);
            foldout.Add(colorHor);
            VisualElement hor = new VisualElement();
            hor.style.SetIS_Style(ISFlex.Horizontal);
            svImg.style.width = Length.Percent(79);
            svImg.style.SetIS_Style(ISMargin.None);
            svImg.style.marginRight = Length.Percent(5);
            svImg.style.backgroundImage = renderSVimg(h,s,v);
            hor.Add(svImg);
            hImg.style.width = Length.Percent(15);
            hImg.style.SetIS_Style(ISMargin.None);
            hImg.style.backgroundImage = renderHimg(h);
            hor.Add(hImg);
            hImg.RegisterCallback<PointerDownEvent>(e => { isMouseDown = true;
                h = 1 - e.localPosition.y / height;
                hImg.style.backgroundImage = renderHimg(h);
                svImg.style.backgroundImage = renderSVimg(h, s, v);
                reloadColor();
            });
            hImg.RegisterCallback<PointerUpEvent>(e => { isMouseDown = false; previewImg2.style.backgroundColor = value; previewImg3.style.backgroundColor = value; });
            hImg.RegisterCallback<PointerLeaveEvent>(e => { isMouseDown = false; previewImg2.style.backgroundColor = value; previewImg3.style.backgroundColor = value; });
            hImg.RegisterCallback<PointerMoveEvent>(onHueChanged );
            svImg.RegisterCallback<PointerDownEvent>(e => {
                isMouseDown = true;
                s = e.localPosition.x / height;
                v = 1 - e.localPosition.y / height;
                svImg.style.backgroundImage = renderSVimg(h,s,v);
                reloadColor();
            });
            svImg.RegisterCallback<PointerUpEvent>(e => { isMouseDown = false; previewImg2.style.backgroundColor = value; previewImg3.style.backgroundColor = value; });
            svImg.RegisterCallback<PointerLeaveEvent>(e => { isMouseDown = false; previewImg2.style.backgroundColor = value; previewImg3.style.backgroundColor = value; });
            svImg.RegisterCallback<PointerMoveEvent>(onSVChanged );
            svImg.RegisterCallback<GeometryChangedEvent>(e =>
            {
                if(e.oldRect.height != e.newRect.width)
                {
                    height = e.newRect.width;
                    svImg.style.height = height;
                    hImg.style.height = height;
                    colorHor.style.height = height*0.2f;
                    colorHor.style.marginBottom = height * 0.05f;
                }
            });
            foldout.Add(hor);
            var border = new ISBorder(DocStyle.Current.FrontgroundColor, 2);
            previewImg.style.SetIS_Style(border);
            previewImg2.style.SetIS_Style(border);
            previewImg3.style.SetIS_Style(border);

            hor = new VisualElement();
            hor.style.SetIS_Style(ISFlex.Horizontal);
            hor.style.marginTop = 10;
            a.style.SetIS_Style(ISMargin.None);
            a.style.SetIS_Style(ISRadius.Percent(15));
            a.style.marginRight = Length.Percent(5);
            a.style.width = Length.Percent(69);
            a.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            a.value = 1f;
            a[0].style.SetIS_Style(DocStyle.Current.MainText);
            a[0].style.minWidth = Length.Percent(5);
            a.RegisterValueChangedCallback(val =>
            {
                reloadColor();
            });
            hor.Add(a);
            htmlColor.value = '#' + ColorUtility.ToHtmlStringRGBA(initVal);
            htmlColor.style.SetIS_Style(ISMargin.None);
            htmlColor.style.SetIS_Style(DocStyle.Current.MainText);
            htmlColor.style.width = Length.Percent(25);
            htmlColor[0].style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            htmlColor.RegisterValueChangedCallback(val =>
            {
                Color color;
                if(ColorUtility.TryParseHtmlString(val.newValue,out color))
                {
                    Color.RGBToHSV(color, out h, out s, out v);
                    reloadColor();
                }
            });
            hor.Add(htmlColor);
            foldout.Add(hor);
        }

        bool isMouseDown = false;
        private void onHueChanged(PointerMoveEvent e)
        {
            if (isMouseDown)
            {
                h = 1 - e.localPosition.y / height;
                hImg.style.backgroundImage = renderHimg(h);
                svImg.style.backgroundImage = renderSVimg(h, s, v);
                reloadColor();
            }
        } 
        private void onSVChanged(PointerMoveEvent e)
        {
            if (isMouseDown)
            {
                s = e.localPosition.x / height;
                v = 1 - e.localPosition.y / height;
                svImg.style.backgroundImage = renderSVimg(h, s, v);
                reloadColor();
            }
        }
        private void reloadColor()
        {
            value = Color.HSVToRGB(h, s, v);
            m_color.a = a.value;
            htmlColor.value = '#'+ColorUtility.ToHtmlStringRGBA(value);
            previewImg.style.backgroundColor = value;
        }
    }

}