using System;
using UnityEngine;

/// <summary>
/// Handles all debug logs of applicalion
/// </summary>
public class BasicLog : MessageHandler
{

	/// <summary>
	/// If set to FALSE, there will be no logging
	/// </summary>
	public bool isWorking;

	/// <summary>
	/// Adds the message to log stream.
	/// </summary>
	/// <param name="msg">Message.</param>
	public override void AddMsg (string msg)
	{
        //if (isWorking) {
			string formattedMsg = string.Format ("{0} solitaire: {1} \n", DateTime.Now.ToString ("s"), msg);
			Debug.Log (formattedMsg);
		//}
	}
}
