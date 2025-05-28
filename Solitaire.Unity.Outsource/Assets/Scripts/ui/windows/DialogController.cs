using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ui.windows
{
	public enum DialogType
	{
		YesNo = 0,
		Ok
	}

	public class DialogController : MonoBehaviour
	{

		public GameObject yes;
		public GameObject no;
		public Text message;
		public WindowAnimation window;

		private Tweener s_animation;
		private bool isReady;
		private bool shown;
		private Action<bool> finishAction;

		public bool Ready {
			get{ return isReady; }
		}

		public void Show (string text, DialogType type, Action<bool> act = null, string yesStr = null, string noStr = null)
		{		
			if (shown)
				return;

			//set custom "yes" string if it is specified, get default otherwise
			if (yesStr != null)
				yes.GetComponentInChildren<Text> ().text = yesStr;
			else
				yes.GetComponentInChildren<Text> ().text = LocalizationManager.Instance.GetText ("ok");

			//set custom "no" string if it is specified, get default otherwise
			if (noStr != null)
				no.GetComponentInChildren<Text> ().text = noStr;
			else
				no.GetComponentInChildren<Text> ().text = LocalizationManager.Instance.GetText ("close");
		
			shown = true;
			gameObject.SetActive (true);

			//enable buttons depending on dialog type
			no.SetActive (type.Equals (DialogType.YesNo));
			yes.SetActive (type.Equals (DialogType.YesNo) || type.Equals (DialogType.Ok));		

			//set dialog text
			message.text = text;

			//set callback
			finishAction = act;

			window.Show ();

			isReady = true;
		}

		public void Hide ()
		{		
			if (!shown)
				return;
		
			shown = false;	

			window.Hide (() => {
				gameObject.SetActive (false);
			});
		}

		public void Finish (bool arg)
		{
			if (!isReady)
				return;

			Hide ();

			if (finishAction != null)
				finishAction (arg);
		}
	}
}