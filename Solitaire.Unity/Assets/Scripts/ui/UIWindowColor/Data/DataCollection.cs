using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ui.UIWindowColor.Data
{
    [CreateAssetMenu(fileName = "DataCollection", menuName = "Data UI/Data Collection")]
    public class DataCollection : ScriptableObject
    {
        public Sprite ShirtForClosedCollectionCard;
        public string CollectedDescription;
        public List<PanelTypeCollection> ImageTypeCollections;
        public List<Collection> Collections;
    }
}