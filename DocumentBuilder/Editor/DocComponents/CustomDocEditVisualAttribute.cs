using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI_Editor.DocumentBuilder
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomDocEditVisualAttribute : Attribute
    {
        public CustomDocEditVisualAttribute(string menuPath, int priority = 0)
        {
            MenuPath = menuPath;
            Priority = priority;
        }
        public float Priority;
        public string MenuPath;
    }

}