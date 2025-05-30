using System.Collections.Generic;
using System.Linq;
using ui.NewUIWindows.Data;
using ui.NewUIWindows.Enum;
using ui.NewUIWindows.Infrastructure.Models.@interface;

namespace ui.NewUIWindows.Infrastructure.Models
{
    public class ModelSetCollections : IModelSetCollections
    {
        private List<IModelCardForCollection> _modelCardForCollections;
        private readonly List<TypeCard> _cards;

        public ModelSetCollections(string name, IEnumerable<TypeCard> cards, int collectionQuantityParts,
            int collectionQuantityCollected)
        {
            NameCollection = name;
            CollectionQuantityParts = collectionQuantityParts;
            CollectionQuantityCollected = collectionQuantityCollected;
            _cards = new List<TypeCard>(cards);
        }

        public string NameCollection { get; }
        public int CollectionQuantityParts { get; private set; }
        public int CollectionQuantityCollected { get; }

        public void CreateCard()
        {
            _modelCardForCollections = new List<IModelCardForCollection>();

            foreach (TypeCard card in _cards)
            {
                IModelCardForCollection modelCardForCollection =
                    new ModelCardForCollection(card.TypeOfLayout, card.StateCard, card.Solitairelayout, card.CardFace);
                modelCardForCollection.CreateCard();
                _modelCardForCollections.Add(modelCardForCollection);
                modelCardForCollection.PathIsCollected += AddCollectedCardPart;
            }
        }

        public void AddCollectedCardPart() =>
            CollectionQuantityParts++;

        public IModelCardForCollection ChangeCard(StateCard stats, string idCardCollectionCart,
            string idCardStateInLayout = null)
        {
            IModelCardForCollection modelCardForCollection =
                _modelCardForCollections.FirstOrDefault(cardForCollection => cardForCollection.IDCard == idCardCollectionCart);

            modelCardForCollection!.ChangeCardStats(stats);

            if (idCardStateInLayout != null)
                modelCardForCollection!.UpdateCardStateInLayout(stats, idCardStateInLayout);

            if (stats == StateCard.Ready)
                modelCardForCollection.PathIsCollected += AddCollectedCardPart;

            return modelCardForCollection;
        }
        
        public IModelCardForCollection ChangeType(string idCardCollectionCart,
            TypeCollectionCart typeCollection)
        {
            IModelCardForCollection modelCardForCollection =
                _modelCardForCollections.FirstOrDefault(cardForCollection => cardForCollection.IDCard == idCardCollectionCart);

            modelCardForCollection!.ChangeCollectionCardType(typeCollection);

            return modelCardForCollection;
        }
        

        public IModelCardForCollection GetModelCardForCollections(string idCard) =>
            _modelCardForCollections.FirstOrDefault(modelCardForCollection => modelCardForCollection.IDCard == idCard);
    }
}