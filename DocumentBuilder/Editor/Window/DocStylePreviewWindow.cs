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
            DocStyle.Current = DocRuntimeData.Instance.CurrentStyle.Get(false);
            docPreview = AssetDatabase.LoadAssetAtPath<SODocPage>(AssetDatabase.GUIDToAssetPath("63540aec46af1414faabf82062162966"));
            root = rootVisualElement;
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            container = DocRuntime.NewScrollView();
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
            if(selectStyle.value != null)
            {
                DocStyle.Current = (selectStyle.value as SODocStyle).Get(false);
            }
            else
            {
                DocStyle.Current = DocRuntimeData.Instance.CurrentStyle.Get(false);
            }
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
            container.Add(new DocPageVisual(docPreview));
            foreach (var ve in container.Children())
                ve.style.SetIS_Style(DocStyle.Current.ElementMarginPadding);
        }
    }
}
