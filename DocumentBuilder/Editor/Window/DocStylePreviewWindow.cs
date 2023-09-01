using NaiveAPI.DocumentBuilder;
using NaiveAPI_Editor.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocStylePreviewWindow : EditorWindow
    {
        [MenuItem("Tools/NaiveAPI/DocumentBuilder/DocStyle Preview", priority = 4)]
        public static void ShowWindow()
        {
            GetWindow<DocStylePreviewWindow>("DocStyle Preview");
        }
        ScrollView container;
        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            container = DocRuntime.NewScrollView();
            container.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            var selectStyle = DocEditor.NewObjectField<SODocStyle>("Style", e =>
            {
                if(e.newValue == null) return;
                DocStyle.Current = (e.newValue as SODocStyle).Get();
            });
            selectStyle.style.backgroundColor = DocStyle.Current.BackgroundColor;
            selectStyle.style.SetIS_Style(ISPadding.Pixel(10));
            root.Add(selectStyle);
            root.Add(container);
            root.schedule.Execute(() =>
            {
                root.style.backgroundColor = DocStyle.Current.BackgroundColor;
                container.Clear();

                container.Add(DocRuntime.NewLabel("This is Label"));
                container.Add(DocRuntime.NewTextElement("This is TextElement"));
                container.Add(DocRuntime.NewTextField("TextField"));
                container.Add(DocRuntime.NewDropdownField("Unity Dropdown", new List<string>() { "Select A", "Select B", "Select C" }));
                container.Add(DocRuntime.NewDropdown("Custom Dropdown", new List<string>() { "Select A", "Select B", "Select C" }, (e) => { Debug.Log("Select " + e); }));
                container.Add(DocRuntime.NewButton("Button", () => { Debug.Log("Click !"); }));
                container.Add(DocRuntime.NewCheckButton("CheckButton", () => { Debug.Log("YES !"); }));
                var foldout = DocRuntime.NewFoldout("Foldout");
                foldout.Add(DocRuntime.NewTextElement("Some contents..."));
                container.Add(foldout);
                var scrollview = DocRuntime.NewScrollView();
                scrollview.Add(DocRuntime.NewTextElement("ScrollView line 1"));
                scrollview.Add(DocRuntime.NewTextElement("ScrollView line 2"));
                scrollview.Add(DocRuntime.NewTextElement("ScrollView line 3"));
                scrollview.Add(DocRuntime.NewTextElement("ScrollView line 4"));
                scrollview.Add(DocRuntime.NewTextElement("ScrollView line 5"));
                scrollview.Add(DocRuntime.NewTextElement("ScrollView line 6"));
                scrollview.Add(DocRuntime.NewTextElement("ScrollView line 7"));
                scrollview.Add(DocRuntime.NewTextElement("ScrollView line 8"));
                scrollview.Add(DocRuntime.NewTextElement("ScrollView line 9"));
                scrollview.style.height = 80;
                container.Add(scrollview);
                foreach (var ve in container.Children())
                    ve.style.SetIS_Style(DocStyle.Current.ElementMarginPadding);
            }).Every(500);
        }
    }
}
