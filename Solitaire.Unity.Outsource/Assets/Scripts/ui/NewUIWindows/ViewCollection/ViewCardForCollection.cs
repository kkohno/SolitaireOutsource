using System.Linq;
using TMPro;
using ui.NewUIWindows.Data;
using UnityEngine;
using UnityEngine.UI;

namespace ui.NewUIWindows.ViewCollection
{
    public class ViewCardForCollection : MonoBehaviour
    {
        public Button CardButton;
        public Image ReadyState;
        public Image AttentionState;
        public Image ImageType;
        public TMP_Text TextType;

        public string IDCard { get; private set; }

        public void SetIDCard(TypeCard card)
        {
            foreach (CardData cardData in card.Solitairelayout.SelectMany(layout => layout.Cards)) 
                IDCard += $"{cardData.SuitCart}{cardData.Value}";
        }
    }
}