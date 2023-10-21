using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.Sample
{
    public class DocStyleElementSample : MonoBehaviour
    {
        UIDocument UID;
        VisualElement root;
        public SODocStyle UsingStyle;
        public SODocPage PreviewPage;
        void Start()
        {
            //if (UsingStyle != null) DocStyle.Current = UsingStyle.Get();
            UID = FindObjectOfType<UIDocument>();
            root = new DocScrollView();
            root.style.backgroundColor = DocStyle.Current.BackgroundColor;
            UID.rootVisualElement.Add(root);

            root.Add(DocRuntime.NewLabel("This is Label"));
            root.Add(new DocTextElement("This is TextElement"));
            root.Add(new DocTextField("TextField"));
            root.Add(DocRuntime.NewDropdownField("Unity Dropdown", new List<string>() { "Select A", "Select B", "Select C" }));
            root.Add(DocRuntime.NewDropdown("Custom Dropdown", new List<string>() { "Select A", "Select B", "Select C" }, (e) => { Debug.Log("Select " + e); }));
            root.Add(DocRuntime.NewButton("Button", () => { Debug.Log("Click !"); }));
            root.Add(DocRuntime.NewCheckButton("CheckButton", () => { Debug.Log("YES !"); }));
            var foldout = DocRuntime.NewFoldout("Foldout");
            foldout.Add(new DocTextElement("Some contents..."));
            root.Add(foldout);
            var scrollview = new DocScrollView();
            scrollview.Add(new DocTextElement("ScrollView line 1"));
            scrollview.Add(new DocTextElement("ScrollView line 2"));
            scrollview.Add(new DocTextElement("ScrollView line 3"));
            scrollview.Add(new DocTextElement("ScrollView line 4"));
            scrollview.Add(new DocTextElement("ScrollView line 5"));
            scrollview.Add(new DocTextElement("ScrollView line 6"));
            scrollview.Add(new DocTextElement("ScrollView line 7"));
            scrollview.Add(new DocTextElement("ScrollView line 8"));
            scrollview.Add(new DocTextElement("ScrollView line 9"));
            scrollview.style.height = 200;
            root.Add(scrollview);
            foreach (var ve in root.Children())
                ve.style.SetIS_Style(DocStyle.Current.ElementMarginPadding);
            root.Add(new DocPageVisual(PreviewPage));
        }
    }
}
