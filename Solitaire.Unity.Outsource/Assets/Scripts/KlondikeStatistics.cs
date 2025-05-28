using UnityEngine;
using System;
using System.Collections;
using ui.windows;

public class KlondikeStatistics : MonoBehaviour
{
	public const string DATE_FORMAT = "dd.MM.yyyy";
	public const string TODAY_KEY = "today";
	public const string GAMES_TODAY_KEY = "gamesToday";
	public const string WINS_TODAY_KEY = "winsToday";
	public const string GAMES_KEY = "games";
	public const string WINS_KEY = "wins";
	public const string WIN_PRC_KEY = "winPrc";
	public const string AVG_SCORE_KEY = "avgScore";

	/// <summary>
	/// Gets the streaming assets file path
	/// </summary>
	/// <value>The SA file path.</value>
	public static string SAFilePath {
		get {
			#if UNITY_EDITOR
			return  "file://" + Application.streamingAssetsPath;    
			#else
			if (Application.platform == RuntimePlatform.Android) {
				return Application.streamingAssetsPath;
			} else if (Application.platform == RuntimePlatform.IPhonePlayer) {
				return "file://" + Application.streamingAssetsPath;         
			}

			return  "";
			#endif
		}
	}

	/// <summary>
	/// Adds the stat entry to persistent storage (player prefs).
	/// </summary>
	/// <param name="isWin">If set to <c>true</c> is window.</param>
	/// <param name="score">Score.</param>
	public void AddStat (bool isWin, int score)
	{				
		string prefix = GameSettings.TypePrefix;

		PlayerPrefs.SetInt (GAMES_TODAY_KEY, PlayerPrefs.GetInt (GAMES_TODAY_KEY, 0) + 1);

		int gamesCount = PlayerPrefs.GetInt (GAMES_KEY + prefix, 0);
		int winsCount = PlayerPrefs.GetInt (WINS_KEY + prefix, 0);
		int avgScore = PlayerPrefs.GetInt (AVG_SCORE_KEY + prefix, 0);

		PlayerPrefs.SetInt (GAMES_KEY + prefix, ++gamesCount);

		if (isWin) {
			PlayerPrefs.SetInt (WINS_TODAY_KEY, PlayerPrefs.GetInt (WINS_TODAY_KEY, 0) + 1);
			PlayerPrefs.SetInt (WINS_KEY + prefix, ++winsCount);
		}

		PlayerPrefs.SetInt (WIN_PRC_KEY + prefix, (int)((float)winsCount / (float)gamesCount * 100));
		PlayerPrefs.SetInt (AVG_SCORE_KEY + prefix, (avgScore * (gamesCount - 1) + score) / gamesCount);	
	}

	void Start ()
	{		
		StartCoroutine (Init ());
	}

	IEnumerator Init ()
	{
		//player prefs stats
		string dateString = PlayerPrefs.GetString (TODAY_KEY, "01.01.0001");
		DateTime date = DateTime.ParseExact (dateString, DATE_FORMAT, null);

		//reset today counters
		if (date != DateTime.Today) {
			DropTodayStats ();
		}

		yield return null;
	}

	/// <summary>
	/// Deletes all today's statistics
	/// </summary>
	public void DropTodayStats ()
	{
		PlayerPrefs.SetString (TODAY_KEY, DateTime.Today.ToString (DATE_FORMAT));
		PlayerPrefs.SetInt (GAMES_TODAY_KEY, 0);
		PlayerPrefs.SetInt (WINS_TODAY_KEY, 0);
	}

	/// <summary>
	/// Deletes all total statistics 
	/// </summary>
	public void DropTotalStats ()
	{
		string typeString;

		foreach (var e in Enum.GetValues(typeof(GameType))) {
			typeString = e.ToString ();
			DropStatWithPrefix (typeString);
		}
		DropStatWithPrefix ("Win");
	}

	/// <summary>
	/// Drops the stat entries with given game type prefix.
	/// </summary>
	/// <param name="prefix">Prefix.</param>
	private void DropStatWithPrefix (string prefix)
	{		
		PlayerPrefs.SetInt (GAMES_KEY + prefix, 0);	
		PlayerPrefs.SetInt (WINS_KEY + prefix, 0);
		PlayerPrefs.SetInt (WIN_PRC_KEY + prefix, 0);
		PlayerPrefs.SetInt (AVG_SCORE_KEY + prefix, 0);
	}

	/// <summary>
	/// Drops all the stats.
	/// </summary>
	public void DropStats ()
	{		
		DropTodayStats ();	
		DropTotalStats ();
	}
}


