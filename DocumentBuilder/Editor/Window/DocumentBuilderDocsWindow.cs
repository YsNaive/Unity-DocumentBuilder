using NaiveAPI_Editor.DocumentBuilder;
using UnityEditor;

namespace NaiveAPI.DocumentBuilder
{
    public class DocumentBuilderDocsWindow : EditorWindow
    {
        private void CreateGUI()
        {
            SODocPage rootPage = DocEditorData.Instance.DocumentBuilderDocsRoot;
            rootVisualElement.Add(new DocBookVisual(rootPage) { DontPlayAnimation = true});
        }
    }
}
