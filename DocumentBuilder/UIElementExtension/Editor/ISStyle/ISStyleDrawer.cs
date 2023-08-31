using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using Unity.Plastic.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.drawer
{
    [CustomPropertyDrawer(typeof(ISStyle), true)]
    public class ISStyleDrawer : PropertyDrawer
    {
        private static Color backgroundColor = new Color(.15f, .15f, .15f);
        private static Color subBackgroundColor = new Color(.2f, .2f, .2f);
        private static Color labelColor = new Color(.5f, .5f, .5f);
        private static Color subLabelColor = new Color(.275f, .275f, .275f);
        private static float labelSpace = 2f;
        private static string[] options = new string[11]{ "Display","Position", "Flex", "Align",
                                                   "Size", "Margin", "Padding", "Text", 
                                                   "Background","Border", "Radius"};
        private SerializedProperty[] properties = new SerializedProperty[12];
        private bool editEnableMask = false;
        private int newMask = 0;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect rect = position;
            Rect backGroundRect = position;
            backGroundRect.height -= labelSpace;
            backGroundRect.x -= 15;
            backGroundRect.width += 15;
            
            EditorGUI.DrawRect(backGroundRect, backgroundColor);
            drawTopBottomLine(backGroundRect, labelColor);
            rect.height = 20;
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
            if (!property.isExpanded) return;
            rect.y += 5;
            rect.x += 7;
            rect.width -= 10;
            
            Rect btnRect = rect;
            btnRect.y -= 3;
            btnRect.height = 17;
            btnRect.x = EditorGUIUtility.labelWidth;
            btnRect.width = 45;
            if (GUI.Button(btnRect, "open", EditorStyles.miniTextField))
            {
                foreach (var sp in properties)
                    sp.isExpanded = true;
            }
            btnRect.x = btnRect.xMax;
            if (GUI.Button(btnRect, "close", EditorStyles.miniTextField))
            {
                foreach (var sp in properties)
                    sp.isExpanded = false;
            }
            if (((properties[0].intValue & 1) == 1))
            {
                if (!editEnableMask)
                {
                    btnRect.x = btnRect.xMax;
                    btnRect.x += 10;
                    btnRect.width = 120;
                    if (GUI.Button(btnRect, "Edit Mask", EditorStyles.miniTextField))
                    {
                        editEnableMask = true;
                        newMask = properties[0].intValue;
                    }
                }
                else
                {
                    rect.height = 18;
                    rect.width /= 4;
                    btnRect.x = btnRect.xMax;
                    btnRect.x = btnRect.xMax;
                    rect.y = rect.yMax;
                    if (GUI.Button(rect, "Save"))
                    {
                        editEnableMask = false;
                        properties[0].intValue = newMask;
                    }
                    rect.x = rect.xMax;
                    if (GUI.Button(rect, "Cancel"))
                    {
                        editEnableMask = false;
                    }
                    rect.x -= rect.width;
                    rect.width *= 2;
                    rect.height = 20;
                }


                //properties[0].intValue = (EditorGUI.MaskField(rect.NextY(), properties[0].intValue >> 1, options) << 1) + 1;
            }
            if (editEnableMask)
            {
                int m = (int)Mathf.Pow(2,11);
                int nextMask = 0;
                rect.y += 18 * 12 + 6;
                rect.height -= 2;
                Color orgColor = GUI.color;
                for(int i = 10; i>=0; i--)
                {
                    rect.y -= 18;
                    bool val = (newMask & m) == m;
                    GUI.color = val ? new Color(.7f, .9f, .7f) : new Color(.7f, .55f, .55f);
                    if (GUI.Button(rect, options[i]))
                        val = !val;
                    nextMask += val?1:0;
                    nextMask <<= 1;
                    m >>= 1;
                }
                GUI.color = orgColor;
                nextMask++;
                newMask = nextMask;
                rect.y += 18 * 12;
                rect.height += 2;
            }
            else
            {
                int mask = 2;
                for (int i = 1; i < 12; i++)
                {
                    if ((mask & properties[0].intValue) == mask)
                    {
                        rect.y = rect.yMax;
                        rect.height = EditorGUI.GetPropertyHeight(properties[i]);
                        backGroundRect = rect;
                        backGroundRect.x -= 15;
                        backGroundRect.width += 15;
                        EditorGUI.DrawRect(backGroundRect, subBackgroundColor);
                        backGroundRect.height = 20;
                        EditorGUI.DrawRect(backGroundRect, subLabelColor );
                        drawTopBottomLine(backGroundRect, backgroundColor);
                        EditorGUI.PropertyField(rect, properties[i] , true);
                        rect.y += labelSpace;
                    }
                    mask <<= 1;
                }
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            loadProperties(property);
            float output = 20;
            if (property.isExpanded)
            {
                if(editEnableMask)
                    output += 18 * 12 + 16;
                else
                    output += calISHeight();
            }
            output += labelSpace;
            return output;
        }
        private void loadProperties(SerializedProperty property)
        {
            properties[0] = property.FindPropertyRelative("EnableMask");
            properties[1] = property.FindPropertyRelative(options[0]);
            properties[2] = property.FindPropertyRelative(options[1]);
            properties[3] = property.FindPropertyRelative(options[2]);
            properties[4] = property.FindPropertyRelative(options[3]);
            properties[5] = property.FindPropertyRelative(options[4]);
            properties[6] = property.FindPropertyRelative(options[5]);
            properties[7] = property.FindPropertyRelative(options[6]);
            properties[8] = property.FindPropertyRelative(options[7]);
            properties[9] = property.FindPropertyRelative(options[8]);
            properties[10] = property.FindPropertyRelative(options[9]);
            properties[11] = property.FindPropertyRelative(options[10]);
        }
        private float calISHeight()
        {
            float output = 0f;
            int mask = 2;
            for (int i = 1; i < 12; i++)
            {
                if ((mask & properties[0].intValue) == mask)
                {
                    output += EditorGUI.GetPropertyHeight(properties[i]);
                    output += labelSpace;
                }
                mask <<= 1;
            }
            output += 12;
            return output;
        }

        private void drawTopBottomLine(Rect rect, Color color)
        {
            rect.height = 1.5f;
            EditorGUI.DrawRect(rect, color);
            rect.y += 18.5f;
            EditorGUI.DrawRect(rect, color);
            rect.y += 1.5f;
        }
    }
}
