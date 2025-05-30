using System;
using System.Collections.Generic;
using System.Linq;
using ui.NewUIWindows.Data;
using ui.NewUIWindows.Enum;
using ui.NewUIWindows.Infrastructure.Models.@interface;
using UnityEngine;

namespace ui.NewUIWindows.Infrastructure.Models
{
    public class ModelCardForCollection : IModelCardForCollection
    {
        public event Action<IModelCardForCollection> CardClicked;
        public event Action<IModelCardForCollection> FinishCardClicked;
        public event Action PathIsCollected; 

        private readonly List<SolitaireLayout> _solitaireLayout;
        private List<IModelCard> _modelCards;

        public ModelCardForCollection(TypeCollectionCart typeOfLayout, StateCard stateCard,
            IEnumerable<SolitaireLayout> solitaireLayout, Sprite face)
        {
            State = stateCard;
            Face = face;
            _solitaireLayout = new List<SolitaireLayout>(solitaireLayout);
            TypeOfLayout = typeOfLayout;
            SetIDCard();
        }


        public string IDCard { get; private set; }
        public StateCard State { get; private set; }
        public Sprite Face { get; }
        public TypeCollectionCart TypeOfLayout { get; private set; }

        public void OnButtonClicked() =>
            CardClicked?.Invoke(this);

        public void OnButtonFinishClicked() =>
            FinishCardClicked?.Invoke(this);

        public void CreateCard()
        {
            _modelCards = new List<IModelCard>();

            foreach (SolitaireLayout solitaireLayout in _solitaireLayout)
            {
                if (TypeOfLayout == TypeCollectionCart.Relax)
                    _modelCards.Add(new ModelCardForRelax(solitaireLayout.StateCard, solitaireLayout.Cards,
                        solitaireLayout.NumberCard, solitaireLayout.CloseCardImage, solitaireLayout.OpenCardImage,
                        solitaireLayout.AttentionCardImage));

                else if (TypeOfLayout == TypeCollectionCart.Classic)
                    _modelCards.Add(new ModelCardForClassic(solitaireLayout.StateCard, solitaireLayout.Cards,
                        solitaireLayout.NumberCard, solitaireLayout.CloseCardImage, solitaireLayout.OpenCardImage,
                        solitaireLayout.AttentionCardImage));

                else if (TypeOfLayout == TypeCollectionCart.Journey)
                    _modelCards.Add(new ModelCardsForJourney(solitaireLayout.StateCard, solitaireLayout.Cards,
                        solitaireLayout.NumberCard, solitaireLayout.CloseCardImage, solitaireLayout.OpenCardImage,
                        solitaireLayout.AttentionCardImage));
            }
        }

        public void UpdateCardStateInLayout(StateCard stats, string iDCard)
        {
            IModelCard modelCard = _modelCards.FirstOrDefault(card => card.IDCard == iDCard);
            modelCard!.ChangeStateCard(stats);
            
            if(stats == StateCard.Ready)
                PathIsCollected?.Invoke();
        }

        public void ChangeCardStats(StateCard stats) => 
            State = stats;

        public void ChangeCollectionCardType(TypeCollectionCart stats)
        {
            TypeOfLayout = stats;
            State = StateCard.Ready;
        }

        public List<IModelCard> GetModelCards() =>
            new(_modelCards);

        private void SetIDCard()
        {
            foreach (CardData cardData in _solitaireLayout.SelectMany(layout => layout.Cards))
                IDCard += $"{cardData.SuitCart}{cardData.Value}";
        }
    }
}