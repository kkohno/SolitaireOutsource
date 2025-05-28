using System;
using UnityEngine;

/// <summary>
/// Particle effect which simulates fireworks explosion
/// </summary>
public class SaluteEffect:GameEffect
{
	public ParticleSystem[] particles;

	#region implemented abstract members of GameEffect

	/// <summary>
	/// Play the specified explosion.
	/// </summary>
	/// <param name="callback">Callback.</param>
	public override void Play (Action<GameEffect> callback)
	{
		//make sure that object is active
		gameObject.SetActive (true);

		float maxDuration = 0f;

		//calculate max duration among all particle effects
		//it will be whole effect duration
		foreach (var p in particles) {
			maxDuration = Mathf.Max (maxDuration, p.main.duration + p.main.startDelay.constant);
			p.Clear ();
			p.Play ();
		}		

		//after duration invoke callback
		if (callback != null)
			StartCoroutine (CoroutineExtension.ExecuteAfterTime (maxDuration, () => {
				callback (this);
			}));
	}

	/// <summary>
	/// Stop the specified explosion.
	/// </summary>
	/// <param name="callback">Callback.</param>
	public override void Stop (Action<GameEffect> callback)
	{
		//clear all particles
		foreach (var p in particles) {			
			p.Stop ();
			p.Clear ();
		}

		//hide game object
		gameObject.SetActive (false);

		if (callback != null)
			callback (this);
	}

	#endregion

	public SaluteEffect ()
	{
	}
}


