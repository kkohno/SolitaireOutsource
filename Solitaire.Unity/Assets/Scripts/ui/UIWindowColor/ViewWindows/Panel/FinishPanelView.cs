using System.Collections.Generic;
using TMPro;
using ui.UIWindowColor.ViewWindows.AbstractClass;
using ui.UIWindowColor.ViewWindows.Elements;
using UnityEngine.UI;

namespace ui.UIWindowColor.ViewWindows.Panel
{
    public class FinishPanelView : PanelView
    {
        public TMP_Text TopText;
        public Button NewGameButton;
        public Button RepeatButton;
        public List<ElementStringFinishView> ElementStringStats;
    }
}