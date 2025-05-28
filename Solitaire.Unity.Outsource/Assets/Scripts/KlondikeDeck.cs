using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ui;
using ui.windows;

public class KlondikeDeck : Slot
{
	const float offsetX = 0.2f;
	const float offsetY = 0.1f;
	const char shownSeparator = ',';
	int cursor;
	List<Card> shown;
	int shownStep = 1;
	int shownMaxCount = 3;
	int flips = 0;
	GameType type;
	SpriteRenderer iconRenderer;
	public bool IsReady = false;

	public int Flips {
		get{ return flips; }
		private set {
			flips = value;
		}
	}

	/// <summary>
	/// Creates the deck object.
	/// </summary>
	/// <returns>The deck object.</returns>
	public static GameObject CreateDeckObject ()
	{

		//create deck game objects programmatically
		GameObject deckObject = new GameObject ("deck");
		//place deck on background layer behind all cards
		deckObject.layer = UILayout.BACKGROUND_LAYER_ID;
		deckObject.SetActive (false);
		//add click listener to deck
		deckObject.AddComponent<DeckController> ();
		//add collider to make clicks possible
		BoxCollider2D collider = deckObject.AddComponent<BoxCollider2D> ();
		//stretch collider to fit card size
		collider.size = ThemeManager.CardSize;

		return deckObject;
	}

	public KlondikeDeck (int index, GameObject o) : base (index, o)
	{		
		//base order is just random big int
		//it helps render culling
		baseOrder = 200;
		cursor = 0;
		shown = new List<Card> ();

		if (o != null) {

			// create icon object and add renderer component
			GameObject iconObject = new GameObject ();
			iconObject.layer = UILayout.BACKGROUND_LAYER_ID;
			iconObject.SetActive (false);
			iconObject.transform.SetParent (o.transform);
			iconObject.name = "deckImage";

			iconRenderer = iconObject.AddComponent<SpriteRenderer> ();
			iconRenderer.gameObject.SetActive (false);
			iconRenderer.sortingOrder = -2;
			//icon is half transparent
			iconRenderer.color = new Color (1f, 1f, 1f, 0.5f);
		}

		glowSprite.gameObject.layer = UILayout.BACKGROUND_LAYER_ID;
	}

	/// <summary>
	/// Gets the current active card (last card in shown list).
	/// </summary>
	/// <value>The current active card.</value>
	public Card CurrentActiveCard {
		get {
			if (shown == null || shown.Count < 1)
				return null;
			else
				return shown.Last ();
		}
	}

	/// <summary>
	/// Gets or sets the type of the deck.
	/// </summary>
	/// <value>The type of the deck.</value>
	public GameType DeckType {
		get{ return type; }
		set { 			

			// define how much card we show in deck
			// and how much cards we flip every deck move
			switch (type = value) {
			case GameType.OneOnOne:
				shownMaxCount = shownStep = 1;
				break;
			case GameType.OneOnThree:
				shownMaxCount = 3;
				shownStep = 1;
				break;
			case GameType.ThreeOnThree:
				shownMaxCount = shownStep = 3;
				break;
			}
		}
	}

	/// <summary>
	/// Gets deck move score according to game type and rules variant
	/// </summary>
	/// <returns>The move score.</returns>
	private int DeckMoveScore ()
	{
		int res = 0;

		if (cursor != -1 || GameSettings.Instance.Data.ScoreType == ScoreType.Standart)
			return res;
		
		//-20 - за переворот колоды, если она уже три раза была перевернута;
		if (Flips >= 3)
			res = -20;		
		
		//-100 - за переворот карты, когда оттуда сдается по одной карте;
		if (GameSettings.Instance.Data.DeckType != GameType.ThreeOnThree)
			res = -100;
		
		return res;
	}

