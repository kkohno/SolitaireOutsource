using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://habrahabr.ru/post/216185/
public static class CoroutineExtension
{
	/// <summary>
	/// The coroutine counter dict for each group name
	/// </summary>
	static private readonly Dictionary<string, int> Runners = new Dictionary<string, int> ();

	/// <summary>
	/// Add coroutine to parallel group
	/// </summary>
	/// <param name="coroutine">Coroutine.</param>
	/// <param name="parent">Parent.</param>
	/// <param name="groupName">Group name.</param>
	public static void ParallelCoroutinesGroup (this IEnumerator coroutine, MonoBehaviour parent, string groupName)
	{
		if (!Runners.ContainsKey (groupName))
			Runners.Add (groupName, 0);

		Runners [groupName]++;
		parent.StartCoroutine (DoParallel (coroutine, parent, groupName));
	}

	/// <summary>
	/// Execute coroutine parallel with others
	/// </summary>
	/// <param name="coroutine">Coroutine.</param>
	/// <param name="parent">Parent.</param>
	/// <param name="groupName">Group name.</param>
	static IEnumerator DoParallel (IEnumerator coroutine, MonoBehaviour parent, string groupName)
	{
		yield return parent.StartCoroutine (coroutine);
		Runners [groupName]--;
	}

	/// <summary>
	/// Count the routines in group.
	/// </summary>
	/// <param name="groupName">Group name.</param>
	static public int Count (string groupName)
	{
		return Runners.ContainsKey (groupName) ? Runners [groupName] : 0;
	}

	/// <summary>
	/// If there are active coroutines in group with specified name
	/// </summary>
	/// <returns><c>true</c>, if there are active coroutines, <c>false</c> otherwise.</returns>
	/// <param name="groupName">Group name.</param>
	public static bool GroupProcessing (string groupName)
	{		
		return (Runners.ContainsKey (groupName) && Runners [groupName] > 0);
	}

	/// <summary>
	/// Executes specified action after given delay.
	/// </summary>
	/// <param name="time">Delay in seconds.</param>
	/// <param name="action">Action.</param>
	public static IEnumerator ExecuteAfterTime (float time, System.Action action)
	{       
		
		yield return new WaitForSecondsRealtime (time);
		if (action != null)
			action ();
	}
}

