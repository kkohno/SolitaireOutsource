using System;
using System.Collections.Generic;
using ui.NewUIWindows.Data;
using ui.NewUIWindows.Enum;
using ui.NewUIWindows.Infrastructure.Models.@interface;
using UnityEngine;

namespace ui.NewUIWindows.Infrastructure.Models
{
    public class ModelCardsForJourney : IModelCard
    {
        private readonly List<CardData> _cards;
        public event Action<IModelCard> OnCardClicked;

        public ModelCardsForJourney(StateCard state, List<CardData> cards, int numberCard, Sprite closeCardImage,
            Sprite openCardImage, Sprite attentionCardImage)
        {
            SetIDCard(cards);
            State = state;
            _cards = cards;
            NumberCard = numberCard;
            CloseCardImage = closeCardImage;
            OpenCardImage = openCardImage;
            AttentionCardImage = attentionCardImage;
        }

        public string IDCard { get; private set; }
        public StateCard State { get; private set; }
        public int NumberCard { get; }
        public Sprite CloseCardImage { get; }
        public Sprite OpenCardImage { get; }
        public Sprite AttentionCardImage { get; }

        public List<CardData> GetCardCollectionLayout() =>
            new(_cards);

        public void ChangeStateCard(StateCard stateCard) =>
            State = stateCard;

        public void OnButtonClicked() =>
            OnCardClicked?.Invoke(this);

        private void SetIDCard(List<CardData> solitaireLayoutCards)
        {
            foreach (CardData cardData in solitaireLayoutCards)
                IDCard += $"{cardData.SuitCart}{cardData.Value}";
        }
    }
}