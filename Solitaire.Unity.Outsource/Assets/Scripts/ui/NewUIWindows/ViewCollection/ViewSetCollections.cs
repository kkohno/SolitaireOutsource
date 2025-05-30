using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ui.NewUIWindows.ViewCollection
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