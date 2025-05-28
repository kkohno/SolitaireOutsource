using System;
using UnityEngine;

public class KlondikeSlot:Slot
{
	
	public KlondikeSlot (int index, Vector2 cardSize, GameObject o) : base (index, cardSize, o)
	{			
		if (go != null) {
			//go.transform.position = position = new Vector3 (index * cardSize.x, 1.5f, 0f);
			go.GetComponentInChildren<SpriteRenderer> ().color = new Color (0.3f, 0.3f, 0.3f, 0.5f);
		}
	}

	#region Slot

	public override Vector3 Position{
		set{ 

			base.Position = value;

			for (int i=0; i< Size; i++) {				
				Cards[i].gameObject.transform.position = position;
			}
		}
	}

	public override string Name {
		get { 
			return "FreeSlot_" + index;
		}
	}

	public override bool CheckCard (Card c)
	{		
		if (c == null)
			return false;

		if (Size == 0 && c.Value == 0) 			
			return true;		
		else if (Head != null) {
			return Head.Suit == c.Suit && c.Value - Head.Value == 1;
		}

		return false;
	}

	public override void RemoveCard (Card c)
	{
		base.RemoveCard (c);
		int count = Size;
		if (count>0) {
			Cards [count - 1].SetOrder (1001);
			if (count > 1) {
				Cards [count - 2].gameObject.SetActive (true);
			}
		}
	}

	public override void AddCard (Card c, bool withAnimation = false, float delay = 0f)
	{	
		//Debug.Log (withAnimation);	
		base.AddCard (c, withAnimation);

		int count = Size;

		if (count > 1) {
			Cards [count - 2].SetOrder (1000);
			if (count > 2) {
				Cards [count - 3].gameObject.SetActive (false);
			}
		}

		c.SetOrder (1001);

		Vector3 pos = new Vector3 (position.x, position.y, -0.1f * Size);

		if (!withAnimation)
			c.gameObject.transform.position = pos;
		else
			c.MoveWithAnimation (pos, CardAnimationSettings.CardMoveDuration, true);
	}

	#endregion
}


