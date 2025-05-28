using System.Collections.Generic;
using ui.UIWindowColor.ViewWindows.AbstractClass;
using ui.UIWindowColor.ViewWindows.Elements;
using UnityEngine.UI;

namespace ui.UIWindowColor.ViewWindows.Panel
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