using System.Collections.Generic;
using ui.NewUIWindows.AbstractClass;
using ui.NewUIWindows.ViewCollection;

namespace ui.NewUIWindows.ViewWindows.Panel
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