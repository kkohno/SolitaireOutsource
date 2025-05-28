using System;
using UnityEngine;
using UnityEngine.UI;

namespace ui.windows
{
	public class StatisticsWindow : MonoBehaviour
	{

		public WindowAnimation window;

		public Text currentLabel;
		public Text gamesTxt;
		public Text winsTxt;
		public Text winPrcTxt;
		public Text avgScoreTxt;

		private int index = 0;

		public void Show ()
		{
			GameType currentType = (GameType)index;

			//check if gametype is valid
			if (!Enum.IsDefined (typeof(GameType), currentType) && index < Enum.GetNames (typeof(GameType)).Length) {			
				return;
			}		

			RefreshText ();

			//disable game during that menu
			KlondikeController.Instance.SetObjectsVisibility (false);	
			KlondikeController.Instance.PauseGame ();

			gameObject.SetActive (true);
			window.Show ();
		}

		/// <summary>
		/// Set values to all text labels according to current game type index
		/// </summary>
		private void RefreshText ()
		{
			string prefix = IndexPrefix ();

			//set the title of this gametype
			currentLabel.text = LocalizationManager.Instance.GetText ("deck" + prefix);

			//add comment only fot OneOnThree gameType
			if (prefix == GameType.OneOnThree.ToString ())
				currentLabel.text += " " + (LocalizationManager.Instance.GetText ("threeVisible"));
		
			//read stats from playerPrefs using current game type prefix
			gamesTxt.text = PlayerPrefs.GetInt (KlondikeStatistics.GAMES_KEY + prefix, 0).ToString ();
			winsTxt.text = PlayerPrefs.GetInt (KlondikeStatistics.WINS_KEY + prefix, 0).ToString ();
			winPrcTxt.text = PlayerPrefs.GetInt (KlondikeStatistics.WIN_PRC_KEY + prefix, 0).ToString () + "%";
			avgScoreTxt.text = PlayerPrefs.GetInt (KlondikeStatistics.AVG_SCORE_KEY + prefix, 0).ToString ();
		}


		/// <summary>
		/// Flush all stats.
		/// </summary>
		public void Flush ()
		{	
			UILayout.Instance.dialog.Show (LocalizationManager.Instance.GetText ("flushQuestion"), DialogType.YesNo, (isYes) => {
				if (isYes) {
					KlondikeController.Instance.statManager.DropStats ();
					RefreshText ();
				}
			});
		}

		/// <summary>
		/// Show previous game type statistics
		/// </summary>
		public void Prev ()
		{
			int prev = index--;
			GameType currentType = (GameType)index;

			if (!Enum.IsDefined (typeof(GameType), currentType)) {
				index = prev;
				return;
			}	
			RefreshText ();
		}

		/// <summary>
		/// Show next game type statistics
		/// </summary>
		public void Next ()
		{
			int prev = index++;

			if (index > Enum.GetNames (typeof(GameType)).Length) {
				index = prev;
				return;
			}	

			RefreshText ();
		}

		/// <summary>
		/// Game type to string prefix
		/// </summary>
		/// <returns>The string prefix.</returns>
		private string IndexPrefix ()
		{
			if (index == Enum.GetNames (typeof(GameType)).Length) {
				return "Win";
			} else {
				GameType currentType = (GameType)index;
				if (Enum.IsDefined (typeof(GameType), currentType)) {				
					return currentType.ToString ();
				} else {
					return string.Empty;
				}
			}
		}

		public void Hide ()
		{	
			KlondikeController.Instance.SetObjectsVisibility (true);	
			KlondikeController.Instance.ResumeGame ();
			window.Hide (() => {
				gameObject.SetActive (false);
			});
		}
	}
}
