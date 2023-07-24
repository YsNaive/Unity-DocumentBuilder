using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.DocumentBuilder
{
    [CreateAssetMenu(menuName ="Naive API/Doc Page")]
    public class SODocPage : ScriptableObject
    {
        public List<SODocPage> SubPages;
        public List<DocComponent> Components;
    }
}
