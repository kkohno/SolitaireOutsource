using UnityEngine;
using System;
using ui.windows;

public enum EffectStyle
{
	Dust = 0,
	Glow
}

public enum FinalEffectStyle
{
	Salute = 0,
	Bubbles,
	OldScool
}

public class EffectsManager : MonoBehaviour
{
	//current background effect style (set in inspector)
	public EffectStyle Style;
	//current final animation style (set in inspector)
	public FinalEffectStyle FinalStyle;

	//effect prefabs and particles
	public GameEffect[] SlotEffects;
	public GameObject[] TraceParticles;
	public GameEffect[] Effects;

	//animation curve for old scool animation
	public AnimationCurve fallCurve;

	//if final animation must be chosen randomly every round(game)
	public bool isRandomFinal;

	private ParticleSystem[] traceParticles;
	private FinalAnimation[] finalAnimations;
	private static EffectsManager instance;
	private Coroutine final;

	void Awake ()
	{
		instance = this;
		traceParticles = new ParticleSystem[TraceParticles.Length];
		finalAnimations = new FinalAnimation[Enum.GetNames (typeof(FinalEffectStyle)).Length];
	}

	public static EffectsManager Instance {
		get{ return instance; }
	}

	/// <summary>
	/// Crates the effect of current style.
	/// </summary>
	/// <returns>The instance of effect with current style.</returns>
	public GameEffect CrateEffectOfCurrentStyle ()
	{
		return Instantiate (SlotEffects [(int)Style]);
	}

	/// <summary>
	/// Crates the effect with given style index.
	/// </summary>
	/// <returns>The effect instance.</returns>
	/// <param name="style">Style.</param>
	public GameEffect CrateEffect (FinalEffectStyle style)
	{		
		return Instantiate (Effects [(int)style]);
	}

	/// <summary>
	/// Gets the final animation instance of current style from pool or creates one.
	/// </summary>
	/// <value>The final animation controller instance.</value>
	private FinalAnimation FinalAnimation {
		get { 
			if (finalAnimations [(int)FinalStyle] == null) {
				finalAnimations [(int)FinalStyle] = CreateFinalAnimation (FinalStyle);
			}
			return finalAnimations [(int)FinalStyle];
		}
	}

	/// <summary>
	/// Gets a value indicating whether this instance is playing.
	/// </summary>
	/// <value><c>true</c> if this instance is playing; otherwise, <c>false</c>.</value>
	public bool IsFinalPlaying {
		get{ return FinalAnimation.IsPlaying; }
	}

	/// <summary>
	/// Starts the final animation.
	/// </summary>
	/// <param name="callback">Callback.</param>
	public void StartFinalAnimation (Action callback)
	{
		//pick random final effect from available 
		if (isRandomFinal)
			FinalStyle = (FinalEffectStyle)(UnityEngine.Random.Range (0, Enum.GetNames (typeof(FinalEffectStyle)).Length));

		//forbid orientation change during animation
		OrientationHandler.RotationEnabled = false;

		//actually play final animation
		final = StartCoroutine (FinalAnimation.Play (callback));	
	}

	/// <summary>
	/// Skips the final animation routine if player doesnt want to watch it.
	/// </summary>
	public void SkipFinalAnimation ()
	{				
		// if we skip animation during the beginning we must first of all stop Play routine
		if (final != null)
			StopCoroutine (final);	

		//than stop already started animations
		FinalAnimation.Stop ();

		OrientationHandler.RotationEnabled = GameSettings.Instance.Data.ChangeOrient;
	}

	/// <summary>
	/// Creates the final animation instance of givent animation style.
	/// </summary>
	/// <returns>The final animation instance.</returns>
	/// <param name="style">Animation style.</param>
	private FinalAnimation CreateFinalAnimation (FinalEffectStyle style)
	{
		FinalAnimation res = null;

		if (style == FinalEffectStyle.Salute) {
			res = new SaluteFinalAnimation (KlondikeController.Instance.Foundation);
		} else if (style == FinalEffectStyle.Bubbles) {
			res = new BubblesFinalAnimation (KlondikeController.Instance.Foundation);
		} else if (style == FinalEffectStyle.OldScool) {
			res = new OldscoolFinalAnimation (KlondikeController.Instance.Foundation);
		}

		return res;
	}

	/// <summary>
	/// Gets the trace particles object which will follow pressed cards.
	/// </summary>
	/// <value>The trace particles object.</value>
	public ParticleSystem TraceParticlesObject {
		get {			
			if (traceParticles [(int)Style] == null) {
				GameObject go = Instantiate (TraceParticles [(int)Style], transform);
				traceParticles [(int)Style] = go.GetComponent<ParticleSystem> ();
			}
			return traceParticles [(int)Style];
		}
	}

}


