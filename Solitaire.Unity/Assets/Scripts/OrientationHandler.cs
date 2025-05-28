using UnityEngine;
using System;
using System.Collections;

public class OrientationHandler : MonoBehaviour
{

	public static event Action OnResolutionChange;
	public static float CheckDelay = 0.05f;
	static Vector2 resolution;
	static bool isAlive = false;
	private static OrientationHandler instance;
	private static Coroutine lockRoutine;

	public static OrientationHandler Instance {
		get{ return instance; }
	}

	void Awake ()
	{
		instance = this;	
	}

	public void Init ()
	{		
		StartCoroutine (CheckForResolutionChange ());
	}

	/// <summary>
	/// Gets or sets a value indicating whether screen orientatino change is enabled.
	/// </summary>
	/// <value><c>true</c> if rotation enabled; otherwise, <c>false</c>.</value>
	public static bool RotationEnabled {
		get{ return isAlive; }
		set { 
			isAlive = value;

			Screen.autorotateToPortrait = value;
			Screen.autorotateToPortraitUpsideDown = value;	
			Screen.autorotateToLandscapeLeft = value;
			Screen.autorotateToLandscapeRight = value;		
		}
	}

	/// <summary>
	/// Checks for resolution change. This is endless routine which check for change every given interval and 
	/// notifies subscribers about changes
	/// </summary>
	/// <returns>The for change.</returns>
	IEnumerator CheckForResolutionChange ()
	{
		resolution = new Vector2 (Screen.width, Screen.height);

        while (true) {

			if (resolution.x != Screen.width || resolution.y != Screen.height) {				
				resolution = new Vector2 (Screen.width, Screen.height);

				if (OnResolutionChange != null)
					OnResolutionChange ();
			}
            
            yield return new WaitForSeconds (CheckDelay);
		}
	}
}
