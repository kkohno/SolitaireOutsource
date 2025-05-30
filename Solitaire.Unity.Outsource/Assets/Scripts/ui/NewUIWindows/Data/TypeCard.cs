using System;
using System.Collections.Generic;
using ui.NewUIWindows.Enum;
using UnityEngine;

namespace ui.NewUIWindows.Data
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