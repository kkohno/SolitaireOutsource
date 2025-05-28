using System;
using UnityEngine;

namespace ui.UIWindowColor.DrowUIAttribute
{
    public class ShowIfEnumAttribute : PropertyAttribute
    {
        public string FieldName;
        public ComparisonType Comparison;
        public object[] CompareValues;

        public ShowIfEnumAttribute(string fieldName, ComparisonType comparison, params object[] compareValues)
        {
            FieldName = fieldName;
            Comparison = comparison;
            CompareValues = compareValues;
        }
    }
}