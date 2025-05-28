using UnityEngine;
using DG.Tweening;
using System;

/// <summary>
/// Bubble bleak game effect controller.
/// </summary>
public class BubbleBleakController : GameEffect
{

	public GameObject bleak;
	[Range (0.1f, 3f)]
	public float inflateDuration = 1f;
	[Range (0.01f, 1f)]
	public float blowDuration = 0.7f;
	public AnimationCurve inflateCurve;
	public ParticleSystem drops;
	public SpriteRenderer symbol;

	private Vector3 prev;
	private Tweener t_inflation;
	private Sequence s_symbolFall;
	private Sequence s_blowUp;
	private Material bubbleMaterial;
	private Action<GameEffect> stopCallback;

	void Awake ()
	{				
		bubbleMaterial = GetComponent<SpriteRenderer> ().material;
	}

	/// <summary>
	/// Inflate (appear) animation of bubble.
	/// </summary>
	private void PlayInflateAnimation ()
	{	
		//complete previous animation if it is
		if (s_blowUp != null && s_blowUp.IsPlaying ())
			s_blowUp.Complete ();

		//prepare symbol for animation
		PrepareCardSymbol ();

		//clear particles system if it is still playing
		drops.Clear ();

		//shrink bubble object to small value to prepeare it for inflation
		gameObject.transform.localScale = Vector2.one * 0.1f;

		//enable renderers
		bleak.GetComponent<SpriteRenderer> ().enabled = true;
		GetComponent<SpriteRenderer> ().enabled = true;

		if (t_inflation == null) {		
			// create scale animation with curve to immitate inertia	
			t_inflation = gameObject.transform.DOScale (Vector2.one, inflateDuration).SetEase (inflateCurve);
			t_inflation.Play ();
		} else {
			//if animation already created just restart it
			t_inflation.Restart ();
		}
	}

	/// <summary>
	/// Animation of bubble  blows up.
	/// </summary>
	private void PlayBlowAnimation ()
	{	
		//complete previous animation
		if (t_inflation != null && t_inflation.IsPlaying ())
			t_inflation.Complete ();

		//set material threshold to initial value
		bubbleMaterial.SetFloat ("_Threshold", 0f);

		//enable bubble renderer
		GetComponent<SpriteRenderer> ().enabled = true;

		//create animation tween that rotates card and moves it to the bottom of the screen
		PrepareCardSymbol ();
		float minY = Camera.main.transform.position.y - Camera.main.orthographicSize - symbol.bounds.size.y;
		float speedFall = Mathf.Abs (transform.position.y - minY) / 6f;
		s_symbolFall = DOTween.Sequence ();
		s_symbolFall.Insert (blowDuration, symbol.transform.DOMoveY (minY, speedFall).SetEase (Ease.InOutSine).SetEase (Ease.InQuad));
		s_symbolFall.Insert (blowDuration, symbol.transform.DORotate (new Vector3 (0f, 0f, UnityEngine.Random.Range (-180, 180)), speedFall, RotateMode.WorldAxisAdd).SetEase (Ease.Linear));
		s_symbolFall.SetAutoKill (true);
		s_symbolFall.Play ();

		//create animation that blows bubble up and make drops
		if (s_blowUp == null) {

			s_blowUp = DOTween.Sequence ();

			// increase material threshold to the value where bubble will totally dissapear
			s_blowUp.Insert (0f, DOTween.To (
				() => bubbleMaterial.GetFloat ("_Threshold"),
				(x) => bubbleMaterial.SetFloat ("_Threshold", x),
				1.1f,
				blowDuration
			));

			//smoothly hide bleak by decreasing its alpha
			s_blowUp.Insert (0, DOTween.ToAlpha (
				() => bleak.GetComponent<SpriteRenderer> ().color,
				(x) => bleak.GetComponent<SpriteRenderer> ().color = x,
				0f,
				blowDuration / 2f
			));

			//insert callback which run particle systems after explosion is over
			s_blowUp.InsertCallback (blowDuration, () => {
				drops.Clear ();
				drops.Play ();
			});

			s_blowUp.OnComplete (() => {

				//return initial animation state to make sequance more reusable
				bleak.GetComponent<SpriteRenderer> ().color = Color.white;
				bubbleMaterial.SetFloat ("_Threshold", 0f);
				GetComponent<SpriteRenderer> ().enabled = false;
				bleak.GetComponent<SpriteRenderer> ().enabled = false;

				// notify caller that animation is over
				// it needed for animation controller
				if (stopCallback != null)
					StartCoroutine (CoroutineExtension.ExecuteAfterTime (1f, () => {
						if (stopCallback != null)
							stopCallback (this);
					}));
				
			});

			s_blowUp.Play ();

		} else {

			//if animation already exists just restart it
			s_blowUp.Restart ();
		}

	}

	/// <summary>
	/// Prepares the card symbol for animation.
	/// </summary>
	private void PrepareCardSymbol ()
	{
		if (s_symbolFall != null && s_symbolFall.IsPlaying ()) {			
			s_symbolFall.Kill ();
		}
		symbol.transform.localRotation = Quaternion.identity;
		symbol.transform.localPosition = Vector3.zero;
	}

	/// <summary>
	/// Sets the card symbol of given index to this instance of effect.
	/// </summary>
	/// <param name="index">Index.</param>
	public void SetSymbol (int index)
	{
		symbol.sprite = ThemeManager.SymbolSprite (index);
	}

	#region implemented abstract members of GameEffect

	public override void Play (System.Action<GameEffect> callback)
	{
		PlayInflateAnimation ();
	}

	public override void Stop (System.Action<GameEffect> callback)
	{
		stopCallback = callback;
		PlayBlowAnimation ();
	}

	#endregion
}
