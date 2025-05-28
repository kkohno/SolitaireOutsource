using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Editor based card animation settings
/// </summary>
public class CardAnimationSettings : MonoBehaviour {

	private static CardAnimationSettings instance;
	[Range(1f,3f)]
	public float _SelectedScale;
	[Range(0.1f,3f)]
	public float _SelectDuration;
	[Range(0.1f,3f)]
	public float _CardMoveDuration;
	[Range(0.01f,0.5f)]
	public float _CardRotateDuration;
	[Range(0.05f,3f)]
	public float _CardGroupDelay;
	[Range(0.1f,3f)]
	public float _FailShakeDuration;
	[Range(0.01f,1f)]
	public float _ShakeDistanceThres;
	[Range(0.01f,1f)]
	public float _ShakeStrength;
	[Range(0.1f,3f)]
	public float _SuccessDuration;
	public AnimationCurve _ShakeCurve;

	public static float SelectedScaleRate {
		get{ 
			
			//default value
			if (instance == null)
				return 1.1f;
			
			return instance._SelectedScale;		
		}
	}

	public static float SelectDuration {
		get{ 

			//default value
			if (instance == null)
				return 0.3f;

			return instance._SelectDuration;		
		}
	}

	public static float CardMoveDuration {
		get{ 

			//default value
			if (instance == null)
				return 0.15f;

			return instance._CardMoveDuration;		
		}
	}

	public static float CardGroupDelay {
		get{ 

			//default value
			if (instance == null)
				return 0.1f;

			return instance._CardGroupDelay;		
		}
	}

	public static float FailShakeDuration {
		get{ 

			//default value
			if (instance == null)
				return 0.5f;

			return instance._FailShakeDuration;		
		}
	}

	public static float SuccessDuration {
		get{ 

			//default value
			if (instance == null)
				return 0.7f;

			return instance._SuccessDuration;		
		}
	}

	public static float ShakeDistanceThres {
		get{ 

			//default value
			if (instance == null)
				return 0.1f;

			return instance._ShakeDistanceThres;		
		}
	}

	public static float ShakeStrength {
		get{ 

			//default value
			if (instance == null)
				return 0.2f;

			return instance._ShakeStrength;		
		}
	}

	public static float CardRotateDuration {
		get{ 

			//default value
			if (instance == null)
				return 0.05f;

			return instance._CardRotateDuration;		
		}
	}

	public static AnimationCurve ShakeCurve {
		get{ 

			//default value
			if (instance == null)
				return new AnimationCurve(new Keyframe[]{new Keyframe(0,0),new Keyframe(0.3f, 1), new Keyframe(0.66f,-1f),new Keyframe(1f,0f)});

			return instance._ShakeCurve;		
		}
	}

	void Awake () {
		instance = this;
	}

}
