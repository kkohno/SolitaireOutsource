using System.Runtime.InteropServices;
using UnityEngine;
using Beebyte.Obfuscator;

internal class PickeriOS : IPicker
{
	#if UNITY_IPHONE
	[DllImport ("__Internal")]
	[Skip]
	private static extern void Unimgpicker_show (string outputFileName, int maxSize);
	#endif

	public void Show (string outputFileName, int maxSize)
	{
		#if UNITY_IPHONE
		//just invoke static native method which invokes a gallery
		Unimgpicker_show (outputFileName, maxSize);
		#else
		var receiver = GameObject.Find ("Unimgpicker");
		if (receiver != null) {
			receiver.SendMessage ("OnFailure", "This is not IOS platform !");
		}	
		#endif
	}
}
