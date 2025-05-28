using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using ui.windows;

/// <summary>
/// Slot that can contain cards
/// </summary>
public abstract class Slot
{
	protected Vector3 slotPosition;
	protected GameObject slotObject;
	protected int baseOrder = 0;
	protected int index;
	protected SpriteRenderer glowSprite;
	protected SpriteRenderer mainSprite;
	protected Tweener t_hint;

	/// <summary>
	/// Gets the list of all cards.
	/// </summary>
	/// <value>The cards list.</value>
	public List<Card> Cards{ get; private set; }

	/// <summary>
	/// Gets the size of this slot (cards count).
	/// </summary>
	/// <value>The size.</value>
	public int Size {
		get{ return Cards.Count; }
	}

	/// <summary>
	/// Gets or sets the position of this slot.
	/// </summary>
	/// <value>The position.</value>
	public virtual Vector3 Position {
		set { 
			slotPosition = value;
			if (slotObject != null)
				slotObject.transform.position = slotPosition;
		}
		get { 
			return slotPosition;
		}
	}

	/// <summary>
	/// Gets the name of this slot.
	/// </summary>
	/// <value>The name.</value>
	public virtual string Name {
		get { 
			return "Slot_" + index;
		}
	}

	/// <summary>
	/// Gets the top (last) card of this slot.
	/// </summary>
	public Card Head { get { return Size > 0 ? Cards [Size - 1] : null; } }

	/// <summary>
	/// Gets or sets a value indicating whether this slot is shown.
	/// </summary>
	/// <value><c>true</c> if this instance is shown; otherwise, <c>false</c>.</value>
	public bool IsShown {
		get{ return slotObject.activeSelf; }
		set{ slotObject.SetActive (value); }
	}

