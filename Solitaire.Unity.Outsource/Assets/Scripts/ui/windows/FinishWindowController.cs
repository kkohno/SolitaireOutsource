using UnityEngine;
using UnityEngine.UI;

namespace ui.windows
{
	public class FinishWindowController : MonoBehaviour {
	
		public Text msg;
		public Text score;
		public Text time;
		public Text gamesToday;
		public Text winsToday;
		public WindowAnimation window;

		public Button restart;
		public Button newGame;

		public void Show (bool isSuccess){
		
			//during this window hide all objects and pause game
			KlondikeController.Instance.PauseGame ();
			KlondikeController.Instance.SetObjectsVisibility(false);

			gameObject.SetActive (true);
			window.Show ();
		}

		/// <summary>
		/// Handles new game button click.
		/// </summary>
		/// <param name="newShuffle">If set to <c>true</c> cards must be shuffled, otherwise deal same cards.</param>
		public void NewGame(bool newShuffle){
			Close ();
			KlondikeController.Instance.NewGame (newShuffle, AdsCase.EndGame);
		}

		public void Close(){		

			//return original game state
			KlondikeController.Instance.ResumeGame ();
			KlondikeController.Instance.SetObjectsVisibility(true);

			window.Hide (()=>{
				gameObject.SetActive (false);
			});
		}

	}
}
