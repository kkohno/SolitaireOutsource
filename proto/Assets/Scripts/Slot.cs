using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Slot on which card can be placed;
/// </summary>
public abstract class Slot
{
	public List<Card> Cards{ get; private set; }

	protected Vector3 position;
	protected Vector2 cardSize;
	protected GameObject go;
	protected int baseOrder = 0;
	protected int index;

	public Card Head{ get { return Size > 0 ? Cards [Size - 1] : null; } }

	public Slot (int index, Vector2 cardSize, GameObject o)
	{
		this.go = o;
		this.index = index;
		this.cardSize = cardSize;

		position = Vector3.zero;

		Cards = new List<Card> ();
	}

	public virtual bool ContainsCard (Card c)
	{
		return Cards.Contains (c);		
	}

	public virtual void Clear(){
		Cards.Clear ();
	}


	public virtual Vector3 Position{
		set{ 
			position = value;
			if(go!=null)
				go.transform.position = position;
		}
		get{ 
			return position;
		}
	}

	public virtual string Name{
		get{ 
			return "Slot_" + index;
		}
	}

	public virtual void RemoveCard (Card c)
	{
		if (Cards.Contains (c))
			Cards.Remove (c);
	}

	public virtual void RemoveGroup (CardGroup cg)
	{
		
		RemoveCard (cg.Root);

		if (cg.Size > 1) {
			foreach (var e in cg.Elements) {
				RemoveCard (e);
			}

			//ListCards ();
		}

		//Debug.Log (string.Format("stack {0} len is now {1}",index,Length));
	}

	public bool IsPointInsideCell (Vector2 point)
	{
		return (point.x >= position.x - cardSize.x &&
		point.x <= position.x + cardSize.x &&
		point.y >= position.y - cardSize.y &&
		point.y <= position.y + cardSize.y);

	}

	public bool IsRectInsideCell (Rect rect)
	{

		return (rect.max.x >= position.x - cardSize.x &&
			rect.min.x <= position.x + cardSize.x &&
			rect.max.y >= position.y - cardSize.y &&
			rect.min.y <= position.y + cardSize.y);
	}

	public int Size {
		get{ return Cards.Count; }
	}

	public void PlaceCard (Card c)
	{
		//Debug.Log (string.Format("adding card {1} to stack {0} ", index, c.name));
		AddCard (c);
	}

	public virtual void PlaceGroup (CardGroup cg, bool withAnimation = false)
	{
		//Debug.Log ("slot place group");
		AddCard (cg.Root,withAnimation);

		if (cg.Size > 1) {
			foreach (var e in cg.Elements) {
				AddCard (e,withAnimation);
			}

			//ListCards ();
		}

	}

	public virtual CardGroup GetGroup (Card c)
	{
		CardGroup res = new CardGroup (c);
		res.Slot = this;
		return res;
	}

	public virtual void AddCard (Card c, bool withAnimation=false, float delay = 0f)
	{		
		Cards.Add (c);

		int count = Cards.Count;
		c.SetOrder (count - 1);

		c.SetSide (false);
		c.gameObject.SetActive (true);
	}

	public virtual bool CheckCard (Card c)
	{
		return true;
	}

	/*public virtual Type GetType(){
		return this.GetType();
	}*/

	//DEBUG
	protected void ListCards ()
	{
		
		for (int i = 0; i < Size; i++) {
			Debug.Log (Card.CardString (Cards [i]));
		}
	}
}


