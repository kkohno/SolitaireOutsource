using Scripts.Debuging;
using Scripts.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ui;
using ui.windows;
using UnityEngine;
using Zenject;

public class CardManager : MonoBehaviour
{
    [Inject]
    IDecksService _decks;

    public GameObject cardPrefab;

    private string shuffleName;
    private Dictionary<GameType, List<int[]>> winnable;
    private Card[] shuffledCards;
    private Card[] cards;

    public delegate void CardEvent(CardEventArgs a);

    public event CardEvent CardPressed;
    public event CardEvent CardReleased;

    GameObject cardRoot;

    public const int CARD_COUNT = 52;

    public void Init(GameObject cardRoot)
    {
        this.cardRoot = cardRoot;
        InitCards();

        ApplyTheme();
        ApplyCardBack();
        ApplyCardsStyle();

        ThemeManager.ThemeLoaded += ApplyTheme;
        ThemeSettings.CardBackChanged += ApplyCardBack;
        ThemeSettings.CardStyleChanged += ApplyCardsStyle;
    }

    /// <summary>
    /// Creates the card object instance from prefab.
    /// </summary>
    /// <returns>The card object instance.</returns>
    /// <param name="i">The card index. Must be from 0 to 52. Indicates value and suit of the card.</param>
    private Card CreateCardObject(int i)
    {
        Card c = Instantiate(cardPrefab, cardRoot.transform).GetComponent<Card>();
        c.Index = i;
        c.name = string.Format("card_{0}({1})", i, Card.GetCardName(c));
        c.gameObject.SetActive(false);

        //subscribe on user input events
        c.Pressed += OnCardPressed;
        c.Released += OnCardReleased;

        return c;
    }

    private void OnCardPressed(CardEventArgs a)
    {
        if (CardPressed != null)
            CardPressed(a);
    }

    private void OnCardReleased(CardEventArgs a)
    {
        if (CardReleased != null)
            CardReleased(a);
    }

    /// <summary>
    /// Inits the cards. Creates card objects or simply resets them if they were created earlier.
    /// </summary>
    private void InitCards()
    {
        if (cards == null) {
            //create cards, apply theme textures
            cards = new Card[CARD_COUNT];
            for (int i = 0; i < CARD_COUNT; i++) {
                cards[i] = CreateCardObject(i);
            }
        }
        else {
            foreach (var c in cards) {
                c.transform.rotation = Quaternion.identity;
                c.transform.localScale = Vector2.one;
                c.ShadowSize = Vector2.one;
                c.ShadowPosition = Vector2.zero;
                c.SpriteRendererEnabled = true;
            }
        }
    }

    /// <summary>
    /// Applies the cards style sprites to all the cards.
    /// </summary>
    private void ApplyCardsStyle()
    {
        for (int i = 0; i < cards.Length; i++) {
            cards[i].SetCardStyle(ThemeManager.CardSprite(i));
        }
    }

    /// <summary>
    /// Applies the theme to game objects.
    /// </summary>
    private void ApplyTheme()
    {
        for (int i = 0; i < cards.Length; i++) {
            cards[i].SetGlowSprite(ThemeManager.GlowSprite);
            cards[i].SetHintSprite(ThemeManager.HintSprite);
        }
    }

    /// <summary>
    /// Applies the card back sprite to all the cards.
    /// </summary>
    private void ApplyCardBack()
    {
        for (int i = 0; i < cards.Length; i++) {
            cards[i].SetCardBack(ThemeManager.BackSprite);
        }
    }

    /// <summary>
    /// Gets the shuffled cards array.
    /// </summary>
    /// <value>The shuffled cards.</value>
    public Card[] ShuffledCards
    {
        get { return shuffledCards; }
    }

    /// <summary>
    /// Gets or sets the cards array (not shuffled).
    /// </summary>
    /// <value>The cards array.</value>
    public Card[] Cards
    {
        get { return cards; }
        set { cards = value; }
    }

    /// <summary>
    /// Gets the name of the current shuffle.
    /// </summary>
    /// <value>The name of the shuffle.</value>
    public string ShuffleName
    {
        get { return shuffleName; }
    }

