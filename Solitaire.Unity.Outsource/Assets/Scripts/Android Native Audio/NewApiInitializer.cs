using UnityEngine;

/// <summary>
/// Native Sound initializer for new Android APIs.
/// </summary>
public class NewApiInitializer
{

	private static int USAGE_GAME = new AndroidJavaClass ("android.media.AudioAttributes").GetStatic<int> ("USAGE_GAME");
	private static int CONTENT_TYPE_SONIFICATION = new AndroidJavaClass ("android.media.AudioAttributes").GetStatic<int> ("CONTENT_TYPE_SONIFICATION");

	public NewApiInitializer ()
	{
		
	}

	public AndroidJavaObject MakePoolAPI21 (int streams)
	{
		AndroidJavaObject attrs = new AndroidJavaObject ("android.media.AudioAttributes$Builder")
			.Call<AndroidJavaObject> ("setUsage", USAGE_GAME)
			.Call<AndroidJavaObject> ("setContentType", CONTENT_TYPE_SONIFICATION)
			.Call<AndroidJavaObject> ("build");		

		return new AndroidJavaObject ("android.media.SoundPool$Builder")
			.Call<AndroidJavaObject> ("setAudioAttributes", attrs)
			.Call<AndroidJavaObject> ("setMaxStreams", streams)
			.Call<AndroidJavaObject> ("build");
	}
}


