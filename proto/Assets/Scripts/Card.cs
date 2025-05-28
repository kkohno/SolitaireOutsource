using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class CardEventArgs
{
	public Card Card{ get; private set; }

	public CardEventArgs (Card cardRef)
	{
		Card = cardRef;
	}
}

public enum Suit
{
	Clubs,
	Hearts,
	Spades,
	Diamonds
}

public enum CardColor
{
	Red,
	Black
}

public enum ValueName
{
	Ace = 0,
	Two,
	Three,
	Four,
	Five,
	Six,
	Seven,
	Eight,
	Nine,
	Twelve,
	Jack,
	Queen,
	King
}



public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

	public SpriteRenderer sr;
	public SpriteRenderer glow;
	public SpriteRenderer shadow;

	public delegate void CardPressed (CardEventArgs a);

	public event CardPressed Pressed;
	public event CardPressed Released;

	public bool IsUp{ get; private set; }

	public int Index{ get; set; }

	private Sprite face;
	private Sprite jacket;

	private Tweener t_select;
	private Sequence s_success;
	//private Tweener t_fail;
	private Tweener t_move;
	private Tweener t_glow;
	private Tweener t_rotate;

	private Vector3 originalPos;
	private BoxCollider2D _collider;
	//private bool inAnimation;
	private bool isShaking;

	public bool InAnimation {
		get{
			//return inAnimation;
			return (t_glow != null && t_glow.IsPlaying()) ||
				(t_select != null && t_select.IsPlaying()) ||
				(t_move != null && t_move.IsPlaying()) ||
				(t_rotate != null && t_rotate.IsPlaying()) ||
				(s_success != null && s_success.IsPlaying()) ||
			isShaking;

				
		}
	}

	public BoxCollider2D  Collider {
		get {
			if (_collider == null) {
				_collider = GetComponent<BoxCollider2D> ();
			}
			return _collider;
		}
	}

	public static string CardString (Card c)
	{
		return string.Format ("{0} of {1}", (ValueName)c.Value, c.Suit.ToString ().ToLower ());
	}

	public static bool CardsOfOppositeColor (Card c1, Card c2)
	{
		if (c1 == null || c2 == null)
			return false;
		return c1.CardColor != c2.CardColor;			
	}

	public float ColliderH {
		set { 
			Collider.size = new Vector2 (_collider.size.x, value);
			Collider.offset = new Vector2 (0f, sr.sprite.bounds.size.y / 2f - value / 2f);
		}
	}

	public void SetFaceSprite (Sprite spr)
	{		
		sr.sprite = face = spr;
		GetComponent<BoxCollider2D> ().size = new Vector2 (sr.sprite.bounds.size.x, sr.sprite.bounds.size.y);
	}

	public void SetBackSprite (Sprite spr)
	{		
		sr.sprite = jacket = spr;
	}

	public void SetShadowSprite (Sprite spr)
	{		
		shadow.sprite = spr;
	}

	public void SetGlowSprite (Sprite spr)
	{		
		glow.sprite = spr;
	}

	public bool Active {
		get {
			return GetComponent<BoxCollider2D> ().enabled;
		}
		set { 			
			shadow.enabled = GetComponent<BoxCollider2D> ().enabled = value;
		}
	}

	public void SetSide (bool back, bool withAnimation = false, float delay = 0f, int newOrder = -1)
	{	
		IsUp = Active = !back;

		if (newOrder == -1)
			newOrder = sr.sortingOrder;

		Action setAction = () => {
			sr.sprite = back ? jacket : face;
		};

		if (!withAnimation)
			setAction ();
		else {
			
			if (t_rotate != null) {
				t_rotate.Rewind ();
				t_rotate.Kill ();
			}

			t_rotate = gameObject.transform.DOScaleX (0f, CardAnimationSettings.CardRotateDuration).SetLoops (2, LoopType.Yoyo);
			t_rotate.SetAutoKill (false);
			t_rotate.SetDelay (delay);
			t_rotate.Play ();		

			if (gameObject.activeSelf)
				StartCoroutine (CoroutineExtension.ExecuteAfterTime (delay + CardAnimationSettings.CardRotateDuration, () => {				
					setAction ();
					SetOrder(newOrder);
				}));
			else
				setAction ();
		}
	}

	public void StopAllAnimation ()
	{
		if (t_move != null) {
			t_move.Complete ();
			t_move.Kill ();
		}

		StopCoroutine (ShakeRoutine ());

		if (t_select != null) {
			t_select.Rewind ();
			//t_select.Kill ();
		}

		if (t_glow != null) {
			t_glow.Rewind ();
			glow.enabled = false;
			//t_glow.Kill ();
		}

		if (s_success != null) {
			s_success.Complete ();
			//s_success.Kill ();
		}

	}

	public void SetOrder (int value)
	{		
		shadow.sortingOrder = (sr.sortingOrder = value) - 2;
		glow.sortingOrder = value - 1;
		/*
		 = value - 1; 
		shadow.sortingOrder = value - 2;*/

		//transform.localPosition = new Vector3 (transform.localPosition.x, transform.localPosition.y, -value * 0.1f);
	}

	public void SetLayer (string layer)
	{
		glow.sortingLayerName = shadow.sortingLayerName = sr.sortingLayerName = layer;
	}

	public bool IsPointInside2D (Vector2 point)
	{

		Bounds b = sr.sprite.bounds;

		bool res = (b.min.x + transform.position.x <= point.x &&
		           b.max.x + transform.position.x >= point.x &&
		           b.min.y + transform.position.y <= point.y &&
		           b.max.y + transform.position.y >= point.y);		
		
		return res;
	}

	public bool IsRectInside2D (Rect rect)
	{
		Bounds b = sr.sprite.bounds;

		bool res = (b.min.x + transform.position.x <= rect.min.x &&
			b.max.x + transform.position.x >= rect.max.x &&
			b.min.y + transform.position.y <= rect.min.y &&
			b.max.y + transform.position.y >= rect.max.y);		

		return res;
	}

	public CardColor CardColor {
		get { 
			
			return (Suit == Suit.Clubs || Suit == Suit.Spades) ? CardColor.Black : CardColor.Red;
				
		}
	}

	public Suit Suit {
		get { 
			return (Suit)(Index / 13);
		}
	}

	public int Value {
		get { 			
			return Index - ((Index) / 13) * 13; 
		}
	}

	public void MoveWithAnimation (Vector3 pos, float duration = 0.1f, bool playSuccess = false, float delay = 0f, Action callback=null)
	{
		if (t_move != null && t_move.IsPlaying()) {
			t_move.Pause ();
			t_move.Kill ();
		}

		//inAnimation = true;

		t_move = gameObject.transform.DOMove (pos, duration);
		t_move.SetAutoKill (true);
		t_move.SetDelay (delay);
			
		t_move.OnComplete (() => {
			//inAnimation = false;
			if (playSuccess) {
				SuccessAnimation ();
			}		
			if(callback!=null)
				callback();
		});

		t_move.Play ();
	}

	public void SelectAnimation ()
	{
		float duration = CardAnimationSettings.SelectDuration;

		if (s_success != null) {
			s_success.Rewind ();
		}

		StopCoroutine (ShakeRoutine ());

		/*if (t_fail != null) {
			t_fail.Complete ();
		}*/

		if (s_success != null) {
			s_success.Rewind ();
		}

		glow.enabled = true;
		if (t_glow == null) {
			t_glow = DOTween.ToAlpha (() => glow.color, (x) => glow.color = x, 1f, duration);
			//t_glow.OnComplete (()=>{glow.enabled= false;});
			t_glow.SetAutoKill (false);
		} else {
			t_glow.OnStepComplete (null);
			t_glow.Restart ();
		}

		if (t_select == null) {			
			t_select = gameObject.gameObject.transform.DOScale (CardAnimationSettings.SelectedScaleRate, duration);
			t_select.SetAutoKill (false);
			t_select.Play ();
		} else {			
			t_select.OnStepComplete (null);
			t_select.Restart ();
		}
	}

	public void ReleaseAnimation (Action endAction = null)
	{
		if (endAction != null) {
			t_select.OnStepComplete (() => {
				endAction ();
			});
		}

		t_select.PlayBackwards ();
		t_glow.PlayBackwards ();
		t_glow.OnStepComplete (() => {
			glow.enabled = false;
		});
	}

	public void SuccessAnimation ()
	{	
		float duration = CardAnimationSettings.SuccessDuration;
		float alphaStart = duration / 2f;

		if (t_glow != null && t_glow.IsPlaying ()) {
			t_glow.Complete ();
			t_glow.Rewind ();
		}
		glow.enabled = true;
		glow.color = Color.white;

		if (s_success == null) {
			s_success = DOTween.Sequence ();
			s_success.Insert (0f, glow.gameObject.transform.DOScale (Vector2.one * 2f, duration));
			s_success.Insert (alphaStart, DOTween.ToAlpha (() => glow.color, (x) => glow.color = x, 0f, duration - alphaStart));
			s_success.OnComplete (() => {
				glow.transform.localScale = Vector3.one;
				glow.enabled = false;
			});
			s_success.SetAutoKill (false);
			s_success.Play ();
		} else {			
			s_success.Restart ();
		}
	}
	/*public void FailAnimation(){
		float duration = CardAnimationSettings.FailShakeDuration;

		if (t_fail == null) {
			t_fail = gameObject.transform.DOShakePosition (duration, new Vector3 (sr.bounds.size.x * CardAnimationSettings.ShakeStrength, 0f, 0f), 10,0);
			t_fail.Play ();
			return;
		} else if (t_fail.IsPlaying ()) {
			t_fail.Rewind ();
		}

		t_fail.Restart ();
	}*/

	public void FailAnimation ()
	{
		StopShake ();
		StartCoroutine (ShakeRoutine ());
	}

	private void StopShake ()
	{
		StopCoroutine (ShakeRoutine ());

		if (isShaking)
			gameObject.transform.position = originalPos;
		isShaking = false;
	}

	private IEnumerator ShakeRoutine ()
	{
		isShaking = true;
		float time = 0f;
		float speed = 1f / CardAnimationSettings.FailShakeDuration;
		originalPos = gameObject.transform.position;

		while (time < 1f) {
			gameObject.transform.position = originalPos + new Vector3 (CardAnimationSettings.ShakeStrength * CardAnimationSettings.ShakeCurve.Evaluate (time), 0f, 0f);
			time += Time.deltaTime * speed;
			yield return new WaitForFixedUpdate ();
		}
		isShaking = false;
	}

	#region IPointerDownHandler implementation

	public void OnPointerDown (PointerEventData eventData)
	{	
		if (InAnimation)
			return;
		
		//Debug.Log ("card clicked " + name);
		if (Pressed != null)
			Pressed.Invoke (new CardEventArgs (this));
	}

	#endregion

	#region IPointerUpHandler implementation

	public void OnPointerUp (PointerEventData eventData)
	{			
		if (Released != null)
			Released.Invoke (new CardEventArgs (this));
	}

	#endregion
}

