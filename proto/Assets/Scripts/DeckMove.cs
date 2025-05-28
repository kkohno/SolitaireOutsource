using System;
using System.Collections.Generic;
using UnityEngine;

public class DeckMove: IUndoAction
{
	

	KlondikeDeck deckRef;	
	public int CursorState { get;  set;}
	public List<Card> ShownState {get;  set;}

	public DeckMove (KlondikeDeck deckRef)
	{
		this.deckRef = deckRef;
	}

	#region IUndoAction implementation

	public void Debug ()
	{
		
	}

	public void Undo ()
	{
		deckRef.UndoAction (this);
	}

	public bool CanUndo ()
	{
		throw new NotImplementedException ();
	}

	#endregion
}


