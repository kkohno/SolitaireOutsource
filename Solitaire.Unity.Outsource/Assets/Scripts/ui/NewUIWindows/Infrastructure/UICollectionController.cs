using System;
using System.Collections.Generic;
using System.Linq;
using ui.NewUIWindows.AbstractClass;
using ui.NewUIWindows.Data;
using ui.NewUIWindows.Enum;
using ui.NewUIWindows.Infrastructure.Models;
using ui.NewUIWindows.Infrastructure.Models.@interface;
using ui.NewUIWindows.ViewCollection;
using ui.NewUIWindows.ViewWindows.Panel;
using UnityEngine;
using UnityEngine.UI;
using CardData = ui.NewUIWindows.Data.CardData;

namespace ui.NewUIWindows.Infrastructure
{
    public class UICollectionController : MonoBehaviour
    {
        public event Action<IModelCard> ButtonClicked;
        public event Action<IModelCardForCollection> FinishButtonClicked;

        public DataCollection CollectionData;
        public ViewDataCards ViewCards;

        public ViewSetCollections PrefabSetCollections;

        public ViewCardForCollection PrefabCardForCollection;
        public ViewCardsForRelax PrefabCardsForRelax;
        public ViewCardForClassic PrefabCardForClassic;

        public ClassicPanelView PrefabClassicPanelView;
        public RelaxPanelView PrefabRelaxPanelView;
        public JourneyPanelView PrefabJourneyPanelView;
        public CollectionPanelView PrefabCollectionPanelView;

        private Dictionary<ViewSetCollections, IModelSetCollections> _collections;

        private GameObject _currentPanel;

        public void InitializeCollection(DataCollection dataLoad = null)
        {
            if (dataLoad == null)
                dataLoad = CollectionData;
            else
                CollectionData = dataLoad;

            _collections = new Dictionary<ViewSetCollections, IModelSetCollections>();

            foreach (Collection collection in dataLoad.Collections)
            {
                IModelSetCollections modelSetCollections = new ModelSetCollections(collection.Name, collection.Cards,
                    collection.QuantityParts, collection.QuantityCollected);
                modelSetCollections.CreateCard();


                List<ViewCardForCollection> viewCardForCollections = new List<ViewCardForCollection>();

                ViewSetCollections setCollection =
                    Instantiate(PrefabSetCollections, PrefabCollectionPanelView.Content.transform);

                setCollection.NameSet.text = collection.Name;
                setCollection.SetViewCardForCollection(viewCardForCollections);

                foreach (TypeCard card in collection.Cards)
                {
                    ViewCardForCollection cardForCollection =
                        Instantiate(PrefabCardForCollection, setCollection.ContentForCards.transform);

                    cardForCollection.SetIDCard(card);
                    SetViewStateCollectionCard(cardForCollection, card.StateCard, collection.Name);

                    viewCardForCollections.Add(cardForCollection);
                }

                _collections.Add(setCollection, modelSetCollections);
            }

            List<ViewCardForClassic> viewCardForClassics = CreateViewCards(PrefabCardForClassic,
                PrefabClassicPanelView.Content.transform,
                TypeCollectionCart.Classic);

            PrefabClassicPanelView.SetViewCardForClassics(viewCardForClassics);

            List<ViewCardsForRelax> viewCardsForRelaxes = CreateViewCards(PrefabCardsForRelax,
                PrefabRelaxPanelView.Content.transform, TypeCollectionCart.Relax);

            PrefabRelaxPanelView.SetViewCardsForRelax(viewCardsForRelaxes);

            Subscribe();
        }


        public void ChangeStateCart(string nameCollection, StateCard state, string idCardCollectionCart,
            string idCardStateInLayout = null)
        {
            IModelSetCollections modelSetCollections =
                _collections.Values.FirstOrDefault(x => x.NameCollection == nameCollection);

            IModelCardForCollection modelCardForCollection =
                modelSetCollections!.ChangeCard(state, idCardCollectionCart, idCardStateInLayout);

            UpdateInformationView(modelCardForCollection, modelSetCollections);
        }

        public void ChangeType(string nameCollection, string idCardCollectionCart, TypeCollectionCart typeCollection)
        {
            var modelSetCollections =
                _collections.Values.FirstOrDefault(x => x.NameCollection == nameCollection);

            IModelCardForCollection modelCardForCollection =
                modelSetCollections!.ChangeType(idCardCollectionCart, typeCollection);
            
            UpdateInformationView(modelCardForCollection, modelSetCollections);
        }

