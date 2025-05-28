using UnityEngine;

/// <summary>
/// Editor based card animation settings
/// </summary>
public class CardAnimationSettings : MonoBehaviour {
	
	#region Singleton 
	private static CardAnimationSettings instance;

	void Awake () {
		instance = this;
	}
	#endregion

	#region Inspector
	[Range(1f,1.5f)]
	public float _SelectedScale;
	[Range(0.1f,1f)]
	public float _SelectDuration;
	[Range(0.1f,1f)]
	public float _CardMoveDuration;
	[Range(0.01f,1f)]
	public float _CardRotateDuration;
	[Range(0.05f,1f)]
	public float _CardGroupDelay;
	[Range(0.1f,1f)]
	public float _FailShakeDuration;
	[Range(0.01f,1f)]
	public float _ShakeDistanceThres;
	[Range(0.01f,1f)]
	public float _ShakeStrength;
	[Range(0.1f,1f)]
	public float _SuccessDuration;
	[Range(0.05f,1f)]
	public float _GlowDuration;

	public AnimationCurve _ShakeCurve;
	public AnimationCurve _WindowCurve;
	#endregion

	#region AccessProperties
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

	public static float GlowDuration {
		get{ 

			//default value
			if (instance == null)
				return 0.1f;

			return instance._GlowDuration;		
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

	public static AnimationCurve WindowCurve {
		get{ 

			//default value
			if (instance == null)
				return new AnimationCurve(new Keyframe[]{new Keyframe(0,0), new Keyframe(1f,1f)});

			return instance._WindowCurve;		
		}
	}
	#endregion


}
