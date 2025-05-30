using TMPro;
using ui.NewUIWindows.AbstractClass;
using ui.NewUIWindows.DrowUIAttribute;
using ui.NewUIWindows.Enum;
using UnityEngine.UI;

namespace ui.NewUIWindows.ViewWindows.Panel
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