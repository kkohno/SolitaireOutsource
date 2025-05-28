using UnityEngine;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using Beebyte.Obfuscator;

public enum AdsCase
{
    None = 0,
    Undo,
    Restart,
    EndGame
}

[Skip]
public class AdsManager : MonoBehaviour, IInterstitialAdListener
{

    public string appKey;
    private bool isShowingAd;
    private static AdsManager instance;

    public delegate void AdsEvent();

    public static event AdsEvent AdStarted;
    public static event AdsEvent AdFinished;

    public static AdsManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Dim();
    }

    void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Dim();
        }
    }

    private void Dim()
    {/*
#if UNITY_ANDROID && !UNITY_EDITOR
		var buildVersion = new AndroidJavaClass("android.os.Build$VERSION");
		if (buildVersion.GetStatic<int>("SDK_INT") >= 11)
		{
			using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					activity.Call("runOnUiThread", new AndroidJavaRunnable(DimRunable));
				}
			}
		}
#endif*/
    }

    public static void DimRunable()
    {/*
#if UNITY_ANDROID && !UNITY_EDITOR
		using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				using (var window = activity.Call<AndroidJavaObject>("getWindow"))
				{
					using (var view = window.Call<AndroidJavaObject>("getDecorView"))
					{
						const int SYSTEM_UI_FLAG_LOW_PROFILE = 1;

						view.Call("setSystemUiVisibility", SYSTEM_UI_FLAG_LOW_PROFILE);
					}
				}
			}
		}
#endif*/
    }

    /// <summary>
    /// Init this instance.
    /// </summary>
    public void Init()
    {
        isShowingAd = false;

        if (appKey == string.Empty || appKey.Length != 48)
            return;

        //platform dependent ads initialization
        if (Application.platform == RuntimePlatform.Android)
            InitAndroid();
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            InitIOS();
		
    }

    /// <summary>
    /// Android specific initialization
    /// </summary>
    private void InitAndroid()
    {
		/*
#if UNITY_ANDROID
        AppsFlyer.setAppID(Application.identifier);
        AppsFlyer.init("ewVfXy4eavTcRaRzrsKWAA");
#endif
		Appodeal.setInterstitialCallbacks (this);
		Appodeal.initialize (appKey, Appodeal.INTERSTITIAL);*/
	}

	/// <summary>
	/// iOS specific initialization
	/// </summary>
	private void InitIOS ()
	{
        /*Appodeal.initialize(appKey, Appodeal.INTERSTITIAL);
        Appodeal.setInterstitialCallbacks(this);

        AppsFlyer.setAppID("295830095");
        AppsFlyer.trackAppLaunch();*/
	}

	// is interstitial ads ready to be shown
	public static bool isInterstitialLoaded ()
	{
		//return Appodeal.isLoaded (Appodeal.INTERSTITIAL);
        return false;
    }

	/// <summary>
	/// Shows the ad.
	/// </summary>
	/// <param name="adType">Ad type.</param>
	/// <param name="callback">Callback.</param>
	/// <param name="palcement">Palcement.</param>
	/// <param name="delay">Show delay.</param>
	public IEnumerator ShowAd (int adType, Action<bool> callback = null, string placement = "", float delay = 0f)
	{			
		
		//check if there is no active ad already
		//we show only single ad at the same time
		/*if (isShowingAd) {			
			if (callback != null)
				callback (false);
			yield break;
		}

		//check if ad is ready to be shown
		//only try to show ad if its loaded
		if (!Appodeal.isLoaded (adType)) {
			RaiseEvent (AdFinished);
			if (callback != null)
				callback (false);
			yield break;
		}

		//wait for delayed showing
		if (delay > 0f)
			yield return new WaitForSecondsRealtime (delay);	

		//try to show ads, if placement is not empty we use it
		//if (placement.Equals (string.Empty))
			isShowingAd = Appodeal.show (adType);
		//else
		//	isShowingAd = Appodeal.show (adType, placement);

		Debug.Log ("AppodealUnity " + isShowingAd);

		//we disable all user input, all game timers and make game object invisible during thw ads
		KlondikeController.Instance.SetObjectsVisibility (false);
		KlondikeController.Instance.EventsSytemEnabled = false;
		KlondikeController.Instance.PauseGame ();

		//only if ads show we notify subscribers
		if (isShowingAd) {
            // todo вырезана аналитика
            // Answers.LogCustom ("showAds", new Dictionary<string, object> (){ { "adType", adType }, { "placement", placement } });
            RaiseEvent (AdStarted);
		}

		// wait for ads to stop (closed by user or finished)
		// isShowingAd flag should be set in callbacks (e.g. onInterstitialClosed)
		while (isShowingAd)
			yield return new WaitForEndOfFrame ();

		//when ads is finished (closed) we will resume game, show objects and allow user input using this action
		KlondikeController.Instance.SetObjectsVisibility (true);
		KlondikeController.Instance.EventsSytemEnabled = true;
		KlondikeController.Instance.ResumeGame ();

		//only after ads finished (closed) or failed to show we run callback and return event system to original state
		if (callback != null) {			
			callback (true);
		}*/

		yield break;
    }

	/// <summary>
	/// Shows the interstitial ad.
	/// </summary>
	/// <param name="callback">Callback.</param>
	/// <param name="palcement">Palcement.</param>
	/// <param name="delay">Delay.</param>
	public IEnumerator ShowInterstitial (Action<bool> callback = null, string placement = "", float delay = 0f)
	{			
		//yield return StartCoroutine (ShowAd (Appodeal.INTERSTITIAL, callback, placement, delay));
		yield break;
	}

	/// <summary>
	/// Raises the event.
	/// </summary>
	/// <param name="e">Event.</param>
	private void RaiseEvent (AdsEvent e)
	{
		if (e != null)
			e ();
	}

	/// <summary>
	/// Do ads showing now
	/// </summary>
	public bool IsShowingAds {
		get{ return isShowingAd; }
	}

#region IInterstitialAdListener implementation

	public void onInterstitialLoaded (bool b)
	{
	}

	public void onInterstitialFailedToLoad ()
	{
	}

	public void onInterstitialShown ()
	{
		isShowingAd = true;
	}

	public void onInterstitialClicked ()
	{
	}

	public void onInterstitialClosed ()
	{ 		
		isShowingAd = false;
		RaiseEvent (AdFinished);
	}

    public void onInterstitialExpired()
    {
    }

#endregion
}
