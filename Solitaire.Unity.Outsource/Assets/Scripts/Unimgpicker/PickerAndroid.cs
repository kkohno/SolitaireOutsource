
using UnityEngine;

internal class PickerAndroid : IPicker
{
	private static readonly string receiverName = "Unimgpicker";

	#if UNITY_ANDROID

	private static readonly string PickerClass = "com.kakeragames.unimgpicker.Picker";

	public void Show (string outputFileName, int maxSize)
	{		
		try {
			using (var picker = new AndroidJavaClass (PickerClass)) {
				//just invoke static native method which invokes a gallery
				picker.CallStatic ("show", outputFileName, maxSize);
			}	
		} catch (AndroidJavaException ex) {
			SendMessage ("OnFailure", ex.Message);
		}
	}

	#else

	public void Show (string outputFileName, int maxSize)
	{
		SendMessage("OnFailure", "Its not Android platform!");
	}

	#endif

	private void SendMessage (string method, string msg)
	{
		var receiver = GameObject.Find (receiverName);
		if (receiver != null) {
			receiver.SendMessage (method, msg);
		}
	}
}
