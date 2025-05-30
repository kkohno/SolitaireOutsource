using System.Collections.Generic;
using UnityEngine;

namespace ui.NewUIWindows.Data
{
    [CreateAssetMenu(fileName = "ViewDataCards", menuName = "Data UI/View Data Cards")]
    public class ViewDataCards : ScriptableObject
    {
        public List<CardView> Cards;
    }
}