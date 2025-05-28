using TMPro;
using ui.UIWindowColor.DrowUIAttribute;
using ui.UIWindowColor.Enum;
using ui.UIWindowColor.ViewWindows.AbstractClass;
using UnityEngine.UI;

namespace ui.UIWindowColor.ViewWindows.Elements
{
    public class SwitcherSettingsView : ElementSettingsView
    {
        [ShowIfEnum("Element", ComparisonType.Equals, ElementSettings.Rules)]
        public Toggle Default;
        
        [ShowIfEnum("Element", ComparisonType.Equals, ElementSettings.Rules)]
        public Image DefaultOff;
       
        [ShowIfEnum("Element", ComparisonType.Equals, ElementSettings.Rules)]
        public Image DefaultOn;

        [ShowIfEnum("Element", ComparisonType.Equals, ElementSettings.Rules)]
        public Toggle WindowsStyle;
        
        [ShowIfEnum("Element", ComparisonType.Equals, ElementSettings.Rules)]
        public Image WindowsStyleOff;
        
        [ShowIfEnum("Element", ComparisonType.Equals, ElementSettings.Rules)]
        public Image WindowsStyleOn;

        [ShowIfEnum("Element", ComparisonType.Equals, ElementSettings.Draw)]
        public Toggle Card1;
        
        [ShowIfEnum("Element", ComparisonType.Equals, ElementSettings.Draw)]
        public Image Card1Off;
        
        [ShowIfEnum("Element", ComparisonType.Equals, ElementSettings.Draw)]
        public Image Card1On;

        [ShowIfEnum("Element", ComparisonType.Equals, ElementSettings.Draw)]
        public Toggle Card1Visible3;
        
        [ShowIfEnum("Element", ComparisonType.Equals, ElementSettings.Draw)]
        public Image Card1Visible3Off;
        
        [ShowIfEnum("Element", ComparisonType.Equals, ElementSettings.Draw)]
        public Image Card1Visible3On;
        
        [ShowIfEnum("Element", ComparisonType.NotEquals, ElementSettings.Draw, ElementSettings.Rules)]
        public Button Button;
    }
}