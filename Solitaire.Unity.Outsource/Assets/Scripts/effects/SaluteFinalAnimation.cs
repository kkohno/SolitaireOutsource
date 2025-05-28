using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SaluteFinalAnimation : FinalAnimation
{
	private Sequence CreateCardAnimation (Card c, float delay)
	{		

		float scaleDuration = 0.1f;
		float jumpDuration = 0.2f;
		float jumpY = ThemeManager.CardSize.y;

		Vector3 rotationVector = new Vector3 (0f, 0f, 270f);

		//calculate random position on screen to move card there
		Vector3 curPos = RandomPosition;

		//calculate animation duration, rotation ammount and delay before next loop
		float dist = Vector3.Distance (c.transform.position, curPos);
		float moveDuration = dist / 6f;

		Vector3 realRotation = rotationVector * Mathf.Sign (UnityEngine.Random.Range (-1, 1)) * dist;

		Sequence res = DOTween.Sequence ();
		//1) little jump before main movement
		res.Insert (0f, c.gameObject.transform.DOBlendableMoveBy (new Vector3 (0f, jumpY, 0f), jumpDuration).SetEase (Ease.Linear));
		//2) main movement to random position
		res.Insert (0f, c.gameObject.transform.DOBlendableMoveBy (curPos - c.transform.position, moveDuration).SetEase (Ease.Linear));
		//3) rotation 
		res.Insert (0f, c.gameObject.transform.DORotate (realRotation, moveDuration, RotateMode.WorldAxisAdd).SetEase (Ease.Linear));
		//4) shrink before explosion
		res.Insert (moveDuration - moveDuration * scaleDuration, c.gameObject.transform.DOScale (Vector3.zero, scaleDuration).SetEase (Ease.Linear));
		//we will destroy tweens manually so disable autokill
		res.SetAutoKill (false);

		return res;
	}

	private void PlayExplosionOnPosition (Vector3 position)
	{

		//get explosion effect from pool, set it position and play 
		GameEffect curEffect = FinalEffect;
		curEffect.transform.position = position;
		SoundManager.Instance.PlaySound (13, 0.7f, SoundManager.Instance.RandomPitch ());
		curEffect.Play ((eff) => {		

			//after effect is over this callback will be invoked
			//we force particle effect to stop and clear to be ready to use it again immediately
			eff.Stop (null);

			//and return effect instance to pool
			effectPool.Add (eff);
		});

	}

	#region IFinalAnimation implementation

	/// <summary>
	/// Initializes a new instance of the <see cref="SaluteFinalAnimation"/> class.
	/// </summary>
	/// <param name="_foundation">Foundation slots array.</param>
	public SaluteFinalAnimation (Slot[] _foundation) : base (_foundation)
	{
		effectStyle = FinalEffectStyle.Salute;
	}

	protected override void StartBgSounds ()
	{
		SoundManager.Instance.PlayBackgroundSound (1, 0.3f, "fireworks");
		SoundManager.Instance.PlayBackgroundSound (2, 1f, "applause");
	}

	protected override void StopBgSounds ()
	{
		SoundManager.Instance.StopBackgroundSound ("fireworks");
		SoundManager.Instance.StopBackgroundSound ("applause");
	}

	/// <summary>
	/// Play final animation.
	/// </summary>
	/// <param name="callback">Callback.</param>
	public override IEnumerator Play (Action callback)
	{
		
		IsPlaying = true;

		StartBgSounds ();

		InitIndexes ();

		//actions we do after whole animation
		animationCallback = () => {		

			//notify caller about animation end
			if (callback != null)
				callback ();			

			IsPlaying = false;

			StopBgSounds ();
			callback = null;
		};

		int index;
	
		float maxDelay = 0.5f; 
		float minDelay = 0.15f;
		float delayStep;
		bool complete = false;
		Slot curSlot;
		Sequence curTweener;

		int cardCount = 0;

		while (!complete) {	
			
			delayStep = Mathf.Lerp (maxDelay, minDelay, cardCount / CardManager.CARD_COUNT);

			try {
				//pick random slot among all
				curSlot = foundation [index = RandomIndex];

				//pick top card and remove it from slot, aplly appropriate layer and order
				Card c = curSlot.Head; 

				PrepareCardForFly (c, index);

				curSlot.RemoveCard (c);

				//if it was last card from slot we exclude it from our randomizator
				if (curSlot.Size == 0) {
					indexes.Remove (index);

					//if there is no more slots with cards its time to finish loop
					complete = indexes.Count == 0;
				}

				//create animation tween
				curTweener = CreateCardAnimation (c, delayStep);

				//after main animation start explosion effect
				curTweener.OnComplete (() => {	
				
					//hide card 
					c.gameObject.SetActive (false);

					//play explosion
					PlayExplosionOnPosition (c.transform.position);
				});

				//try not to lose references to tweens to clear resources effective
				tweens.Add (curTweener);

				// if it last card animation append whole animation callback to it
				if (complete) {				
					curTweener.AppendCallback (animationCallback);
				}

				curTweener.Play ();
				cardCount++;

			} catch {
				//if any exception occured we just stop the animation
				animationCallback ();
				break;
			}

			yield return new WaitForSeconds (delayStep);
		}
	}

	/// <summary>
	/// Stop this animation.
	/// </summary>
	public override void Stop ()
	{		
		//force all tweens to stop, rewind and then clear list
		foreach (var t in tweens) {
			t.Rewind ();
			t.Kill ();
		}

		tweens.Clear ();

		//force all explosion particle animations to stop and dissapear
		//also return them to pool if they not did it already (if animation was stoped during playing)
		foreach (var e in allEffects) {
			if (!effectPool.Contains (e))
				effectPool.Add (e);
			e.gameObject.SetActive (false);
			e.Stop (null);
		}

		//enable user input
		StopBgSounds ();
		IsPlaying = false;
	}

	#endregion
}


