using System;
using ui.NewUIWindows.DrowUIAttribute;
using ui.NewUIWindows.Enum;
using UnityEngine;

namespace ui.NewUIWindows.Data
{
    [Serializable]
    public class PanelTypeCollection
    {
        public TypeCollectionCart TypeOfLayout;
        public Sprite SpritePanel;

        [ShowIfEnum("TypeOfLayout", ComparisonType.NotEquals, TypeCollectionCart.Solved, TypeCollectionCart.Default)]
        public string Description;
    }
}