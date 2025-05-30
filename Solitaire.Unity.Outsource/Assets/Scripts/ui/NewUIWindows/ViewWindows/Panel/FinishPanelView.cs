using System.Collections.Generic;
using TMPro;
using ui.NewUIWindows.AbstractClass;
using ui.NewUIWindows.ViewWindows.Elements;
using UnityEngine.UI;

namespace ui.NewUIWindows.ViewWindows.Panel
{
    public class FinishPanelView : PanelView
    {
        public TMP_Text TopText;
        public Button NewGameButton;
        public Button RepeatButton;
        public List<ElementStringFinishView> ElementStringStats;
    }
}