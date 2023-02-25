using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DocumentBuilder
{
    public enum DocComponentType
    {
        Description,
        NameAndUsage,
        FuncDisplay,
        Label,
        Picture,
        Matrix,
        DividerLine,
    }
    [System.Serializable]
    public class DocComponent
    {
        public DocComponentType ComponentType;
        public List<string> Text;
        public Texture2D Picture;

        public DocComponent()
        {
            ComponentType = DocComponentType.Description;
            Text = new List<string>();
            Text.Add("");
            Picture = null;
        }
        public DocComponent(DocComponentType docComponentType)
        {
            ComponentType = docComponentType;
            Text = new List<string>();

            switch (ComponentType)
            {
                case DocComponentType.Description:
                    Text.Add("");
                    break;
                case DocComponentType.NameAndUsage:
                    Text.Add("Label");
                    break;
                case DocComponentType.FuncDisplay:
                    Text.Add("void");
                    Text.Add("Func");
                    Text.Add("int\nvalue");
                    Text.Add("description");
                    break;
                case DocComponentType.Label:
                    Text.Add("Label");
                    break;
                case DocComponentType.Picture:
                    Text.Add((0.75f).ToString());
                    break;
                case DocComponentType.DividerLine:
                    Text.Add("");
                    break;
                case DocComponentType.Matrix:
                    Text.Add("%column%");
                    Text.Add("%column%");
                    break;
            }
        }
        public DocComponent(DocComponent docComponent)
        {
            ComponentType = docComponent.ComponentType;
            Text = new List<string>();
            foreach (string str in docComponent.Text)
                Text.Add(new string(str));
            Picture = docComponent.Picture;
        }

        public override string ToString()
        {
            StringBuilder data = new StringBuilder();
            data.Append("%DOC_COMPONENT%");
            data.Append(ComponentType);
            data.Append("%DATA%");
            data.Append(AssetDatabase.GetAssetPath(Picture));
            data.Append("%DATA%");
            bool isFirst = true;
            foreach (var str in Text)
            {
                if (isFirst)
                    isFirst = false;
                else
                    data.Append("%TEXT%");
                data.Append(str);
            }
            return data.ToString();
        }
        public bool FromString(string data)
        {
            if (data.Contains("%DOC_COMPONENT%"))
            {
                string[] datas = data.Split("%DATA%");
                ComponentType = (DocComponentType)Enum.Parse(typeof(DocComponentType), datas[0].Replace("%DOC_COMPONENT%", ""));
                if (ComponentType == DocComponentType.Picture)
                    Picture = (Texture2D)AssetDatabase.LoadAssetAtPath(datas[1], typeof(Texture2D));
                Text.Clear();
                foreach (var str in datas[2].Split("%TEXT%"))
                    Text.Add(str);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}