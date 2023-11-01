using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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