using System.Collections.Generic;
using TMPro;
using ui.NewUIWindows.AbstractClass;
using ui.NewUIWindows.ViewWindows.Elements;
using UnityEngine.UI;

namespace ui.NewUIWindows.ViewWindows.Panel
{
    public class ThemeSettingsPanelView : PanelView
    {
        public List<TMP_Text> InfoInPanel;
        public List<Image> BorderLines;
        public List<StyleButtonView> StyleButtons;
        public List<CardButtonView> CardButtons;
    }
}