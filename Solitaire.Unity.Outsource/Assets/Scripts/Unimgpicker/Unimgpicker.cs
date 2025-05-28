using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beebyte.Obfuscator;

/// <summary>
/// Crossplatform gallery image picker. Creates wrapper for native plugin and handles its callback messages;
/// </summary>
[Skip]
public class Unimgpicker : MonoBehaviour
{
	
	public delegate void ImageDelegate (string path);

	public delegate void ErrorDelegate (string message);

	public event ImageDelegate Completed;
	public event ErrorDelegate Failed;

	private IPicker picker;
	private static Unimgpicker instance;

	public static Unimgpicker Instance {
		get{ return instance; }
	}

	void Awake ()
	{
		instance = this;
	}

	/// <summary>
	/// Show native gallery
	/// </summary>
	public void Show ()
	{
		//specify image namge and maximmum size in pixels
		picker.Show ("pickedImage", 512);
	}

	/// <summary>
	/// Handles image picker complete message
	/// </summary>
	/// <param name="path">Picked image path.</param>
	private void OnComplete (string path)
	{
		//notify subscribers about success
		var handler = Completed;
		if (handler != null) {
			handler (path);
		}
	}

	/// <summary>
	/// Handles image picker failure message
	/// </summary>
	/// <param name="message">Error message or failure reason.</param>
	private void OnFailure (string message)
	{
		//notify subscribers about failure
		var handler = Failed;
		if (handler != null) {
			handler (message);
		}
	}

	// Use this for initialization
	void Start ()
	{
		// create plugin wrapper of current platform
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			picker = new PickeriOS ();
		else if (Application.platform == RuntimePlatform.Android)
			picker = new PickerAndroid ();
		else
			picker = new PickerUnsupported ();
	}

}
