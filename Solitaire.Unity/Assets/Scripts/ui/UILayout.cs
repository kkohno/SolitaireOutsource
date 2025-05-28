using TMPro;
using ui.translatable;
using ui.windows;
using UnityEngine;
using UnityEngine.UI;

namespace ui
{
	/// <summary>
	/// User interface layout. Controlls positions and state of all UI elements, handles user input events
	/// </summary>
	public class UILayout : MonoBehaviour
	{

		public RectTransform topPanel;
		public Image finishCardsBg;
		public TMP_Text txtTimeLabel;
		public TMP_Text txtScoreLabel;
		public TMP_Text txtTime;
		public TMP_Text txtScore;
		public DialogController dialog;
		public FinishWindowController finishWindow;
		public AddPanelController addPanel;
		public ThemeSettings themeSettings;
		public GameObject loadingScreen;
		public Button btnAutoPlay;
		public Button btnHint;
		public BackgroundSpriteResizer backgroundSizer;

		public const float FACE_OFFSET_Y = 2.4f;
		public const float FACE_OFFSET_X = 2f;
		public const float BACK_OFFSET_Y = 8f;

		public const int DEFAULT_LAYER_ID = 0;
		public const int IGNORE_LAYER_ID = 3;
		public const int BACKGROUND_LAYER_ID = 8;

		public const string SORTING_DEFAULT = "Default";
		public const string SORTING_TOP = "Layer1";

		private static UILayout instance;
		private CardLayout curLayout;

		UIVariant[] layoutVariants;
		TranslatableText[] _translatable;
		TranslatableText[] Translatable
		{
			get
			{
				if (_translatable != null) return _translatable;
				_translatable = GetComponentsInChildren<TranslatableText>(true);
				return _translatable;
			}
		}

		#region Events

		public delegate void GameControllerEvent ();

		public static event GameControllerEvent LayoutChanged;

		#endregion

		/// <summary>
		/// Gets the cuurent type of the layout.
		/// </summary>
		/// <value>The type of the layout.</value>
		public CardLayout LayoutType {
			get{ return curLayout; }
		}

		public static UILayout Instance {
			get{ return instance; }
		}

		void Awake ()
		{
			Screen.orientation = ScreenOrientation.AutoRotation;
			instance = this;
			//LocalizationManager.LanguageLoaded += LanguageLoaded;
		}

		void Start ()
		{

			//get all orientation-dependent objects
			layoutVariants = GetComponentsInChildren<UIVariant> (true);

			//get all translatable objects is scene
			//if (translatable == null)
			//	translatable = GetComponentsInChildren<TranslatableText> (true);


			GameSettings.Instance.HintsEnabledChanged += OnHintEnabledChanged;
			OrientationHandler.OnResolutionChange += OnResolutionChange;

			OnHintEnabledChanged ();
		}


		/// <summary>
		/// Returns layout of current resolution.
		/// </summary>
		/// <returns>Card layout type.</returns>
		public static CardLayout LayoutOfCurrentResolution ()
		{
			if (Screen.width > Screen.height)
			{
				return CardLayout.Horizontal;
			}
			else
			{
				return CardLayout.Vertical;
			}
		}

		/// <summary>
		/// Handles the resolution change event.
		/// </summary>
		public void OnResolutionChange ()
		{	
			//change the layout to fit the current resolution
			Layout (LayoutOfCurrentResolution ());	

			//notify subscribers that layout was changed
			//if (LayoutChanged != null)
			// LayoutChanged ();
		}

		/// <summary>
		/// Gets or sets a value indicating whether current game can be autoplayed.
		/// </summary>
		/// <value><c>true</c> if auto play enabled; otherwise, <c>false</c>.</value>
		public bool AutoPlayEnabled {
			get{ return btnAutoPlay.gameObject.activeSelf; }
			set{ btnAutoPlay.gameObject.SetActive (value); }
		}

		/// <summary>
		/// Sets autoplay visibility.
		/// </summary>
		/// <param name="value">If set to <c>true</c> autoplay btn is visible.</param>
		public void AutoPlayVisible (bool value)
		{		
			btnAutoPlay.GetComponent<Image>().enabled = value ;
			btnAutoPlay.GetComponentInChildren<Text>().enabled = value ;
		}

		/// <summary>
		/// Handles language loaded event.
		/// </summary>
		/* private void LanguageLoaded ()
		{		
			//TODO refactor
			// change size of text labels if german or spanish language is selected (strings are too long)
			txtScoreLabel.fontSize = txtTimeLabel.fontSize = 
				(LocalizationManager.Instance.CurrentLangId == 3 || LocalizationManager.Instance.CurrentLangId == 4) 
					? 42 : 52;

			//update all translatable objects
			foreach (var t in Translatable)
				t.Translate ();	
		} */

