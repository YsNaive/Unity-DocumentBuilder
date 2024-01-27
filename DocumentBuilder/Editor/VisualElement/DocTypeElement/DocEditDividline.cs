using System;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomDocEditVisual("Dividline", 3)]
    public class DocEditDividline : DocEditVisual
    {
        [Obsolete]public override string DisplayName => "Dividline";

        public override string VisualID => "0";

        protected override void OnCreateGUI()
        {}

        public override string ToMarkdown(string dstPath)
        {
            return "\n---";
        }
    }
}
