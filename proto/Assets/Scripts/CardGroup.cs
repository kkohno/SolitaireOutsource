using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardGroup
{
	public Card Root{ get; private set; }

	public Slot Slot{ get; set; }

	public bool Active{ get; private set; }

	public float ShadowOffset{ get; set;}

	public List<Card> Elements{ get; private set; }

	List<Vector3> offsets;
	Tweener t_shadow;
	float shadowY;

	public CardGroup (Card c)
	{
		ShadowOffset = 10f;
		Active = true; 
		Root = c;
	}

	public int Size {
		get { 
			return Elements == null ? 1 : Elements.Count + 1;
		}
	}

	public float Length {
		get { 
			return offsets == null ? 0f : Mathf.Abs(offsets[offsets.Count - 1].y);
		}
	}

	public static float StrechShadowY(float offset, Vector2 size){
		return (offset + size.y) / size.y;
	}

	public void AddElement (Card c)
	{
		
		if (Elements == null)
			Elements = new List<Card> ();
		if (offsets == null)
			offsets = new List<Vector3> ();
		
		Elements.Add (c);
		offsets.Add (c.transform.position - Root.transform.position);
	}

	public string Layer {
		get{ return Root.sr.sortingLayerName; }
		set { 
			
			Root.SetLayer (value);

			if (Elements == null)
				return;

			for (int i = 0; i < Elements.Count; i++) {			
				Elements [i].SetLayer (value);
			}
		}
	}

	public Vector3 Position {
		set {
			Root.transform.position = value;

			if (Elements == null)
				return;

			for (int i = 0; i < Elements.Count; i++) {			
				Elements [i].transform.position = value + offsets [i];
			}
		}
		get { 
			return Root.transform.position;
		}
	}

	public void SelectAnimation ()
	{		
		
		int count;

		if (offsets != null && (count = offsets.Count) > 0) {		
			//offset of last card in group
			//!!! during selection scaling our offsets will not change, so we must correct value with scale delta
			float lastOffset = Mathf.Abs (offsets [count - 1].y) / CardAnimationSettings.SelectedScaleRate;	

			//calculate size of shadow to fit all group
			float scaleY = StrechShadowY(lastOffset,Root.sr.bounds.size);
				//(lastOffset + Root.sr.bounds.size.y) / Root.sr.bounds.size.y;

			//float scaleY = StrechShadow(lastOffset, Root.sr.bounds.size);

			//stretch root shadow for all group
			Root.shadow.transform.localScale = new Vector3 (1f, scaleY, 1f);

			//correcting Y pos of shadow with pivot at center according new scale
			shadowY = (scaleY - 1) * Root.sr.bounds.size.y / 2;

			//place shadow with center pivot to fit group 
			Root.shadow.transform.localPosition = new Vector3(0f, -shadowY, 0f);
		}

		//animate shadow of group appearing
		t_shadow = Root.shadow.transform.DOLocalMove (
			new Vector3 (Root.shadow.transform.localScale.x / ShadowOffset, -shadowY - Root.sr.bounds.size.y / ShadowOffset, 0f),  CardAnimationSettings.SelectDuration);				
		t_shadow.SetAutoKill (true);
		t_shadow.Play ();

		//run selection animation for all elements and root
		Root.SelectAnimation ();

		if (Elements != null && Elements.Count > 0)
			foreach (var e in Elements) {
				
				//as we use one shadow we hide all the element individual shadows
				e.shadow.enabled = false;

				e.SelectAnimation ();
			}
	}

	public void ReleaseAnimation (Action endAction = null)
	{
		//to prevent this group from updating from controller during release animation we mark it as innactive
		Active = false;
		Root.ReleaseAnimation (endAction);	
		//return regular individual shadow size
		Root.shadow.transform.localScale = Vector3.one;
		//correcting shadow position as its size changed
		Root.shadow.transform.localPosition = new Vector3 (Root.shadow.transform.localScale.x / ShadowOffset, -Root.sr.bounds.size.y / ShadowOffset, 0f);

		//animate shadow disappearance
		t_shadow = Root.shadow.transform.DOLocalMove (Vector3.zero, CardAnimationSettings.SelectDuration);
		t_shadow.SetAutoKill (true);
		t_shadow.Play ();

		//and animate each element 
		if (Elements != null && Elements.Count > 0) {

			Tweener t_elemShadow;
			foreach (var e in Elements) {
				//place individual shadow to proper place
				e.shadow.transform.localPosition = new Vector3 (e.sr.bounds.size.x / ShadowOffset, -e.sr.bounds.size.y / ShadowOffset, 0f);				
				t_elemShadow = e.shadow.transform.DOLocalMove (Vector3.zero, CardAnimationSettings.SelectDuration);
				t_elemShadow.SetAutoKill (true);
				t_elemShadow.Play ();
				e.ReleaseAnimation ();
				e.shadow.enabled = true;
			}
		} 	
	}

	public void MoveWithAnimation (Vector3 pos)
	{
		
		float duration = CardAnimationSettings.CardMoveDuration;
		float delay = CardAnimationSettings.CardGroupDelay;

		Root.MoveWithAnimation (pos, duration);

		if (Elements != null && Elements.Count > 0)
			for (int i = 0; i < Elements.Count; i++) {
				Elements [i].MoveWithAnimation (pos + offsets [i], duration, false, delay * (i+1));	
			}
	}

	public void FailShake(){
		Root.FailAnimation();
		if (Elements != null && Elements.Count > 0)
			for (int i = 0; i < Elements.Count; i++) {
				Elements [i].FailAnimation();	
			}
	}
}

