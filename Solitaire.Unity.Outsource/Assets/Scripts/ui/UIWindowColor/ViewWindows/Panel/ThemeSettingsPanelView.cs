using System.Collections.Generic;
using TMPro;
using ui.UIWindowColor.ViewWindows.AbstractClass;
using ui.UIWindowColor.ViewWindows.Elements;
using UnityEngine.UI;

namespace ui.UIWindowColor.ViewWindows.Panel
{
    public class ThemeSettingsPanelView : PanelView
    {
        public List<TMP_Text> InfoInPanel;
        public List<Image> BorderLines;
        public List<StyleButtonView> StyleButtons;
        public List<CardButtonView> CardButtons;
    }
}