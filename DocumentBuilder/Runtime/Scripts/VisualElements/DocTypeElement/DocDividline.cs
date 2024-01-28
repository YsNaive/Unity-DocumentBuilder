namespace NaiveAPI.DocumentBuilder
{
    public class DocDividline : DocVisual
    {
        public override string VisualID => "0";

        protected override void OnCreateGUI()
        {
            style.height = DocStyle.Current.MainTextSize *0.3f;
            style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            style.marginTop = style.height;
            style.marginBottom = style.height;
        }
        public static DocComponent CreateComponent()
        {
            DocComponent component = new DocComponent();
            component.VisualID = "0";

            return component;
        }
    }
}