	/// <summary>
	/// Performs the deck action.
	/// </summary>
	/// <returns>The deck action.</returns>
	/// <param name="animate">If set to <c>true</c> use animation.</param>
	public IUndoableUserMove PerformDeckAction (bool animate = true)
	{
		//if deck can't be flipped - exit method
		if (!CanBeFlipped)
			return null;

		// is animation enabled by user?
		if (!GameSettings.Instance.Data.BgAnimEnabled)
			animate = false;

		//if deck is empty just exit method
		if (Size < 1)
			return null;

		//stop all previous animations
		if (InAnimation) {			
			foreach (var c in Cards) {
				c.StopAllAnimation ();
			}
		}

		//create deck move object to store in history
		DeckMove res = new DeckMove (DeckMoveScore ());
		res.CursorState = cursor;
		List<Card> shownState = new List<Card> ();
		shownState.InsertRange (0, shown);
		res.ShownState = shownState;

		//if deck is empty - flip all the deck back
		if (DeckEnded) {		
			ResetDeck (animate);
			return res;
		}

		//show new card(s)
		ShowCards ();

		//count whole deck flips
		if (DeckEnded) {		
			Flips++;
		}

		//update shown cards positions and sorting order
		UpdateShown (false, animate, animate);

		//update flushed cards positions and layers
		UpdateFlushed (animate);

		return res;
	}

	/// <summary>
	/// Shows new cards from deck.
	/// </summary>
	private void ShowCards ()
	{
		//find out deck position of new cards
		int start = cursor;
		float delay = 0;

		//show some new cards cards. How much is defined by "shownStep" var
		for (int i = start; i < start + shownStep; i++) {

			//if we already show maximum available cards remove one from beginning to flushed pile
			if (shown.Count == shownMaxCount) {	
				shown.RemoveAt (0);
			}

			//show new card
			shown.Add (Cards [i]);	

			//play flip sound
			SoundManager.Instance.PlaySound (3, 0.5f, delay, SoundManager.Instance.RandomPitch ());

			//calculate delay for next sound
			delay += CardAnimationSettings.CardMoveDuration;

			//if deck is ended we stop showing process
			if (cursor >= Size - 1) {
				cursor = -1;				
				break;
			} else
				cursor++;
		}
	}

	/// <summary>
	/// Resets the deck.
	/// </summary>
	/// <param name="withAnimation">If set to <c>true</c> use animation.</param>
	private void ResetDeck (bool withAnimation)
	{
		// play sound depending on cards count
		if (Size > 1)
			SoundManager.Instance.PlaySound (2, 0.4f, SoundManager.Instance.RandomPitch ());
		else
			SoundManager.Instance.PlaySound (5, 0.5f, SoundManager.Instance.RandomPitch ());	

		cursor = 0;
		int size = Size;

		//iterate through all deck cards
		for (int i = 0; i < size; i++) {
			
			//reset card state and restore default deck order
			ReturnCardToDeck (Cards [i], i);

			//stop all animations if there some
			Cards [i].StopAllAnimation ();

			if (withAnimation) {

				// define animation start position
				// start position = flushed pile position
				Cards [i].transform.position = FlippedCardPosition (shownMaxCount - 1);			
				//move cards with animation to deck position
				Cards [i].MoveWithAnimation (slotPosition, CardAnimationSettings.CardMoveDuration, i * CardAnimationSettings.CardGroupDelay / 4f);
			} else
				//move cards instantly to deck position
				Cards [i].MoveInstantly (slotPosition);

			//enable renderer only for last card in deck and hide others
			//that will save some GPU time
			Cards [i].ShadowRendererEnabled = i == size - 1;
		}

		//clear list of shown cards
		shown.Clear ();

		iconRenderer.gameObject.SetActive (false);
	}

	/// <summary>
	/// Gets the list of cards that are available for picking from deck.
	/// </summary>
	/// <value>The available cards.</value>
	public List<Card> AvailableCards {
		get { 
			
			List<Card> res = new List<Card> ();

			//if deck is empty there is no any available cards
			if (Size < 1)
				return res;

			//if we cant flip cards anymore the only available card is current active card
			if (DeckEnded && !CanBeFlipped) {
				res.Add (CurrentActiveCard);
				return res;
			}

			//as if we use deck from its current state
			if (shown.Count > 0) {				
				res.AddRange (DeckCardsFrom (cursor != -1 ? cursor - 1 : Size - 1));
			}

			//as if we use deck from the beginning
			res.AddRange (DeckCardsFrom (shownStep - 1).Where (x => !res.Contains (x)));

			return res;
		}
	}

