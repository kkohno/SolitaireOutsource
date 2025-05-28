using System.Collections.Generic;
using ui.UIWindowColor.ViewCollection;
using ui.UIWindowColor.ViewWindows.AbstractClass;

namespace ui.UIWindowColor.ViewWindows.Panel
{
    public class ClassicPanelView : CollectionPanel
    {
        private List<ViewCardForClassic> _viewCardForClassics;

        public void SetViewCardForClassics(List<ViewCardForClassic> viewCardForClassics) =>
            _viewCardForClassics = viewCardForClassics;

        public List<ViewCardForClassic> GetViewCardForClassics() =>
            _viewCardForClassics;
    }
}