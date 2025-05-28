using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace Scripts.DebugLog
{
    public sealed class DebugButtons : MonoBehaviour
    {
        [Inject]
        DiContainer _container;

        [SerializeField]
        DebugMethodButton _buttonPrefab;
        HashSet<object> _objects = new();

        void Awake()
        {
            Register(_container);
        }

        void Register(DiContainer container)
        {
            if (container == null) return;
            // инжектим из родителей
            foreach (var parent in container.ParentContainers)
                Register(parent);

            var atype = typeof(DebugAttribute);

            // инжектим то, что в контейнере
            foreach (var i in container.AllContracts.ToArray()) {
                var attribute = i.Type.GetCustomAttribute(atype, true);
                if (attribute == null) continue;

                Debug.Log($"{i.Type.Name}");
                RegisterObject(container.Resolve(i.Type), (DebugAttribute)attribute);
            }
        }
        public void RegisterObject(object obj, DebugAttribute objectAttribute)
        {
            // ограничитель
            if (obj == null) return;
            // ограничитель повторной регистрации
            if (_objects.Contains(obj)) return;

            // запоминаем объект
            _objects.Add(obj);

            // перебор методов
            foreach (var method in obj.GetType().GetMethods()) {
                var methodAttribute = method.GetCustomAttributes(typeof(DebugAttribute), true);
                if (methodAttribute.Length == 0) continue;
                if (methodAttribute.Length != 1)
                    throw new ArgumentException($"too many attributes {typeof(DebugAttribute)}");
                // создаем кнопку
                Debug.Log($"{objectAttribute.Text}=>{((DebugAttribute)methodAttribute[0]).Text}");
                var button = Instantiate(_buttonPrefab, transform);
                button.Initialize(obj, method, objectAttribute, (DebugAttribute)methodAttribute[0]);
            }

            foreach (var interf in obj.GetType().GetInterfaces()) {
                foreach (var method in interf.GetMethods()) {
                    var methodAttribute = method.GetCustomAttributes(typeof(DebugAttribute), true);
                    if (methodAttribute.Length == 0) continue;
                    if (methodAttribute.Length != 1)
                        throw new ArgumentException($"too many attributes {typeof(DebugAttribute)}");
                    // создаем кнопку
                    Debug.Log($"{objectAttribute.Text}=>{((DebugAttribute)methodAttribute[0]).Text}");
                    var button = Instantiate(_buttonPrefab, transform);
                    button.Initialize(obj, method, objectAttribute, (DebugAttribute)methodAttribute[0]);
                }
            }
        }
    }
}