using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ui.UIWindowColor.ViewWindows.AbstractClass
{
    public abstract class CollectionPanel : PanelView
    {
        public Image BackgroundContent;
        public Image BackgroundPrize;
        public Image CardImage;
        public Image BotLine;
        public TMP_Text NameCollection;
        public TMP_Text DescriptionCollection;
        public TMP_Text PrizeText;
        public TMP_Text ProgressText;
        public GameObject Content;
    }
}