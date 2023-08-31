using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    public static class IStyleExtension
    {
        public static void SetPostionPixel(this IStyle element,Vector2 pos) { SetPostionPixel(element, pos.x, pos.y); }
        public static void SetPostionPixel(this IStyle element,float x, float y)
        {
            element.left = new Length(x, LengthUnit.Pixel);
            element.top = new Length(y, LengthUnit.Pixel);
        }
        public static void SetPostionPercent(this IStyle element, Vector2 pos) { SetPostionPercent(element, pos.x, pos.y); }
        public static void SetPostionPercent(this IStyle element,float x, float y)
        {
            element.left = new Length(x, LengthUnit.Percent);
            element.top = new Length(y, LengthUnit.Percent);
        }
        public static IStyle FocusOnMouse(this IStyle element, float offsetX = 0, float offsetY = 0)
        {
            element.position = Position.Absolute;
            element.left = new Length(((Input.mousePosition.x + offsetX) / Screen.width) * 100, LengthUnit.Percent);
            element.top = new Length(((Screen.height - Input.mousePosition.y + offsetY) / Screen.height) * 100, LengthUnit.Percent);
            return element;
        }
        public static void ClearMarginPadding(this IStyle element)
        {
            element.marginTop = 0;
            element.marginLeft = 0;
            element.marginRight = 0;
            element.marginBottom = 0;
            element.paddingTop = 0;
            element.paddingLeft = 0;
            element.paddingRight = 0;
            element.paddingBottom = 0;
        }        
        public static void ClearPadding(this IStyle element)
        {
            element.paddingTop = 0;
            element.paddingLeft = 0;
            element.paddingRight = 0;
            element.paddingBottom = 0;
        }
        public static void ClearMargin(this IStyle element)
        {
            element.marginTop = 0;
            element.marginLeft = 0;
            element.marginRight = 0;
            element.marginBottom = 0;
        }

        #region IS_Style setting
        public static void SetIS_Style(this IStyle element, ISStyle style)
        {
            if (style.IsEnable(ISStyleFlag.Display))
                element.SetIS_Style(style.Display);
            if (style.IsEnable(ISStyleFlag.Position))
                element.SetIS_Style(style.Position);
            if (style.IsEnable(ISStyleFlag.Flex))
                element.SetIS_Style(style.Flex);
            if (style.IsEnable(ISStyleFlag.Align))
                element.SetIS_Style(style.Align);
            if (style.IsEnable(ISStyleFlag.Size))
                element.SetIS_Style(style.Size);
            if (style.IsEnable(ISStyleFlag.Margin))
                element.SetIS_Style(style.Margin);
            if (style.IsEnable(ISStyleFlag.Padding))
                element.SetIS_Style(style.Padding);
            if (style.IsEnable(ISStyleFlag.Text))
                element.SetIS_Style(style.Text);
            if (style.IsEnable(ISStyleFlag.Background))
                element.SetIS_Style(style.Background);
            if (style.IsEnable(ISStyleFlag.Border))
                element.SetIS_Style(style.Border);
            if (style.IsEnable(ISStyleFlag.Radius))
                element.SetIS_Style(style.Radius);
        }
        public static IStyle SetIS_Style(this IStyle element, ISDisplay display)
        {
            element.opacity = display.Opacity;
            element.display = display.Display;
            element.visibility = display.Visibility;
            element.overflow = display.Overflow;
            return element;
        }
        public static IStyle SetIS_Style(this IStyle element, ISPosition position)
        {
            element.position = position.Position;
            element.top = position.Top;
            element.left = position.Left;
            element.right = position.Right;
            element.bottom = position.Bottom;
            return element;
        }
        public static IStyle SetIS_Style(this IStyle element, ISFlex flex)
        {
            element.flexBasis = flex.Basis;
            element.flexDirection = flex.Direction;
            element.flexWrap = flex.Wrap;
            element.flexGrow = flex.FlexGrow;
            return element;
        }
        public static IStyle SetIS_Style(this IStyle element, ISAlign align)
        {
            element.justifyContent = align.Content;
            element.alignItems = align.Items;
            return element;
        }
        public static IStyle SetIS_Style(this IStyle element, ISSize size)
        {
            element.width = size.Width;
            element.height = size.Height;
            return element;
        }
        public static IStyle SetIS_Style(this IStyle element, ISMargin margin)
        {
            element.marginLeft = margin.Left;
            element.marginTop = margin.Top;
            element.marginRight = margin.Right;
            element.marginBottom = margin.Bottom;
            return element;
        }
        public static IStyle SetIS_Style(this IStyle element, ISPadding padding)
        {
            element.paddingLeft = padding.Left;
            element.paddingTop = padding.Top;
            element.paddingRight = padding.Right;
            element.paddingBottom = padding.Bottom;
            return element;
        }
        public static IStyle SetIS_Style(this IStyle element, ISText text)
        {
            if (text.FontAsset != null)
            {
                element.unityFontDefinition = new StyleFontDefinition(text.FontAsset);
                element.unityFont = text.FontAsset.sourceFontFile;
            }
            element.fontSize = text.FontSize;
            element.unityTextAlign = text.Align;
            element.color = text.Color;
            element.flexWrap = text.Wrap;
            element.unityFontStyleAndWeight = text.FontStyle;
            return element;
        }
        public static IStyle SetIS_Style(this IStyle element, ISBackground background)
        {
            element.backgroundColor = background.Color;
            element.backgroundImage = background.StyleBackground;
            element.unityBackgroundImageTintColor = background.ImageTint;
            element.unityBackgroundScaleMode = background.ScaleMode;
            return element;
        }
        public static IStyle SetIS_Style(this IStyle element, ISBorder border)
        {
            element.borderLeftColor = border.LeftColor;
            element.borderTopColor = border.TopColor;
            element.borderRightColor = border.RightColor;
            element.borderBottomColor = border.BottomColor;
            element.borderLeftWidth = border.Left;
            element.borderTopWidth = border.Top;
            element.borderRightWidth = border.Right;
            element.borderBottomWidth = border.Bottom;
            return element;
        }
        public static IStyle SetIS_Style(this IStyle element, ISRadius radius)
        {
            element.borderTopLeftRadius = (Length)radius.TopLeft;
            element.borderBottomLeftRadius = (Length)radius.BottomLeft;
            element.borderTopRightRadius = (Length)radius.TopRight;
            element.borderBottomRightRadius = (Length)radius.BottomRight;
            return element;
        }
        #endregion
    }
}
