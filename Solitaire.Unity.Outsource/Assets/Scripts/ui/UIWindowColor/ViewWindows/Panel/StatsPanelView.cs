using System.Collections.Generic;
using ui.UIWindowColor.ViewWindows.AbstractClass;
using ui.UIWindowColor.ViewWindows.Elements;
using UnityEngine.UI;

namespace ui.UIWindowColor.ViewWindows.Panel
{
    public class StatsPanelView : PanelView
    {
        public List<ElementStringStatsView> Elements;
        public Button GeneralButton;
        public Button Inlast30daysButton;
        public Button ResetStatsButton;
        public Button LogIButton;
    }
}