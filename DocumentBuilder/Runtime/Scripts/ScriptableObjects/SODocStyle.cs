using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    [CreateAssetMenu(menuName = "Naive API/DocumentBuilder/new DocStyle")]
    public class SODocStyle : ScriptableObject
    {
        public float RuntimeFontSizeScale = 2f;
        public float EditorFontSizeScale = 1f;
        [SerializeField]
        private DocStyle DocStyle = new DocStyle();

        // NonSerialized
        private DocStyle instance;
        private void OnEnable()
        {
            instance = null;
        }
        private void OnDisable()
        {
            instance = null;
        }
        public DocStyle Get() { return Get(Application.isPlaying); }
        public DocStyle Get(bool isRuntime)
        {
            if (instance == null)
            {
                instance = DocStyle.Copy();
            }
            float scale = isRuntime ? RuntimeFontSizeScale : EditorFontSizeScale;
            if(instance.ComponentSpace.Value != (DocStyle.ComponentSpace * scale).Value)
            {
                instance.ComponentSpace = (DocStyle.ComponentSpace * scale);
                instance.ScrollerWidth = (DocStyle.ScrollerWidth * scale);
                instance.MainTextSize = (int)(DocStyle.MainTextSize * scale);
                instance.LabelTextSize = (int)(DocStyle.LabelTextSize * scale);
                instance.ButtonTextSize = (int)(DocStyle.ButtonTextSize * scale);
                instance.ElementMarginPadding.Margin *= scale;
                instance.ElementMarginPadding.Padding *= scale;
                if(instance.LabelWidth.Unit == LengthUnit.Pixel)
                    instance.LabelWidth = (DocStyle.LabelWidth * scale);
                if(instance.LineHeight.Unit == LengthUnit.Pixel)
                    instance.LineHeight = (DocStyle.LineHeight * scale);
            }
            return instance;
        }

        private static Texture2D m_whiteArrow;
        public static Texture2D WhiteArrow
        {
            get
            {
                if (m_whiteArrow == null)
                {
                    m_whiteArrow = new Texture2D(1, 1);
                    m_whiteArrow.LoadImage(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAAXNSR0IArs4c6QAAAZxJREFUeJztm7GOgkAURY+b/VVIqKxs/AAbKioS+FitHkFiNqvDvLmD71QM0XHu8Q4hESEIgiAIvpZT6gRN09y35+Z5Tp7Xi6SFvgpv1CLh40Wuw1+v1+X87XZ7ep26iJ/UCdbhAS6Xy9P4r5YokCzgFefzma7rlnHTNHdVEVkEGGsJoNmGrAIApmlimqZlrNaG7AKMtQTQaYObANBsg6sAQ6kNRQSAThuKCTBKt6G4AHjdBq/PlhBglNgSUgLAf0vICQDfC6SkAMOjDdICIH8b5AUYudpQjQDI04aqBBh7tqFKAbDfzVO1AozULVG9AEhrwyEEGNtrw384lIBPOJSAtm3ffs9vhnW4sw3+zo8x1TcgJTxU3IDU4EaVDdgrPFTWgD2DG9U0IEd4qKABuYIb0g3IHR5EG+AR3JBrgGd4EGvAOrzXozUSAry/9TXFt0DJ8FCwAaWDG0UaoBIenBugFNxwa4BieHBogGpwI6uAcRyXY7XgRhYBwzA8jVXDww7XgO3D0X3fL8fzPJ+Uw0M8Lh9/mAiCIAiCL+YBAFvXYZRLUdUAAAAASUVORK5CYII="));
                    m_whiteArrow.Apply();
                }
                return m_whiteArrow;

            }
        }
    }
}