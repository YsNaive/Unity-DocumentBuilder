using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI_UI
{
    [System.Serializable]
    public struct RichText
    {
        public string Text;
        public Color Color;
        public int FontSize;
        // 1:b,  2:i,  4:u,  8:s
        [SerializeField] private int fontStyleMask;

        public override string ToString()
        {
            string begin = "",end = "";
            if(FontSize != 0)
            {
                begin += $"<size={FontSize}>";
                end = "</size>" + end;
            }
            if(Color != Color.clear)
            {
                begin += $"<color=#{ColorUtility.ToHtmlStringRGBA(Color)}>";
                end = "</color>"+ end;
            }
            if (Bold)
            {
                begin += "<b>";
                end = "</b>" + end;
            }
            if (Italics)
            {
                begin += "<i>";
                end = "</i>" + end;
            }
            if (Underline)
            {
                begin += "<u>";
                end = "</u>" + end;
            }
            if (Strikethrough)
            {
                begin += "<s>";
                end = "</s>" + end;
            }
            return begin+Text+end;
        }

        public static implicit operator string(RichText richText)
        {
            return richText.ToString();
        }
        #region get set
        public bool Bold 
        {
            get => (fontStyleMask & 1) > 0;
            set
            {
                if (value) fontStyleMask |= 1;
                else fontStyleMask &= 14;
            }
        }
        public bool Italics
        {
            get => (fontStyleMask & 2) > 0;
            set
            {
                if (value) fontStyleMask |= 2;
                else fontStyleMask &= 13;
            }
        }
        public bool Underline
        {
            get => (fontStyleMask & 4) > 0;
            set
            {
                if (value) fontStyleMask |= 4;
                else fontStyleMask &= 11;
            }
        }
        public bool Strikethrough
        {
            get => (fontStyleMask & 8) > 0;
            set
            {
                if (value) fontStyleMask |= 8;
                else fontStyleMask &= 7;
            }
        }
        #endregion
    }
}
