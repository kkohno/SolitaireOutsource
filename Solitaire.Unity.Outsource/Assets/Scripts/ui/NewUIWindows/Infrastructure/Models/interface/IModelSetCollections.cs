using ui.NewUIWindows.Enum;

namespace ui.NewUIWindows.Infrastructure.Models.@interface
{
    public interface IModelSetCollections
    {
        public string NameCollection { get; }
        public int CollectionQuantityParts { get; }
        public int CollectionQuantityCollected { get; }
        void AddCollectedCardPart();
        public void CreateCard();

        public IModelCardForCollection ChangeCard(StateCard stats, string idCardCollectionCart,
            string idCardStateInLayout = null);

        IModelCardForCollection ChangeType(string idCardCollectionCart,
            TypeCollectionCart typeCollection);

        IModelCardForCollection GetModelCardForCollections(string idCard);
    }
}