	/// <summary>
	/// Undo deck move.
	/// </summary>
	/// <param name="action">Deck move object.</param>
	public void UndoAction (DeckMove action)
	{			
		cursor = action.CursorState;

		// index of card which will be active after undo action (or -1 if no active cards will be after undo)
		int index1 = action.ShownState.Count > 0 ? Cards.IndexOf (action.ShownState.Last ()) : -1;
		// index of card which is active now, before undo
		int index2 = CurrentActiveCard!=null ? Cards.IndexOf (CurrentActiveCard) : -1; 

		//step is how many cards we must return to deck during undo
		//if step is negative that means abs(step) is how many cards we must pick from deck to flushed pile
		int step = index2 - index1;

		//return current shown cards to deck
		if (shown != null && shown.Count > 0) {
			
			float delay = 0f;
			int count = shown.Count;

			// iterate through current shown cards which need to be returned
			for (int i = count - 1; i >= count - step && i >= 0; i--) {				
				
				Card s = shown [i];

				//stop all animation 
				s.StopAllAnimation ();
				//flip cards to face-down and set valid deck order
				s.SetSide (true, true, delay, baseOrder - i);

				//return card to deck position and order
				if (!GameSettings.Instance.Data.BgAnimEnabled) {
					s.MoveInstantly (slotPosition);
					ReturnCardToDeck (s);
				} else
					s.MoveWithAnimation (slotPosition, CardAnimationSettings.CardRotateDuration, delay, () => {
						ReturnCardToDeck (s);
					});
				
				//increase delay for cascade effect
				delay += CardAnimationSettings.CardRotateDuration;
			}
		}

		//modify number of flips if we undo deck flip
		if (action.CursorState == Size - step) {			
			Flips--;
		}

		//apply saved shown state
		shown = action.ShownState;	

		//update cards position and order
		UpdateFlushed (DeckEnded, DeckEnded ? (shownMaxCount * CardAnimationSettings.CardRotateDuration) / FlushedSize : 0f);
		UpdateShown (true, false);
	}

	/// <summary>
	/// Gets or sets the layer of deck.
	/// </summary>
	/// <value>The layer index.</value>
	public int Layer {
		get{ return slotObject.layer; }
		set { 
			slotObject.layer = value;
			mainSprite.gameObject.layer = value;
			glowSprite.gameObject.layer = value;
			iconRenderer.gameObject.layer = value;
		}
	}

	/// <summary>
	/// Gets a value indicating whether deck is ended.
	/// </summary>
	/// <value><c>true</c> if deck ended; otherwise, <c>false</c>.</value>
	public bool DeckEnded {
		get { 
			return cursor == -1;
		}
	}

	/// <summary>
	/// Gets a value indicating whether this deck can be flipped once more.
	/// </summary>
	/// <value><c>true</c> if deck can be flipped; otherwise, <c>false</c>.</value>
	public bool CanBeFlipped {
		get { 
			return Size > 0 && (!GameSettings.Instance.Data.ThreeFlips || Flips < 3);
		}
	}

	/// <summary>
	/// Gets the count of cards which were flushed from the deck
	/// </summary>
	/// <value>The size of the flushed.</value>
	private int FlushedSize {
		get { 
			return Mathf.Max (0, (DeckEnded ? Size : cursor) - (shownMaxCount - 1) - 1);
		}
	}

