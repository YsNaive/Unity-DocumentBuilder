using NaiveAPI.DocumentBuilder;
using System;
using System.Text;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [CustomDocEditVisual("Charts/Items")]
    public sealed class DocEditItems : DocEditVisual<DocItems.Data>
    {
        [Obsolete] public override string DisplayName => "Advance/Items";
        public override string VisualID => "8";

        VisualElement fieldContainer;
        protected override void OnCreateGUI()
        {
            var modeSelector = new DSEnumField<DocItems.Mode>("Icon Mode", visualData.Mode, evt =>
            {
                visualData.Mode = evt.newValue;
                SaveDataToTarget();
            });
            Add(modeSelector);
            fieldContainer = new ();
            Add(fieldContainer);
            while (Target.TextData.Count < 1)
                Target.TextData.Add("");
            foreach(var text in Target.TextData)
                fieldContainer.Add(createItemField(text));

            fieldContainer.RegisterCallback<ChangeEvent<string>>(evt => { saveContent(); });
        }

        void saveContent()
        {
            while (Target.TextData.Count < fieldContainer.childCount)
                Target.TextData.Add("");
            while (Target.TextData.Count > fieldContainer.childCount)
                Target.TextData.RemoveAt(Target.TextData.Count - 1);

            int i = 0;
            foreach(var ve in fieldContainer.Children())
            {
                var text = ve.Q<DSTextField>();
                if (text == null) continue;
                Target.TextData[i] = text.value;
                i++;
            }
        }
        VisualElement createItemField(string initValue = "")
        {
            var root = new DSHorizontal();
            var textField = new DSTextField();
            textField.SetValueWithoutNotify(initValue);
            textField.style.flexGrow = 1f;
            var add = new DSButton("+", DocStyle.Current.SuccessColor, () =>
            {
                fieldContainer.Insert(fieldContainer.IndexOf(root)+1, createItemField());
            });
            var sub = new DSButton("-", DocStyle.Current.DangerColor, () =>
            {
                if (fieldContainer.childCount <= 1)
                    textField.value = "";
                else
                    fieldContainer.Remove(root);
                saveContent();
            });
            root.Add(textField);
            root.Add(add);
            root.Add(sub);
            return root;
        }

        public override string ToMarkdown(string dstPath)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str in Target.TextData)
            {
                if (str == Target.TextData[^1])
                    stringBuilder.Append("- " + str);
                else
                    stringBuilder.Append("- " + str).AppendLine("<br>");
            }

            return stringBuilder.ToString();
        }
    }
}
