using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

namespace ui.windows
{
	public class GameSettings : MonoBehaviour
	{
	
		#region Inspector

		public Toggle[] deckTypeToggles;
		public Toggle[] scoreTypeToggles;

		public ButtonSwitch btnTimerSwitch;
		public ButtonSwitch btnTapMoveSwitch;
		public ButtonSwitch btnHintSwitch;
		public ButtonSwitch btnFinalAnimSwitch;
		public ButtonSwitch btnBgAnimSwitch;
		public ButtonSwitch btnThreeDecksSwitch;
		public ButtonSwitch btnChangeOrientation;

		public Dropdown langsDropdown;
		public DialogController dialog;
		public GameObject ui;
		public WindowAnimation window;
		public Slider soundScroll;

		#endregion

		private float soundOffThres = 0.1f;
		private bool shown = false;

		public bool Modyfied { get; private set; }

		#region Singleton

		void Awake ()
		{
			fileName = "settings.json";
			settingsPath = System.IO.Path.Combine (Application.persistentDataPath,	fileName);
			instance = this;
			settings = new SettingsData ();
		}

		/// <summary>
		/// Subscribe to all controllers events
		/// </summary>
		void EnableListeners ()
		{	

			//switch buttons
			btnTimerSwitch.StateChanged += OnTimerEnabledChanged;
			btnTapMoveSwitch.StateChanged += OnTapEnabledChanged;
			btnHintSwitch.StateChanged += OnHintEnabledChanged;
			btnFinalAnimSwitch.StateChanged += OnFinalEnabledChanged;
			btnBgAnimSwitch.StateChanged += OnBgAnimEnabledChanged;
			btnThreeDecksSwitch.StateChanged += OnThreeDecksEnabledChanged;
			btnChangeOrientation.StateChanged += OnOrientationEnabledChanged;

			// dropdowns
			langsDropdown.onValueChanged.AddListener (delegate {
				LanguageChanged ();
			});

			//toggles
			foreach (var t in scoreTypeToggles) {
				t.onValueChanged.AddListener ((res) => {
					OnScoreTypeChanged (res);
				});
			}

			foreach (var t in deckTypeToggles) {
				t.onValueChanged.AddListener ((res) => {
					OnTypeChanged (res);
				});
			}
		}

		/// <summary>
		/// Unsubscribe from all controllers events
		/// </summary>
		void DisableListeners ()
		{	
		
			btnTimerSwitch.StateChanged -= OnTimerEnabledChanged;
			btnTapMoveSwitch.StateChanged -= OnTapEnabledChanged;
			btnHintSwitch.StateChanged -= OnHintEnabledChanged;
			btnFinalAnimSwitch.StateChanged -= OnFinalEnabledChanged;
			btnBgAnimSwitch.StateChanged -= OnBgAnimEnabledChanged;
			btnThreeDecksSwitch.StateChanged -= OnThreeDecksEnabledChanged;
			btnChangeOrientation.StateChanged -= OnOrientationEnabledChanged;

			langsDropdown.onValueChanged.RemoveAllListeners ();	

			//toggles
			foreach (var t in scoreTypeToggles) {
				t.onValueChanged.RemoveAllListeners ();
			}

			foreach (var t in deckTypeToggles) {
				t.onValueChanged.RemoveAllListeners ();
			}
		}

		private static GameSettings instance;
		private SettingsData settings;

		public static GameSettings Instance {
			get{ return instance; }
		}

		public SettingsData Data {
			get{ return settings; }
		}

		#endregion

		#region Events

		public delegate void SettingsChanged ();

		public event SettingsChanged DeckTypeChanged;
		public event SettingsChanged ScoreTypeChanged;
		public event SettingsChanged TimerEnableChangedd;
		public event SettingsChanged TapMoveEnabledChanged;
		public event SettingsChanged HintsEnabledChanged;
		public event SettingsChanged FinalAnimEnabledChanged;
		public event SettingsChanged BgAnimEnabledChanged;
		public event SettingsChanged ThreeDecksEnabledChanged;
		public event SettingsChanged BackgroundSelected;
		public event SettingsChanged CardStyleSelected;

		private bool RaiseEvent (SettingsChanged _delegate)
		{
			if (_delegate == null)
				return false;

			_delegate ();
			return true;
		}

		#endregion

		private string fileName;
		private string settingsPath;

		/// <summary>
		/// Gets the selected toggle identifier from group.
		/// </summary>
		/// <returns><c>true</c>, if selected toggle identifier was found, <c>false</c> otherwise.</returns>
		/// <param name="arr">Toggle array.</param>
		/// <param name="index">Result toggle index.</param>
		public static bool GetSelectedToggleId (Toggle[] arr, out int index)
		{
			IEnumerator<Toggle> enumerator = arr [0].group.ActiveToggles ().GetEnumerator ();

			if (enumerator.MoveNext ()) {
				for (int i = 0; i < arr.Length; i++) {
					if (arr [i] == enumerator.Current) {
						index = i;
						SoundManager.Instance.PlaySound (0);
						return true;
					}
				}
			}

			index = 0;
			return false;
		}


