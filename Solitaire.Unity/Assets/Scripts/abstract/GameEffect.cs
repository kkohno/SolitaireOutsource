using System;
using UnityEngine;

/// <summary>
/// Just an effect(animation) abstraction, that can only Play and Stop basically
/// </summary>
public abstract class GameEffect: MonoBehaviour
{
	
	public abstract void Play (Action<GameEffect> callback);

	public abstract void Stop (Action<GameEffect> callback);

}


