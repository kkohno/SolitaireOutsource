using System;
using System.Collections.Generic;
using ui.NewUIWindows.Enum;
using UnityEngine;

namespace ui.NewUIWindows.Infrastructure.Models.@interface
{
    public interface IModelCardForCollection
    {
        event Action<IModelCardForCollection> CardClicked;

        event Action<IModelCardForCollection> FinishCardClicked;
        event Action PathIsCollected;

        string IDCard { get; }
        public Sprite Face { get; }
        public StateCard State { get; }
        public TypeCollectionCart TypeOfLayout { get; }

        public void UpdateCardStateInLayout(StateCard stats, string iDCard);

        void ChangeCardStats(StateCard stats);

        void OnButtonClicked();
        void OnButtonFinishClicked();
        public void CreateCard();
        void ChangeCollectionCardType(TypeCollectionCart stats);
        List<IModelCard> GetModelCards();
    }
}