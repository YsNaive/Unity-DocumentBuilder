using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [CreateAssetMenu(menuName = "Naive API/DocumentBuilder/DocComponents")]
    public class SODocComponents : ScriptableObject
    {
        public List<DocComponent> Components = new List<DocComponent>();
    }

}