		/// <summary>
		/// Convert boolean argument to string. 
		/// </summary>
		/// <returns>"on" if arg is true, otherwise returns "off".</returns>
		/// <param name="arg">If set to <c>true</c> argument.</param>
		public static string BoolToOnOffStr (bool arg)
		{
			return LocalizationManager.Instance.GetText (arg ? "on" : "off");
		}

		/// <summary>
		/// Gets the game type string prefix.
		/// </summary>
		/// <value>The type prefix.</value>
		public static string TypePrefix {
			get { 
				return GameSettings.Instance.Data.ScoreType == ScoreType.Windows ? "Win" : GameSettings.Instance.Data.DeckType.ToString ();
			}

		}

		public void OnVolumeChanged ()
		{		
			settings.SoundEnabled = (settings.Volume = soundScroll.value) > soundOffThres;	
			Modyfied = true;
		}

		public void OnTimerEnabledChanged (bool value)
		{
			dialog.Show (LocalizationManager.Instance.GetText ("deckChangedQuestion"), DialogType.YesNo, (isYes) => {
				if (isYes) {
					settings.TimerEnabled = value;
					RaiseEvent (TimerEnableChangedd);
					Modyfied = true;
					Hide ();
					return;
				} else {
					btnTimerSwitch.State = !btnTimerSwitch.State;
				}
			});

		}

		public void OnTapEnabledChanged (bool value)
		{
			settings.TapMoveEnabled = value;
			RaiseEvent (TapMoveEnabledChanged);
			Modyfied = true;
		}

		public void OnHintEnabledChanged (bool value)
		{
			settings.HintsEnabled = value;
			RaiseEvent (HintsEnabledChanged);
			Modyfied = true;
		}

		public void OnFinalEnabledChanged (bool value)
		{
			settings.FinalAnimEnabled = value;
			RaiseEvent (FinalAnimEnabledChanged);
			Modyfied = true;
		}

		public void OnBgAnimEnabledChanged (bool value)
		{
			settings.BgAnimEnabled = value;
			RaiseEvent (BgAnimEnabledChanged);
			Modyfied = true;
		}

		public void OnOrientationEnabledChanged (bool value)
		{
			settings.ChangeOrient = OrientationHandler.RotationEnabled = value;
			Modyfied = true;
		}

		public void OnThreeDecksEnabledChanged (bool value)
		{		
			if (settings.ThreeFlips == value) {			
				return;
			}

			dialog.Show (LocalizationManager.Instance.GetText ("deckChangedQuestion"), DialogType.YesNo, (isYes) => {
				if (isYes) {				
					settings.ThreeFlips = value;
					RaiseEvent (ThreeDecksEnabledChanged);
					Modyfied = true;
					Hide ();
					return;
				} else {				
					btnThreeDecksSwitch.State = !value;
				}
			});

		}

		public void OnTypeChanged (bool value)
		{		
			if (!value)
				return;

			int foundId;

			if (GameSettings.GetSelectedToggleId (deckTypeToggles, out foundId)) {

				if (foundId != (int)settings.DeckType) {
					dialog.Show (LocalizationManager.Instance.GetText ("deckChangedQuestion"), DialogType.YesNo, (isYes) => {
						if (isYes) {								
							settings.DeckType = (GameType)foundId;
							RaiseEvent (DeckTypeChanged);
							Modyfied = true;
							Hide ();
							return;
						} else {
							deckTypeToggles [(int)settings.DeckType].isOn = true;
						}
					});
				}

			}
		}

		// if some settings depend on other we declare this logic here
		private void ApplyDependentLayout ()
		{	
			//when windows score is chosen we disable deck type toggles
			foreach (var d in deckTypeToggles) {
				d.interactable = !(settings.ScoreType == ScoreType.Windows);
				d.isOn = false;
			}

			//when windows score is chosen we can't change deck flips rule
			btnThreeDecksSwitch.Active = !(settings.ScoreType == ScoreType.Windows);

			if (settings.ScoreType == ScoreType.Windows) {
				//preset deck type and flip counter settings for windows game type
				deckTypeToggles [(int)(settings.DeckType = GameType.ThreeOnThree)].isOn = true;
				btnThreeDecksSwitch.State = settings.ThreeFlips = true;
			} else {
				//if we dont use windows game type just fill this controlls with settings values
				deckTypeToggles [(int)settings.DeckType].isOn = true;
				btnThreeDecksSwitch.State = settings.ThreeFlips;
			}
		}

		public void OnScoreTypeChanged (bool value)
		{
			if (!value)
				return;

			int foundId;

			if (GameSettings.GetSelectedToggleId (scoreTypeToggles, out foundId)) {
				if (foundId != (int)settings.ScoreType) {

					dialog.Show (LocalizationManager.Instance.GetText ("deckChangedQuestion"), DialogType.YesNo, (isYes) => {
						if (isYes) {					
							settings.ScoreType = (ScoreType)foundId;

							ApplyDependentLayout ();

							RaiseEvent (ScoreTypeChanged);		
							Modyfied = true;
							Hide ();
							return;
						} else {
							scoreTypeToggles [(int)settings.ScoreType].isOn = true;
						}
					});						
				}
			}
		}

