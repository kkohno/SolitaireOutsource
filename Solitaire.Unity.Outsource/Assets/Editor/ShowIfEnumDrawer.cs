using System;
using System.Linq;
using ui.NewUIWindows.DrowUIAttribute;
using ui.NewUIWindows.Enum;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomPropertyDrawer(typeof(ShowIfEnumAttribute))]
    public class ShowIfEnumDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            var attr = attribute as ShowIfEnumAttribute;
            var field = new PropertyField(property);
            field.BindProperty(property);

            var path = property.propertyPath;
            var parentPath = path.Contains('.') ? path[..path.LastIndexOf('.')] : "";
            var comparePath = string.IsNullOrEmpty(parentPath) ? attr.FieldName : $"{parentPath}.{attr.FieldName}";
            var compareProperty = property.serializedObject.FindProperty(comparePath);

            if (compareProperty == null)
            {
                Debug.LogWarning($"[ShowIf] Не удалось найти свойство '{comparePath}'");
                container.Add(field);
                return container;
            }

            bool Compare()
            {
                try
                {
                    switch (compareProperty.propertyType)
                    {
                        case SerializedPropertyType.Enum:
                            int enumValue = compareProperty.enumValueIndex;
                            return CompareEnums(enumValue);
                        default:
                            return true;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ShowIf] Ошибка сравнения: {e.Message}");
                    return true;
                }
            }

            bool CompareEnums(int value)
            {
                // Сравниваем с несколькими значениями
                var targetInts = attr.CompareValues.Select(v => Convert.ToInt32(v)).ToArray();
                return attr.Comparison switch
                {
                    ComparisonType.Equals => targetInts.Contains(value),
                    ComparisonType.NotEquals => !targetInts.Contains(value),
                    _ => true
                };
            }

            void UpdateVisibility()
            {
                field.style.display = Compare() ? DisplayStyle.Flex : DisplayStyle.None;
            }

            // Подписываемся на изменение
            field.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                UpdateVisibility();

                var root = field.GetFirstAncestorOfType<VisualElement>();
                if (root != null)
                {
                    var tracker = new VisualElement();
                    tracker.schedule.Execute(() =>
                    {
                        compareProperty.serializedObject.Update();
                        UpdateVisibility();
                    }).Every(100);
                    root.Add(tracker);
                }
            });

            container.Add(field);
            return container;
        }
    }
}