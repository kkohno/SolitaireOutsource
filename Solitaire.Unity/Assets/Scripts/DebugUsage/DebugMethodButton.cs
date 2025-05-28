using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.DebugLog
{
    public sealed class DebugMethodButton : MonoBehaviour
    {
        [SerializeField]
        Button _button;
        [SerializeField]
        Text _text;

        public void Initialize(object obj, MethodInfo method, DebugAttribute objectAttribute, DebugAttribute methodAttribute)
        {
            _text.text = $"{objectAttribute.Text}/{methodAttribute.Text}";
            _button.onClick.AddListener(() => method.Invoke(obj, new object[] { }));
        }
    }
}