		public void TryToSetCardStyle (int index, Action<bool> resultCallback)
		{
			if (index != settings.CardStyle) {			
				settings.CardStyle = index;
				RaiseEvent (CardStyleSelected);
				Modyfied = true;
				resultCallback (true);
			} else
				resultCallback (false);
		}

		public void TryToSetBg (int index, Action<bool> resultCallback)
		{
			if (index != settings.Background) {			
				settings.Background = index;
				RaiseEvent (BackgroundSelected);
				Modyfied = true;
				resultCallback (true);
			} else
				resultCallback (false);
		}

		public void TryToSetCardBack (int index, Action<bool> resultCallback)
		{
			if (index != settings.CardBack) {			
				settings.CardBack = index;
				resultCallback (true);
				Modyfied = true;
			} else
				resultCallback (false);
		}

		/// <summary>
		/// Load settings from persistent storage into memory.
		/// </summary>
		public async Task Load ()
		{			
			if (!System.IO.File.Exists (settingsPath)) {
				using (WWW www = new WWW (System.IO.Path.Combine (KlondikeStatistics.SAFilePath, fileName))) {
					while (!www.isDone) {
						await Task.Yield();
					}
					if (www.error == null) {
						System.IO.File.WriteAllBytes (settingsPath, www.bytes);
					}
				}
			}

			ParseSettings (settingsPath);
		}

		/// <summary>
		/// Deserealizes given JSON string to SettingsData object
		/// </summary>
		/// <returns>The settings data objec.</returns>
		/// <param name="path">JSON path.</param>
		private SettingsData ParseSettings (string path)
		{
			try {
				string json = System.IO.File.ReadAllText (path);
				settings = JsonMapper.ToObject<SettingsData> (json);
			} catch {
				settings = new SettingsData ();
			}

			return settings;
		}

		/// <summary>
		/// Languages the changed.
		/// </summary>
		private void LanguageChanged ()
		{	
			Modyfied = true;
			SoundManager.Instance.PlaySound (0);

			//save lang setting
			Data.Language = LocalizationManager.Instance.AvailableLangs [langsDropdown.value] [0];

			//load language data
			LocalizationManager.Instance.ReadLangXml (Data.Language);

			//update switches labels according to new language
			btnTimerSwitch.UpdateLabel ();
			btnTapMoveSwitch.UpdateLabel ();
			btnHintSwitch.UpdateLabel ();
			btnFinalAnimSwitch.UpdateLabel ();
			btnBgAnimSwitch.UpdateLabel ();
			btnThreeDecksSwitch.UpdateLabel ();
			btnChangeOrientation.UpdateLabel ();
		}

		/// <summary>
		/// Save settings data to persistent storage (JSON file).
		/// </summary>
		public void Save ()
		{		
			//check if settings actually were modified, otherwise quit method
			if (!Modyfied)
				return;

			string json = JsonMapper.ToJson (settings);
			System.IO.File.WriteAllText (settingsPath, json);
			KlondikeController.Instance.msgHandler.AddMsg ("Settings saved: " + settingsPath);
			Modyfied = false;
		}

		public void Show ()
		{	
			if (shown)
				return;

			//pause the game and hide objects during settings menu
			KlondikeController.Instance.PauseGame ();
			KlondikeController.Instance.SetObjectsVisibility (false);

			//apply settings to controllers

			btnTimerSwitch.State = settings.TimerEnabled;		
			btnTapMoveSwitch.State = settings.TapMoveEnabled;
			btnHintSwitch.State = settings.HintsEnabled;
			btnFinalAnimSwitch.State = settings.FinalAnimEnabled;
			btnBgAnimSwitch.State = settings.BgAnimEnabled;
			btnChangeOrientation.State = settings.ChangeOrient;

			scoreTypeToggles [(int)settings.ScoreType].isOn = true;	

			soundScroll.value = (float)settings.Volume;

			//instantiate dropdown options for all the available languages
			langsDropdown.ClearOptions ();
			foreach (var l in LocalizationManager.Instance.AvailableLangs) {
				langsDropdown.options.Add (new Dropdown.OptionData (l [1]));
			}
		
			langsDropdown.value = LocalizationManager.Instance.CurrentLangId;
			langsDropdown.RefreshShownValue ();	

			ApplyDependentLayout ();

			EnableListeners ();

			ui.SetActive (true);
			window.Show ();
			shown = true;
		}

		public void Hide ()
		{		
			if (!shown)
				return;
		
			DisableListeners ();
			KlondikeController.Instance.ResumeGame ();
			KlondikeController.Instance.SetObjectsVisibility (true);
			Save ();
			window.Hide (() => {
				ui.SetActive (false);
			});
			shown = false;
		}

	}
}
