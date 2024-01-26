using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                        "DocVisual",
                        typeof(DocVisual),
                        typeof(DocVisual<>),
                        typeof(DocLabel),
                        typeof(DocDescription)
                        )
                    ),
                new ScriptAPIMenuDefinitionNode(
                    "Editor",
                    new ScriptAPIMenuDefinitionNode(
                        "DocVisual",
                        typeof(DocEditVisual),
                        typeof(DocEditVisual<>),
                        typeof(DocEditLabel),
                        typeof(DocEditDescription)
                        )
                    )
                );
    }

}