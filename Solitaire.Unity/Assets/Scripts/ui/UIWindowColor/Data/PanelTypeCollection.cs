using System;
using ui.UIWindowColor.DrowUIAttribute;
using ui.UIWindowColor.Enum;
using UnityEngine;

namespace ui.UIWindowColor.Data
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