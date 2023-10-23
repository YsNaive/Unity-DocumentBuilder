using NaiveAPI.DocumentBuilder;
using NaiveAPI_Editor.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.DocumentBuilder
{
    // in progress
    public class DocStylePreviewWindow : EditorWindow
    {
        //[MenuItem("Tools/NaiveAPI/DocumentBuilder/DocStyle Preview", priority = 4)]
        //public static void ShowWindow()
        //{
        //    GetWindow<DocStylePreviewWindow>("DocStyle Preview");
        //}
        ScrollView container;
        VisualElement root;
        SODocPage docPreview;
        ObjectField selectStyle;
        private void CreateGUI()
        {
            docPreview = AssetDatabase.LoadAssetAtPath<SODocPage>(AssetDatabase.GUIDToAssetPath("63540aec46af1414faabf82062162966"));
            root = rootVisualElement;
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            container = new DSScrollView();
            container.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            DocStyle.Current.BeginLabelWidth(ISLength.Pixel(60));
            selectStyle = DocEditor.NewObjectField<SODocStyle>("Style", e =>
            {
                if(e.newValue == null) return;
                RepaintContainer();
            });
            var repaintBtn = DocRuntime.NewButton("Repaint",DocStyle.Current.SuccessColor,() =>
            {
                RepaintContainer();
            });
            DocStyle.Current.EndLabelWidth();
            root.Add(DocRuntime.NewHorizontalBar(1f,selectStyle,null,null,null,repaintBtn));
            root[0].style.SetIS_Style(ISPadding.Pixel(10));
            root[0].style.backgroundColor = DocStyle.Current.BackgroundColor;
            root[0].style.minHeight = 50;
            root.Add(container);
            RepaintContainer();

        }
        public void RepaintContainer()
        {
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            container.Clear();

            container.Add(DocRuntime.NewLabel("This is Label"));
            container.Add(new DSTextElement("This is TextElement"));
            container.Add(new DSTextField("TextField"));
            container.Add(DocRuntime.NewDropdownField("Unity Dropdown", new List<string>() { "Select A", "Select B", "Select C" }));
            container.Add(DocRuntime.NewDropdown("Custom Dropdown", new List<string>() { "Select A", "Select B", "Select C" }, (e) => { Debug.Log("Select " + e); }));
            container.Add(DocRuntime.NewButton("Button", () => { Debug.Log("Click !"); }));
            container.Add(DocRuntime.NewCheckButton("CheckButton", () => { Debug.Log("YES !"); }));
            var foldout = DocRuntime.NewFoldout("Foldout");
            foldout.Add(new DSTextElement("Some contents..."));
            container.Add(foldout);
            var scrollview = new DSScrollView();
            scrollview.Add(new DSTextElement("ScrollView line 1"));
            scrollview.Add(new DSTextElement("ScrollView line 2"));
            scrollview.Add(new DSTextElement("ScrollView line 3"));
            scrollview.Add(new DSTextElement("ScrollView line 4"));
            scrollview.Add(new DSTextElement("ScrollView line 5"));
            scrollview.Add(new DSTextElement("ScrollView line 6"));
            scrollview.Add(new DSTextElement("ScrollView line 7"));
            scrollview.Add(new DSTextElement("ScrollView line 8"));
            scrollview.Add(new DSTextElement("ScrollView line 9"));
            scrollview.style.height = 80;
            container.Add(scrollview);
            container.Add(new DocPageVisual(docPreview));
            foreach (var ve in container.Children())
                ve.style.SetIS_Style(DocStyle.Current.ElementMarginPadding);
        }
    }
}
