using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Stack of cards
/// </summary>
public class KlondikeCardStack: Slot
{
	private float maxLen;
	private float faceOffset;
	//private float realOffset;
	private float backOffset;
	private float maxOffset;

	//private float length;

	public KlondikeCardStack (int index, Vector2 cardSize, GameObject o) : base (index, cardSize, o)
	{
		position = new Vector3 (index * cardSize.x, 0f, 0f);	
	}

	public float MaxLen {
		get{ return maxLen; }
		set { 
			
			float prev = maxLen;
			maxLen = value;

			if (maxLen != prev) {				
				CalculateOffset ();
				UpdatePosition ();
			}

		}
	}

	public float BackOffset {
		get{ return backOffset; }
		set { 
			float prev = backOffset;
			backOffset = value;

			if (backOffset != prev) {
				UpdatePosition ();
			}
		}
	}

	public float MaxOffset {
		get{ return maxOffset; }
		set { 			
			float prev = maxOffset;
			maxOffset = value;

			if (maxOffset != prev && prev == faceOffset) {
				faceOffset = maxOffset;
				UpdatePosition ();
			}
		}
	}

	public override Vector3 Position {
		set { 
			base.Position = value;
			UpdatePosition ();

		}
	}

	private void UpdatePosition ()
	{		
		for (int i = 0; i < Size; i++) {			
			Cards [i].gameObject.transform.position = new Vector3 (position.x, position.y - CardOffset (i), 0f);
		}
	}

	//first face up card in stack
	public Card FirstFaceUp {
		get {
			
			for (int i = 0; i < Size; i++)
				if (Cards [i].IsUp) {
					//Debug.Log (string.Format ("{0} top card is {1}", Name, Card.CardString(Cards [i])));
					return Cards [i];
				}			

			return null;			
		}
	}

	#region Slot

	public override void Clear ()
	{
		base.Clear ();
		//length = 0;
	}

	public override string Name {
		get { 
			return "Stack_" + index;
		}
	}

	public override void RemoveCard (Card c)
	{
		base.RemoveCard (c);

		faceUpCount--;

		if (Head != null) {
			if (!Head.IsUp) {
				Head.SetOrder (1000 + faceUpCount);		
				Head.SetSide (false,true);	
				faceUpCount++;
			}
			Head.ColliderH = cardSize.y;
		} 
	}

	public override void RemoveGroup (CardGroup cg)
	{		
		base.RemoveGroup(cg);

		if (CardOffset (Size - 1) <= maxLen) {			
			CalculateOffset();
			UpdatePosition();
		}
	}

	public override bool CheckCard (Card c)
	{
		return (Size == 0 && c.Value == 12) || (Card.CardsOfOppositeColor (c, Head) && Head.Value - c.Value == 1 && c.Value > 0);
		//return true;
	}

	public override void PlaceGroup (CardGroup cg, bool withAnimation = false)
	{
		
		float groupMaxSize = (cg.Size - 1) * faceOffset;	 

		if (CardOffset (Size - 1) + groupMaxSize > maxLen) {			
			CalculateOffset(cg);
			UpdatePosition ();
		}

		AddCard (cg.Root, withAnimation);

		float delay = CardAnimationSettings.CardGroupDelay;
		if (cg.Size > 1) {
			foreach (var e in cg.Elements) {
				AddCard (e, withAnimation, delay);
				delay += CardAnimationSettings.CardGroupDelay;
			}
		}  
	}

	int faceUpCount = 0;

	public float CardOffset (int index)
	{
		float res = 0f;

		if (index >= Size || index <= 0)
			return res;

		for (int i = 0; i < index; i++) {
			res += Cards [i].IsUp ? faceOffset : backOffset;
		}
		return res;
	}

	public override void AddCard (Card c, bool withAnimation = false, float delay = 0f)
	{	
		Card prev = Head;
		Cards.Add (c);	

		int count = Cards.Count;
		int order = c.IsUp ? 1000 + faceUpCount : count;

		c.SetOrder (order);
		c.ColliderH = cardSize.y;
		c.gameObject.SetActive (true);

		if (c.IsUp) {
			faceUpCount++;
		}

		if (count > 1) {			
			prev.ColliderH = Head.IsUp ? faceOffset : backOffset;
		}
	
		Vector3 newPos = new Vector3 (position.x, position.y - CardOffset (count - 1), c.transform.position.z);

		if (!withAnimation)
			c.transform.position = newPos;
		else
			c.MoveWithAnimation (newPos, CardAnimationSettings.CardMoveDuration, false, delay);
	}

	private void CalculateOffset (CardGroup groupToAdd = null)
	{			
		if ((Size == 0 || faceUpCount < 2) && groupToAdd == null) {						
			faceOffset = maxOffset;
			return;
		}

		float newOffset = MaxOffset;
		
		int faceDownCount = Size - faceUpCount;
		newOffset = (MaxLen - faceDownCount * BackOffset) / ((faceUpCount - 1) + (groupToAdd == null ? 0 : groupToAdd.Size));
		faceOffset = Mathf.Min (maxOffset, newOffset);
	}

	public override CardGroup GetGroup (Card c)
	{
		CardGroup res = base.GetGroup (c);
	
		int ind = Cards.IndexOf (c);

		for (int i = ind + 1; i < Size; i++) {			
			res.AddElement (Cards [i]);
		}
		return res;
	}

	#endregion
}


