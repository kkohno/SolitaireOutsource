using System.Collections.Generic;
using UnityEngine;

namespace ui.NewUIWindows.Data
{
    [CreateAssetMenu(fileName = "LayoutRelaxation", menuName = "Data UI/Layout Relaxation")]
    public class LayoutRelaxationCollection : ScriptableObject
    {
        public List<CardData> cards = new();
    }
}