    /// <summary>
    /// Sets the cards visibility.
    /// </summary>
    /// <param name="value">If set to <c>true</c> then cards are visible.</param>
    public void SetCardsVisibility(bool value)
    {
        foreach (var c in cards) {
            //make card invisible by changing its layer to something what every camera will ignore (cull)
            c.Layer = value ? UILayout.DEFAULT_LAYER_ID : UILayout.IGNORE_LAYER_ID;
        }
    }

    /// <summary>
    /// Sets the shuffled array.
    /// </summary>
    /// <param name="newShuffle">New shuffle.</param>
    public void SetShuffle(Card[] newShuffle)
    {
        shuffledCards = newShuffle;
    }

    /// <summary>
    /// Gets a value indicating whether all the cards are face-up.
    /// </summary>
    /// <value><c>true</c> if all cards are face-up; otherwise, <c>false</c>.</value>
    public bool IsAllCardsUp
    {
        get
        {

            foreach (var c in cards) {
                if (!c.IsUp)
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Cards of given indexes
    /// </summary>
    /// <returns>List of card references of give indexes.</returns>
    /// <param name="indexes">Indexes.</param>
    public List<Card> CardIndexesToRefs(int[] indexes)
    {
        List<Card> res = new List<Card>();

        if (cards == null || indexes == null)
            return res;

        for (int i = 0; i < indexes.Length; i++) {
            res.Add(CardIndexToRef(indexes[i]));
        }

        return res;
    }

    /// <summary>
    /// Gets card of given index
    /// </summary>
    /// <returns>The card reference.</returns>
    /// <param name="index">Card index (from zero to deck size).</param>
    public Card CardIndexToRef(int index)
    {
        if (cards == null || index > cards.Length - 1)
            return null;

        return cards[index];
    }

    /// <summary>
    /// Loads the shuffle from saved winnable base or shuffle randomly.
    /// </summary>
    /// <param name="simulation"></param>
    /// <param name="gameType"></param>
    /// <returns>id выбранного расклада</returns>
   /* public async Task<int> Shuffle(bool simulation, GameType gameType)
    {
        shuffleName = "random";

        //if (!simulation && IsWinnableAvailable(gameType)) {
        if (!simulation) {
            Debug.Log("solitaire: Shuffle !simulation");
            if (winnable[gameType] == null) {
                Debug.Log("solitaire: winnable[gameType] == null");
                shuffledCards = ShuffleRandomly();
                return 0;
            }

            int r = UnityEngine.Random.Range(0, winnable[gameType].Count);
            //int[] curShuffle = winnable[gameType][r];
            Debug.Log("solitaire: _decks.Get(gameType)");
            //var layout = await _decks.Get(gameType);
            //var data = layout.ToGameControllerDataFormat();
            Debug.Log("solitaire: _decks.Get(gameType) end");

            if (shuffledCards == null)
                shuffledCards = new Card[CARD_COUNT];

           Debug.Log($"solitaire: shuffledCards.Length={shuffledCards.Length}; curShuffle.Length={data.Length}");
            Debug.Log($"solitaire: curShuffle: {data.Dump()}");
            Debug.Log($"solitaire: shuffledCards: {shuffledCards.Dump()}");
            shuffledCards[0] = cards[0];
            for (int i = 0; i < 52; i++) {
                shuffledCards[i] = cards[data[i]];
            }
            Debug.Log($"solitaire: shuffledCards: {shuffledCards.Dump()}");

            shuffleName = "winnable" + r;
            return layout.Id;
        }
        else {
            Debug.Log("solitaire: simulation");
            shuffledCards = ShuffleRandomly();
            return 0;
        }
    }*/

    /// <summary>
    /// Shuffles cards randomly.
    /// </summary>
    private Card[] ShuffleRandomly()
    {
        return cards.OrderBy(x => UnityEngine.Random.Range(0, cards.Length)).ToArray();
    }

    /// <summary>
    /// Determines whether is winnable shuffle available at the monent.
    /// </summary>
    /// <returns><c>true</c> if winnable is available; otherwise, <c>false</c>.</returns>
    private bool IsWinnableAvailable(GameType gameType)
    {
        int gameCount = PlayerPrefs.GetInt(KlondikeController.GAMES_TOTAL_KEY, 1);

        // general rule, which describes when we use saved winnable shuffle
        // to make game more comfortable and easy for new users
        bool timeToWinnable = gameCount <= 10 ? (gameCount == 1 || gameCount % 2 == 0) :
            gameCount <= 50 ? (gameCount % 3 == 0) :
            gameCount <= 200 ? (gameCount % 4 == 0) : false;

        return timeToWinnable &&
        (winnable != null) &&
        winnable.ContainsKey(gameType) &&
        winnable[gameType] != null;
    }

    /// <summary>
    /// Copies precalculated winnable combinations file to persistent data storage.
    /// </summary>
    public async Task CopyWinnableBase()
    {
        IEnumerable<GameType> types = Enum.GetValues(typeof(GameType)).Cast<GameType>();
        winnable = new Dictionary<GameType, List<int[]>>();
        List<int[]> combination;
        foreach (var t in types) {

            if (t == GameType.OneOnThree)
                continue;

            string toPath = System.IO.Path.Combine(Application.persistentDataPath, string.Format("winnable_{0}.json", t.ToString()));
            string fromURL = System.IO.Path.Combine(Application.streamingAssetsPath, string.Format("winnable_{0}.json", t.ToString()));

            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.LinuxEditor)
                fromURL = "file://" + fromURL;

            if (!System.IO.File.Exists(toPath)) {
                System.IO.File.WriteAllBytes(toPath, await WWWUtils.GetBytesFromURL(fromURL));
            }

            if (JsonToWinnable(System.IO.File.ReadAllText(toPath), out combination))
                winnable.Add(t, combination);

            await Task.Yield();
        }
        winnable.Add(GameType.OneOnThree, winnable[GameType.OneOnOne]);
    }

    /// <summary>
    /// Parse (deserialize) JSON string to list of winnable combinations.
    /// </summary>
    /// <returns><c>true</c>, if success, <c>false</c> otherwise.</returns>
    /// <param name="json">Json string.</param>
    /// <param name="combination">Combinations list.</param>
    private bool JsonToWinnable(string json, out List<int[]> combination)
    {
        try {
            combination = LitJson.JsonMapper.ToObject<List<int[]>>(json);
            return true;
        }
        catch {
            combination = null;
            return false;
        }
    }

    /// <summary>
    /// Saves the winnable shuffle to the persistent data storage.
    /// </summary>
    /// <param name="type">Current deck type.</param>
    /// <param name="value">Difficulty of current shuffle.</param>
    public void SaveCurrentShuffle(string deckType, int difficulty)
    {

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, string.Format("winnable_{0}.json", deckType));
        List<int[]> combinations;

        try {
            //if file already exists we must read and parse it first
            if (System.IO.File.Exists(path)) {
                combinations = LitJson.JsonMapper.ToObject<List<int[]>>(System.IO.File.ReadAllText(path));
            }
            else {
                combinations = new List<int[]>();
            }
        }
        catch (Exception e) {
            KlondikeController.Instance.msgHandler.AddMsg("Error during SaveCurrentShuffle: " + e.Message);
            return;
        }

        int[] curShuffle = shuffledCards.Select(x => x.Index).ToArray();

        //calculate pseudo hash
        int curHash = curShuffle.GetPseudoHash();

        //check for duplicate
        foreach (var c in combinations) {
            if (c.GetPseudoHash() == curHash) {
                return;
            }
        }

        //save difficulty if last array cell
        int[] newEntry = new int[CARD_COUNT + 1];
        curShuffle.CopyTo(newEntry, 0);
        newEntry[CARD_COUNT] = difficulty;
        combinations.Add(newEntry);

        //write modified JSON to disk
        System.IO.File.WriteAllText(path, LitJson.JsonMapper.ToJson(combinations));

    }

    public async Task<int> Shuffle(bool simulationEnabled, GameType deckDeckType)
    {
        throw new NotImplementedException();
    }
}


