using System;
using System.Collections.Generic;

namespace ui.UIWindowColor.Data
{
    [Serializable]
    public class Collection
    {
        public string Name;
        public int QuantityCollected;
        public int QuantityParts;
        public List<TypeCard> Cards = new();
    }
}