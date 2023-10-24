using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocPageFactory_MethodAPI : DocPageFactory
    {
        public override string DisplayName => "Method";

        public override SODocComponents InitComponents => throw new System.NotImplementedException();

        public override Texture2D InitIcon => throw new System.NotImplementedException();

        public override void AfterPageCreate(SODocPage createdPage)
        {
            throw new System.NotImplementedException();
        }

        public override VisualElement CreateEditGUI()
        {
            throw new System.NotImplementedException();
        }

        public override string IsValid()
        {
            throw new System.NotImplementedException();
        }
    }

}