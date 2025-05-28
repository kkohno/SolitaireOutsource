using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Library
{
    public sealed class ActivateDeactivateToggle : MonoBehaviour
    {
        [SerializeField]
        Toggle _toggle;
        [SerializeField]
        bool _invert;
        [SerializeField]
        GameObject[] _objects;

        void Awake()
        {
            _toggle.onValueChanged.AddListener(x => {
                var value = x;
                if (_invert) value = !value;
                foreach (var o in _objects) {
                    o.SetActive(value);
                }
            });
        }
    }
}