using Scripts.GameEvents;
using ui.windows;
using UnityEngine;

/// <summary>
/// Class that finds, shows and manage game hints.
/// </summary>
public class HintEngine
{
	private KlondikeController controller;
    IGameEventsService _events;

    public HintEngine (KlondikeController controller, IGameEventsService events)
	{		
		this.controller = controller;
		this._events = events;
	}

	private Move hint;

	/// <summary>
	/// Gets the current hint.
	/// </summary>
	/// <value>The current hint.</value>
	public Move CurrentHint {
		get{ return hint; }
	}

	/// <summary>
	/// Gets a value indicating whether this instance has hint.
	/// </summary>
	/// <value><c>true</c> if this instance has hint; otherwise, <c>false</c>.</value>
	public bool HasHint {
		get{ return hint != null; }
	}

	/// <summary>
	/// Gets a value indicating whether this instance has valid hint.
	/// </summary>
	/// <value><c>true</c> if this instance has valid hint; otherwise, <c>false</c>.</value>
	public bool HasValidHint {
		get{ return HasHint && hint.IsPossible; }
	}

	/// <summary>
	/// Clears the hint.
	/// </summary>
	public void ClearHint ()
	{
		hint = null;
	}

	/// <summary>
	/// Shows the current hint.
	/// </summary>
	private void ShowCurrentHint ()
	{
        // highlight card that you must move
		hint.From.PlayHintAnimation (hint.Cards);

		// we dont highlight destination slot if our hint suggest us to flip deck
		// in this case we only highlight deck
		if (hint.From != controller.Deck || controller.Deck.CurrentActiveCard == hint.Cards.Root)
			hint.To.PlayHintAnimation (null);
    }


	/// <summary>
	/// We show this hint when player is already lost, but game is not ended because deck ended
	private void ShowLoseHint ()
	{
		if (controller.Deck.CanBeFlipped && !controller.Deck.DeckEnded) {
			controller.Deck.PlayHintAnimation (null);
		}
	}

	/// <summary>
	/// Hides the hint.
	/// </summary>
	public void HideHint ()
	{		
		if (HasValidHint) {
			hint.From.StopHintAnimation ();
			hint.To.StopHintAnimation ();
		} else {
			controller.Deck.StopHintAnimation ();
		}
	}

	/// <summary>
	/// Shows the hint.
	/// </summary>
	public void ShowHint ()
	{	
		//if hints are disabled or final animation is running we dont show hints
		if (!GameSettings.Instance.Data.HintsEnabled || EffectsManager.Instance.IsFinalPlaying)
			return;

		if (HasValidHint) {	

			ShowCurrentHint ();
            _events.ShowHint();
        } else {

			ShowLoseHint ();
			_events.ShowLooseHint();
		}

	}

	/// <summary>
	/// Searchs the possible move.
	/// </summary>
	public void SearchPossibleMove ()
	{	

		if (!controller.IsGamePlayable)
			return;

		var deckCards = controller.Deck.AvailableCards;
		Move move = hint = null;
		CardGroup group;
		Card first;

		foreach (var s in controller.Stacks) {

			//from stack to another to face up new card
			if ((first = s.FirstFaceUp) != null) {						
				int index = s.Cards.IndexOf (first);
				group = s.GetGroup (first);
				foreach (var target in controller.Stacks) {					
					if (target != s && target.CanPlaceGroup (group) && !(target.Size == 0 && index == 0)) {												
						hint = new Move (s, group, target);
						return;
					} 
				}
			}	
		}

		foreach (var s in controller.Stacks) {
			//from stack head to foundation slot
			foreach (var f in controller.Foundation) {				
				if (f.CanPlaceCard (s.Head)) {	
					move = new Move (s, new CardGroup (s.Head), f);
					if (move.ShowNewCard) {	
						hint = move;
						return;
					} else {
						break;
					}	
				}
			}

			if (move != null)
				break;		
		}


		if (move != null) {
			hint = move;
			return;
		}

		//from stack to another to free foundation card
		foreach (var s in controller.Stacks) {
			if ((first = s.FirstFaceUp) != null) {	
				int index = s.Cards.IndexOf (first);

				for (int i = index; i < s.Size - 1; i++) {
					foreach (var f in controller.Foundation) {						
						if (f.CanPlaceCard (s.Cards [i])) {
							group = s.GetGroup (s.Cards [i + 1]);
							foreach (var t in controller.Stacks) {								
								if (t != s && t.CanPlaceGroup (group)) {	
									hint = new Move (s, group, t);
									return;
								}
							}
						}
					}
				}
			}
		}

		//from foundation to stack to open new card
		foreach (var f in controller.Foundation) {
			foreach (var s in controller.Stacks) {				
				if (s.CanPlaceCard (f.Head)) {
					foreach (var benifit in controller.Stacks) {
						if (benifit == s)
							continue;

						if ((first = benifit.FirstFaceUp) != null && KlondikeCardStack.CheckCard (first, f.Head) && first.Value != 12 && f.Head.Value != 12) {															
							hint = new Move (f, new CardGroup (f.Head), s);
							return;
						}
					}
				}
			}
		}

		// from deck
		foreach (var d in deckCards) {		
			// from deck to foundation	
			foreach (var f in controller.Foundation) {				
				if (f.CanPlaceCard (d)) {	
					hint = new Move (controller.Deck, new  CardGroup (d), f);
					;
					return;
				} 
			}

			// from deck to stack
			foreach (var s in controller.Stacks) {				
				if (s.CanPlaceCard (d)) {	
					hint = new Move (controller.Deck, new  CardGroup (d), s);
					;
					return;
				} 
			}
		}

		controller.IsGamePlayable = false;
	}


}