        private void UpdateInformationView(IModelCardForCollection modelCardForCollection, IModelSetCollections modelSetCollections)
        {
            SetInformationForPanel(modelCardForCollection.TypeOfLayout, modelSetCollections,
                modelCardForCollection.Face);

            if (TryResetElementCollection(modelSetCollections) == false)
                return;

            UnsubscribeAll();
            Subscribe();
        }

        private ViewSetCollections GetKeyCollection(IModelSetCollections modelSetCollections) =>
            (from keyValuePair in _collections
                where keyValuePair.Value.NameCollection == modelSetCollections.NameCollection
                select keyValuePair.Key).FirstOrDefault();

        private void Subscribe()
        {
            foreach (ViewSetCollections viewSetCollections in _collections.Keys)
            {
                IModelSetCollections collections = _collections[viewSetCollections];

                foreach (ViewCardForCollection viewCardForCollection in viewSetCollections.ViewCardForCollections)
                {
                    IModelCardForCollection modelCardForCollection =
                        collections.GetModelCardForCollections(viewCardForCollection.IDCard);

                    viewCardForCollection.CardButton.onClick.AddListener(modelCardForCollection!.OnButtonClicked);
                    modelCardForCollection.CardClicked += OnOpenPanel;

                    SetInformationForPanel(modelCardForCollection.TypeOfLayout, collections,
                        modelCardForCollection.Face);

                    if (modelCardForCollection.TypeOfLayout == TypeCollectionCart.Journey)
                        CheckActiveFinishButtonJourney(collections.CollectionQuantityParts,
                            collections.CollectionQuantityCollected, modelCardForCollection);
                }
            }
        }

        private void UnsubscribeAll()
        {
            foreach (ViewSetCollections viewSetCollections in _collections.Keys)
            {
                IModelSetCollections collections = _collections[viewSetCollections];

                foreach (ViewCardForCollection viewCardForCollection in viewSetCollections.ViewCardForCollections)
                {
                    IModelCardForCollection modelCardForCollection =
                        collections.GetModelCardForCollections(viewCardForCollection.IDCard);

                    viewCardForCollection.CardButton.onClick.RemoveListener(modelCardForCollection!.OnButtonClicked);
                    modelCardForCollection.CardClicked -= OnOpenPanel;

                    if (modelCardForCollection.TypeOfLayout != TypeCollectionCart.Journey)
                        return;

                    PrefabJourneyPanelView.FineshCardButton.onClick.RemoveListener(modelCardForCollection
                        .OnButtonFinishClicked);
                    modelCardForCollection.FinishCardClicked -= OnClickedFinishButton;
                }
            }
        }

