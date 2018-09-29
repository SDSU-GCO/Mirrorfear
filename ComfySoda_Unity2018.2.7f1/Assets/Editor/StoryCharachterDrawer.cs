using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace cs
{
    [CustomPropertyDrawer(typeof(StoryCharachterGameSupervisor.StoryCharacterLoader))]
    public class StoryCharachterDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            //int i=0;
            //while (i< label.text.Length && label.text[i]!=' ')
            //{
            //    i++;
            //}
            //StringBuilder stringBuilder = new StringBuilder();
            //stringBuilder.Append("IDTag, Name, Sprite:");
            //while(i<label.text.Length)
            //{
            //    stringBuilder.Append(label.text[i]);
            //    i++;
            //}

            label.text = "IDTag, Display Name, Sprite:";

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var amountRect = new Rect(position.x, position.y, position.width*0.33f, position.height);
            var unitRect = new Rect(position.x + (position.width * 0.33f), position.y, position.width * 0.33f, position.height);
            var nameRect = new Rect(position.x + (position.width * 0.66f), position.y, position.width * 0.33f, position.height);

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("ID"), GUIContent.none);
            EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("name"), GUIContent.none);
            EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("sprite"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}