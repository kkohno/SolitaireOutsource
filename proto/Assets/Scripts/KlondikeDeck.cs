using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class KlondikeDeck : Slot
{
	int cursor;
	List<Card> shown;
	Card topCard;
	int count = 3;
	float offset = 0.2f;

	public bool IsReady=false;

	public KlondikeDeck (int index, Vector2 cardSize, GameObject o) : base (index, cardSize, o)
	{		
		baseOrder = 2000;
		cursor = 0;
		shown = new List<Card> ();
	}

	public IUndoAction ShowCards ()
	{
		if (Size < 1)
			return null;

		if (InAnimation) {
			//return;		
			foreach (var c in Cards) {
				c.StopAllAnimation ();
			}
		}

		DeckMove res = new DeckMove(this);

		res.CursorState = cursor;
		List<Card> shownState = new List<Card> ();
		shownState.InsertRange (0, shown);
		res.ShownState = shownState;

		if (cursor == -1) {
			cursor = 0;

			for (int i = 0; i < Size; i++) {
				//Cards [i].transform.position = new Vector3 (position.x, position.y, -0.1f * i);
				ReturnCardToDeck(Cards[i],i);
				Cards [i].transform.position = new Vector3 (position.x - cardSize.x - (count - 1) * offset, position.y, position.z);
				Cards [i].MoveWithAnimation(position, CardAnimationSettings.CardMoveDuration,false, i * CardAnimationSettings.CardGroupDelay/2f);
			}

			shown.Clear ();
			return res;
		}

		int start = cursor;

		for (int i = start; i < start + count; i++) {			
			
			if (shown.Count == count) {				
				
				Card s = shown [0];
				//Debug.Log (Card.CardString(s) +" " + (position.x - cardSize.x - (count - 1) * offset));
				s.MoveWithAnimation (new Vector3 (position.x - cardSize.x - (count - 1) * offset, position.y, position.z),
					CardAnimationSettings.CardMoveDuration,
					false,
					0,
					() => {	
						s.shadow.enabled = false;
						s.glow.enabled  = false;
						//Debug.Log(Card.CardString(s)+ " " + s.sr.sortingOrder);
						//s.SetOrder(baseOrder);
						//s.gameObject.SetActive (false);
					}
				);

				shown.RemoveAt (0);
			}

			shown.Add (Cards [i]);		
			//Debug.Log (Card);

			if (cursor == Size - 1) {
				cursor = -1;
				break;
			} else
				cursor++;
			
		}

		UpdateShown (true);
		return res;
	}

	public List<Card> AvailableCards {
		get { 			

			List<Card> res = new List<Card> ();

			if (Size < 1)
				return res;

			//as if we use deck from its current state
			if (shown.Count > 0) {				
				res.AddRange (DeckCardsFrom (cursor != -1 ? cursor - 1 : Size - 1));
			}

			//as if we use deck from beginning
			res.AddRange (DeckCardsFrom (count - 1).Where (x => !res.Contains (x)));

			return res;
		}
	}

	public void UndoAction(DeckMove action){
		cursor = action.CursorState;

		//return current shown set to deck
		if (shown != null && shown.Count > 0) {
			float delay = 0f;
			for (int i= shown.Count-1;i>=0 ;i--) {
				Card s = shown [i];
				s.StopAllAnimation ();
				//s.gameObject.transform.position = position;
				s.SetSide(true,true,delay);
				s.SetOrder (baseOrder - i);
				s.MoveWithAnimation (position,0.5f,false,delay,()=>{
					ReturnCardToDeck (s);
				});

				delay += CardAnimationSettings.CardRotateDuration;
			}
		}

		index = (cursor == -1 ? Size - 1 : action.CursorState) - (count-1);

		for (int i = 0; i < index; i++) {
			//Debug.Log (i+": "+Card.CardString(Cards [i]) + " " + Cards [i].name);
			Cards [i].transform.position = new Vector3 (position.x - cardSize.x - (count - 1) * offset, position.y, position.z);
			Cards [i].StopAllAnimation ();
		}
			
		shown = action.ShownState;
		UpdateShown ();
	}

	private void ReturnCardToDeck(Card c, int index = -1, bool rotate = false){
		if(index == -1)
			index = Cards.IndexOf(c);
		
		c.SetOrder(baseOrder - index);
		c.StopAllAnimation ();
		c.SetSide (true);
		c.gameObject.SetActive (true);
		c.shadow.enabled = index == Size - 1;
	}

	private List<Card> DeckCardsFrom (int from)
	{
		//Debug.Log (from);
		List<Card> res = new List<Card> ();
		int s = Size;	

		for (int i = from; i < s; i = i + count) {	
			if (!res.Contains (Cards [i]))
				res.Add (Cards [i]);			
		}

		if (!res.Contains (Cards [s - 1])) {
			res.Add (Cards [s - 1]);
		}
		return res;
	}

	private void UpdateShown (bool withRotation = false)
	{
		//TODO animation
		Card c;
		float delay = 0f;
		for (int j = shown.Count; j > 0; j--) {
			
			delay = j * CardAnimationSettings.CardRotateDuration;
			
			c = shown [j - 1];

			c.GetComponent<BoxCollider2D> ().enabled = j == shown.Count;
			c.StopAllAnimation ();
			c.MoveWithAnimation (new Vector3 (position.x - cardSize.x - (shown.Count - j) * offset, position.y, shown [j - 1].transform.position.z),
				CardAnimationSettings.CardMoveDuration,
				false, 
				delay);			
			
			c.SetSide (false, withRotation, delay, baseOrder + (cursor == -1 ? Size : cursor) +(j + 1));

			c.gameObject.SetActive (true);
			c.Active = (j == shown.Count);
		}
	}

	private bool InAnimation {
		get {
			foreach (var c in Cards) {
				if (c.InAnimation)
					return true;
			}
			return false;
		}
	}

	#region Slot

	public override Vector3 Position {
		set { 
			
			base.Position = value;

			for (int i = 0; i < Size; i++) {				
				Cards [i].gameObject.transform.position = position;
			}
		}
	}

	public override bool ContainsCard (Card c)
	{
		//return Cards.Contains (c);
		return AvailableCards.Contains (c);		
	}

	public override string Name {
		get { 
			return "KlondikeDeck";
		}
	}

	public override void Clear ()
	{

		base.Clear ();

		cursor = 0;
		shown.Clear ();
	}

	public override void RemoveCard (Card c)
	{
		if (shown.Contains (c)) {
			shown.Remove (c);
			if (cursor > 0)
				cursor--;
			UpdateShown (false);
			//Debug.Log("cursor now " + cursor);
		}

		base.RemoveCard (c);
	}

	public override void AddCard (Card c, bool withAnimation = false, float delay = 0f)
	{
		c.ColliderH = cardSize.y;

		if (!IsReady) {
			base.AddCard (c);
			int count = Size;
			c.gameObject.transform.position = position;
			c.SetSide (true);
			c.SetOrder (baseOrder - count);

			if (count > 1) {
				Cards [count - 2].shadow.enabled = false;
			}
			c.shadow.enabled = true;
		} else {
			shown.Add (c);
			UpdateShown ();
		}

	}

	#endregion

}


