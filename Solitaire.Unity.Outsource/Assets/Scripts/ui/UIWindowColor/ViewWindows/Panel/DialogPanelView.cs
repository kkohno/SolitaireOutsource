using TMPro;
using ui.UIWindowColor.DrowUIAttribute;
using ui.UIWindowColor.ViewWindows.AbstractClass;
using UnityEngine.UI;

namespace ui.UIWindowColor.ViewWindows.Panel
{
    public class DialogPanelView : PanelView
    {
        [ShowIfEnum("Name", ComparisonType.Equals, TypePanel.DialogWith2buttonsRow, TypePanel.DialogWith3Buttons)]
        public TMP_Text InfoInPanelText;

        public Button OkButton;
        public Button CancelButton;

        [ShowIfEnum("Name", ComparisonType.Equals, TypePanel.DialogWith3Buttons)]
        public Button SolveButton;
    }
}