	/// <summary>
	/// Updates flushed cards. Their position, sorting order and components activity.
	/// </summary>
	/// <param name="withAnimation">If set to <c>true</c> use animation.</param>
	/// <param name="delay">Delay in seconds.</param>
	public void UpdateFlushed (bool withAnimation = true, float delay = 0)
	{		
		
		if (!GameSettings.Instance.Data.BgAnimEnabled)
			withAnimation = false;		

		Card c;
		int size = FlushedSize;
		float d = 0f;

		//iterate through all flushed cards
		for (int i = 0; i < size; i++) {	
			c = Cards [i];

			//calculate position of flushed pile
			Vector3 pos = FlippedCardPosition (shown.Count - 1);				
			 
			//move to new position with animation or not
			if (withAnimation)
				c.MoveWithAnimation (pos,
					CardAnimationSettings.CardMoveDuration,
					d			
				);
			else
				c.MoveInstantly (pos);
		
			d += delay;			

			//set card order to keep every sprite on its layer
			c.SetSortingOrder (baseOrder + i);
			//turn card face-down
			c.SetSide (false);
			//deactivate the card
			c.Active = false;
			//enable shadow renderer only on top card to save some GPU on transparent rendering
			c.ShadowRendererEnabled = i == size - 1;
			//disable useless at the moment component
			c.GlowRendererEnabled = false;
		}
	}

	/// <summary>
	/// Calculates position of flipped deck card depending on its index
	/// </summary>
	/// <returns>The card position.</returns>
	/// <param name="cardIndex">Card index.</param>
	private Vector3 FlippedCardPosition (int cardIndex)
	{

		if (UILayout.Instance.LayoutType == CardLayout.Vertical)
			return new Vector3 (DeckXPos (cardIndex), slotPosition.y, slotPosition.z);
		else
			return new Vector3 (slotPosition.x, DeckYPos (cardIndex), slotPosition.z);			
	}

	/// <summary>
	/// Returns the card to main deck. Updates its order and state according to its index.
	/// </summary>
	/// <param name="c">Card.</param>
	/// <param name="index">Index.</param>
	/// <param name="rotate">If set to <c>true</c> rotate.</param>
	private void ReturnCardToDeck (Card c, int index = -1, bool rotate = false)
	{
		
		if (index == -1)
			index = Cards.IndexOf (c);		
		
		c.SetSortingOrder (baseOrder - index);
		c.SetSide (true);
		c.gameObject.SetActive (true);
		c.ShadowRendererEnabled = index == Size - 1;

	}

	/// <summary>
	/// Returns list of deck cards starting from given index.
	/// </summary>
	/// <returns>Card list.</returns>
	/// <param name="from">From index.</param>
	private List<Card> DeckCardsFrom (int from)
	{		
		List<Card> res = new List<Card> ();
		int s = Size;	

		for (int i = from; i < s; i = i + shownStep) {	
			if (!res.Contains (Cards [i]))
				res.Add (Cards [i]);			
		}

		if (!res.Contains (Cards [s - 1])) {
			res.Add (Cards [s - 1]);
		}

		return res;
	}

	/// <summary>
	/// Updates the shown cards.
	/// </summary>
	/// <param name="backwards">If set to <c>true</c> use backwards animation.</param>
	/// <param name="withRotation">If set to <c>true</c> use rotation.</param>
	/// <param name="withAnimation">If set to <c>true</c> use animation.</param>
	public void UpdateShown (bool backwards = false, bool withRotation = false, bool withAnimation = true)
	{
		if (!GameSettings.Instance.Data.BgAnimEnabled)
			withAnimation = false;
		
		Card c;
		float delay = 0f;
		int size = shown.Count;

		if (size == 0)
			return;

		int start = size;
		int end = 0;
		int step = -1;

		while (start != end) {	
			//calculate animation delay
			delay = (backwards ? ((size - start + shownMaxCount)) : start) * CardAnimationSettings.CardRotateDuration;		
			//get current card
			c = shown [start - 1];

			//stop all animation if there are some
			c.StopAllAnimation ();

			//calculate shown card position according to its index and layout type
			Vector3 pos = FlippedCardPosition (shown.Count - start);					

			//move card to new position
			if (withAnimation) {
				c.MoveWithAnimation (pos,
					CardAnimationSettings.CardMoveDuration,
					delay);			
			} else
				c.MoveInstantly (pos);

			//modify card order 
			int order = baseOrder + (DeckEnded ? Size : cursor) + (start + 1) * c.LayerCount;

			c.SetSortingOrder (order);
			//set card face up
			c.SetSide (false, withRotation, delay);
			//enable shadow renderer
			c.ShadowRendererEnabled = true;
			//activate object itself
			c.gameObject.SetActive (true);		

			//enable collider only for last shown card (active card)
			c.Active = (start == shown.Count);

			start += step;
		}

		//update deck sprite
		if (CanBeFlipped)
			iconRenderer.sprite = ThemeManager.RecycleSprite;
		else
			iconRenderer.sprite = ThemeManager.StopSprite;	

		//show icon sprite only if deck is ended 
		iconRenderer.gameObject.SetActive (DeckEnded && Size > 0);
	}

