using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    public class DocGoogleChart : DocVisual<DocGoogleChart.Data>
    {
        public enum ChType
        {
            TexFormula,
        }
        public class Data
        {
            List<(string from, string to)> ASCIITable = new() {
                ("+","%2B"),
                ("\\","%5C"),
                (" ","+"),
            };
            public ChType ChType = ChType.TexFormula;
            public int ChHeightPx = 25;
            public string Contents = "";
            public float Scale = 1f;
            public string ToURL()
            {
                StringBuilder sb = new();
                sb.Append("https://chart.apis.google.com/chart?cht=");
                if(ChType == ChType.TexFormula)
                {
                    sb.Append("tx");
                    sb.Append("&chl=");
                    sb.Append("&chf=bg,s,00000000&chco=").Append(ColorUtility.ToHtmlStringRGB(DocStyle.Current.LabelText.Color));
                    sb.Append("&chs=").Append(ChHeightPx);
                    string encodeContent = Contents;
                    foreach (var pair in ASCIITable)
                        encodeContent = encodeContent.Replace(pair.from, pair.to);
                    sb.Append("&chl=").Append(encodeContent);
                }
                return sb.ToString();
            }
        }
        public override string VisualID => "9";

        protected override void OnCreateGUI()
        {
            if(visualData.Contents == "")
            {
                Add(DocDescription.Create("Empty Formula", DocDescription.DescriptionType.Warning));
                return;
            }
            Add(DocImage.Create(visualData.ToURL(), visualData.Scale));
        }

    }

}