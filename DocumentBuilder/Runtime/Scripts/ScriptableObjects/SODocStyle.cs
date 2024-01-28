using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [CreateAssetMenu(menuName = "Naive API/DocumentBuilder/new DocStyle")]
    public class SODocStyle : ScriptableObject
    {
        [SerializeField]
        private DocStyle DocStyle = new DocStyle();

        public DocStyle Get()
        {
            return DocStyle.Copy();
        }
    }
}