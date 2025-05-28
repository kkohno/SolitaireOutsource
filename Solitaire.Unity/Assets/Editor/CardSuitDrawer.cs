using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.ComponentModel;

namespace Editor
{
    [CustomPropertyDrawer(typeof(Suit))]
    [CustomPropertyDrawer(typeof(ValueName))]
    public class CardSuitDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var enumType = fieldInfo.FieldType;
            var enumNames = Enum.GetNames(enumType);
            var enumValues = Enum.GetValues(enumType);

            string[] displayNames = new string[enumNames.Length];

            for (int i = 0; i < enumNames.Length; i++)
            {
                var member = enumType.GetField(enumNames[i]);
                var description = member.GetCustomAttribute<DescriptionAttribute>();
                displayNames[i] = description != null ? description.Description : enumNames[i];
            }

            property.enumValueIndex = EditorGUI.Popup(position, label.text, property.enumValueIndex, displayNames);

            EditorGUI.EndProperty();
        }
    }
}