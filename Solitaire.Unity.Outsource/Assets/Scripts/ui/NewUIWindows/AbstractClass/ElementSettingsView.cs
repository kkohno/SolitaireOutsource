using System.Collections.Generic;
using TMPro;
using ui.NewUIWindows.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace ui.NewUIWindows.AbstractClass
{
    public abstract class ElementSettingsView : MonoBehaviour
    {
        public ElementSettings Element;
        public List<TMP_Text> InfoInPanel;
        public Image BotLine;
    }
}