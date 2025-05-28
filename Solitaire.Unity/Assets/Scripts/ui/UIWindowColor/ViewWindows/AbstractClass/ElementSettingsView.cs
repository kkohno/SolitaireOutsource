using System.Collections.Generic;
using TMPro;
using ui.UIWindowColor.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace ui.UIWindowColor.ViewWindows.AbstractClass
{
    public abstract class ElementSettingsView : MonoBehaviour
    {
        public ElementSettings Element;
        public List<TMP_Text> InfoInPanel;
        public Image BotLine;
    }
}