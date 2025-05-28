using System.Collections.Generic;
using System.Text;
using LitJson;
using System.Linq;

/// <summary>
/// Class represents deck move (click on deck)
/// </summary>
public class DeckMove: IUndoableUserMove
{
	public int CursorState { get; set; }

	public List<Card> ShownState { get; set; }

	#region IUndoAction implementation

	private int[] showStateIndexes;

	public string AsString {
		get {
			return "ShowDeck";
		}
	}

	public int Price {
		get;
		private set;
	}

	public DeckMove ()
	{
		Price = 0;
	}

	public DeckMove (int score)
	{
		Price = -score;
	}

	/// <summary>
	/// Initialize object from JSON data
	/// </summary>
	/// <param name="data">Data.</param>
	public void FromJSON (JsonData data)
	{				
		CursorState = (int)data ["cursor"];
		showStateIndexes = JsonMapper.ToObject<int[]> (data ["shown"].ToJson ());
	}

	/// <summary>
	/// Serialize move to JSON string
	/// </summary>
	/// <returns>The JSON representation of the object.</returns>
	public string ToJSON ()
	{
		FormFromData ();

		StringBuilder sb = new StringBuilder ();
		JsonWriter writer = new JsonWriter (sb);

		writer.WriteObjectStart ();

		writer.WritePropertyName ("moveType");
		writer.Write ("deck");	

		writer.WritePropertyName ("cursor");
		writer.Write (CursorState);

		writer.WritePropertyName ("shown");
		writer.WriteArrayStart ();

		for (int i = 0; i < ShownState.Count; i++) {			
			writer.Write (ShownState [i].Index);
		}

		writer.WriteArrayEnd ();
	
		writer.WriteObjectEnd ();

		return sb.ToString ();
	}

	/// <summary>
	/// Initialize object from current game data
	/// </summary>
	private void FormFromData ()
	{
		if (showStateIndexes != null) {
			ShownState = KlondikeController.Instance.shuffleManager.CardIndexesToRefs (showStateIndexes).ToList ();
			showStateIndexes = null;
		}
	}

	/// <summary>
	/// Undo this move
	/// </summary>
	public void Undo ()
	{
		FormFromData ();		
		KlondikeController.Instance.Deck.UndoAction (this);
	}

	/// <summary>
	/// Determines whether this move can be undone.
	/// </summary>
	/// <returns><c>true</c> if this this move can be undone; otherwise, <c>false</c>.</returns>
	public bool CanUndo ()
	{
		return true;
	}

	#endregion
}


