using System;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;

public class BubblesFinalAnimation: FinalAnimation
{
	
	/// <summary>
	/// Prepares and plays the bubble infaltion animation.
	/// </summary>
	/// <param name="c">Card, which holds the effect.</param>
	private void PlayBubbleEffect (Card c)
	{
		//get effect from pool
		GameEffect res = FinalEffect;

		//set the card symbol
		(res as BubbleBleakController).SetSymbol (CardSymbolIndex (c));

		//set position and parent of effect instance
		res.transform.SetParent (c.transform);
		res.transform.transform.localPosition = Vector3.zero;

		//enable effect
		res.gameObject.SetActive (true);

		//store effect reference to access it later in anonimous methods
		relation.Add (c.name, res);

		res.Play (null);	
	}

	/// <summary>
	/// Plays bubble explosion animation.
	/// </summary>
	/// <param name="c">Card which holds the effect</param>
	private void PlayBlowEffect (Card c)
	{
		if (!relation.ContainsKey (c.name))
			return;

		//play bubble blow up sound
		SoundManager.Instance.PlaySound (12, 0.5f, SoundManager.Instance.RandomPitch ());

		//we stop bubble effect (blow up animation)
		relation [c.name].Stop ((eff) => {
			
			//hide bubble object
			c.gameObject.SetActive (false);

			//return effect original parent object
			eff.transform.SetParent (EffectsManager.Instance.transform);
			//after bubble disappeared we return effect to pool
			effectPool.Add (eff);

		});

	}

	/// <summary>
	/// Creates the card animation.
	/// </summary>
	/// <returns>The card animation sequence.</returns>
	/// <param name="c">Card object.</param>
	private Sequence CreateCardAnimation (Card c)
	{

		//get random position on screen, where we wil move the card
		Vector3 curPos = RandomPosition;

		float dist = Vector3.Distance (c.transform.position, curPos);
		// calculate main animation duration
		float duration = dist / 6f;

		Sequence res = DOTween.Sequence ();	
		res.Insert (0f, c.gameObject.transform.DOBlendableMoveBy (curPos - c.transform.position, duration).SetEase (Ease.Linear));

		//move card after delay, after bubble animation
		res.PrependInterval (0.3f);

		//kill tweens manually so disable autokill
		res.SetAutoKill (false);

		return res;

	}

	/// <summary>
	/// Cards the index of the symbol for given card.
	/// </summary>
	/// <returns>The symbol index.</returns>
	/// <param name="c">Card object.</param>
	private int CardSymbolIndex (Card c)
	{
		int res = (int)c.Suit;

		if (c.Value > 8 && c.Value < 12)
			res += (c.Value - 8) * 4;

		return res;
	}

	#region IFinalAnimation implementation

	public BubblesFinalAnimation (Slot[] _foundation) : base (_foundation)
	{
		effectStyle = FinalEffectStyle.Bubbles;
	}

	protected override void PrepareCardForFly (Card c, int index)
	{	
		base.PrepareCardForFly (c, index);
		c.SpriteRendererEnabled = false;
	}

	protected override void StartBgSounds ()
	{
		SoundManager.Instance.PlayBackgroundSound (0, 0.03f, "bubbleBg");
		SoundManager.Instance.PlayBackgroundSound (2, 1f, "applause");
	}

	protected override void StopBgSounds ()
	{
		SoundManager.Instance.StopBackgroundSound ("bubbleBg");
		SoundManager.Instance.StopBackgroundSound ("applause");
	}

	/// <summary>
	/// Play final animation.
	/// </summary>
	/// <param name="callback">Callback.</param>
	public override System.Collections.IEnumerator Play (Action callback)
	{
		IsPlaying = true;

		StartBgSounds ();

		InitIndexes ();

		int index;
		float maxDelay = 0.3f; 
		float minDelay = 0.1f;
		bool complete = false;
		Slot curSlot;
		Sequence curTweener;	

		int cardCount = 0;
		float delayStep;

		//create animation callback with final actions
		animationCallback = () => {
			if (callback != null)
				callback ();		
		
			StopBgSounds ();

			IsPlaying = false;
			callback = null;
		};

		while (!complete) {
			try {
				//get random slot among left		
				index = RandomIndex;

				curSlot = foundation [index];

				// pick top card from random slot
				Card c = curSlot.Head; 

				PrepareCardForFly (c, index);

				curSlot.RemoveCard (c);		

				//if it was last card from slot we exclude it from our randomizator
				if (curSlot.Size == 0) {
					indexes.Remove (index);
					complete = indexes.Count == 0;
				}
		
				//get bubble effect from pool, set it position and play
				PlayBubbleEffect (c);	

				//create main animation tween
				curTweener = CreateCardAnimation (c);

				//set move animation final actions
				curTweener.OnComplete (() => {	
					PlayBlowEffect (c);			
				});

				//if it was last iteration we notify about animation completeness by calling callback
				if (complete) {				
					curTweener.AppendCallback (animationCallback);
				}

				tweens.Add (curTweener);

				curTweener.Play ();
			} catch {
				//if any exception occured we just stop the animation
				animationCallback ();
				break;
			}

			delayStep = Mathf.Lerp (maxDelay, minDelay, cardCount / 52f);
			
			yield return new WaitForSeconds (delayStep);
		}
	}

	#endregion
}


