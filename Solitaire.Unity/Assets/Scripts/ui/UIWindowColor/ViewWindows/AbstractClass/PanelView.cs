using System.Collections.Generic;
using TMPro;
using ui.UIWindowColor.DrowUIAttribute;
using UnityEngine;
using UnityEngine.UI;

namespace ui.UIWindowColor.ViewWindows.AbstractClass
{
    public abstract class PanelView : MonoBehaviour
    {
        public TypePanel Name;

        [ShowIfEnum("Name", ComparisonType.NotEquals, TypePanel.Bot, TypePanel.Default)]
        public Image Background;

        [ShowIfEnum("Name", ComparisonType.Equals, TypePanel.ThemeSettings, TypePanel.Settings, TypePanel.Stats,
            TypePanel.Premium, TypePanel.CollectModels, TypePanel.Collection, TypePanel.Classic, TypePanel.Relax, TypePanel.Journey)]
        public Image CloseButton;

        [ShowIfEnum("Name", ComparisonType.NotEquals, TypePanel.Bot, TypePanel.Top, TypePanel.Additional,
            TypePanel.Default)]
        public TMP_Text Title;
    }
}