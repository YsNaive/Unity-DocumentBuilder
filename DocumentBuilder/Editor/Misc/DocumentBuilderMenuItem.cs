using NaiveAPI.DocumentBuilder;
using UnityEditor;
using static UnityEditor.EditorWindow;
namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocumentBuilderMenuItem
    {
        [MenuItem("Tools/NaiveAPI/Script API", priority = 1)]
        #region Script API
        public static void GetScriptAPI()
        {
            GetWindow<ScriptAPIWindow>("Script API");
        }
        #endregion
        [MenuItem("Tools/NaiveAPI/Script API Editor", priority = 2)]
        #region Script API Editor
        public static void GetScriptAPIEditor()
        {
            GetWindow<ScriptAPIEditorWindow>("Script API Editor");
        }
        #endregion

        [MenuItem("Tools/NaiveAPI/Document Builder Docs", priority = 20)]
        #region DocBuilder Docs
        public static void ShowDocumentBuilderDocs()
        {
            GetWindow<DocumentBuilderDocsWindow>("DocumentBuilder docs");
        }
        #endregion
        [MenuItem("Tools/NaiveAPI/Document Editor", priority = 21)]
        #region Doc Editor
        public static void ShowDocEditor()
        {
            m_editorInstance = GetWindow<DocEditorWindow>("Document Editor");
        }
        public static DocEditorWindow DocEditorInstance
        {
            get
            {
                m_editorInstance ??= GetWindow<DocEditorWindow>("Document Editor");
                return m_editorInstance;
            }
        }
        static DocEditorWindow m_editorInstance;
        #endregion
        [MenuItem("Tools/NaiveAPI/DocVisual Debugger", priority = 22)]
        #region DocVisual Debugger
        public static void ShowDocVisualDebug()
        {
            GetWindow<DocVisualDebugWindow>("DocVisual Debugger");
        }
        #endregion
        [MenuItem("Tools/NaiveAPI/Document Exporter", priority = 23)]
        #region Doc Exporter
        public static void ShowDocExporter()
        {
            GetWindow<DocExporterWindow>("Document Exporter");
        }
        #endregion
    }
}