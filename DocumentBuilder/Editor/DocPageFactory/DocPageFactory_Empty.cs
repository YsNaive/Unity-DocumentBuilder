using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocPageFactory_Empty : DocPageFactory
    {
        public override string DisplayName => "Empty";

        public override SODocComponents InitComponents => null;

        public override Texture2D InitIcon => null;

        public override void AfterPageCreate(SODocPage createdPage)
        {
        }

        public override VisualElement CreateEditGUI()
        {
            return new VisualElement();
        }

        public override string IsValid()
        {
            return "";
        }
    }
}