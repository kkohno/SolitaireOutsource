using System.Collections.Generic;
using TMPro;
using ui.UIWindowColor.Data;
using UnityEngine;

namespace ui.UIWindowColor.ViewCollection
{
    public class ViewSetCollections : MonoBehaviour
    {
        public TMP_Text NameSet;
        public GameObject ContentForCards;
        public List<ViewCardForCollection> ViewCardForCollections;
        
        public void SetViewCardForCollection(List<ViewCardForCollection> viewCardForCollection) => 
            ViewCardForCollections = viewCardForCollection;
    }
}