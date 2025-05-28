using System.Collections.Generic;
using ui.UIWindowColor.ViewCollection;
using ui.UIWindowColor.ViewWindows.AbstractClass;

namespace ui.UIWindowColor.ViewWindows.Panel
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