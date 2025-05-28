using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using ui;

public class OldscoolFinalAnimation :FinalAnimation
{

	private ClearFlagsImageEffect cfEffect;
	private float minDelay = 0.2f;
	private float maxDelay = 0.7f;
	private float step = 0.015f;

	private float[] CalculateMoveOffset (Card c)
	{
		float camH = Camera.main.orthographicSize * 2f;
		float[] res = new float[2];

		if (UILayout.Instance.LayoutType == CardLayout.Vertical) {					
			res [0] = Mathf.Sign (UnityEngine.Random.Range (-1, 1)) * UnityEngine.Random.Range (5f, 5f);
			res [1] = -(c.transform.position.y - Camera.main.transform.position.y + camH / 2f - c.Size.y / 2f);
		} else {
			res [0] = UnityEngine.Random.Range (5f, 15f);
			res [1] = -(c.transform.position.y - Camera.main.transform.position.y + camH / 2f - c.Size.y / 2f);
		}

		return res;
	}

	private Sequence CreateCardAnimation (Card c)
	{
		
		Sequence curTween = DOTween.Sequence ();

		float[] moveOffset = CalculateMoveOffset (c);			

		curTween.Insert (0f, 
			c.transform.DOBlendableMoveBy (
				new Vector3 (0f, moveOffset [1], 0f),
				4f).SetEase (EffectsManager.Instance.fallCurve));
		curTween.Insert (0f, 
			c.transform.DOBlendableMoveBy (
				new Vector3 (moveOffset [0], 0f, 0f),
				4f).SetEase (Ease.Linear));

		curTween.OnComplete (() => {
			c.gameObject.SetActive (false);
		});

		return curTween;
	}

	#region IFinalAnimation implementation

	protected override void StartBgSounds ()
	{
		SoundManager.Instance.PlayBackgroundSound (2, 1f, "applause");
	}

	protected override void StopBgSounds ()
	{
		SoundManager.Instance.StopBackgroundSound ("applause");
	}

	public OldscoolFinalAnimation (Slot[] _foundation) : base (_foundation)
	{		
		// try to get effect component from main camera
		cfEffect = Camera.main.gameObject.GetComponent<ClearFlagsImageEffect> ();

		//if there is no component - add it
		if (cfEffect == null)
			cfEffect = Camera.main.gameObject.AddComponent<ClearFlagsImageEffect> ();
		
		//set current animation effect style
		effectStyle = FinalEffectStyle.OldScool;

		//synchronize effect camera size and position with main camera
		UnclearedCameraController.Instance.SyncWithOriginal ();
	}

	public override System.Collections.IEnumerator Play (Action callback)
	{	
		
		IsPlaying = true;

		//wait a little before animation
		yield return new WaitForSeconds (0.5f);

		//enable image effect which overlaps frames
		cfEffect.Enabled = true;

		//set culling mask to background only, because default layer now will be rendered with uncleared camera
		Camera.main.cullingMask = 1 << UILayout.BACKGROUND_LAYER_ID;

		// wait a little to make sure that background objects are rendered
		yield return new WaitForSeconds (0.1f);

		//disable all foundation object to prevent render garbage and transparent object overlaping
		foreach (var f in foundation) {			
			foreach (var c in f.Cards) {
				c.gameObject.SetActive (false);
			}
		}

		Sequence curTween;

		float delay = maxDelay;

		int iterations = CardManager.CARD_COUNT / foundation.Length;

		for (int i = 1; i <= iterations; i++) {

			foreach (var f in foundation) {
				
				if (f.Size == 0)
					continue;	
				
				Card current;
				current = f.Cards [f.Size - i];
				current.ShadowRendererEnabled = false;

				curTween = CreateCardAnimation (current);

				current.gameObject.SetActive (true);
				current.ShadowRendererEnabled = true;

				curTween.Play ();

				SoundManager.Instance.PlaySound (5, 0.4f, SoundManager.Instance.RandomPitch ());

				tweens.Add (curTween);
			
				yield return new WaitForSeconds (delay);

				//every step delay becomes smaller until it reaches minDelay border
				if (delay > minDelay)
					delay -= step;
			}
		
		}

		StopBgSounds ();

		if (callback != null) {
			callback ();	
			cfEffect.Enabled = false;
		}
	}

	public override void Stop ()
	{		
		base.Stop ();

		cfEffect.Enabled = false;

		//return original culling mask
		Camera.main.cullingMask = 1 << UILayout.BACKGROUND_LAYER_ID | 1 << UILayout.DEFAULT_LAYER_ID;
	
		IsPlaying = false;
	}

	#endregion

}


