using UnityEngine;
using System.IO;

internal class PickerUnsupported : IPicker
{
	public void Show (string outputFileName, int maxSize)
	{
		#if UNITY_EDITOR
		var receiver = GameObject.Find ("Unimgpicker");
		if (receiver != null) {
			string path = Path.Combine (Application.dataPath, "textures/splash/splash.png");

			if (File.Exists (path))				
				//test mechanism in editor using current splash image
				receiver.SendMessage ("OnComplete", path);
			else
				receiver.SendMessage ("OnFailure", "No file: " + path);
		}	
		#else
		var receiver = GameObject.Find ("Unimgpicker");
		if (receiver != null) {
		receiver.SendMessage ("OnFailure", "This is not Editor platform !");
		}	
		#endif
	}
}
