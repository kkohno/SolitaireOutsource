using System;
using UnityEngine;

/// <summary>
/// Move placeholder.
/// </summary>
public class Move: IUndoAction
{
		
	public Card Card { get; private set; }
	public Slot From { get; private set; }
	public Slot To { get; private set; }

	private int  score;
	private bool showNewCard;
	//DEBUG
	private int randomNumber;

	public Move (Slot from, Card card, Slot to)
	{
		if (card == null) {			
			return;
		}
		
		From = from;
			
		Card = card;
		randomNumber = UnityEngine.Random.Range (0, 100);
		//Debug.Log (randomNumber + " move with card: " + card.name);
		
		To = to;

		score = 0;

		if (To.GetType () == typeof(KlondikeSlot))
			score += 10;

		int index = From.Cards.IndexOf (Card);

		showNewCard = index > 0 && !From.Cards [index - 1].IsUp;

		if(showNewCard)
			score += 5;

		if (From.GetType() == typeof(KlondikeDeck) && To.GetType() == typeof(KlondikeCardStack))
			score += 5;		
	}

	public bool IsPossible {
		get {
			return From.ContainsCard (Card) && To.CheckCard (Card);
		}
	}

	public int Score {
		get {
			return score;
		}
	}

	#region IUndoAction implementation

	public void Debug ()
	{
		UnityEngine.Debug.Log(Card == null);
	}

	public void Undo ()
	{	
		
		//Debug.Log (randomNumber + " " + (Card == null));

		To.RemoveCard (Card);

		if (showNewCard)
			From.Head.SetSide (true,true);
		
		From.AddCard (Card,true);
	}

	public bool CanUndo ()
	{
		return true;
	}

	#endregion
}


