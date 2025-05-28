using Scripts.Debuging;
using Scripts.GameEvents;
using Scripts.GameState;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ui;
using ui.windows;
using UnityEngine;
using Zenject;

public enum CardLayout
{
    Vertical = 0,
    Horizontal
}

public enum ScoreType
{
    Standart = 0,
    Windows
}

public enum GameType
{
    OneOnOne = 0,
    OneOnThree,
    ThreeOnThree
}

public class KlondikeController : MonoBehaviour
{
    [Inject]
    IGameEventsService _events;
    [Inject]
    IGameStateService _gameState;

    public MessageHandler msgHandler;
    public KlondikeStatistics statManager;
    public CardManager shuffleManager;
    public UnityEngine.EventSystems.EventSystem eventSystem;

    public bool testState = false;
    public bool simulationEnabled = false;
    public GameType simulationType = GameType.OneOnOne;
    public int simulationThreshold = 400;

    public const string GAMES_TOTAL_KEY = "gamesTotal";
    public const int DECK_SIZE = 24;
    const int PENALTY_SECONDS = 10;
    const int PENALTY_SCORE = 2;
    const int AUTO_HINT_INTERVAL = 10;
    const int STACK_COUNT = 7;
    const int FOUNDATION_COUNT = 4;
    const int RESTART_ADS_INTERVAL = 2;
    const int ENDGAME_ADS_INTERVAL = 3;

    KlondikeCardStack[] stacks;
    KlondikeSlot[] foundationSlots;
    KlondikeDeck deck;
    Dictionary<string, Slot> slots;

    CardGroup pressedCards;
    Vector2 pressedOffset;
    Vector2 originalCardsPos;
    string originalCardsLayer;
    TimeSpan gameTime;
    int totalTime;
    int penaltyTime;
    int timeWithoutUserAction;
    List<IUndoableUserMove> moveHistory;

    GameObject rootObject;
    HintEngine hintEngine;
    StateManager stateManager;

    int score;

    int undoCount = 0;
    int restartCount = 0;
    int gameEndCount = 0;

    bool isGamePlayable;
    bool isGamePaused;
    bool isDealingInProgress;
    bool isInitInProgress;
    bool isAnyActionPerformed;
    int simulationValue = 0;

    static KlondikeController instance;

    #region MonoBehaviour

    void Awake()
    {
        instance = this;

        //initialize private fields
        stateManager = new JSONStateManager(instance);
        hintEngine = new HintEngine(instance, _events);
        slots = new Dictionary<string, Slot>();
        originalCardsPos = Vector3.zero;
        pressedCards = null;

        shuffleManager.CardPressed += OnCardPressed;
        shuffleManager.CardReleased += OnCardReleased;

        Application.targetFrameRate = 60;
    }

    async void Start()
    {
        // todo вырезана аналитика
        // Answers.LogCustom ("appStart");

        //initialize appodeal ads
        AdsManager.Instance.Init();

        //initialize game controller
        await GameInit();
    }

    void Update()
    {
        if (IsGamePaused)
            return;

        //timer related actions
        UpdateTimers();

        //simulation actions
        UpdateSimulation();

        if (IsCardsPressed)
            UpdatePositions();

    }

    void OnApplicationPause(bool appApused)
    {
        if (appApused && IsStateCanBeSaved) {
            SaveGameState();
        }
    }

    #endregion

    #region Properties


    /// <summary>
    /// Gets a value indicating whether this game can be saved.
    /// </summary>
    /// <value><c>true</c> if this game can be saved; otherwise, <c>false</c>.</value>
    public bool IsStateCanBeSaved
    {
        // state can be saved if:
        // * it is not simulation mode
        // * final animation is NOT in progress
        get
        {
            return !SimulationEnabled &&
            !EffectsManager.Instance.IsFinalPlaying &&
            !isDealingInProgress &&
            !isInitInProgress &&
            IsGamePlayable;
        }
    }

    /// <summary>
    /// Gets a value indicating whether dealing is in progress.
    /// </summary>
    /// <value><c>true</c> if dealing is in progress now; otherwise, <c>false</c>.</value>
    public bool IsDealingInProgress
    {
        get { return isDealingInProgress; }
    }

