using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocVisualDebugWindow : EditorWindow
    {
        VisualElement root;
        SplitView splitView;
        float propertyChangeTime;
        private void CreateGUI()
        {
            root = rootVisualElement;
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;

            DSScrollView left = new(), rightTop = new(), rightBottom = new();
            var padding = ISPadding.Pixel(10);
            left.style.SetIS_Style(padding);
            rightTop.style.SetIS_Style(padding);
            rightBottom.style.SetIS_Style(padding);
            var rightSplitView = new SplitView(FlexDirection.Column, 50);
            rightSplitView.Add(rightTop);
            rightSplitView.Add(rightBottom);
            splitView = new();
            splitView.Add(left);
            splitView.Add(rightSplitView);
            root.Add(splitView);
            float percent;
            if (!float.TryParse(DocCache.LoadData("DocVisualDebugger.txt"), out percent))
                percent = 80f;
            splitView.SplitPercent = percent;
            left.mode = ScrollViewMode.Vertical;
            left.Add(new DSTextElement("Edit Mode"));
            left.Add(DocVisual.Create(DocDividline.CreateComponent()));
            var component = new DocComponent();
            var field = new DocComponentField(component, true);
            field.style.marginBottom = 15;
            left.Add(field);
            left.Add(new DSTextElement("ViewMode"));
            left.Add(DocVisual.Create(DocDividline.CreateComponent()));
            var visual = DocVisual.Create(component);
            left.Add(visual);

            rightTop.Add(new DSTextElement("Serialized Data"));
            rightTop.Add(DocVisual.Create(DocDividline.CreateComponent()));
            var menuPathText = new DSTextField("MenuPath: ");
            var idText = new DSTextField("ID:");
            var verText = new DSTextField("Version:");
            var jsonText = new DSTextField("JsonData:") { multiline = true };
            var textContainer = new VisualElement();
            var objsContainer = new VisualElement();
            rightTop.Add(idText);
            rightTop.Add(verText);
            rightTop.Add(jsonText);
            rightTop.Add(new DSTextElement("TextData:"));
            rightTop.Add(textContainer);
            rightTop.Add(new DSTextElement("ObjsData:"));
            rightTop.Add(objsContainer);
            rightTop.SetEnabled(false);

            var propertyChangeContainer = new VisualElement();
            propertyChangeContainer.SetEnabled(false);
            rightBottom.Add(new DSTextElement("Property Change"));
            rightBottom.Add(propertyChangeContainer);

            field.OnPropertyChanged += (info) => {
                left.Remove(visual);
                visual = DocVisual.Create(component);
                left.Add(visual);
                if(info.Contains("VisualID"))
                    idText.value = component.VisualID;
                if (info.Contains("VisualVersion"))
                    verText.value = component.VisualVersion.ToString();
                if (info.Contains("JsonData"))
                    jsonText.value = component.JsonData;
                if (info.Contains("TextData"))
                {
                    textContainer.Clear();
                    foreach (var text in component.TextData)
                        textContainer.Add(new DSTextField() { value = text });
                }
                if (info.Contains("ObjsData"))
                {
                    objsContainer.Clear();
                    foreach (var obj in component.ObjsData)
                        objsContainer.Add(new ObjectField() { value = obj });
                }
                if(Time.realtimeSinceStartup - propertyChangeTime > 0.5f)
                    propertyChangeContainer.Clear();
                propertyChangeContainer.Add(new DSTextElement($"- {info}"));
                propertyChangeTime = Time.realtimeSinceStartup;
            };
        }
        private void OnDisable()
        {
            DocCache.SaveData("DocVisualDebugger.txt", splitView.SplitPercent.ToString());
        }
    }
}