        private void SetViewStateCollectionCard(ViewCardForCollection cardForCollection, StateCard cardStateCard,
            string collectionNameSet)
        {
            InactiveIconCollectionCard(cardForCollection);

            IModelCardForCollection modelCardForCollection =
                GetModelCardForCollection(collectionNameSet, cardForCollection.IDCard);

            switch (cardStateCard)
            {
                case StateCard.Ready:
                    cardForCollection.ReadyState.gameObject.SetActive(false);
                    cardForCollection.ImageType.sprite = CollectionData.ImageTypeCollections
                        .FirstOrDefault(typeCollection => typeCollection.TypeOfLayout == TypeCollectionCart.Solved)!
                        .SpritePanel;
                    cardForCollection.TextType.text = TypeCollectionCart.Solved.ToString();
                    modelCardForCollection!.ChangeCollectionCardType(TypeCollectionCart.Solved);
                    Unsubscribe(modelCardForCollection);
                    break;
                case StateCard.Closed:
                    cardForCollection.CardButton.image.sprite = CollectionData.ShirtForClosedCollectionCard;
                    Unsubscribe(modelCardForCollection);
                    break;
                case StateCard.Attention:
                    cardForCollection.AttentionState.gameObject.SetActive(true);
                    break;
                case StateCard.Time:
                case StateCard.Open:
                case StateCard.Gray:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cardStateCard), cardStateCard, null);
            }
        }

        private IModelCardForCollection GetModelCardForCollection(string collectionNameSet, string iDCard)
        {
            IModelSetCollections modelSetCollections = _collections.Values.FirstOrDefault(
                collection => collection.NameCollection == collectionNameSet);


            IModelCardForCollection modelCardForCollection =
                modelSetCollections!.GetModelCardForCollections(iDCard);
            return modelCardForCollection;
        }

        private void InactiveIconCollectionCard(ViewCardForCollection cardForCollection)
        {
            cardForCollection.AttentionState.gameObject.SetActive(false);
            cardForCollection.ReadyState.gameObject.SetActive(false);
        }

        private void CheckActiveFinishButtonJourney(int quantityParts, int quantityCollected,
            IModelCardForCollection modelCardForCollection)
        {
            if (quantityParts + 1 != quantityCollected)
                return;

            PrefabJourneyPanelView.FineshCardButton.onClick.AddListener(modelCardForCollection.OnButtonFinishClicked);
            modelCardForCollection.FinishCardClicked += OnClickedFinishButton;
        }

        private void OnClickedFinishButton(IModelCardForCollection modelCardForCollection) =>
            FinishButtonClicked?.Invoke(modelCardForCollection);

        private void SetInformationForPanel(TypeCollectionCart typeOfLayout, IModelSetCollections collections,
            Sprite cardFace)
        {
            switch (typeOfLayout)
            {
                case TypeCollectionCart.Classic:
                    UpdateInfoInPanel(PrefabClassicPanelView, cardFace, collections.CollectionQuantityParts,
                        collections.CollectionQuantityCollected, TypeCollectionCart.Classic
                    );
                    break;
                case TypeCollectionCart.Relax:
                    UpdateInfoInPanel(PrefabRelaxPanelView, cardFace, collections.CollectionQuantityParts,
                        collections.CollectionQuantityCollected, TypeCollectionCart.Relax
                    );
                    break;
                case TypeCollectionCart.Journey:
                    UpdateInfoInPanel(PrefabJourneyPanelView, cardFace, collections.CollectionQuantityParts,
                        collections.CollectionQuantityCollected, TypeCollectionCart.Journey
                    );
                    break;
                case TypeCollectionCart.Solved:
                case TypeCollectionCart.Default:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateInfoInPanel<T>(T prefab, Sprite cardFace, int quantityParts, int quantityCollected,
            TypeCollectionCart parameter)
            where T : CollectionPanel
        {
            prefab.DescriptionCollection.text = CollectionData.ImageTypeCollections
                .FirstOrDefault(panelTypeCollection =>
                    panelTypeCollection.TypeOfLayout == parameter)!.Description;
            prefab.CardImage.sprite = cardFace;
            prefab.ProgressText.text =
                $"{CollectionData.CollectedDescription} {quantityParts} / {quantityCollected}";

            if (parameter == TypeCollectionCart.Journey)
                PrefabJourneyPanelView.FineshCardButton.image.sprite = cardFace;
        }

        private void OnCardClicked(IModelCard modelCard) =>
            ButtonClicked?.Invoke(modelCard);

        private void OnOpenPanel(IModelCardForCollection modelCardForCollection)
        {
            if (_currentPanel.activeInHierarchy)
                _currentPanel.SetActive(false);

            switch (modelCardForCollection.TypeOfLayout)
            {
                case TypeCollectionCart.Classic:
                    _currentPanel = PrefabClassicPanelView.gameObject;
                    ClassicPanelDraw(modelCardForCollection);
                    break;

                case TypeCollectionCart.Relax:
                    _currentPanel = PrefabRelaxPanelView.gameObject;
                    RelaxPanelDraw(modelCardForCollection);
                    break;

                case TypeCollectionCart.Journey:
                    _currentPanel = PrefabJourneyPanelView.gameObject;
                    JourneyPanelDraw(modelCardForCollection);
                    break;

                case TypeCollectionCart.Default:
                case TypeCollectionCart.Solved:
                    Unsubscribe(modelCardForCollection);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _currentPanel.SetActive(true);
        }

        private void JourneyPanelDraw(IModelCardForCollection modelCardForCollection)
        {
            for (int i = 0; i < modelCardForCollection.GetModelCards().Count; i++)
            {
                IModelCard modelCard = modelCardForCollection.GetModelCards()[i];

                SetImageInJourneyCard(i, modelCard);
                SubscribeCardOnPanel(PrefabJourneyPanelView.JourneyCards[i].AttentionCard, modelCard);
                SetViewIconJourneyCard(PrefabJourneyPanelView.JourneyCards[i],
                    modelCard.State);
            }
        }

        private void RelaxPanelDraw(IModelCardForCollection modelCardForCollection)
        {
            for (int i = 0; i < PrefabRelaxPanelView.GetViewCardsForRelax().Count; i++)
            {
                ViewCardsForRelax viewCardForClassic = PrefabRelaxPanelView.GetViewCardsForRelax()[i];
                IModelCard modelCard = modelCardForCollection.GetModelCards()[i];

                SetImageWithRelax(modelCard, viewCardForClassic);
                SubscribeCardOnPanel(viewCardForClassic.ActiveButton, modelCard);
                SetViewIconRelaxCard(viewCardForClassic, modelCard.State);
            }
        }

        private void ClassicPanelDraw(IModelCardForCollection modelCardForCollection)
        {
            for (int i = 0; i < modelCardForCollection.GetModelCards().Count; i++)
            {
                ViewCardForClassic viewCardForClassic = PrefabClassicPanelView.GetViewCardForClassics()[i];
                IModelCard modelCard = modelCardForCollection.GetModelCards()[i];

                SetImageInClassic(viewCardForClassic, modelCard);
                SubscribeCardOnPanel(viewCardForClassic.AttentionCard, modelCard);
                SetViewStateClassicCard(modelCardForCollection.GetModelCards()[i].State,
                    viewCardForClassic);
            }
        }

        private void SetImageInJourneyCard(int index, IModelCard modelCard)
        {
            PrefabJourneyPanelView.JourneyCards[index].AttentionCard.image.sprite =
                modelCard.AttentionCardImage;
            PrefabJourneyPanelView.JourneyCards[index].OpenImage.sprite =
                modelCard.OpenCardImage;
            PrefabJourneyPanelView.JourneyCards[index].CloseImage.sprite =
                modelCard.CloseCardImage;
        }

        private void SetViewStateClassicCard(StateCard solitaireLayoutStateCard, ViewCardForClassic cardsForClassic)
        {
            InactiveIconClassicCard(cardsForClassic);

            switch (solitaireLayoutStateCard)
            {
                case StateCard.Open:
                    cardsForClassic.OpenImage.gameObject.SetActive(true);
                    break;
                case StateCard.Gray:
                    cardsForClassic.AttentionCard.gameObject.SetActive(true);
                    break;
                case StateCard.Ready:
                    cardsForClassic.OpenImage.gameObject.SetActive(true);
                    cardsForClassic.ReadyIcon.gameObject.SetActive(true);
                    break;
                case StateCard.Closed:
                    cardsForClassic.CloseImage.gameObject.SetActive(true);
                    break;
                case StateCard.Attention:
                    cardsForClassic.AttentionCard.gameObject.SetActive(true);
                    cardsForClassic.AttentionIcon.gameObject.SetActive(true);
                    break;
                case StateCard.Time:
                    cardsForClassic.AttentionCard.gameObject.SetActive(true);
                    cardsForClassic.TimeIcon.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(solitaireLayoutStateCard), solitaireLayoutStateCard,
                        null);
            }
        }

        private void SubscribeCardOnPanel(Button button, IModelCard modelCard)
        {
            button.onClick.AddListener(modelCard!.OnButtonClicked);
            modelCard.OnCardClicked += OnCardClicked;
        }

        private bool TryResetElementCollection(IModelSetCollections modelSetCollections)
        {
            ViewSetCollections keyCollection = GetKeyCollection(modelSetCollections);

            if (keyCollection == null)
                return false;

            _collections.Remove(keyCollection);
            _collections.Add(keyCollection, modelSetCollections);
            return true;
        }

        private static void SetImageInClassic(ViewCardForClassic viewCardForClassic, IModelCard modelCard)
        {
            viewCardForClassic.AttentionCard.image.sprite =
                modelCard.AttentionCardImage;
            viewCardForClassic.CloseImage.sprite = modelCard.CloseCardImage;
            viewCardForClassic.OpenImage.sprite = modelCard.OpenCardImage;
        }

        private void SetViewIconJourneyCard(ViewJourneyCard journeyCard, StateCard stateCard)
        {
            InactiveIconJourneyCard(journeyCard);

            switch (stateCard)
            {
                case StateCard.Open:
                    journeyCard.OpenImage.gameObject.SetActive(true);
                    break;
                case StateCard.Gray:
                    journeyCard.AttentionCard.gameObject.SetActive(true);
                    journeyCard.AttentionCard.image.color = Color.grey;
                    break;
                case StateCard.Ready:
                    journeyCard.OpenImage.gameObject.SetActive(true);
                    journeyCard.ReadyIcon.gameObject.SetActive(true);
                    break;
                case StateCard.Closed:
                    journeyCard.CloseImage.gameObject.SetActive(true);
                    break;
                case StateCard.Attention:
                    journeyCard.AttentionCard.gameObject.SetActive(true);
                    journeyCard.AttentionIcon.gameObject.SetActive(true);
                    break;
                case StateCard.Time:
                    journeyCard.AttentionCard.gameObject.SetActive(true);
                    journeyCard.TimeIcon.gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stateCard), stateCard, null);
            }
        }

        private void InactiveIconJourneyCard(ViewJourneyCard journeyCard)
        {
            journeyCard.AttentionCard.gameObject.SetActive(false);
            journeyCard.OpenImage.gameObject.SetActive(false);
            journeyCard.CloseImage.gameObject.SetActive(false);
            journeyCard.AttentionIcon.gameObject.SetActive(false);
            journeyCard.ReadyIcon.gameObject.SetActive(false);
            journeyCard.TimeIcon.gameObject.SetActive(false);
        }

        private void SetImageWithRelax(IModelCard modelCard, ViewCardsForRelax viewCardForClassic)
        {
            for (int y = 0; y < modelCard.GetCardCollectionLayout().Count; y++)
            {
                CardData card = modelCard.GetCardCollectionLayout()[y];
                CardView view = GetViewCard(card);

                SetViewStateRelaxCard(modelCard.State, viewCardForClassic, y, view);
            }
        }

        private void SetViewStateRelaxCard(StateCard stateCard, ViewCardsForRelax cardsForRelax,
            int index, CardView cardView)
        {
            switch (stateCard)
            {
                case StateCard.Open:
                case StateCard.Ready:
                    cardsForRelax.CardsImage[index].sprite = cardView.IconCardFace;
                    break;
                case StateCard.Closed:
                    cardsForRelax.CardsImage[index].sprite = cardView.IconCardBack;
                    break;
                default:
                    cardsForRelax.CardsImage[index].sprite = cardView.IconCardBack;
                    cardsForRelax.CardsImage[index].color = Color.gray;
                    break;
            }
        }


        private void InactiveIconClassicCard(ViewCardForClassic cardsForClassic)
        {
            cardsForClassic.AttentionCard.gameObject.SetActive(false);
            cardsForClassic.CloseImage.gameObject.SetActive(false);
            cardsForClassic.OpenImage.gameObject.SetActive(false);
            cardsForClassic.ReadyIcon.gameObject.SetActive(false);
            cardsForClassic.AttentionIcon.gameObject.SetActive(false);
            cardsForClassic.TimeIcon.gameObject.SetActive(false);
        }

        private List<T> CreateViewCards<T>(T prefab, Transform parent, TypeCollectionCart parameter)
            where T : MonoBehaviour
        {
            List<T> viewCards = new();

            foreach (Collection collection in CollectionData.Collections)
            {
                foreach (SolitaireLayout solitaireLayout in collection.Cards.FirstOrDefault(typeCard =>
                             typeCard.TypeOfLayout == parameter)!.Solitairelayout)
                {
                    T element = Instantiate(prefab, parent);
                    viewCards.Add(element);
                }
            }

            return viewCards;
        }

        private void SetViewIconRelaxCard(ViewCardsForRelax cardsForRelax, StateCard stateCard)
        {
            InactiveIconRelaxCard(cardsForRelax);

            if (stateCard == StateCard.Attention)
            {
                cardsForRelax.AttentionImage.gameObject.SetActive(true);
                cardsForRelax.ActiveButton.gameObject.SetActive(true);
            }
            else if (stateCard == StateCard.Ready)
            {
                cardsForRelax.ReadyImage.gameObject.SetActive(true);
            }
            else if (stateCard == StateCard.Time)
            {
                cardsForRelax.TimeImage.gameObject.SetActive(true);
                cardsForRelax.ActiveButton.gameObject.SetActive(true);
            }
        }

        private void InactiveIconRelaxCard(ViewCardsForRelax cardsForRelax)
        {
            cardsForRelax.AttentionImage.gameObject.SetActive(false);
            cardsForRelax.ReadyImage.gameObject.SetActive(false);
            cardsForRelax.TimeImage.gameObject.SetActive(false);
            cardsForRelax.ActiveButton.gameObject.SetActive(false);
        }

        private CardView GetViewCard(CardData cardData) =>
            ViewCards.Cards.FirstOrDefault(viewCardsCard =>
                cardData.SuitCart == viewCardsCard.SuitCart && cardData.Value == viewCardsCard.Value);

        private void Unsubscribe(IModelCardForCollection modelCardForCollection)
        {
            ViewCardForCollection cardForCollection = GetViewCardForCollection(modelCardForCollection.IDCard);
            cardForCollection.CardButton.onClick.RemoveListener(modelCardForCollection.OnButtonClicked);
            modelCardForCollection.CardClicked -= OnOpenPanel;
        }

        private ViewCardForCollection GetViewCardForCollection(string idCard) =>
            _collections.Keys.SelectMany(viewSetCollections => viewSetCollections.ViewCardForCollections)
                .FirstOrDefault(viewCardForCollection => viewCardForCollection.IDCard == idCard);
    }
}