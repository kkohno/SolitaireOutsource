using System.Collections.Generic;
using ui.UIWindowColor.ViewCollection;
using ui.UIWindowColor.ViewWindows.AbstractClass;
using UnityEngine.UI;

namespace ui.UIWindowColor.ViewWindows.Panel
{
    public class JourneyPanelView : CollectionPanel
    {
        public Image PathImage;
        public Button FineshCardButton;
        public List<ViewJourneyCard> JourneyCards;
    }
}