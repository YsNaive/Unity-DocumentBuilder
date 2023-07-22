using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DocPicture : DocVisual

{
    public override string DisplayName => "Image";
    public override VisualElement CreateEditGUI(DocComponent docComponent, int width)
    {
        Texture2D texture;
        if (docComponent.ObjData.Count != 0)
            texture = (Texture2D) docComponent.ObjData[0];
        else
            texture = null;
        Data data = JsonUtility.FromJson<Data>(docComponent.JsonData);
        if (data == null)
            data = new Data();
        VisualElement root = new VisualElement();
        TextField textField = new TextField();
        textField.label = "scale";
        textField.value = data.scale + "";
        textField.style.width = width;
        textField.RegisterValueChangedCallback(value =>
        {
            float.TryParse(value.newValue, out data.scale);
        });
        ObjectField objectField = new ObjectField();
        objectField.objectType = typeof(Texture2D);
        objectField.value = texture;
        objectField.style.width = width;
        root.Add(textField);
        root.Add(objectField);
        return root;
    }

    public override VisualElement CreateViewGUI(DocComponent docComponent, int width)
    {
        VisualElement root = new VisualElement();
        Data data = JsonUtility.FromJson<Data>(docComponent.JsonData);
        if (data == null)
            data = new Data();
        Label label = new Label();
        label.style.width = width * 0.3f;
        label.style.height = 50;
        float imageWidth, height;

        Texture2D texture;
        if (docComponent.ObjData.Count > 0 && docComponent.ObjData[0] != null)
        {
            texture = (Texture2D)docComponent.ObjData[0];
            root.style.backgroundImage = texture;
            if (width < texture.width * data.scale || data.scale == -1)
                imageWidth = width;
            else
                imageWidth = texture.width * data.scale;
            height = texture.height * (imageWidth / texture.width);
            label.visible = false;
        }
        else
        {
            root.style.backgroundColor = Color.black;
            label.text = "No Image";
            imageWidth = width;
            height = imageWidth * 0.6f;
        }
        ISPosition position = new ISPosition();
        root.style.width = imageWidth;
        root.style.height = height;
        position.Left = ISStyleLength.Pixel(imageWidth * 0.35f);
        position.Top = ISStyleLength.Pixel((height - 50) / 2);
        label.style.SetIS_Style(DocStyle.Current.LabelText);
        label.style.unityTextAlign = TextAnchor.MiddleCenter;
        label.style.SetIS_Style(position);
        root.Add(label);

        return root;
    }

    public override DocComponent SaveTo(VisualElement visualElement, DocComponent docComponent)
    {
        Data data = new Data();
        if (!float.TryParse(((TextField)visualElement[0]).value, out data.scale))
            data.scale = -1;

        Texture2D texture = (Texture2D)((ObjectField)visualElement[1]).value;

        docComponent.JsonData = JsonUtility.ToJson(data);
        docComponent.ObjData.Clear();
        docComponent.ObjData.Add(texture);

        return docComponent;
    }

    public class Data
    {
        public float scale = -1;
    }
}