		/// <summary>
		/// Update hint button availability according to current game settings
		/// </summary>
		private void OnHintEnabledChanged ()
		{	
			btnHint.interactable = GameSettings.Instance.Data.HintsEnabled;
		}

		/// <summary>
		/// Handles auto play button click
		/// </summary>
		public void OnAutoPlayClicked ()
		{
			//begin auto play process
			StartCoroutine (KlondikeController.Instance.AutoPlay ());
		}

		/// <summary>
		/// Handles undo button clicked
		/// </summary>
		public void OnUndoClicked ()
		{
			KlondikeController.Instance.UndoLastMove ();
		}

		/// <summary>
		/// Handles show hint button clicked
		/// </summary>
		public void OnHintClicked ()
		{	

			KlondikeController.Instance.HintManager.ShowHint ();

		}

		/// <summary>
		/// Shows the finish window when game was ended (win\loss).
		/// </summary>
		/// <param name="isSuccess">If set to <c>true</c> game is win.</param>
		public void ShowFinishWindow (bool isSuccess)
		{		
			//if settings is open hide them
			GameSettings.Instance.Hide ();

			//if add panel is open hide it
			addPanel.Hide();

			//if theme settings is open hide them
			themeSettings.Hide ();

			//if game is in proccess pause it
			KlondikeController.Instance.PauseGame ();

			//set window title according to game result
			finishWindow.msg.text = LocalizationManager.Instance.GetText (isSuccess ? "winText" : "loseText");	

			//play sound according to game result
			SoundManager.Instance.PlaySound (isSuccess ? 9 : 10, 1f);

			//fill statistic data
			finishWindow.gamesToday.text = PlayerPrefs.GetInt (KlondikeStatistics.GAMES_TODAY_KEY).ToString ();
			finishWindow.winsToday.text = PlayerPrefs.GetInt (KlondikeStatistics.WINS_TODAY_KEY).ToString ();

			//fill current game data
			finishWindow.time.text = KlondikeController.Instance.ElapsedTimeString;
			finishWindow.score.text = KlondikeController.Instance.Score.ToString ();

			//show the window
			finishWindow.Show (isSuccess);
		}

		/// <summary>
		/// Handles the exit button.
		/// </summary>
		public void OnCloseClicked ()
		{

			//pause game timers and events
			KlondikeController.Instance.PauseGame ();

			//ask player if he surely wants to quit
			dialog.Show (LocalizationManager.Instance.GetText ("exitQuestion"), DialogType.YesNo, (yes) => {					

				if (yes) {					
					KlondikeController.Instance.SaveGameState ();
					Application.Quit ();	
				} else
					KlondikeController.Instance.ResumeGame ();

			});		
		}

		/// <summary>
		/// Fit UI and game components to current resolution
		/// </summary>
		public void Layout ()
		{
			Layout (LayoutOfCurrentResolution ());
		}

