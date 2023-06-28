using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    [System.Serializable]
    public class SerializableVisualElement<T>:ISerializationCallbackReceiver where T : VisualElement
    {

        public UIDocument UIDocument;
        private VisualElement element;
        [SerializeField, HideInInspector] private string elementName;
        [SerializeField, HideInInspector] private string typeName;
        public T Element
        {
            get
            {
                if (Equals(element, null)) 
                    element = UIDocument.rootVisualElement.Q(elementName);
                return element as T;
            }
        }
        public void OnBeforeSerialize()
        {
            typeName = typeof(T).AssemblyQualifiedName;
        }
        public void OnAfterDeserialize()
        {
        }
    }
}
