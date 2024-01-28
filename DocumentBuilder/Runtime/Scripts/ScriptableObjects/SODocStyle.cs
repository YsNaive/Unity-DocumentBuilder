using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [CreateAssetMenu(menuName = "Naive API/Doc Style",order =3)]
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