		/// <summary>
		/// Fit UI and game components according to given layout type
		/// </summary>
		public void Layout (CardLayout layout)
		{		
			if (KlondikeController.Instance.IsDealingInProgress)
				return;

			//background cards behind finish window enabled only in vertical layout
			finishCardsBg.GetComponent<Image> ().enabled = layout == CardLayout.Vertical;

			//apply layout to all orientation-dependent objects
			foreach (var u in layoutVariants) {
				u.SetVariant (layout);
			}

			//how much space we must indent from the top of the screen 
			float topOffsetPx = topPanel.sizeDelta.y;

			curLayout = layout;

			float cardSpacing = ThemeManager.CardSize.x / 8f;
			float camSize = 1f;
			float aspect = (float)Screen.width / (float)Screen.height;

			//calculate camera orthographic size

			if (layout == CardLayout.Vertical)
				// we need enough width to place all stacks with spaces between them
				camSize = (((ThemeManager.CardSize.x * (KlondikeController.Instance.Stacks.Length)) + (cardSpacing * 8f)) / aspect) / 2f;
			else if (layout == CardLayout.Horizontal) {		
				// we need enough height to place four foundation slots with spaces between them and top panel
				float camSizeY = (ThemeManager.CardSize.y * 4f + cardSpacing * 5f) / 2f / (1 - ((topOffsetPx) / GetComponent<RectTransform> ().sizeDelta.y));				
				float camSizeX = (((ThemeManager.CardSize.x * (KlondikeController.Instance.Stacks.Length + 3)) + (cardSpacing * (KlondikeController.Instance.Stacks.Length+4))) / aspect) / 2f;				

				camSize = Mathf.Max (camSizeY, camSizeX);
			}

			//apply calculated size to camera
			Camera.main.orthographicSize = camSize;

			//calculate camera size in units
			float screenHeightInUnits = camSize * 2f;
			float screenWidthInUnits = screenHeightInUnits * aspect;

			// calculate top offset in units
			float topOffsetUnits = topOffsetPx / GetComponent<RectTransform> ().sizeDelta.y * screenHeightInUnits + cardSpacing;

			//calculate, how much height left for cards in our screen
			//we will use it to shrink big stacks, when they overflow screen height (in landscape mode)
			float leftForCards = screenHeightInUnits - topOffsetUnits - cardSpacing;

			//in vertical mode stacks starts below foundation slots and deck
			//so we must correct leftForCards with this value
			if (layout == CardLayout.Vertical)
				leftForCards -= cardSpacing + ThemeManager.CardSize.y;		

			//calculate center stack index
			int midStack = (KlondikeController.Instance.Stacks.Length / 2);

			//modify camera position to be in the center of play field
			Camera.main.transform.position = new Vector3 (cardSpacing + midStack * (ThemeManager.CardSize.x + cardSpacing), -screenHeightInUnits / 2f + ThemeManager.CardSize.y / 2f + topOffsetUnits, -5f);

			//place free slots
			for (int i = 0; i < KlondikeController.Instance.Foundation.Length; i++) {

				if (layout == CardLayout.Vertical)
					//in vertical mode free slots placed horizontally at the top of game field
					KlondikeController.Instance.Foundation [i].Position = new Vector3 (cardSpacing + i * (ThemeManager.CardSize.x + cardSpacing), 0f, 0f);
				else if (layout == CardLayout.Horizontal)
					//in horizontal mode free slots placed vertically in the left of the screen
					KlondikeController.Instance.Foundation [i].Position = new Vector3 (Camera.main.transform.position.x - screenWidthInUnits / 2f + ThemeManager.CardSize.x / 2f + cardSpacing, -i * (ThemeManager.CardSize.y + cardSpacing), 0f);
			
				KlondikeController.Instance.Foundation [i].IsShown = true;
			}

			//place all card stacks
			for (int i = 0; i < KlondikeController.Instance.Stacks.Length; i++) {
			
				if (layout == CardLayout.Vertical)
					//in the vertical mode stacks are at the top, bot under the foundation slots and deck
					KlondikeController.Instance.Stacks [i].Position = new Vector3 (cardSpacing + i * (ThemeManager.CardSize.x + cardSpacing), -ThemeManager.CardSize.y - cardSpacing, 0f);
				else if (layout == CardLayout.Horizontal)
					//in the horizontal mode stacks are at the top of the screen
					KlondikeController.Instance.Stacks [i].Position = new Vector3 (cardSpacing + i * (ThemeManager.CardSize.x + cardSpacing), 0f, 0f);
			
				KlondikeController.Instance.Stacks [i].MaxOffset = ThemeManager.CardSize.y / FACE_OFFSET_Y;
				KlondikeController.Instance.Stacks [i].MaxLen = leftForCards - ThemeManager.CardSize.y;
				KlondikeController.Instance.Stacks [i].BackOffset = ThemeManager.CardSize.y / BACK_OFFSET_Y;
				KlondikeController.Instance.Stacks [i].IsShown = true;
			}

			if (layout == CardLayout.Vertical)
				//in vertical mode deck in top right corner of the screen (with spacing padding)
				KlondikeController.Instance.Deck.Position = new Vector3 (6f * ThemeManager.CardSize.x + cardSpacing * 7f, 0f, 0f);
			else if (layout == CardLayout.Horizontal)
				//in hotizontal mode deck int the bottom right corner, but with offset from the bottom equals to card height
				KlondikeController.Instance.Deck.Position = new Vector3 (Camera.main.transform.position.x + screenWidthInUnits / 2f - ThemeManager.CardSize.x - cardSpacing, Camera.main.transform.position.y - ThemeManager.CardSize.y, 0f);

			KlondikeController.Instance.Deck.UpdateFlushed (false);
			KlondikeController.Instance.Deck.UpdateShown (false, false, false);
			KlondikeController.Instance.Deck.IsShown = true;

			//make background fit to screen size and camera positions
			backgroundSizer.RefreshLayout (new Vector2 (screenWidthInUnits, screenHeightInUnits));

		}
	}
}
