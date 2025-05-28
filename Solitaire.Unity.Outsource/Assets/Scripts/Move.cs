using System.Text;
using LitJson;
using ui.windows;

/// <summary>
/// Move placeholder.
/// </summary>
public class Move: IUndoableUserMove
{
	public CardGroup Cards { get; private set; }

	public Slot From { get; private set; }

	public Slot To { get; private set; }

	public bool ShowNewCard{ get { return showNewCard; } }

	private int score;
	private bool showNewCard;
	private bool wellFormed = false;

	public Move ()
	{
		
	}

	public Move (Slot from, CardGroup card, Slot to)
	{
		if (card == null) {			
			return;
		}
		
		From = from;
			
		Cards = card;
		
		To = to;

		int index = From.Cards.IndexOf (Cards.Root);

		showNewCard = index > 0 && !From.Cards [index - 1].IsUp;

		score = CalculateScore ();

		wellFormed = true;
	}

	/// <summary>
	/// Calculates the score of this move.
	/// </summary>
	/// <returns>The score of move.</returns>
	private int CalculateScore ()
	{
		if (GameSettings.Instance.Data.ScoreType == ScoreType.Standart)
			return CalculateStandart ();
		else {
			return CalculateWindows ();
		}
	}

	/// <summary>
	/// Calculates the score using standart rules.
	/// </summary>
	/// <returns>Score.</returns>
	private int CalculateStandart ()
	{
		int res = 0;
		if (To.GetType () == typeof(KlondikeSlot) && From.GetType () != typeof(KlondikeSlot))
			res += 10;

		if (From.GetType () == typeof(KlondikeSlot) && To.GetType () != typeof(KlondikeSlot))
			res += -10;

		if (showNewCard)
			res += 5;

		if (From.GetType () == typeof(KlondikeDeck) && To.GetType () == typeof(KlondikeCardStack))
			res += 5;	

		return res;
	}

	/// <summary>
	/// Calculates the score using windows-like rules.
	/// </summary>
	/// <returns>Score.</returns>
	private int CalculateWindows ()
	{		
		// http://pcabc.ru/wxp/wxp18.html:	
		int res = 0;

		// +10 - за положенную карту в дом (наверх вправо);
		if (To.GetType () == typeof(KlondikeSlot) && From.GetType () != typeof(KlondikeSlot))
			res += 10;

		// +5 - добавленную в ряд карт из колоды;
		if (To.GetType () == typeof(KlondikeCardStack) && From.GetType () == typeof(KlondikeDeck))
			res += 5;

		// +5 - за перевернутую карту и за карту
		if (showNewCard)
			res += 5;

		//-15 - за карту, взятую из дома;
		if (From.GetType () == typeof(KlondikeSlot) && To.GetType () != typeof(KlondikeSlot))
			res -= 15;

		return res;
	}

	public override string ToString ()
	{
		return string.Format ("[Move: Cards={0}, From={1}, To={2}, Score={3}, Price={4}]", Cards.Root.name + (Cards.Size > 1 ? string.Format ("(and {0} more)", Cards.Size - 1) : string.Empty), From.Name, To.Name, Score, Price);
	}

	/// <summary>
	/// Gets a value indicating whether this move is possible.
	/// </summary>
	/// <value><c>true</c> if this move is possible; otherwise, <c>false</c>.</value>
	public bool IsPossible {
		get {			
			return  From.CanBePicked (Cards) && To.CanPlaceGroup (Cards);
		}
	}

	/// <summary>
	/// Gets the score of this move.
	/// </summary>
	/// <value>The score.</value>
	public int Score {
		get {
			return score;
		}
	}

	/// <summary>
	/// Refreshes the cards of this move.
	/// </summary>
	public void RefreshCards ()
	{
		//if card group of this move changed we need apply this changes
		Cards = From.GetGroup (Cards.Root);
	}

	#region IUndoAction implementation

	/// <summary>
	/// Gets string representaton of this move.
	/// </summary>
	/// <value>As string.</value>
	public string AsString {
		get {
			return this.ToString ();
		}
	}

	int rootId;
	int[] elemIds;
	string fromName;
	string toName;

	/// <summary>
	/// Forms move object from game data. Used for save game.
	/// </summary>
	private void FormFromData ()
	{

		Cards = new CardGroup (KlondikeController.Instance.shuffleManager.CardIndexToRef (rootId));
		for (int i = 0; i < elemIds.Length; i++) {
			Cards.AddElement (KlondikeController.Instance.shuffleManager.CardIndexToRef (elemIds [i]));
		}	

		From = KlondikeController.Instance.Slots [fromName];
		To = KlondikeController.Instance.Slots [toName];

		wellFormed = true;
	}

	/// <summary>
	/// Forms move object from JSON data. Used for load game.
	/// </summary>
	/// <param name="data">Data.</param>
	public void FromJSON (JsonData data)
	{	
		score = (int)data ["score"];
		showNewCard = (bool)data ["showNewCard"];

		rootId = (int)data ["mainCard"];
		elemIds = JsonMapper.ToObject<int[]> (data ["elements"].ToJson ());
		fromName = data ["fromSlot"].ToString ();
		toName = data ["toSlot"].ToString ();
	}

	/// <summary>
	/// Serialize this move to JSON string
	/// </summary>
	/// <returns>The JSON string.</returns>
	public string ToJSON ()
	{
		if (!wellFormed)
			FormFromData ();
		
		StringBuilder sb = new StringBuilder ();
		JsonWriter writer = new JsonWriter (sb);

		writer.WriteObjectStart ();

		writer.WritePropertyName ("moveType");
		writer.Write ("card");

		writer.WritePropertyName ("mainCard");
		writer.Write (Cards.Root.Index);

		writer.WritePropertyName ("elements");
		writer.WriteArrayStart ();
		foreach (var c in Cards.Elements) {
			writer.Write (c.Index);
		}			
		writer.WriteArrayEnd ();

		writer.WritePropertyName ("fromSlot");
		writer.Write (From.Name);

		writer.WritePropertyName ("toSlot");
		writer.Write (To.Name);

		writer.WritePropertyName ("score");
		writer.Write (score);

		writer.WritePropertyName ("showNewCard");
		writer.Write (showNewCard);

		writer.WriteObjectEnd ();

		return sb.ToString ();
	}

	/// <summary>
	/// Gets the price of this move. How much score costs to undo this move.
	/// </summary>
	/// <value>The price.</value>
	public int Price {
		get {
			return -score;
		}
	}

	/// <summary>
	/// Undo this move.
	/// </summary>
	public void Undo ()
	{			
		if (!wellFormed)
			FormFromData ();		

		To.RemoveGroup (Cards);

		if (showNewCard) {
			From.Head.SetSide (true);
			From.Head.ShadowRendererEnabled = From.Size == 1;
		}
		
		From.UpdateOrder ();
		From.PlaceGroup (Cards, true);
	}

	/// <summary>
	/// Determines whether this move can be undone.
	/// </summary>
	/// <returns><c>true</c> if this move can be undone; otherwise, <c>false</c>.</returns>
	public bool CanUndo ()
	{
		return true;
	}

	#endregion
}


