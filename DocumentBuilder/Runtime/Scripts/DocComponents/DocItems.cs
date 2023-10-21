using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class DocItems : DocVisual
    {
        public override string VisualID => "8";

        private VisualElement itemsVisual;
        private Texture2D texture;

        protected override void OnCreateGUI()
        {
            Data data = JsonUtility.FromJson<Data>(Target.JsonData);
            if (data == null) return;
            itemsVisual = generateItemsVisual(data);
            this.Add(itemsVisual);
            this.RegisterCallback<GeometryChangedEvent>(iconResize);
        }

        private VisualElement generateItemVisual(string description)
        {
            VisualElement root = DocRuntime.NewEmptyHorizontal();

            VisualElement icon = DocRuntime.NewEmpty();
            if (Target.ObjsData[0] is Texture2D)
            texture = (Texture2D)Target.ObjsData[0];
            icon.style.backgroundImage = texture;
            icon.style.unityBackgroundImageTintColor = DocStyle.Current.SubFrontgroundColor;

            TextElement descriptionField = new DocTextElement(description);
            descriptionField.style.paddingLeft = DocStyle.Current.MainTextSize;

            root.Add(icon);
            root.Add(descriptionField);

            return root;
        }

        private void iconResize(GeometryChangedEvent e)
        {
            foreach (VisualElement child in itemsVisual.Children())
            {
                child[0].style.width = child[1].resolvedStyle.height;
                child[0].style.height = child[1].resolvedStyle.height;
            }
            this.UnregisterCallback<GeometryChangedEvent>(iconResize);
        }

        private VisualElement generateItemsVisual(Data data)
        {
            VisualElement root = DocRuntime.NewEmpty();

            for (int i = 0;i < data.num; i++)
            {
                root.Add(generateItemVisual(Target.TextData[i]));
            }

            return root;
        }

        public class Data
        {
            public int num = 1;
        }
    }
}
