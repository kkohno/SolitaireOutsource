using System;
using System.Collections.Generic;
using ui.NewUIWindows.DrowUIAttribute;
using ui.NewUIWindows.Enum;
using UnityEngine;

namespace ui.NewUIWindows.Data
{
    [Serializable]
    public class SolitaireLayout
    {
        public TypeCollectionCart SetTheSameType;
        public StateCard StateCard;
        
        [ShowIfEnum("SetTheSameType", ComparisonType.Equals, TypeCollectionCart.Journey)]
        public int NumberCard;

        [ShowIfEnum("SetTheSameType", ComparisonType.Equals, TypeCollectionCart.Journey, TypeCollectionCart.Classic)]
        public Sprite CloseCardImage;

        [ShowIfEnum("SetTheSameType", ComparisonType.Equals, TypeCollectionCart.Journey, TypeCollectionCart.Classic)]
        public Sprite OpenCardImage;

        [ShowIfEnum("SetTheSameType", ComparisonType.Equals, TypeCollectionCart.Journey, TypeCollectionCart.Classic)]
        public Sprite AttentionCardImage;

        public List<CardData> Cards;
    }
}