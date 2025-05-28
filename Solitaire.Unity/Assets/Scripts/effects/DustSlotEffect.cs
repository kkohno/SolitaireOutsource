using System;
using UnityEngine;

/// <summary>
/// Dust slot effect.
/// </summary>
public class DustSlotEffect: GameEffect
{
	private ParticleSystem dustBurst;

	// make sure that component ref isnt empty
	private void CheckComponent ()
	{
		if (dustBurst == null)
			dustBurst = GetComponent<ParticleSystem> ();
	}

	private void StopAndClearAnimation ()
	{
		//if animation alreadu playing - stop it and clear particles
		if (dustBurst.isPlaying) {
			dustBurst.Clear ();
			dustBurst.Stop ();
		}
	}

	#region ISlotEffect implementation

	public override void Play (Action<GameEffect>callback)
	{		
		CheckComponent ();

		StopAndClearAnimation ();

		//make sure game object is active
		gameObject.SetActive (true);

		//play particle systems
		dustBurst.Play ();
				
		//after particle systems animation ended perform end actions
		StartCoroutine (CoroutineExtension.ExecuteAfterTime (dustBurst.main.duration, () => {

			//notify caller about animation end
			if (callback != null)
				callback (this);

			//hide game object
			gameObject.SetActive (false);

		}));
	}

	public override void Stop (Action<GameEffect>callback)
	{
		
		CheckComponent ();

		//stop all animation
		StopAndClearAnimation ();

		//hide object
		gameObject.SetActive (false);

		//notify caller
		if (callback != null)
			callback (this);
	}

	#endregion
}


