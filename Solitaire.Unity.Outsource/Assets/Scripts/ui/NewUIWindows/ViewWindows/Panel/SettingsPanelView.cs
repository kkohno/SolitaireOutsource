using System.Collections.Generic;
using ui.NewUIWindows.AbstractClass;
using ui.NewUIWindows.ViewWindows.Elements;
using UnityEngine.UI;

namespace ui.NewUIWindows.ViewWindows.Panel
{
    public class SettingsPanelView : PanelView
    {
        public Button ApplyButton;
        public Button CenselButton;
        public List<SwitcherSettingsView> SettingsViews;
    }
}