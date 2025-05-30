using System.Collections.Generic;
using ui.NewUIWindows.AbstractClass;
using ui.NewUIWindows.ViewCollection;

namespace ui.NewUIWindows.ViewWindows.Panel
{
    public class RelaxPanelView : CollectionPanel
    {
       private List<ViewCardsForRelax> _viewCardsForRelax;
       
       public void SetViewCardsForRelax(List<ViewCardsForRelax> viewCardsForRelax) =>
           _viewCardsForRelax = viewCardsForRelax;

       public List<ViewCardsForRelax> GetViewCardsForRelax() =>
           _viewCardsForRelax;
    }
}