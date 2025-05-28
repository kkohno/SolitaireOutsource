using System;
using UnityEngine;

/// <summary>
/// Abstraction for logging system
/// </summary>
public abstract class MessageHandler: MonoBehaviour
{
	public abstract void AddMsg (string msg);
}


