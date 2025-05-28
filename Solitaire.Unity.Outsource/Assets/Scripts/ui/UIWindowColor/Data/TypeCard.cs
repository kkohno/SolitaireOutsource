using System;
using System.Collections.Generic;
using ui.UIWindowColor.Enum;
using UnityEngine;

namespace ui.UIWindowColor.Data
{
    [Serializable]
    public class TypeCard
    {
        public TypeCollectionCart TypeOfLayout;
        public StateCard StateCard;
        public Sprite CardFace;
        public List<SolitaireLayout> Solitairelayout;
    }
}