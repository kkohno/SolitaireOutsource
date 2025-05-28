using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ui.windows;
using UnityEngine;

/// <summary>
/// Klondike state manager interface (saves\loads history, card states etc.)
/// </summary>
public abstract class StateManager
{
	protected List<SlotState> slotStates;
	protected KlondikeController gameController;

	public StateManager (KlondikeController controller)
	{
		this.gameController = controller;
	}

	public int Score { get; set; }

	public int DeckFlips{ get; set; }

	public int Time { get; set; }

	public int[] Shuffle{ get; set; }

	public int[] CardStates{ get; set; }

	public List<IUndoableUserMove> History { get; set; }

	public GameType Type{ get; set; }

	public ScoreType ScoreType{ get; set; }

	/// <summary>
	/// Gets the states of all slots.
	/// </summary>
	/// <value>The states list.</value>
	public List<SlotState> States {
		get { 
			if (slotStates != null)
				return slotStates;
			else
				return new List<SlotState> ();
		}
	}

	/// <summary>
	/// Last game elapsed time.
	/// </summary>
	/// <value>The time span.</value>
	public TimeSpan TimeSpan {
		set {
			Time = (int)value.TotalSeconds;
		}
		get{ return TimeSpan.FromSeconds (Time); }
	}

	/// <summary>
	/// Clears the current in-memory state.
	/// </summary>
	public void ClearState ()
	{
		if (slotStates != null)
			slotStates.Clear ();
		
		if (History != null)
			History.Clear ();
		
		Shuffle = null;
	}

	/// <summary>
	/// Adds the state of the slot to manager.
	/// </summary>
	/// <param name="state">State.</param>
	public void AddSlotState (SlotState state)
	{
		if (slotStates == null)
			slotStates = new List<SlotState> ();

		slotStates.Add (state);
	}

	/// <summary>
	/// Saves the state of the game controller in persistent storage (JSON, DB).
	/// </summary>
	/// <param name="saveHistory">If set to <c>true</c> save history.</param>
	public void SaveGameState (bool saveHistory = true)
	{		
		
		//clear previous data
		ClearState ();

		//apply all the game parameters
		Score = gameController.Score;
		TimeSpan = gameController.GameTime;
		Type = gameController.Deck.DeckType;
		ScoreType = GameSettings.Instance.Data.ScoreType;

		//save all slots states
		foreach (var s in gameController.Slots.Values) {			
			AddSlotState (s.GetState ());
		}

		//save all cards states and shuffle
		CardStates = gameController.shuffleManager.Cards.Select (x => x.IsUp ? 1 : 0).ToArray ();
		Shuffle = gameController.shuffleManager.ShuffledCards.Select (x => x.Index).ToArray ();

		//save history if needed
		if (saveHistory) {		
			History = gameController.History.ToList ();
		}

		//flush all the data to persistent storage
		Write ();
	}

	/// <summary>
	/// Applies saved game state which read from persistent storage to the controller,
	/// </summary>
	public async Task ApplySavedState ()
	{		
		gameController.History = History.ToList ();
		GameSettings.Instance.Data.DeckType = Type;
		GameSettings.Instance.Data.ScoreType = ScoreType;
		gameController.Score = Score;
		gameController.GameTime = TimeSpan;

		// apply saved game state to controller
		gameController.shuffleManager.SetShuffle (new Card[CardManager.CARD_COUNT]);
		for (int i = 0; i < CardManager.CARD_COUNT; i++) {

			gameController.shuffleManager.ShuffledCards [i] = gameController.shuffleManager.Cards [Shuffle [i]];	

			if (CardStates [i] == 1)
				gameController.shuffleManager.Cards [i].SetSide (false);
		}

		//apply all states to slots 
		foreach (var state in States) {			
			gameController.Slots [state.SlotName].SetState (state);
		}

		
		gameController.Deck.IsReady = true;

	}

	public abstract Task Init ();

	public abstract bool HasState { get; }

	public abstract bool Write ();

	public abstract void ClearStorage ();

	public abstract Task Read ();
}


