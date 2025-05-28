using System;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Glow slot effect.
/// </summary>
public class GlowSlotEffect: GameEffect
{
	private SpriteRenderer glowRenderer;
	private Sequence s_success;

	/// <summary>
	/// Checks the component, and if its not ready  - prepare it.
	/// </summary>
	private void CheckComponent ()
	{		
		// if renderer is null - prepare it and animation object
		if (glowRenderer == null) {
			
			glowRenderer = GetComponent<SpriteRenderer> ();

			//set glow sprite from current theme
			glowRenderer.sprite = ThemeManager.GlowSprite;

			//if theme is changed we must change the glow sprite so subscribe to the event
			ThemeManager.ThemeLoaded += OnThemeLoaded;

			//calculate animation durations
			float duration = CardAnimationSettings.SuccessDuration;
			float alphaStart = duration / 3f;

			s_success = DOTween.Sequence ();

			// increase object scale
			s_success.Insert (0f, gameObject.transform.DOScale (Vector2.one * 1.5f, duration).SetEase (Ease.InSine));

			//decrease object alpha
			s_success.Insert (alphaStart, DOTween.ToAlpha (() => glowRenderer.color, (x) => glowRenderer.color = x, 0f, duration - alphaStart));

			//kill it manually, so disable autokill
			s_success.SetAutoKill (false);
		}	

	}

	/// <summary>
	/// Handles theme loaded event and refreshes glow sprite according to current theme.
	/// </summary>
	private void OnThemeLoaded ()
	{
		glowRenderer.sprite = ThemeManager.GlowSprite;
	}

	#region ISlotEffect implementation

	public override void Play (Action<GameEffect> callback)
	{				
		CheckComponent ();

		//enable object before aniamtion
		gameObject.SetActive (true);
	
		s_success.OnComplete (() => {	

			//hide object after animation	
			gameObject.SetActive (false);

			//notify caller
			if (callback != null)
				callback (this);
		});

		//rewind animation to make sure it will play from the beginning
		s_success.Rewind ();	

		s_success.Play ();
	}

	public override void Stop (Action<GameEffect> callback)
	{
		s_success.Rewind ();

		gameObject.SetActive (false);

		if (callback != null)
			callback (this);
	}

	#endregion

	public GlowSlotEffect ()
	{
	}
}