    /// <summary>
    /// Gets the hint manager.
    /// </summary>
    /// <value>The hint manager.</value>
    public HintEngine HintManager
    {
        get { return hintEngine; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this game is playable.
    /// </summary>
    /// <value><c>true</c> if this game is playable; otherwise, <c>false</c>.</value>
    public bool IsGamePlayable
    {
        get { return isGamePlayable; }
        set { isGamePlayable = value; }
    }

    /// <summary>
    /// Gets a value indicating whether this game is paused.
    /// </summary>
    /// <value><c>true</c> if this game is paused; otherwise, <c>false</c>.</value>
    public bool IsGamePaused
    {
        get { return isGamePaused; }
    }

    /// <summary>
    /// Gets the elapsed time string.
    /// </summary>
    /// <value>The elapsed time string.</value>
    public string ElapsedTimeString
    {
        get { return string.Format("{0:00}:{1:00}", gameTime.Minutes, gameTime.Seconds); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether events sytem enabled.
    /// </summary>
    /// <value><c>true</c> if events sytem enabled; otherwise, <c>false</c>.</value>
    public bool EventsSytemEnabled
    {
        get { return eventSystem.enabled; }
        set
        {
            eventSystem.enabled = value;
        }
    }

    /// <summary>
    /// Gets or sets the elapsed game time.
    /// </summary>
    /// <value>The game time.</value>
    public TimeSpan GameTime
    {
        get { return gameTime; }
        set
        {
            gameTime = value;
            UILayout.Instance.txtTime.text = ElapsedTimeString;
        }
    }

    /// <summary>
    /// Gets the foundation slots array.
    /// </summary>
    /// <value>The foundation slots array.</value>
    public Slot[] Foundation
    {
        get { return foundationSlots; }
    }

    /// <summary>
    /// Gets a value indicating whether any card can be pressed.
    /// </summary>
    /// <value><c>true</c> if any card can be pressed; otherwise, <c>false</c>.</value>
    public bool IsCardsPressed
    {
        get { return pressedCards != null && pressedCards.Active; }
    }

    public static KlondikeController Instance
    {
        get { return instance; }
    }

    /// <summary>
    /// Gets or sets the game score.
    /// </summary>
    /// <value>The score.</value>
    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            UILayout.Instance.txtScore.text = score.ToString();
        }
    }

    /// <summary>
    /// Gets the dictionary of all slots.
    /// </summary>
    /// <value>The slots.</value>
    public Dictionary<string, Slot> Slots
    {
        get { return slots; }
    }

    /// <summary>
    /// Gets the klondike deck.
    /// </summary>
    /// <value>The deck.</value>
    public KlondikeDeck Deck
    {
        get { return deck; }
    }

    /// <summary>
    /// Gets the stacks array.
    /// </summary>
    /// <value>The stacks.</value>
    public KlondikeCardStack[] Stacks
    {
        get { return stacks; }
    }

    /// <summary>
    /// Gets or sets the history of undoable moves.
    /// </summary>
    /// <value>The history.</value>
    public List<IUndoableUserMove> History
    {
        get { return moveHistory; }
        set { moveHistory = value; }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Sets the objects visibility by changing its layer.
    /// </summary>
    /// <param name="value">If set to <c>true</c> value.</param>
    public void SetObjectsVisibility(bool value)
    {
        shuffleManager.SetCardsVisibility(value);
        UILayout.Instance.AutoPlayVisible(value);
        SetRootObjectVisibility(value);
        SetFoundationVisibility(value);
        SetDeckVisibility(value);
    }
    /// <summary>
    /// Automatically completes current game.
    /// </summary>
    public IEnumerator AutoPlay()
    {
        EventsSytemEnabled = false;
        IsGamePlayable = false;
        UILayout.Instance.AutoPlayEnabled = false;
        PauseGame();

        if (!shuffleManager.IsAllCardsUp && deck.Size == 0) {
            yield break;
        }

        bool complette = false;
        int k = 0;
        while (!complette) {
            complette = true;
            foreach (var s in stacks) {
                //search foundation slot for this card
                foreach (var f in foundationSlots) {
                    if (f.CanPlaceCard(s.Head)) {

                        complette = false;
                        Move move = new Move(s, new CardGroup(s.Head), f);
                        Score += move.Score;
                        //add card to founded slot
                        f.AddCard(s.Head, GameSettings.Instance.Data.BgAnimEnabled);
                        //and remove card from previous slot
                        s.RemoveCard(s.Head);

                        yield return new WaitForSeconds(0.15f);

                        //play sound only each second card not to make a mess
                        if (k % 2 == 0) {
                            SoundManager.Instance.PlaySound(5, 1f);
                            //SoundManager.Instance.PlaySound (8, 1f, CardAnimationSettings.CardMoveDuration, 0.7f + k * 0.03f);
                            SoundManager.Instance.PlaySongNote();
                        }

                        k++;
                    }
                }
            }
        }

        EventsSytemEnabled = true;
        OnWin();
    }

    /// <summary>
    /// Saves the state of the game to persistent data storage.
    /// </summary>
    /// <param name="saveHistory">If set to <c>true</c> save history.</param>
    public void SaveGameState(bool saveHistory = true)
    {
        /// Saves the state of the game in persistent storage (JSON, DB).
        if (isDealingInProgress || testState)
            return;

        stateManager.SaveGameState();
    }

    /// <summary>
    /// Adds the lose stat.
    /// </summary>
    public void AddLoseStat()
    {
        if (isAnyActionPerformed) {
            statManager.AddStat(false, score);
            _events.LooseGame();
            // todo вырезана аналитика
            // Answers.LogLevelEnd (shuffleManager.ShuffleName, score, false);
        }
    }

    /// <summary>
    /// Processes the deck click.
    /// </summary>
    public void ProcessDeckClick()
    {

        //if deck if empty or if final animation is in progress we must not handle the click, so we just quit method
        if (deck.Size == 0 || EffectsManager.Instance.IsFinalPlaying)
            return;

        // register that user made some action
        OnAnyUserAction();

        // actually try to perform deck flipping and store reference to history action
        IUndoableUserMove deckMove = deck.PerformDeckAction();

        if (deckMove != null) {

            //modify score if deckMove is not free (e.g in windows style score system)
            Score -= deckMove.Price;

            //store current user move in history to be able to undo it
            moveHistory.Add(deckMove);
        }

        CheckGame();
    }

    /// <summary>
    /// Undos the last move.
    /// </summary>
    public void UndoLastMove()
    {
        //we cant undo move if history is empty or final animation is playing
        if (moveHistory.Count < 1 || EffectsManager.Instance.IsFinalPlaying)
            return;

        // release cards first of all
        if (IsCardsPressed)
            return;

        //actual undo actions
        Action<bool> undoActions = (success) => {

            OrientationHandler.RotationEnabled = GameSettings.Instance.Data.ChangeOrient;

            //get last move in history list
            IUndoableUserMove move = moveHistory.Last();

            //modify score 
            Score += move.Price;

            move.Undo();
            moveHistory.Remove(move);

            CheckGame();
        };

        OnAnyUserAction();
        undoCount++;
        _events?.CancelMove();

        if (undoCount != 0 && undoCount % GameSettings.Instance.Data.UndoForAds == 0) {
            OrientationHandler.RotationEnabled = false;
            StartCoroutine(AdsManager.Instance.ShowInterstitial(undoActions, "undoMoveAds"));
        }
        else
            undoActions(false);
    }

    /// <summary>
    /// Handles click on restart button
    /// </summary>
    /// <param name="newShuffle">If set to <c>true</c> then create new shuffle, otherwise deal current.</param>
    public void OnRestartClicked(bool newShuffle)
    {
        restartCount++;
        _events.RestartGame(deck.DeckType);

        Action<bool> restartAction = (isRestart) => {

            if (isRestart) {
                NewGame(newShuffle, AdsCase.Restart);
            }
            else {
                //if user declined restart dialog we dont restart the game, but show ads anyway
                CheckAndShowAds((arg) => {
                    OrientationHandler.RotationEnabled = GameSettings.Instance.Data.ChangeOrient;
                }, AdsCase.Restart);
            }
        };

        //we dont count restart as loss if:
        // final animation is playing (we already won)
        // user haven't done any actions
        if (!EffectsManager.Instance.IsFinalPlaying && (isAnyActionPerformed || Score != 0 || GameTime > TimeSpan.FromMilliseconds(0))) {

            UILayout.Instance.dialog.Show(LocalizationManager.Instance.GetText("restartQuestion"), DialogType.YesNo, (yes) => {

                if (yes) {
                    AddLoseStat();
                }

                restartAction(yes);
            }
            );
        }
        else
            restartAction(true);

    }

    /// <summary>
    /// Starts the new game.
    /// </summary>
    /// <param name="newShuffle">If set to <c>true</c> new shuffle will be created.</param>
    /// <param name="adsCase">Ads case.</param>
    public void NewGame(bool newShuffle, AdsCase adsCase)
    {
        //if final animation now working we must skip it
        EffectsManager.Instance.SkipFinalAnimation();

        //count total games played since app was installed
        PlayerPrefs.SetInt(GAMES_TOTAL_KEY, PlayerPrefs.GetInt(GAMES_TOTAL_KEY, 1) + 1);

        Action<bool> restartAction = async (success) => {

            isInitInProgress = true;

            //reset all game objects state to initial
            InitGameobjects();

            //shuffle cards if we play new deck
            var layoutId = _gameState.LayoutId;
            if (newShuffle) {
                layoutId = await shuffleManager.Shuffle(SimulationEnabled, deck.DeckType);
            }

            isInitInProgress = false;

            SoundManager.Instance.ResetSongCursor();

            //run dealing card process
            await DealCards(GameSettings.Instance.Data.BgAnimEnabled);
            _events.NewGame(deck.DeckType, layoutId);
        };

        // we try to show ads only if its not a simulation and if case(placement) was provided
        if (adsCase != AdsCase.None && !SimulationEnabled)
            CheckAndShowAds(restartAction, adsCase);
        else
            restartAction(false);
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void PauseGame()
    {
        //if user holds any cards - release them
        if (IsCardsPressed)
            OnInvalidMove();

        isGamePaused = true;
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
    public void ResumeGame()
    {
        if (EffectsManager.Instance.IsFinalPlaying)
            return;

        isGamePaused = false;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Sets the root object visibility.
    /// </summary>
    /// <param name="value">If set to <c>true</c> root is visible.</param>
    private void SetRootObjectVisibility(bool value)
    {
        rootObject.layer = value ? UILayout.DEFAULT_LAYER_ID : UILayout.IGNORE_LAYER_ID;
    }

    /// <summary>
    /// Sets the foundation slots visibility.
    /// </summary>
    /// <param name="value">If set to <c>true</c> foundation is visible.</param>
    private void SetFoundationVisibility(bool value)
    {
        foreach (var f in foundationSlots) {
            f.Layer = value ? UILayout.BACKGROUND_LAYER_ID : UILayout.IGNORE_LAYER_ID;
        }
    }

    /// <summary>
    /// Sets the deck object visibility.
    /// </summary>
    /// <param name="value">If set to <c>true</c> deck is visible.</param>
    private void SetDeckVisibility(bool value)
    {
        deck.Layer = value ? UILayout.BACKGROUND_LAYER_ID : UILayout.IGNORE_LAYER_ID;
    }

    /// <summary>
    /// Simulation actions which must be performed each frame
    /// </summary>
    private void UpdateSimulation()
    {
        if (SimulationEnabled && !isDealingInProgress && hintEngine.HasHint) {
            hintEngine.CurrentHint.RefreshCards();
            if (hintEngine.CurrentHint.IsPossible)
                SimulateMove(hintEngine.CurrentHint);
            return;
        }
    }

    /// <summary>
    /// Updates the positions of pressed card group.
    /// </summary>
    private void UpdatePositions()
    {
        //update position of pressed card according to touch position
        pressedCards.Position = GetTouchPosition() - pressedOffset;

        // update position of trace effect
        EffectsManager.Instance.TraceParticlesObject.gameObject.transform.position = pressedCards.Position;
    }

    /// <summary>
    /// Checks the ads and show it if its time.
    /// </summary>
    /// <param name="callback">Callback after ads.</param>
    /// <param name="adsCase">Ads case.</param>
    private void CheckAndShowAds(Action<bool> callback, AdsCase adsCase)
    {

        if (MustShowAdsWithCase(adsCase)) {
            OrientationHandler.RotationEnabled = false;
            StartCoroutine(AdsManager.Instance.ShowInterstitial(callback, AdsCaseToPlacement(adsCase), 0.2f));
        }
        else
            callback(false);
    }

    /// <summary>
    /// Determines if we must show ads of given case 
    /// </summary>
    /// <returns><c>true</c>, if we must show ads, <c>false</c> otherwise.</returns>
    /// <param name="adsCase">Ads case.</param>
    private bool MustShowAdsWithCase(AdsCase adsCase)
    {
        return (restartCount % RESTART_ADS_INTERVAL == 0 && adsCase == AdsCase.Restart) ||
        (gameEndCount % ENDGAME_ADS_INTERVAL == 0 && adsCase == AdsCase.EndGame);
    }

    /// <summary>
    /// Converts ads case variable to string placement identifier.
    /// </summary>
    /// <returns>The placement string.</returns>
    /// <param name="adsCase">Ads case.</param>
    private string AdsCaseToPlacement(AdsCase adsCase)
    {
        //convert ads case to string placement

        if (adsCase == AdsCase.Restart)
            return "restartGameAds";
        else if (adsCase == AdsCase.EndGame)
            return "endGameAds";
        else
            return string.Empty;
    }

    /// <summary>
    /// Time related actions which must be performed every frame
    /// </summary>
    private void UpdateTimers()
    {
        /// we do not update timer if:
        /// * if player didnt perform any action yet 
        /// * if game is paused
        /// * if we updated time in this second
        if (isAnyActionPerformed && !IsGamePaused && totalTime != DateTime.Now.Second) {

            //count seconds without any action to show hint
            if (!IsCardsPressed)
                timeWithoutUserAction++;

            //if player inactive long enough we show him current hint
            if (timeWithoutUserAction > AUTO_HINT_INTERVAL) {
                if (hintEngine.HasHint)
                    hintEngine.ShowHint();

                //reset the counter
                timeWithoutUserAction = 0;
            }

            if (GameSettings.Instance.Data.TimerEnabled) {
                GameTime = GameTime.Add(TimeSpan.FromSeconds(1));

                //every N seconds we perform time penalty (decrease score)
                if (GameSettings.Instance.Data.ScoreType == ScoreType.Windows && gameTime.Seconds % PENALTY_SECONDS == 0 && penaltyTime != gameTime.Seconds) {
                    penaltyTime = gameTime.Seconds;
                    Score -= PENALTY_SCORE;
                }

            }
            totalTime = DateTime.Now.Second;
        }
    }

    /// <summary>
    /// Initializes game objects, components and dependencies.
    /// </summary>
    /// <returns>The init.</returns>
    private async Task GameInit()
    {
        UILayout.Instance.loadingScreen.SetActive(true);
        isInitInProgress = true;

        msgHandler.AddMsg("1: load app settings");
        await GameSettings.Instance.Load();

        if (SimulationEnabled) {
            msgHandler.AddMsg("2: setting up simulation");
            GameSettings.Instance.Data.BgAnimEnabled = false;
            GameSettings.Instance.Data.DeckType = simulationType;
            Time.timeScale = 10f;
            Application.runInBackground = true;
        }

        msgHandler.AddMsg("3: load sounds");
        await SoundManager.Instance.LoadSounds();

        msgHandler.AddMsg("4: load language");
        await LocalizationManager.Instance.ReadLangXml(GameSettings.Instance.Data.Language);
        msgHandler.AddMsg("4.01: load language end");

        if (testState) {
            msgHandler.AddMsg("4.1: copy test state");
            await CopyTestState();
        }

        msgHandler.AddMsg("5: initialize state manager");
        await stateManager.Init();

        msgHandler.AddMsg("5.1: read saved state");
        await stateManager.Read();

        msgHandler.AddMsg("6: initialize theme manager");
        await ThemeManager.Init();

        msgHandler.AddMsg("7: initialize game objects");
        InitGameobjects();

        msgHandler.AddMsg("8: initialize shuffle base");
        await shuffleManager.CopyWinnableBase();

        ApplyTheme();

        UILayout.Instance.Layout();

        //check if we have saved game state
        if (!SimulationEnabled && stateManager.HasState) {
            msgHandler.AddMsg("9.2: load saved game");
            await stateManager.ApplySavedState();
            IsGamePlayable = true;
            CheckGame();
        }
        else {
            msgHandler.AddMsg("9.3: starting new game");
            await shuffleManager.Shuffle(SimulationEnabled, deck.DeckType);
            msgHandler.AddMsg("9.4: end Shuffle");
            await DealCards(GameSettings.Instance.Data.BgAnimEnabled);
            msgHandler.AddMsg("9.5: end new game");
        }

        //subscribe to crucial events
        GameSettings.Instance.DeckTypeChanged += OnGameTypeChanged;
        GameSettings.Instance.ThreeDecksEnabledChanged += OnGameTypeChanged;
        GameSettings.Instance.TimerEnableChangedd += OnGameTypeChanged;
        GameSettings.Instance.ScoreTypeChanged += OnGameTypeChanged;

        ThemeManager.ThemeLoaded += ApplyTheme;

        OrientationHandler.RotationEnabled = GameSettings.Instance.Data.ChangeOrient;
        msgHandler.AddMsg("9.6:  OrientationHandler.Instance.Init");
        OrientationHandler.Instance.Init();

        msgHandler.AddMsg("9.7:   UILayout.Instance.loadingScreen.SetActive (false)");
        UILayout.Instance.loadingScreen.SetActive(false);

        isInitInProgress = false;
        msgHandler.AddMsg("9.8:   isInitInProgress = false;");
    }

    private async Task CopyTestState()
    {
        string from = System.IO.Path.Combine(Application.streamingAssetsPath, "states.json");

        if (Application.platform != RuntimePlatform.Android)
            from = "file://" + from;

        var bytes = await WWWUtils.GetBytesFromURL(from);

        if (bytes == null || bytes.Length < 1)
            return;

        string to = System.IO.Path.Combine(Application.persistentDataPath, "states.json");
        System.IO.File.WriteAllBytes(to, bytes);
    }

    /// <summary>
    /// Applies theme sprites to game objects.
    /// </summary>
    private void ApplyTheme()
    {
        /// Applies theme images to game objects.

        deck.SetSprite(ThemeManager.SlotSprite);

        foreach (var f in foundationSlots) {
            f.SetSprite(ThemeManager.SlotSprite);
        }
    }

    /// <summary>
    /// Inits the foundation slots.
    /// </summary>
    private void InitFoundation()
    {
        //form 4 result slots
        if (foundationSlots == null)
            foundationSlots = new KlondikeSlot[FOUNDATION_COUNT];

        for (int i = 0; i < FOUNDATION_COUNT; i++) {
            if (foundationSlots[i] == null) {
                GameObject go = new GameObject();
                go.layer = UILayout.BACKGROUND_LAYER_ID;
                go.transform.SetParent(rootObject.transform);
                foundationSlots[i] = new KlondikeSlot(i, go, EffectsManager.Instance.CrateEffectOfCurrentStyle());
                slots.Add(foundationSlots[i].Name, foundationSlots[i]);
            }
            else {
                foundationSlots[i].Clear();
                foundationSlots[i].IsShown = true;
            }
        }
    }

    /// <summary>
    /// Inits the stacks slots.
    /// </summary>
    private void InitStacks()
    {
        if (stacks == null)
            stacks = new KlondikeCardStack[STACK_COUNT];

        //form 7 solitaire stacks and place them
        for (int i = 0; i < STACK_COUNT; i++) {
            if (stacks[i] == null) {
                stacks[i] = new KlondikeCardStack(i, null);
                stacks[i].Object.transform.SetParent(rootObject.transform);
                slots.Add(stacks[i].Name, stacks[i]);
            }
            else
                stacks[i].Clear();
        }
    }

    /// <summary>
    /// Inits the deck slot.
    /// </summary>
    private void InitDeck()
    {
        //create deck and put rest of the cards in it
        if (deck == null) {
            GameObject deckObject = KlondikeDeck.CreateDeckObject();
            deckObject.transform.SetParent(rootObject.transform);
            deckObject.GetComponent<DeckController>().DeckClicked += ProcessDeckClick;

            deck = new KlondikeDeck(0, deckObject);
            slots.Add(deck.Name, deck);
        }
        else {
            deck.Clear();
        }

        deck.DeckType = GameSettings.Instance.Data.DeckType;
    }

    /// <summary>
    /// Inits or resets object fields.
    /// </summary>
    private void InitFields()
    {
        GameTime = new TimeSpan();
        isAnyActionPerformed = false;
        simulationValue = 0;
        undoCount = 0;
        totalTime = 0;
        penaltyTime = -1;
        timeWithoutUserAction = 0;
        Score = 0;

        hintEngine.ClearHint();
        isGamePaused = false;

        if (moveHistory == null)
            moveHistory = new List<IUndoableUserMove>();
        else
            moveHistory.Clear();
    }

    /// <summary>
    /// Inits all the gameobjects.
    /// </summary>
    private void InitGameobjects()
    {
        //hide auto play button
        UILayout.Instance.AutoPlayEnabled = false;

        //root is just empty object to make hierarchy more staight
        if (rootObject == null) {
            rootObject = new GameObject();
            rootObject.name = "RootGameObject";
        }

        //prepare all game objects to new game
        shuffleManager.Init(rootObject);
        InitFields();
        InitStacks();
        InitFoundation();
        InitDeck();

    }

    /// <summary>
    /// Handles game type change event
    /// </summary>
    private void OnGameTypeChanged()
    {
        restartCount++;
        NewGame(true, AdsCase.Restart);
    }

    /// <summary>
    /// Action that must be performed when user lost the game
    /// </summary>
    private void OnLose()
    {
        // count game ends to show ads properly
        gameEndCount++;

        //disable all user input to prevent illegal actions during delay
        EventsSytemEnabled = false;

        IsGamePlayable = false;

        //show final window after a delay to give player some time to understand what happened
        StartCoroutine(CoroutineExtension.ExecuteAfterTime(2f, () => {

            //add lose statistics
            AddLoseStat();

            UILayout.Instance.ShowFinishWindow(false);

            //return user input
            EventsSytemEnabled = true;

        }));
    }

    /// <summary>
    /// Action that must be performed when user won the game
    /// </summary>
    private void OnWin()
    {
        gameEndCount++;
        _events.WinGame();

        IsGamePlayable = false;

        // todo вырезана аналитика
        // Answers.LogLevelEnd (shuffleManager.ShuffleName, score, true);
        statManager.AddStat(true, score);
        PauseGame();
        stateManager.ClearStorage();
        UILayout.Instance.AutoPlayEnabled = false;

        if (GameSettings.Instance.Data.FinalAnimEnabled) {
            EffectsManager.Instance.StartFinalAnimation(OnFinalEnded);
        }
        else
            OnFinalEnded();

    }

    /// <summary>
    /// Action that must be performed right after the final animation was ended
    /// </summary>
    private void OnFinalEnded()
    {
        ///actions after final animation

        //show finish window with congratulations
        UILayout.Instance.ShowFinishWindow(true);

        // return ability to change orientation and user input
        // because they were disabled during final animation
        OrientationHandler.RotationEnabled = GameSettings.Instance.Data.ChangeOrient;
        EventsSytemEnabled = true;
    }

    /// <summary>
    /// Handles card released event
    /// </summary>
    /// <param name="a">Card event arguments.</param>
    private void OnCardReleased(CardEventArgs a)
    {
        OnAnyUserAction();

        if (IsGamePaused || pressedCards == null || !pressedCards.Active)
            return;

        EffectsManager.Instance.TraceParticlesObject.Stop();
        EffectsManager.Instance.TraceParticlesObject.gameObject.SetActive(false);

        Slot targetSlot;

        if (!CardCanBeMovedToSlot(pressedCards.Root, out targetSlot)) {
            OnInvalidMove();
            return;
        }

        pressedCards.PlayReleaseAnimation(() => {
            pressedCards.Layer = originalCardsLayer;
        });

        SoundManager.Instance.PlaySound(5, 1f);
        MoveCardsBetweenSlots(pressedCards.Slot, pressedCards, targetSlot);

        CheckGame();
    }

    /// <summary>
    /// Handles card press event
    /// </summary>
    /// <param name="a">Card event arguments.</param>
    private void OnCardPressed(CardEventArgs a)
    {
        //user cant press cards when game is paused
        if (IsGamePaused)
            return;

        //user cant press new cards if some cards already pressed
        if (IsCardsPressed)
            return;

        //user cant press face-down cards
        if (!a.Card.IsUp) {
            msgHandler.AddMsg("Нельзя нажимать на карту: " + Card.GetCardName(a.Card));
            return;
        }

        OnAnyUserAction();

        Slot cardSlot = FindSlotWithCard(a.Card);
        if (cardSlot != null) {
            //store pressed card (group) reference
            pressedCards = cardSlot.GetGroup(a.Card);
            pressedCards.StopAllAnimation();

            if (GameSettings.Instance.Data.BgAnimEnabled) {
                pressedCards.PlaySelectAnimation();
            }
        }
        else {
            msgHandler.AddMsg("Невалидное состояние карты: " + Card.GetCardName(a.Card));
            return;
        }

        originalCardsPos = pressedCards.Position;
        originalCardsLayer = pressedCards.Root.SortingLayer;
        pressedOffset = GetTouchPosition() - originalCardsPos;

        SoundManager.Instance.PlaySound(4, 0.5f);

        pressedCards.Layer = UILayout.SORTING_TOP;

        //turn card effect on
        if (GameSettings.Instance.Data.BgAnimEnabled) {
            EffectsManager.Instance.TraceParticlesObject.gameObject.transform.position = pressedCards.Position;
            EffectsManager.Instance.TraceParticlesObject.gameObject.SetActive(true);
            EffectsManager.Instance.TraceParticlesObject.Play();
        }
    }

    /// <summary>
    /// Actions that must be performed after any user action (click on card, click on deck, ...)
    /// </summary>
    private void OnAnyUserAction()
    {
        /// Handles the any user action event. It useful to track user inactivity period

        // reset counter that counts seconds without any user action
        timeWithoutUserAction = 0;

        // this flag indicates that user began to play
        // we use it for example to enable all game timers which disabled at start
        if (!isAnyActionPerformed)
            isAnyActionPerformed = true;

        // if user did something we stop current hint animation
        hintEngine.HideHint();
    }

    /// <summary>
    /// Deals the cards.
    /// </summary>
    /// <param name="withDelay">If set to <c>true</c> use delay.</param>
    /// <param name="delay">Delay in seconds.</param>
    private async Task DealCards(bool withDelay, float delay = 0.01f)
    {
        //Debug.Log($"solitaire:  DealCards 1");
        //bool prevValue = OrientationHandler.RotationEnabled;
        OrientationHandler.RotationEnabled = false;

        // todo вырезана аналитика
        // Answers.LogLevelStart (shuffleManager.ShuffleName);
        SoundManager.Instance.PlaySound(11, 0.2f, SoundManager.Instance.RandomPitch());

        isDealingInProgress = !(EventsSytemEnabled = false);

        Card[] shuffle = shuffleManager.ShuffledCards;

        int k = 0;

        deck.IsReady = false;
        //Debug.Log($"solitaire:  DealCards 2");
        //Debug.Log($"solitaire:  shuffle: {shuffle.Dump()}");
        //return all the cards to deck and turn face down
        try {
            for (int i = 0; i < shuffle.Length; i++) {
                //Debug.Log($"solitaire:  DealCards 2 1");
                //Debug.Log($"solitaire:  shuffle[i]={shuffle[i]}");
                if (shuffle[i] == null) continue;

                shuffle[i].MoveInstantly(deck.Position);


                //Debug.Log($"solitaire:  DealCards 2 2");
                shuffle[i].SetSide(true);
                //Debug.Log($"solitaire:  DealCards 2 3");
                if (i < DECK_SIZE) {
                    //Debug.Log($"solitaire:  DealCards 2 4");
                    deck.AddCard(shuffle[i]);
                    //Debug.Log($"solitaire:  DealCards 2 5");
                    k++;
                }
            }
        }
        catch (Exception ex) {
            Debug.Log($"solitaire:  exception: {ex.Message}");
        }

        //Debug.Log($"solitaire:  DealCards 2 6");
        deck.IsReady = true;

        //form 7 solitaire stacks and place cards to them
        //Debug.Log($"solitaire:  DealCards 3");
        for (int i = 0; i < STACK_COUNT; i++) {
            for (int j = 0; j < i + 1; j++) {
                shuffle[k].SetSide(j != i);
                shuffle[i].MoveInstantly(deck.Position);
                stacks[i].AddCard(shuffle[k], withDelay);
                k++;

                if (withDelay) await Task.Delay((int)(delay * 1000));
            }
        }
        //Debug.Log($"solitaire:  DealCards 4");

        //wait until the animation is over, and only then turn on rotation and start game
        if (withDelay) await Task.Delay((int)(CardAnimationSettings.CardMoveDuration * 1000));
        //Debug.Log($"solitaire:  DealCards 5");

        OrientationHandler.RotationEnabled = GameSettings.Instance.Data.ChangeOrient;
        isDealingInProgress = !(EventsSytemEnabled = true);
        IsGamePlayable = true;
        //Debug.Log($"solitaire:  DealCards 6");
        CheckGame();
        //Debug.Log($"solitaire:  DealCards 7");
    }

    /// <summary>
    /// Gets the current touch position.
    /// </summary>
    /// <returns>The touch position.</returns>
    private Vector2 GetTouchPosition()
    {
        Vector3 pos;
        int tCount = Input.touchCount;

#if UNITY_EDITOR
        pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
#else
			if (Input.touchCount < 1)
				return Vector2.zero;	

			pos = Camera.main.ScreenToWorldPoint (Input.touches[tCount-1].position);
#endif

        pos.z = pressedCards.Position.z;
        return pos;
    }

    /// <summary>
    /// Searches the slot with given card
    /// </summary>
    /// <returns>The slot with card.</returns>
    /// <param name="c">Card.</param>
    private Slot FindSlotWithCard(Card c)
    {
        //search among stacks
        try {

            Slot res = stacks.SingleOrDefault(x => x.ContainsCard(c));

            if (res == null) {
                //search among free slots
                res = foundationSlots.SingleOrDefault(x => x.ContainsCard(c));
            }

            if (res == null && deck.ContainsCard(c)) {
                res = deck;
            }

            return res;

        }
        catch (Exception) {
            //one of our collections in illegal state
            return null;
        }

    }

    /// <summary>
    /// Searches slot that could accep given card group.
    /// </summary>
    /// <returns><c>true</c>, if slot for card group was found, <c>false</c> otherwise.</returns>
    /// <param name="c">Card group.</param>
    /// <param name="slot">Result slot.</param>
    private bool FindSlotForCardGroup(CardGroup c, out Slot slot)
    {
        if (hintEngine.HasValidHint && hintEngine.CurrentHint.Cards.Root == c.Root) {
            slot = hintEngine.CurrentHint.To;
            return true;
        }

        //1. search among foundation slots (only for single card)
        if (c.Size == 1)
            foreach (var f in foundationSlots) {
                if (f.CanPlaceCard(c.Root)) {
                    slot = f;
                    return true;
                }
            }

        //2. search among stacks
        foreach (var s in stacks) {
            if (s.CanPlaceCard(c.Root)) {
                slot = s;
                return true;
            }
        }

        slot = null;
        return false;
    }

    /// <summary>
    /// Checks if given card can be moved to target slot.
    /// </summary>
    /// <returns><c>true</c>, if can be moved to slot, <c>false</c> otherwise.</returns>
    /// <param name="c">Card.</param>
    /// <param name="targetSlot">Target slot.</param>
    private bool CardCanBeMovedToSlot(Card c, out Slot targetSlot)
    {
        //check if card was dropped in this place

        bool res = false;

        Vector2 touchPos = GetTouchPosition();
        Rect touchRect = new Rect(touchPos - pressedOffset - ThemeManager.CardSize / 2f, ThemeManager.CardSize);

        for (int i = 0; i < STACK_COUNT; i++) {

            if (stacks[i] == pressedCards.Slot)
                continue;

            //stack cell itself (if our card is king)
            if (stacks[i].IsRectOverlapsSlot(touchRect) && stacks[i].CanPlaceGroup(pressedCards)) {
                targetSlot = stacks[i];
                return true;
            }

            //check all stack heads
            if (stacks[i].Size > 0 && stacks[i].Head.IsRectOverlapsCard(touchRect)) {
                if (res = stacks[i].CanPlaceGroup(pressedCards)) {
                    targetSlot = null;
                    targetSlot = stacks[i];
                    return res;
                }
            }
        }

        //check free slots
        for (int i = 0; i < FOUNDATION_COUNT; i++) {
            if (foundationSlots[i].IsRectOverlapsSlot(touchRect) && pressedCards.Size == 1 && foundationSlots[i].CanPlaceGroup(pressedCards)) {
                targetSlot = foundationSlots[i];
                return true;
            }
        }

        targetSlot = null;
        return false;
    }

    /// <summary>
    /// Moves the cards between slots. Removes from one, and adds to another.
    /// </summary>
    /// <param name="from">Original slot.</param>
    /// <param name="cards">Cards.</param>
    /// <param name="to">Destination slot.</param>
    private void MoveCardsBetweenSlots(Slot from, CardGroup cards, Slot to)
    {
        //create history entry 	
        Move move = new Move(from, cards, to);
        Score += move.Score;
        moveHistory.Add(move);

        //remove cards from slot
        from.RemoveGroup(cards);

        //add them to new slot
        //use move animation only if its not simulation and if animation enabled in settings
        to.PlaceGroup(cards, !SimulationEnabled && GameSettings.Instance.Data.BgAnimEnabled);
    }

    /// <summary>
    /// производит один ход
    /// </summary>
    /// <param name="from">откуда</param>
    /// <param name="cards">что тащить</param>
    /// <param name="to">куда</param>
    public void MoveCard(Slot from, CardGroup cards, Slot to)
    {
        MoveCardsBetweenSlots(from, cards, to);
        CheckGame();
    }

    /// <summary>
    /// Handles situation when user performs invalid move.
    /// </summary>
    private void OnInvalidMove()
    {
        OnAnyUserAction();

        if (pressedCards == null)
            return;

        pressedCards.PlayReleaseAnimation();

        if (!IfCardsWereMoved) {

            if (!TryMoveByTap()) {
                pressedCards.CancelMoveWithShake(originalCardsPos);
            }

            return;

        }
        else {
            //return cards to original position
            SoundManager.Instance.PlaySound(5, 1f);
            pressedCards.Slot.RestoreGroupPos(pressedCards, originalCardsPos, true);
        }

        CheckGame();
    }

    /// <summary>
    /// Tries the move by tap.
    /// </summary>
    /// <returns><c>true</c>, if move by tap was success, <c>false</c> otherwise.</returns>
    private bool TryMoveByTap()
    {
        Slot slot;
        //tap move can be performed only if:
        // its enabled 
        // there is a place found for our card group
        if (GameSettings.Instance.Data.TapMoveEnabled &&
            FindSlotForCardGroup(pressedCards, out slot)) {

            SoundManager.Instance.PlaySound(5, 1f);
            MoveCardsBetweenSlots(pressedCards.Slot, pressedCards, slot);

            CheckGame();
            return true;

        }
        return false;
    }

    /// <summary>
    /// Gets a value indicating whether pressed card group was actually moved, or just was clicked.
    /// </summary>
    /// <value><c>true</c> if if cards were moved; otherwise, <c>false</c>.</value>
    private bool IfCardsWereMoved
    {
        //check if user move card from its original position or just tapped on it
        get { return Vector3.Distance(pressedCards.Position, originalCardsPos) > CardAnimationSettings.ShakeDistanceThres; }
    }

    /// <summary>
    /// Gets a value indicating whether this game is won.
    /// </summary>
    /// <value><c>true</c> if this this game is won; otherwise, <c>false</c>.</value>
    private bool IsWin
    {
        get
        {
            foreach (var f in foundationSlots) {
                if (f.Head == null || f.Head.Value != 11) {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this game is lost.
    /// </summary>
    /// <value><c>true</c> if this game is lost; otherwise, <c>false</c>.</value>
    private bool IsLoss
    {
        get
        {
            // if player is already doomed because there is no possible moves were found  (!isPlayable)
            // we just let him to go through all deck to make sure there is no moves
            // and now when deck is fliped completely (deck.Cursor == -1) we announce a loss
            return !IsWin && !hintEngine.HasHint && deck.DeckEnded;
        }
    }

    /// <summary>
    /// Checks the game after each turn (user move).
    /// </summary>
    private void CheckGame()
    {
        /// Check win and loss after each turn (move).

        if (SimulationEnabled) {
            CheckSimulation();
            return;
        }

        hintEngine.SearchPossibleMove();

        if (IsLoss) {
            OnLose();
        }
        else if (IsWin) {
            OnWin();
        }
        else if (CanAutoPlay) {
            UILayout.Instance.AutoPlayEnabled = true;
        }
        else {
            UILayout.Instance.AutoPlayEnabled = false;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this game can be autocomplete.
    /// </summary>
    /// <value><c>true</c> if this game can be autocomplete; otherwise, <c>false</c>.</value>
    private bool CanAutoPlay
    {
        get { return shuffleManager.IsAllCardsUp && deck.Size == 0; }
    }

    /// <summary>
    /// Checks the simulation mode moves.
    /// </summary>
    private void CheckSimulation()
    {
        hintEngine.SearchPossibleMove();

        if (!hintEngine.HasHint) {
            NewGame(true, AdsCase.EndGame);
        }
        else if (IsWin) {
            SaveWinnableShuffle();
            NewGame(true, AdsCase.EndGame);
        }
        else if (CanAutoPlay) {
            SaveWinnableShuffle();
            NewGame(true, AdsCase.EndGame);
        }
    }

    /// <summary>
    /// Saves the winnable shuffle to persistent storage.
    /// </summary>
    private void SaveWinnableShuffle()
    {
#if UNITY_EDITOR
        if (simulationValue >= simulationThreshold)
            return;

        shuffleManager.SaveCurrentShuffle(deck.DeckType.ToString(), simulationValue);
#endif
    }

    /// <summary>
    /// Simulates the move to find winnable shuffle.
    /// </summary>
    /// <param name="move">Move.</param>
    private void SimulateMove(Move move)
    {
        if (move.From == deck) {
            while (move.Cards.Root != deck.CurrentActiveCard) {
                deck.PerformDeckAction(false);
                simulationValue += 10;
            }
        }
        simulationValue += 1;
        MoveCardsBetweenSlots(move.From, move.Cards, move.To);

        CheckGame();
    }

    /// <summary>
    /// Gets a value indicating whether simulation is ended.
    /// </summary>
    /// <value><c>true</c> if simulation is ended; otherwise, <c>false</c>.</value>
    private bool SimulationEnabled
    {
        get
        {
#if UNITY_EDITOR
            return simulationEnabled;
#else
			return false;
#endif
        }
    }

    #endregion
}
