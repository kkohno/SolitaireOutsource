using System.Collections.Generic;
using TMPro;
using ui.UIWindowColor.ViewWindows.AbstractClass;
using ui.UIWindowColor.ViewWindows.Elements;
using UnityEngine.UI;

namespace ui.UIWindowColor.ViewWindows.Panel
{
    public class SettingsPanelView : PanelView
    {
        public Button ApplyButton;
        public Button CenselButton;
        public List<SwitcherSettingsView> SettingsViews;
    }
}