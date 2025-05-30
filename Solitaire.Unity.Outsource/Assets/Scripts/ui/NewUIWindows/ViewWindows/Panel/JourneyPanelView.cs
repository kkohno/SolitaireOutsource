using System.Collections.Generic;
using ui.NewUIWindows.AbstractClass;
using ui.NewUIWindows.ViewCollection;
using UnityEngine.UI;

namespace ui.NewUIWindows.ViewWindows.Panel
{
    public class JourneyPanelView : CollectionPanel
    {
        public Image PathImage;
        public Button FineshCardButton;
        public List<ViewJourneyCard> JourneyCards;
    }
}