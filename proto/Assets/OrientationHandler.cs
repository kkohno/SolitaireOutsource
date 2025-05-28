using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OrientationHandler : MonoBehaviour
{

	public static event Action<Vector2> OnResolutionChange;
	public static event Action<DeviceOrientation> OnOrientationChange;

	public static float CheckDelay = 0.2f;
	// How long to wait until we check again.

	static Vector2 resolution;
	// Current Resolution
	static DeviceOrientation orientation;
	// Current Device Orientation
	static bool isAlive = true;
	// Keep this script running?

	private static OrientationHandler instance;

	public static OrientationHandler Instance {
		get{ return instance; }
	}

	void Awake ()
	{
		instance = this;
	}

	void Start ()
	{
		StartCoroutine (CheckForChange ());
	}

	IEnumerator CheckForChange ()
	{
		resolution = new Vector2 (Screen.width, Screen.height);
		orientation = Input.deviceOrientation;

		while (isAlive) {

			// Check for a Resolution Change
			if (resolution.x != Screen.width || resolution.y != Screen.height) {
				resolution = new Vector2 (Screen.width, Screen.height);
				if (OnResolutionChange != null)
					OnResolutionChange (resolution);
			}

			// Check for an Orientation Change
			switch (Input.deviceOrientation) {
			case DeviceOrientation.Unknown:            // Ignore
			case DeviceOrientation.FaceUp:            // Ignore
			case DeviceOrientation.FaceDown:        // Ignore
				break;
			default:
				if (orientation != Input.deviceOrientation) {
					orientation = Input.deviceOrientation;
					if (OnOrientationChange != null)
						OnOrientationChange (orientation);
				}
				break;
			}

			yield return new WaitForSeconds (CheckDelay);
		}
	}

	void OnDestroy ()
	{
		isAlive = false;
	}

}