	/// <summary>
	/// Y position offset of deck card with given index.
	/// </summary>
	/// <returns>The Y position.</returns>
	/// <param name="index">Card index.</param>
	private float DeckYPos (int index)
	{
		return slotPosition.y + ThemeManager.CardSize.y + (ThemeManager.CardSize.y * offsetY) + index * (ThemeManager.CardSize.y / UILayout.FACE_OFFSET_Y);
	}

	/// <summary>
	/// X position offset of deck card with given index.
	/// </summary>
	/// <returns>The X position.</returns>
	/// <param name="index">Card index.</param>
	private float DeckXPos (int index)
	{
		return slotPosition.x - ThemeManager.CardSize.x - (ThemeManager.CardSize.x * offsetX) - index * (ThemeManager.CardSize.x / UILayout.FACE_OFFSET_X);
	}

	/// <summary>
	/// Gets a value indicating whether this deck in animation
	/// </summary>
	/// <value><c>true</c> if deck in animation; otherwise, <c>false</c>.</value>
	private bool InAnimation {
		get {
			foreach (var c in Cards) {
				if (c.InAnimation)
					return true;
			}
			return false;
		}
	}

	/// <summary>
	/// Gets the index of the next deck card to show.
	/// </summary>
	/// <value>The cursor.</value>
	public int Cursor {
		get { return cursor; }
	}

	#region Slot

	/// <summary>
	/// Gets the deck state .
	/// </summary>
	/// <returns>The state of this deck.</returns>
	public override SlotState GetState ()
	{
		//store all valuable fields and card states for serialization
		SlotState res = new SlotState ();
		res.SlotName = Name;
		res.AddField ("cursor", cursor);
		res.AddField ("shownStep", shownStep);

		res.AddField ("flips", Flips);
		res.AddField ("shownMaxCount", shownMaxCount);
		res.AddField ("flushedCards", FlushedSize);
		res.AddField ("shownIndexes", String.Join (shownSeparator.ToString (), shown.Select (x => x.Index.ToString ()).ToArray ()));

		if (Cards.Count > 0) {	
			res.CardIndexes = new int[Cards.Count];
			for (int i = 0; i < res.CardIndexes.Length; i++) {
				res.CardIndexes [i] = Cards [i].Index;
			}
		}

		return res;
	}

	/// <summary>
	/// Applies given slotState to deck
	/// <returns>True if success</returns>
	/// <param name="state">Slot state.</param>
	public override bool SetState (SlotState state)
	{		
		
		Cards.Clear ();
		var cards = KlondikeController.Instance.shuffleManager.CardIndexesToRefs (state.CardIndexes);

		if (state.Fields.ContainsKey ("cursor"))
			cursor = (int)state.Fields ["cursor"];

		if (state.Fields.ContainsKey ("flips"))
			Flips = (int)state.Fields ["flips"];
		
		if (state.Fields.ContainsKey ("shownStep"))
			shownStep = (int)state.Fields ["shownStep"];
		
		if (state.Fields.ContainsKey ("shownMaxCount"))
			shownMaxCount = (int)state.Fields ["shownMaxCount"];
		
		string shownIndexesStr = string.Empty;
		if (state.Fields.ContainsKey ("shownIndexes"))
			shownIndexesStr = (string)state.Fields ["shownIndexes"];
		
		if (shownIndexesStr != string.Empty) {
			int[] shownIndexes = shownIndexesStr.Split (shownSeparator).Select (x => int.Parse (x)).ToArray ();
			shown = KlondikeController.Instance.shuffleManager.CardIndexesToRefs (shownIndexes).ToList ();
		}

		foreach (var c in cards)
			AddCard (c);

		UpdateFlushed (false);
		UpdateShown (false, false, false);
		
		return true;
	}

