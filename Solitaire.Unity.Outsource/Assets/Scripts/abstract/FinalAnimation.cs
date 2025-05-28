using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ui;
using UnityEngine;


public abstract class FinalAnimation
{
	protected Slot[] foundation;
	protected List<Tween> tweens;
	protected List<int> indexes;
	protected List<GameEffect> effectPool;
	protected List<GameEffect> allEffects;
	protected Dictionary<string, GameEffect> relation;
	protected TweenCallback animationCallback;
	protected FinalEffectStyle effectStyle;

	public FinalAnimation (Slot[] _foundation)
	{
		this.foundation = _foundation;

		//create all dynamic collections
		tweens = new List<Tween> ();
		effectPool = new List<GameEffect> ();
		relation = new Dictionary<string, GameEffect> ();
		allEffects = new List<GameEffect> ();

		//set playing flag to false
		IsPlaying = false;
	}

	/// <summary>
	/// Create and initialize list of slot indexes (used for picking random slot)
	/// </summary>
	protected void InitIndexes ()
	{
		if (indexes == null)
			indexes = new List<int> ();
		else
			indexes.Clear ();

		for (int i = 0; i < 4; i++) {
			indexes.Add (i);
		}
	}

	/// <summary>
	/// Gets the random index of slot with cards.
	/// </summary>
	/// <value>The random index of slot.</value>
	protected int RandomIndex {
		get { 			
			int random = UnityEngine.Random.Range (0, indexes.Count);
			return indexes [random];
		}
	}

	protected virtual void StartBgSounds ()
	{
		
	}

	protected virtual void StopBgSounds ()
	{
		
	}

	/// <summary>
	/// Gets the final effect from pool or create new if there's none left.
	/// </summary>
	/// <value>The final effect instance.</value>
	protected GameEffect FinalEffect {
		get { 

			GameEffect res;
			if (effectPool.Count > 0) {
				res = effectPool [0];
				effectPool.RemoveAt (0);
			} else {
				res = EffectsManager.Instance.CrateEffect (effectStyle);
				res.transform.SetParent (EffectsManager.Instance.gameObject.transform);
				res.gameObject.SetActive (false);
				allEffects.Add (res);
			}

			return res;
		}
	}

	/// <summary>
	/// Gets the random position on screen.
	/// </summary>
	/// <value>The random position on screen.</value>
	protected Vector3 RandomPosition {
		get {
			float camH = Camera.main.orthographicSize * 2f;
			float camW = camH * Camera.main.aspect;	

			return new Vector3 (
				UnityEngine.Random.Range (Camera.main.transform.position.x - camW / 2f, 
					Camera.main.transform.position.x + camW / 2f),
				UnityEngine.Random.Range (Camera.main.transform.position.y, 
					Camera.main.transform.position.y - camH / 3f),
				1f
			);
		}

	}

	/// <summary>
	/// Prepares the card for animation. Sets the sorting layer and order.
	/// </summary>
	/// <param name="c">Card.</param>
	/// <param name="index">Index of iteration.</param>
	protected virtual void PrepareCardForFly (Card c, int index)
	{		
		c.SetSortingOrder (c.LayerCount * foundation [index].Size);
		c.ShadowRendererEnabled = false;
		c.SortingLayer = UILayout.SORTING_TOP;
	}

	/// <summary>
	/// Gets or sets a value indicating whether this animation is now playing.
	/// </summary>
	/// <value><c>true</c> if this instance is playing; otherwise, <c>false</c>.</value>
	public bool IsPlaying{ get; protected set; }

	/// <summary>
	/// Play the final animation.
	/// </summary>
	/// <param name="callback">Callback.</param>
	public abstract IEnumerator Play (Action callback);

	/// <summary>
	/// Stop this instance.
	/// </summary>
	public virtual void Stop ()
	{
		//clear all tween objects
		foreach (var t in tweens) {
			t.Rewind ();
			t.Kill ();
		}

		//empty reference list
		tweens.Clear ();

		foreach (var e in allEffects) {
			//return all effects to pool
			if (!effectPool.Contains (e))
				effectPool.Add (e);

			//hide and stop effect
			e.gameObject.SetActive (false);
			e.Stop (null);
		}

		//clear card-effect relation dictionary table
		relation.Clear ();

		StopBgSounds ();
		IsPlaying = false;

	}
}


