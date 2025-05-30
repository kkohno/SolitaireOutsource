using System;
using System.Collections.Generic;
using ui.NewUIWindows.Data;
using ui.NewUIWindows.Enum;
using UnityEngine;

namespace ui.NewUIWindows.Infrastructure.Models.@interface
{
    public interface IModelCard
    {
        event Action<IModelCard> OnCardClicked;
        string IDCard { get; }
        public StateCard State { get; }
        public int NumberCard { get; }
        public Sprite CloseCardImage { get; }
        public Sprite OpenCardImage { get; }
        public Sprite AttentionCardImage { get; }
        public List<CardData> GetCardCollectionLayout();
        public void ChangeStateCard(StateCard stateCard);
        void OnButtonClicked();
    }
}