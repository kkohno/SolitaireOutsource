using System.Collections.Generic;
using ui.NewUIWindows.AbstractClass;
using ui.NewUIWindows.ViewWindows.Elements;
using UnityEngine.UI;

namespace ui.NewUIWindows.ViewWindows.Panel
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