using System.Collections.Generic;
using ui.NewUIWindows.AbstractClass;
using ui.NewUIWindows.ViewWindows.Elements;
using UnityEngine.UI;

namespace ui.NewUIWindows.ViewWindows.Panel
{
    public class PremiumPanelView : PanelView
    {
        public List<ElementPremiumView> ElementsPremium;
        public Button PerManthButton;
        public Button PerManthsButton;
        public Button PremiumYearlyButton;
        public Button RestorePurchasesButton;
        public Button TermsOfServiceButton;
    }
}