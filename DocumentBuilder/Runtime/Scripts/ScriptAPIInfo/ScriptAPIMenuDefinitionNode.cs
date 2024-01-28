using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    public abstract class ScriptAPIMenuDefinition
    {
        public abstract ScriptAPIMenuDefinitionNode TreeRoot { get; }
        public VisualElement CreateFoldoutHierarchy(Action<DSFoldout> onFoldoutCreated, Action<DSTypeNameElement> onTypeCreated)
        { return CreateFoldoutHierarchy(TreeRoot, onFoldoutCreated, onTypeCreated); }
        public VisualElement CreateFoldoutHierarchy(ScriptAPIMenuDefinitionNode node,Action<DSFoldout> onFoldoutCreated, Action<DSTypeNameElement> onTypeCreated)
        {
            if (node.IsFolder)
            {
                var root = new DSFoldout(node.Name);
                root.contentContainer.style.paddingLeft = 0;
                root.value = false;
                foreach (var subNode in node)
                    root.Add(CreateFoldoutHierarchy(subNode, onFoldoutCreated, onTypeCreated));
                onFoldoutCreated?.Invoke(root);
                return root;
            }
            else
            {
                var root = new DSTypeNameElement(node.Type);
                onTypeCreated?.Invoke(root);
                return root;
            }
        }
    }
    public class ScriptAPIMenuDefinitionNode : IEnumerable<ScriptAPIMenuDefinitionNode>
    {
        public bool IsFolder => Type == null;

        public Type Type = null;
        public string Name = "";
        public ScriptAPIMenuDefinitionNode[] Definitions;
        public ScriptAPIMenuDefinitionNode() {}
        public ScriptAPIMenuDefinitionNode(string folderName, params ScriptAPIMenuDefinitionNode[] def)
        {
            this.Name = folderName;
            Definitions = def;
        }
        public static implicit operator ScriptAPIMenuDefinitionNode(Type type)
        {
            var result = new ScriptAPIMenuDefinitionNode();
            result.Type = type; 
            return result;
        }

        public IEnumerator<ScriptAPIMenuDefinitionNode> GetEnumerator()
        {
            foreach(var subNode in Definitions)
                yield return subNode;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}