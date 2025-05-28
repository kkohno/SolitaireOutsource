using System;
using System.Collections.Generic;
using ui.UIWindowColor.DrowUIAttribute;
using UnityEngine;

namespace ui.UIWindowColor.Data
{
    [CreateAssetMenu(fileName = "DataStyleWindows", menuName = "Data UI/Data Style Windows")]
    public class DataStyleWindows : ScriptableObject
    {
        public WindowStyle Style;
        public List<WindowDataUI> WindowsData = new();
    }

    [Serializable]
    public class WindowDataUI
    {
    }
}