	/// <summary>
	/// Gets slot game object ref.
	/// </summary>
	public GameObject Object {
		get{ return slotObject; }	
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="Slot"/> class.
	/// </summary>
	/// <param name="index">Index of this slot.</param>
	/// <param name="cardSize">Card size vector.</param>
	/// <param name="o">Game object of this slot.</param>
	public Slot (int index, GameObject o)
	{
		this.slotObject = o;
		this.index = index;

		if (slotObject == null) {
			slotObject = new GameObject ();						
		} 

		if ((mainSprite = slotObject.GetComponent<SpriteRenderer> ()) == null) {
			mainSprite = slotObject.AddComponent<SpriteRenderer> ();			
		}

		slotObject.name = Name;

		//create glow sprite renderer, to animate hints
		GameObject glowObject = new GameObject ();
		glowObject.name = "glow";
		glowSprite = glowObject.AddComponent<SpriteRenderer> ();
		glowSprite.color = new Color (1f, 1f, 1f, 0f);
		glowSprite.sprite = ThemeManager.HintSprite;
		glowSprite.sortingOrder = 300;
		glowObject.transform.SetParent (slotObject.transform);
		glowSprite.enabled = false;	

		slotPosition = Vector3.zero;

		Cards = new List<Card> ();
	}

	/// <summary>
	/// If this slot contains given card
	/// </summary>
	/// <returns><c>true</c>, if contains card, <c>false</c> otherwise.</returns>
	/// <param name="c">Card</param>
	public virtual bool ContainsCard (Card c)
	{
		return Cards.Contains (c);		
	}

	/// <summary>
	/// Clear this slot from cards.
	/// </summary>
	public virtual void Clear ()
	{
		Cards.Clear ();
	}

	/// <summary>
	/// Determines whether given group can be picked from this slot.
	/// </summary>
	/// <returns><c>true</c> if this group can be picked; otherwise, <c>false</c>.</returns>
	/// <param name="cg">Card group.</param>
	public virtual bool CanBePicked (CardGroup cg)
	{
		//we can pick group or single card when it is a head of slot
		return ContainsCard (cg.Root) && (cg.Size > 1 || cg.Root == Head);
	}

	/// <summary>
	/// Removes the card from slot.
	/// </summary>
	/// <param name="c">Card.</param>
	public virtual void RemoveCard (Card c)
	{
		if (Cards.Contains (c))
			Cards.Remove (c);
	}

	/// <summary>
	/// Removes the group from this slot.
	/// </summary>
	/// <param name="cg">Card group</param>
	public virtual void RemoveGroup (CardGroup cg)
	{		
		RemoveCard (cg.Root);

		if (cg.Size > 1) {
			foreach (var e in cg.Elements) {
				RemoveCard (e);
			}
		}
	}

	/// <summary>
	/// Determines whether given point in slot cell(rect)
	/// </summary>
	/// <returns><c>true</c> if given point inside cell of this slot; otherwise, <c>false</c>.</returns>
	/// <param name="point">Point.</param>
	public bool IsPointInsideCell (Vector2 point)
	{
		return (point.x >= slotPosition.x - ThemeManager.CardSize.x &&
		point.x <= slotPosition.x + ThemeManager.CardSize.x &&
		point.y >= slotPosition.y - ThemeManager.CardSize.y &&
		point.y <= slotPosition.y + ThemeManager.CardSize.y);

	}

	/// <summary>
	/// Places (adds) the group to this slot.
	/// </summary>
	/// <param name="cg">Card group.</param>
	/// <param name="withAnimation">If <c>true</c> use the animation.</param>
	public virtual void PlaceGroup (CardGroup cg, bool withAnimation = false)
	{			
		AddCard (cg.Root, withAnimation);
		if (cg.Size > 1) {
			foreach (var e in cg.Elements) {
				AddCard (e, withAnimation);
			}
		}
	}

	/// <summary>
	/// Gets the group starting from given card.
	/// </summary>
	/// <returns>The card group.</returns>
	/// <param name="c">Card.</param>
	public virtual CardGroup GetGroup (Card c)
	{
		CardGroup res = new CardGroup (c);
		res.Slot = this;
		return res;
	}

	/// <summary>
	/// Adds the card to slot.
	/// </summary>
	/// <param name="c">Card.</param>
	/// <param name="withAnimation">If set to <c>true</c> use animation.</param>
	/// <param name="delay">Action delay.</param>
	public virtual void AddCard (Card c, bool withAnimation = false, float delay = 0f)
	{		 	
		Cards.Add (c);

		int count = Cards.Count;
		c.SetSortingOrder (count - 1);
		c.SortingLayer = "Default";
		c.transform.rotation = Quaternion.identity;
		c.SetSide (false);
		c.gameObject.SetActive (true);
	}

	/// <summary>
	/// Checks if card can be placed to this slot.
	/// </summary>
	/// <returns><c>true</c>, if card can be placed, <c>false</c> otherwise.</returns>
	/// <param name="c">Card.</param>
	public virtual bool CanPlaceCard (Card c)
	{
		return true;
	}

	/// <summary>
	/// Checks if card group can be placed to this slot.
	/// </summary>
	/// <returns><c>true</c>, if group can be placed, <c>false</c> otherwise.</returns>
	/// <param name="c">C.</param>
	public virtual bool CanPlaceGroup (CardGroup c)
	{
		return true;
	}

	/// <summary>
	/// Restores group position, if it was moved, but not deleted from slot.
	/// </summary>
	/// <param name="c">Card group.</param>
	/// <param name="pos">Position.</param>
	public virtual void RestoreGroupPos (CardGroup c, Vector3 pos, bool withAnimation)
	{
		if (!GameSettings.Instance.Data.BgAnimEnabled)
			withAnimation = false;
		
		float duration = CardAnimationSettings.CardMoveDuration;
		float delay = CardAnimationSettings.CardGroupDelay;

		if (withAnimation)
			c.Root.MoveWithAnimation (pos, duration);
		else
			c.Root.MoveInstantly (pos);

		for (int i = 0; i < c.Elements.Count; i++) {
			if (withAnimation)
				c.Elements [i].MoveWithAnimation (pos, duration, delay * (i + 1));
			else
				c.Elements [i].MoveInstantly (pos);
		}
	}

	/// <summary>
	/// Update cards order depending on their index
	/// </summary>
	public virtual void UpdateOrder ()
	{
		for (int i = 0; i < Size; i++) {
			Cards [i].SetSortingOrder (i);
		}
	}

	/// <summary>
	/// Set the sprite of this slot.
	/// </summary>
	/// <param name="s">Sprite.</param>
	public virtual void SetSprite (Sprite s)
	{
		if (slotObject == null)
			return;

		mainSprite.sprite = s;
	}

	/// <summary>
	/// Gets the slot state.
	/// </summary>
	/// <returns>The state of this slot.</returns>
	public virtual SlotState GetState ()
	{
		SlotState res = new SlotState ();
		res.SlotName = Name;
		res.AddField ("index", index);

		if (Cards.Count > 0) {	
			res.CardIndexes = new int[Cards.Count];
			for (int i = 0; i < res.CardIndexes.Length; i++) {
				res.CardIndexes [i] = Cards [i].Index;
			}
		}
		return res;
	}

	/// <summary>
	/// Sets the slot state.
	/// </summary>
	/// <returns><c>true</c>, if success, <c>false</c> otherwise.</returns>
	/// <param name="state">Slot state.</param>
	public virtual bool SetState (SlotState state)
	{			
		Cards.Clear ();
		var cards = KlondikeController.Instance.shuffleManager.CardIndexesToRefs (state.CardIndexes).ToArray ();
		if (cards != null && cards.Length > 0) {			
			CardGroup newGroup = new CardGroup (cards);
			PlaceGroup (newGroup, false);
		}

		return true;
	}

	/// <summary>
	/// Stops the hint animation.
	/// </summary>
	public virtual void StopHintAnimation ()
	{
		if (t_hint != null)
			t_hint.Complete ();
	}

	/// <summary>
	/// Plays the hint animation.
	/// </summary>
	/// <param name="c">Card.</param>
	public virtual void PlayHintAnimation (CardGroup c)
	{		
		glowSprite.enabled = true;

		if (t_hint == null) {
			t_hint = Card.CreateHintAnimation (glowSprite);
			t_hint.OnComplete (() => {
				glowSprite.enabled = false;
			});
			t_hint.Play ();
		} else {
			t_hint.Restart ();
		}
	}

	/// <summary>
	/// Determines whether given rect overlaps slot rect.
	/// </summary>
	/// <returns><c>true</c> if given rect overlaps slot rect; otherwise, <c>false</c>.</returns>
	/// <param name="rect">Rect.</param>
	public bool IsRectOverlapsSlot (Rect rect)
	{
		Vector2 pos = slotPosition;
		Rect r = new Rect (
			         pos - ThemeManager.CardSize / 2f, ThemeManager.CardSize
		         );	

		bool res = rect.Overlaps (r);	

		return res;
	}


    /// <summary>
    /// возвращает группу карт, в указанную глубину
    /// </summary>
    /// <param name="depth">глубина группы карт</param>
    public CardGroup GetGroupDepth(int depth)
    {
        var elements = new List<Card>(depth);
        for (var i = Size - depth; i < Size; i++) {
            elements.Add(Cards[i]);
        }

        return new CardGroup(elements.ToArray()) {
            Slot = this
        };
    }
}