	/// <summary>
	/// Plays the hint animation.
	/// </summary>
	/// <param name="c">Card.</param>
	public override void PlayHintAnimation (CardGroup c)
	{		
		
		if (c == null || shown.Count < 1 || shown [shown.Count - 1] != c.Root) {
			base.PlayHintAnimation (c);
		} else {			
			shown [shown.Count - 1].PlayHintAnimation ();
		}
	}

	/// <summary>
	/// Gets or sets the position of this deck.
	/// </summary>
	/// <value>The position.</value>
	public override Vector3 Position {
		set { 
			bool changed = base.Position != value;
			base.Position = value;

			for (int i = 0; i < Size; i++) {				
				Cards [i].MoveInstantly (slotPosition);
			}

			//update positions only if it needed
			if (changed) {				
				UpdateFlushed (false);
				UpdateShown (false, false, false);
			}
		}
	}

	/// <summary>
	/// If this deck contains given card
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	/// <param name="c">Card</param>
	public override bool ContainsCard (Card c)
	{		
		return AvailableCards.Contains (c);		
	}

	/// <summary>
	/// Determines whether given group can be picked from this deck.
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	/// <param name="cg">Card group.</param>
	public override bool CanBePicked (CardGroup cg)
	{		
		return ContainsCard (cg.Root);
	}

	/// <summary>
	/// Gets the name of this deck.
	/// </summary>
	/// <value>The name.</value>
	public override string Name {
		get { 
			return "KlondikeDeck";
		}
	}

	/// <summary>
	/// Clear this deck from cards.
	/// </summary>
	public override void Clear ()
	{
		base.Clear ();
		cursor = 0;	
		Flips = 0;
		shown.Clear ();
		iconRenderer.gameObject.SetActive (false);
	}

	/// <summary>
	/// Removes the card from deck.
	/// </summary>
	/// <param name="c">Card.</param>
	public override void RemoveCard (Card c)
	{
		if (shown.Contains (c)) {

			shown.Remove (c);

			//return one card from flushed pile if wee can
			if (FlushedSize > 0) {								
				shown.Insert (0, Cards [FlushedSize - 1]);
			}

			//modify index of next deck card
			if (cursor > 0)
				cursor--;

			//update shown cards positions
			UpdateShown (false, false, GameSettings.Instance.Data.BgAnimEnabled);			
		}

		base.RemoveCard (c);
	}

	/// <summary>
	/// Adds the card to deck.
	/// </summary>
	/// <param name="c">Card.</param>
	/// <param name="withAnimation">If set to <c>true</c> with animation.</param>
	/// <param name="delay">Delay.</param>
	public override void AddCard (Card c, bool withAnimation = false, float delay = 0f)
	{
		c.ColliderH = ThemeManager.CardSize.y;

		//you can only add card to deck in two situations:
		// * during game restart (dealing)
		// * during undo deck move

		//if deck isn't ready that means that we add card during dealing process
		if (!IsReady) {
			base.AddCard (c);
			int size = Size;
			c.MoveInstantly (slotPosition);
			c.SetSide (true);
			c.SetSortingOrder (baseOrder - size);

			//only first card shadow is enabled to prevent sprite overlaping
			if (size > 1) {
				Cards [size - 2].ShadowRendererEnabled = false;
			}

			c.ShadowRendererEnabled = true;
		} else {		
			//if deck is ready and we try to add card to it that means that is a undo action


			//add card to the end 
			int place = DeckEnded ? Size : cursor; 	
			Cards.Insert (place, c);

			//modify cursor
			if (cursor != -1)
				cursor++;

			//if shown already reached max number of values
			//we remove one card from it to flushed
			if (shown.Count == shownMaxCount) {
				shown [0].SetSortingOrder (baseOrder + FlushedSize);
				shown.RemoveAt (0);
			}

			shown.Add (c);
			UpdateShown ();
			UpdateFlushed ();
		}
	}

	#endregion

}


