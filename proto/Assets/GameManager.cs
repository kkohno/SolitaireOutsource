using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject UI;

	DialogController dialog;

	void Start(){
		dialog = UI.GetComponentInChildren<DialogController> (true);
	}

	public void OnCloseClicked(){
		if (dialog != null) {
			//TODO translate
			dialog.Show ("Are you sure you want \n to quit the app?", DialogType.YesNo, (res) => {				
				if(res)
					Application.Quit();	
			});
		}else
			Application.Quit();	
	}
}
