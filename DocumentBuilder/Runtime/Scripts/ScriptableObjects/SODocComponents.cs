using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [CreateAssetMenu(menuName = "Naive API/Doc Components", order = 1)]
    public class SODocComponents : ScriptableObject
    {
        public List<DocComponent> Components = new List<DocComponent>();
    }

}