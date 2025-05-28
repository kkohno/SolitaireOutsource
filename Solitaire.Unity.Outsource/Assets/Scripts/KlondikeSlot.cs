using System;
using ui;
using UnityEngine;

/// <summary>
/// Klondike foundation slot.
/// </summary>
public class KlondikeSlot:Slot
{
	private GameEffect effect;

	public KlondikeSlot (int index, GameObject o, GameEffect _effect) : base (index, o)
	{			
		if (slotObject != null) {			
			slotObject.name = "freeSlot" + index;
		}
		effect = _effect;
		effect.gameObject.layer = UILayout.BACKGROUND_LAYER_ID;
		effect.transform.SetParent (slotObject.transform);
		effect.gameObject.transform.position = slotPosition;

		mainSprite.sortingOrder = baseOrder = 100;
		glowSprite.gameObject.layer = UILayout.BACKGROUND_LAYER_ID;
	}

	#region Slot

	/// <summary>
	/// Plays the hint animation.
	/// </summary>
	/// <param name="c">Card.</param>
	public override void PlayHintAnimation (CardGroup c)
	{
		base.PlayHintAnimation (c);
	}

	/// <summary>
	/// Gets or sets the position of this foundation slot.
	/// </summary>
	/// <value>The position.</value>
	public override Vector3 Position {
		set { 
			effect.gameObject.transform.position = base.Position = value;
			for (int i = 0; i < Size; i++) {				
				Cards [i].MoveInstantly (slotPosition);
			}
		}
	}

	/// <summary>
	/// Gets or sets the layer.
	/// </summary>
	/// <value>The layer index.</value>
	public int Layer {
		get { 			
			return slotObject.layer;
		}
		set { 			
			effect.gameObject.layer = glowSprite.gameObject.layer = slotObject.layer = value;
		}
	}

	/// <summary>
	/// Gets the name of this foundation slot.
	/// </summary>
	/// <value>The name.</value>
	public override string Name {
		get { 			
			return "FreeSlot_" + index;
		}
	}

	/// <summary>
	/// Checks if card can be placed to this foundation slot.
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	/// <param name="c">Card.</param>
	public override bool CanPlaceCard (Card c)
	{		
		if (c == null)
			return false;

		if (Size == 0 && c.Value == 12)
			return true;
		else if (Head != null) {
			return Head.Suit == c.Suit && (c.Value - Head.Value == 1 || (c.Value == 0 && Head.Value == 12));
		}

		return false;
	}

	/// <summary>
	/// Checks if card group can be placed to this foundation slot.
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	/// <param name="c">C.</param>
	public override bool CanPlaceGroup (CardGroup c)
	{
		if (c == null || c.Size != 1)
			return false;

		return CanPlaceCard (c.Root);	
	}

	/// <summary>
	/// Removes the card from foundation slot.
	/// </summary>
	/// <param name="c">Card.</param>
	public override void RemoveCard (Card c)
	{
		base.RemoveCard (c);
		int count = Size;
		if (count > 0) {
			Cards [count - 1].SetSortingOrder (baseOrder + baseOrder + c.LayerCount);
			Cards [count - 1].Active = true;
			if (count > 1) {
				Cards [count - 2].gameObject.SetActive (true);
			}
		}
	}

	/// <summary>
	/// Adds the card to foundationslot.
	/// </summary>
	/// <param name="c">Card.</param>
	/// <param name="withAnimation">If set to <c>true</c> use animation.</param>
	/// <param name="delay">Delay.</param>
	public override void AddCard (Card c, bool withAnimation = false, float delay = 0f)
	{			
		Cards.Add (c);

		if (Size > 1)
			Cards [Size - 2].Active = false;
		
		c.gameObject.SetActive (true);
		int count = Size;
		c.SortingLayer = UILayout.SORTING_DEFAULT;
		Action addAction = () => {
			if (count > 1) {
				Cards [count - 2].SetSortingOrder (baseOrder);
				if (count > 2) {
					Cards [count - 3].gameObject.SetActive (false);
				}
			}
			c.SetSortingOrder (baseOrder + c.LayerCount);

			if (withAnimation) {				
				effect.Play (null);
			}

			if (KlondikeController.Instance.IsGamePlayable)
				//SoundManager.Instance.PlaySound(8,0.4f,SoundManager.Instance.RandomPitch());
				SoundManager.Instance.PlaySongNote ();
		};

		Vector3 pos = new Vector3 (slotPosition.x, slotPosition.y, -0.1f * Size);

		if (!withAnimation) {
			c.MoveInstantly (pos);
			addAction ();
		} else {	
			c.MoveWithAnimation (pos, CardAnimationSettings.CardMoveDuration, delay, addAction, true);
		}
	}

	#endregion
}


