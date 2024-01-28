using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;

namespace NaiveAPI_Editor.DocumentBuilder
{
    public class DocumentBuilderMenuDefinition : ScriptAPIMenuDefinition
    {
        public override ScriptAPIMenuDefinitionNode TreeRoot =>
            new ScriptAPIMenuDefinitionNode(
                "Document Builder",
                new ScriptAPIMenuDefinitionNode(
                    "Runtime",
                    new ScriptAPIMenuDefinitionNode(
                        "DataType",
                        typeof(DocStyle),
                        typeof(DocComponent)
                        ),
                    new ScriptAPIMenuDefinitionNode(
                        "Misc",
                        typeof(DocCache),
                        typeof(DocRuntime),
                        typeof(TypeReader),
                        typeof(DocumentBuilderParser)
                        ),
                    new ScriptAPIMenuDefinitionNode(
                        "VisualElement",
                        new ScriptAPIMenuDefinitionNode(
                            "DocStyle Element",
                            typeof(DSButton),
                            typeof(DSDropdown),
                            typeof(DSEnumField),
                            typeof(DSFoldout),
                            typeof(DSHorizontal),
                            typeof(DSLabel),
                            typeof(DSScroller),
                            typeof(DSScrollView),
                            typeof(DSStringMenu),
                            typeof(DSTextElement),
                            typeof(DSTextField),
                            typeof(DSToggle),
                            typeof(DSTypeField)
                            ),
                        new ScriptAPIMenuDefinitionNode(
                            "DocType Element",
                            typeof(DocVisual),
                            typeof(DocVisual<>),
                            typeof(DocDescription),
                            typeof(DocLabel),
                            typeof(DocMatrix),
                            typeof(DocItems),
                            typeof(DocImage),
                            typeof(DocSeeAlso),
                            typeof(DocGoogleChart),
                            typeof(DocCodeblock),
                            typeof(DocFuncDisplay),
                            typeof(DocDividline),
                            typeof(DocBookVisual),
                            typeof(DocPageMenu),
                            typeof(DocPageMenuItem),
                            typeof(DocPageVisual)
                            ),
                        new ScriptAPIMenuDefinitionNode(
                            "ScriptAPI Element",
                            typeof(DSScriptAPIElement),
                            typeof(DSFieldInfoElement),
                            typeof(DSPropertyInfoElement),
                            typeof(DSMethodInfoElement),
                            typeof(DSParameterInfoElement),
                            typeof(DSTypeElement),
                            typeof(DSTypeNameElement)
                            ),
                        typeof(GridView),
                        typeof(URLImage)
                        ),// Visual Element
                    new ScriptAPIMenuDefinitionNode(
                        "ScriptAPI Info",
                        typeof(ScriptAPIInfoHolder),
                        typeof(ScriptAPIMenuDefinition)
                        ),
                    new ScriptAPIMenuDefinitionNode(
                        "ScriptableObject",
                        typeof(DocRuntimeData),
                        typeof(SODocStyle),
                        typeof(SODocPage),
                        typeof(SODocComponents),
                        typeof(SOScriptAPIInfo)
                        )
                    ),// Runtime
                new ScriptAPIMenuDefinitionNode(
                    "Editor",
                    new ScriptAPIMenuDefinitionNode(
                        "Misc",
                        typeof(ScriptAPIInfoHandler),
                        typeof(DocumentBuilderMenuDefinition),
                        typeof(DocPageFactory),
                        typeof(DocPageEditorUtils),
                        typeof(DocComponentProperty)
                        ),
                    new ScriptAPIMenuDefinitionNode(
                        "VisualElement",
                        new ScriptAPIMenuDefinitionNode(
                            "DocEditType Element",
                            typeof(CustomDocEditVisualAttribute),
                            typeof(DocComponentField),
                            typeof(DocComponentsField),
                            typeof(DocEditVisual),
                            typeof(DocEditVisual<>),
                            typeof(DocEditDescription),
                            typeof(DocEditLabel),
                            typeof(DocEditMatrix),
                            typeof(DocEditItems),
                            typeof(DocEditImage),
                            typeof(DocEditSeeAlso),
                            typeof(DocEditGoogleChart),
                            typeof(DocEditCodeblock),
                            typeof(DocEditFuncDisplay),
                            typeof(DocEditDividline),
                            typeof(DocPageCreator),
                            typeof(DocPageDeleter)
                            ),
                        typeof(ScriptAPIField),
                        typeof(ScriptAPIMemberField),
                        typeof(DSAssetFolderField)
                        )
                    )// Editor
                );// Document Builder
    }
}