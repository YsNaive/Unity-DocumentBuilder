using NaiveAPI_Editor.DocumentBuilder;
using NaiveAPI_Editor.window;
using NaiveAPI_UI;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public class TestWindow : EditorWindow
    {
        static TestWindow window;
        [MenuItem("Tools/NaiveAPI/Test Window")]
        public static void ShowWindow()
        {
            if (window != null)
            {
                window.Close();
                window = null;
            }
            window = CreateWindow<TestWindow>("Test Window");
        }
        private void CreateGUI()
        {
            rootVisualElement.style.SetIS_Style(ISPadding.Pixel(10));
            rootVisualElement.style.backgroundColor = DocStyle.Current.BackgroundColor;
            rootVisualElement.Add(DocRuntime.NewScrollView());
            rootVisualElement[0].Add(new DocComponentField(new DocComponent()));
            Button button = null;
            var curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            button = DocRuntime.NewButton("Test", () =>
            {
                button.GoToPosition(new Vector2(0, 100),20,50, curve, () =>
                {
                    button.GoToPosition(new Vector2(0, 0), 20, 50,curve);
                });
            });
            rootVisualElement.Add(button);
        }
    }
}
