using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

public enum CardLayout
{
	Vertical = 0,
	Horizontal
}

public class KlondikeController : MonoBehaviour
{
	public ToastMessage toast;
	public GameObject cardPrefab;
	public GameObject slotPrefab;
	public MessageHandler msgHandler;
	public Canvas canvas;

	public Text txtTime;
	public Text txtScore;
	public Text txtMove;

	public UnityEngine.EventSystems.EventSystem eventSystem;

	Card[] cards;
	KlondikeCardStack[] stacks;
	KlondikeSlot[] freeSlots;
	KlondikeDeck deck;
	Move tipMove;
	CardGroup pressed;
	Vector3 originalCardPos;
	Vector3 pressedOffset;
	Vector2 cardSize;
	string originalLayer;
	DeviceOrientation currentOrientation;
	DateTime startTime;
	TimeSpan elapsed;
	int moveCount = 0;
	List<IUndoAction> history;

	int colCount = 7;
	int slotCount = 4;
	int score;
	int penaltySeconds;

	float deltaTime = 0.0f;

	void Awake ()
	{
		Application.targetFrameRate = 60;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(10, 50, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color (1.0f, 1.0f, 0f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}

	// Use this for initialization
	void Start ()
	{
		
		msgHandler.AddMsg ("Инициализация...");
		ThemeManager.Init ();
		cardSize = new Vector2 (ThemeManager.BackSprite.bounds.size.x, ThemeManager.BackSprite.bounds.size.y);

		cards = new Card[52];

		//create cards, apply theme textures, 
		for (int i = 0; i < 52; i++) {
			cards [i] = Instantiate (cardPrefab).GetComponent<Card> ();
			cards [i].name = "card_" + i;
			cards [i].Index = i;
			cards [i].SetFaceSprite (ThemeManager.FaceSprite (i));
			cards [i].SetBackSprite (ThemeManager.BackSprite);
			cards [i].SetShadowSprite (ThemeManager.BackSprite);
			cards [i].SetGlowSprite (ThemeManager.GlowSprite);
			cards [i].gameObject.SetActive (false);
			cards [i].Pressed += OnCardPressed;
			cards [i].Released += OnCardReleased;
		}

		//form 4 result slots
		if (freeSlots == null)
			freeSlots = new KlondikeSlot[slotCount];

		for (int i = 0; i < slotCount; i++) {
			if (freeSlots [i] == null) {
				GameObject go = Instantiate (slotPrefab);
				go.GetComponentInChildren<SpriteRenderer> ().sprite = ThemeManager.SlotSprite;

				freeSlots [i] = new KlondikeSlot (i, cardSize, go);
			}
		}

		stacks = new KlondikeCardStack[colCount];

		//form 7 solitaire stacks and place them
		for (int i = 0; i < colCount; i++) {
			if (stacks [i] == null)
				stacks [i] = new KlondikeCardStack (i, cardSize, null);			
		}

		//create deck and put rest of the cards in it
		GameObject deckObject = new GameObject ();
		deckObject.name = "deck";
		DeckController ctrl = deckObject.AddComponent<DeckController> ();
		ctrl.DeckClicked += DeckClicked;
		BoxCollider2D collider = deckObject.AddComponent<BoxCollider2D> ();
		GameObject deckSpriteObj = new GameObject ();
		SpriteRenderer deckSprite = deckSpriteObj.AddComponent<SpriteRenderer> ();
		deckSprite.sprite = ThemeManager.SlotSprite;
		//deckSprite.sortingOrder = -2000;
		deckSpriteObj.transform.SetParent (deckObject.transform);

		collider.transform.localScale = Vector2.one;
		deck = new KlondikeDeck (0, cardSize, deckObject);


		originalCardPos = Vector3.zero;
		pressed = null;

		Layout (OrientationToLayout (currentOrientation = Input.deviceOrientation));

		Restart ();

		//OrientationHandler.OnOrientationChange += OnOrienationChange;
		//OrientationHandler.OnResolutionChange += OnResolutionChange;

	}

	void Update ()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
		elapsed = DateTime.Now - startTime;

		if (elapsed.Seconds % 10 == 0 && penaltySeconds != elapsed.Seconds) {
			//Debug.Log ("penalty!");
			penaltySeconds = elapsed.Seconds;

			if (score > 2) {
				score -= 2;
			} else
				score = 0;

			txtScore.text = "Score: " + score;
		}

		txtTime.text = string.Format ("{0:00}:{1:00}", elapsed.Minutes, elapsed.Seconds);

		if (pressed == null)
			return;

		if (!pressed.Active)
			return;

		pressed.Position = GetTouchPosition () - pressedOffset;
	}

	private void OnOrienationChange (DeviceOrientation newOrient)
	{
		if (currentOrientation == newOrient)
			return;
				
		Layout (OrientationToLayout (currentOrientation = newOrient));

	}

	private void OnResolutionChange (Vector2 newRes)
	{

		if (currentOrientation == Input.deviceOrientation)
			return;

		Layout (OrientationToLayout (currentOrientation = Input.deviceOrientation));

	}

	private CardLayout OrientationToLayout (DeviceOrientation orientation)
	{

		CardLayout newLayout = CardLayout.Horizontal;

		if (orientation == DeviceOrientation.FaceUp ||
		    orientation == DeviceOrientation.FaceDown ||
		    orientation == DeviceOrientation.Portrait ||
		    orientation == DeviceOrientation.PortraitUpsideDown ||
		    orientation == DeviceOrientation.Unknown)
			newLayout = CardLayout.Vertical;

		return newLayout;
	}

	public void ClearSlots ()
	{
		
		foreach (var s in stacks) {			
			s.Clear ();
		}
		
		foreach (var f in freeSlots)
			f.Clear ();
		
		deck.Clear ();

		msgHandler.AddMsg ("Сброс...");

	}

	public void Restart ()
	{
		if (history == null)
			history = new List<IUndoAction> ();
		else
			history.Clear ();
		penaltySeconds = -1;
		score = 0;
		startTime = DateTime.Now;
		StartCoroutine (StartRoutine ());
	}

	private IEnumerator StartRoutine (float delay = 0.04f)
	{
		OrientationHandler.OnResolutionChange -= OnResolutionChange;
		eventSystem.enabled = false;
		//shuffle randomly

		cards = cards.OrderBy (x => UnityEngine.Random.Range (0, 52)).ToArray ();

		int k = 0;

		deck.IsReady = false;
		for (int i = 0; i < cards.Length; i++) {			
			cards [i].transform.position = deck.Position;		
			cards [i].SetSide (true);
			//TODO magic number
			if (i < 24) {
				deck.AddCard (cards [i]);
				k++;
			}
		}
		deck.IsReady = true;

		//form 7 solitaire stacks and place them
		for (int i = 0; i < colCount; i++) {			
			for (int j = 0; j < i + 1; j++) {
				cards [k].SetSide (j != i);
				stacks [i].AddCard (cards [k], true, 0f);
				k++;
				yield return new WaitForSeconds (delay);
			}
		}

		/*for (int i = k; i < cards.Length; i++) {			
			
		}*/

		StartCoroutine (CheckForNoMoves ());
		eventSystem.enabled = true;
		OrientationHandler.OnResolutionChange += OnResolutionChange;
	}

	public void Layout (CardLayout layout)
	{
		//layout = CardLayout.Horizontal;
		int topOffsetPx = 40;
		int botOffsetPx = 50;

		float cardSpacing = cardSize.x / 10f;

		float aspect = (float)Screen.width / (float)Screen.height;

		Camera.main.orthographicSize = (((cardSize.x * stacks.Length * (layout == CardLayout.Vertical ? 1f : 1.5f)) + (cardSpacing * 8f)) / aspect) / 2f;

		float screenHeightInUnits = Camera.main.orthographicSize * 2f;
		//float screenWidthInUnits = screenHeightInUnits * aspect;
		//float pixInUnitY = Camera.main.pixelHeight / screenHeightInUnits;
		
		float topOffsetUnits = topOffsetPx / canvas.GetComponent<RectTransform> ().sizeDelta.y * screenHeightInUnits + cardSpacing;
		float botOffsetUnits = botOffsetPx / canvas.GetComponent<RectTransform> ().sizeDelta.y * screenHeightInUnits + cardSpacing;

		float leftForCards = screenHeightInUnits - topOffsetUnits - cardSpacing - cardSize.y - botOffsetUnits;

		//Camera.main.transform.position = new Vector3 (screenWidthInUnits / 2f - cardSize.x / 2f, -screenHeightInUnits / 2f + cardSize.y / 2f + topOffsetUnits, -5f);
		int midStack = (stacks.Length / 2);

		Camera.main.transform.position = new Vector3 (cardSpacing + midStack * (cardSize.x + cardSpacing), -screenHeightInUnits / 2f + cardSize.y / 2f + topOffsetUnits, -5f);
		//Debug.Log(aspect);
		//Debug.Log(cardSize.x * 7f / aspect);
		//calculate camera size

		//place free slots
		for (int i = 0; i < freeSlots.Length; i++) {
			freeSlots [i].Position = new Vector3 (cardSpacing + i * (cardSize.x + cardSpacing), 0f, 0f);
		}

		for (int i = 0; i < stacks.Length; i++) {
			stacks [i].Position = new Vector3 (cardSpacing + i * (cardSize.x + cardSpacing), -cardSize.y - cardSpacing, 0f);
			stacks [i].MaxOffset = cardSize.y / 5f;
			stacks [i].MaxLen = leftForCards - cardSize.y;
			stacks [i].BackOffset = cardSize.y / 8f;
			//stacks [i].Offset = (leftForCards - cardSize.y / 2) / ((stacks.Length - 1) + 12);
		}

		deck.Position = new Vector3 (6f * cardSize.x + cardSpacing * 7f, 0f, 0f);

	}

	private Vector3 GetTouchPosition ()
	{		

		Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		pos.z = pressed.Position.z;
		return pos;	

		//TODO
		//return Vector3.zero;
	}

	private void OnCardReleased (CardEventArgs a)
	{
		if (pressed == null || !pressed.Active)
			return;

		pressed.Root.sr.color = Color.white;

		pressed.ReleaseAnimation (() => { 				
			pressed.Layer = originalLayer;
		});

		Slot targetSlot;

		if (!CheckIfCanDrop (pressed.Root, out targetSlot)) {
			//Debug.Log ("invalid turn!");
			msgHandler.AddMsg ("Невалидный ход картой: " + Card.CardString (a.Card));
			CancelTurn ();
			return;
		}

		Move move = new Move (pressed.Slot, pressed.Root, targetSlot);

		score += move.Score;
		history.Add (move);

		moveCount++;
		txtMove.text = "Moves: " + moveCount.ToString ();
		txtScore.text = "Score: " + score.ToString();

		pressed.Slot.RemoveGroup (pressed);
		targetSlot.PlaceGroup (pressed, true);
		FinishTurn (true);
	}

	private void OnCardPressed (CardEventArgs a)
	{

		if (!CheckCardCanPress (a.Card)) {
			//Debug.Log ("cant press card: " + a.Card.name);
			msgHandler.AddMsg ("Нельзя нажимать на карту: " + Card.CardString (a.Card));
			return;
		}

		//TODO force all animations to end
		if (pressed != null) {
			pressed.Layer = originalLayer;
		}

		Slot cardSlot = FindSlot (a.Card);
		if (cardSlot != null) {
			//store pressed card(group) reference
			pressed = cardSlot.GetGroup (a.Card);
			pressed.SelectAnimation ();
		} else {
			//Debug.Log ("invalid card state: " + a.Card.name);
			msgHandler.AddMsg ("Невалидное состояние карты: " + Card.CardString (a.Card));
			return;
		}
			
		originalCardPos = pressed.Position;
		originalLayer = pressed.Root.sr.sortingLayerName;

		pressedOffset = GetTouchPosition () - originalCardPos;

		//TODO bring moving group to the front
		pressed.Layer = "Layer1";

	}

	private bool CheckCardCanPress (Card c)
	{		
		//TODO if this card can be pressed at all
		return c.IsUp;
	}

	private Slot FindSlot (Card c)
	{
		//search among stacks
		Slot res = stacks.SingleOrDefault (x => x.ContainsCard (c));

		if (res == null) {
			//search among free slots
			res = freeSlots.SingleOrDefault (x => x.ContainsCard (c));
		}

		if (res == null && deck.ContainsCard (c)) {
			res = deck;
		}
		return res;
	}

	private bool CheckIfCanDrop (Card c, out Slot targetSlot)
	{
		//TODO check if card was dropped in right place
		bool res = false;
	
		Vector3 touchPos = GetTouchPosition ();
		Rect touchRect = new Rect (touchPos.x, touchPos.y, cardSize.x, cardSize.y);
		#region stacks
		for (int i = 0; i < colCount; i++) {
			
			if (stacks [i] == pressed.Slot)
				continue;

			//stack cell itself (if our card is king)
			if (stacks [i].IsRectInsideCell (touchRect) && stacks [i].CheckCard (pressed.Root)) {					
				targetSlot = stacks [i];
				return true;
			}

			//check all stack heads
			if (stacks [i].Size > 0 && stacks [i].Head.IsRectInside2D (touchRect)) {				
				targetSlot = null;
				if (res = stacks [i].CheckCard (pressed.Root)) {
					targetSlot = stacks [i];
				}	
				return res;
			}
		}
		#endregion

		#region free slots
		//check free slots
		for (int i = 0; i < slotCount; i++) {
			if (freeSlots [i].IsRectInsideCell (touchRect) && pressed.Size == 1 && freeSlots [i].CheckCard (pressed.Root)) {
				targetSlot = freeSlots [i];
				return true;
			}
		}
		#endregion

		targetSlot = null;
		return false;	
	}

	public void DeckClicked ()
	{
		moveCount++;
		txtMove.text = "Moves: " + moveCount.ToString ();

		history.Add (deck.ShowCards ());
	}

	public void Undo(){

		if (history.Count < 1)
			return;		

		IUndoAction a = history.Last ();
		//a.Debug ();

		a.Undo();
		history.Remove (a);
	}

	private void CancelTurn ()
	{
		if (pressed == null)
			return;

		if (Vector3.Distance (pressed.Position, originalCardPos) <= CardAnimationSettings.ShakeDistanceThres) {
			pressed.Position = originalCardPos;
			pressed.FailShake ();
		} else
			pressed.MoveWithAnimation (originalCardPos);
		
		FinishTurn (false);
	}

	private void FinishTurn (bool success)
	{			
		CheckGame ();
		if (tipMove == null || !tipMove.IsPossible) {
			tipMove = null;

			StopCoroutine (CheckForNoMoves ());
			StartCoroutine (CheckForNoMoves ());
		}
	}

	private void CheckGame ()
	{
		bool res = true;

		foreach (var f in freeSlots) {
			if (f.Head==null || f.Head.Value != 12) {
				res = false;
				break;
			}
		}

		if (res) {
			toast.DefaultPosition ();
			toast.Show ("Победа!",3f);
			msgHandler.AddMsg ("Победа!");
		}
	}

	private void PossibleMoveFound (Move move)
	{		
		tipMove = move;
		//ShowTip ();
		//TODO show tip button ???
	}

	public void ShowTip ()
	{
		toast.DefaultPosition();
		toast.Show(string.Format ("Возможный ход: из {0} картой {1} в слот {2}", tipMove.From.Name, Card.CardString (tipMove.Card), tipMove.To.Name),3f);

		if (tipMove != null && tipMove.IsPossible)
			msgHandler.AddMsg (string.Format ("Первый найденый возможный ход: из {0} картой {1} в слот {2}", tipMove.From.Name, Card.CardString (tipMove.Card), tipMove.To.Name));
	}

	//DEBUG
	public void ListDeckCards ()
	{
		//check cards in deck
		var v = deck.AvailableCards;

		foreach (var c in v) {
			Debug.Log (Card.CardString (c));
		}
	}

	private IEnumerator CheckForNoMoves ()
	{
		//TODO hide tip button ???

		//iterate through deck
		var v = deck.AvailableCards;
		bool stacksChecked = false;
		bool deckChecked = v.Count == 0;
		Move move = null;
		var enumer = v.GetEnumerator ();
		
		while (enumer.MoveNext () || !stacksChecked) {
			
			// can we move card from deck to any stack ?
			foreach (var s in stacks) {
				if (!deckChecked && s.CheckCard (enumer.Current)) {							
					move = new Move (deck, enumer.Current, s);
					deckChecked = true;
				}

				if (!stacksChecked) {					
					//can we move any card or group to other and turn new card face up
					Card first;
					if ((first = s.FirstFaceUp) != null) {
						//TODO again iterate through stacks, is it good?
						foreach (var target in stacks) {
							if (target != s && target.CheckCard (first) && !(target.Size == 0 && s.Cards.IndexOf (first) == 0)) {	
								//Debug.Log (string.Format("target.Size: {0} first.Index: {1}",target.Size,first.Index));
								//Debug.Log(first == null);
								PossibleMoveFound (new Move (s, first, target));	
								yield break;
							} 
						}
					}

					//can we move head of this stack to any free slot
					foreach (var f in freeSlots) {
						if (f.CheckCard (s.Head)) {							
							PossibleMoveFound (new Move (s, s.Head, f));				
							yield break;
						}
					}

					yield return null;

				}
			}
			stacksChecked = true;

			// can we move card from deck to any free slot ?
			foreach (var f in freeSlots) {
				if (!deckChecked && f.CheckCard (enumer.Current)) {					
					move = new Move (deck, enumer.Current, f);
					deckChecked = true;
				} 
			}
		
			yield return null;
		}

		if (move != null) {
			
			PossibleMoveFound (move);
		} else {
			toast.DefaultPosition ();
			toast.Show ("Не найдено возможных ходов ! Видимо это проигрыш ...", 3f);
			msgHandler.AddMsg ("Не найдено возможных ходов ! Видимо это проигрыш ...");
		}

		yield return null;
	}
}
