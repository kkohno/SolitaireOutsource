using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using LitJson;

/// <summary>
/// JSON state manager allow  to save and load game state using JSON serialization.
/// </summary>
public class JSONStateManager : StateManager
{

	string fileName;
	bool jsonLoaded;

	public JSONStateManager (KlondikeController controller) : base (controller)
	{
		
	}

	#region implemented abstract members of StateManager

	public override bool Write ()
	{			
		StringBuilder sb = new StringBuilder ();
		JsonWriter writer = new JsonWriter (sb);
		writer.PrettyPrint = false;

		try {
		
			writer.WriteObjectStart ();
			writer.WritePropertyName ("deckType");
			writer.Write ((int)Type);
			writer.WritePropertyName ("scoreType");
			writer.Write ((int)ScoreType);

			if (Shuffle != null) {
				writer.WritePropertyName ("shuffle");
				writer.Write (JsonMapper.ToJson (Shuffle));
			}

			if (CardStates != null) {
				writer.WritePropertyName ("isUp");
				writer.Write (JsonMapper.ToJson (CardStates));
			}

			writer.WritePropertyName ("score");
			writer.Write (Score);
	
			writer.WritePropertyName ("timeInSeconds");
			writer.Write (Time);

			writer.WritePropertyName ("statesArray");
			writer.WriteArrayStart ();

			foreach (var s in slotStates) {			
				writer.Write (JsonMapper.ToJson (s));
			}
			writer.WriteArrayEnd ();

			writer.WritePropertyName ("history");
			writer.WriteArrayStart ();

			if (History != null) {			
				foreach (var h in History) {	
					writer.Write (h.ToJSON ());
				}
			}

			writer.WriteArrayEnd ();

			writer.WriteObjectEnd ();

			System.IO.File.WriteAllText (fileName, sb.ToString ());

			KlondikeController.Instance.msgHandler.AddMsg ("State saved " + fileName);
			return true;
		} catch {
			return false;
		}
	}

	public override void ClearStorage ()
	{
		if (System.IO.File.Exists (fileName))
			System.IO.File.Delete (fileName);
	}

	public override async Task Read ()
    {

        if (!System.IO.File.Exists(fileName))
            return;
		
		try {
			string json = System.IO.File.ReadAllText (fileName);
			JsonReader reader = new JsonReader (json);
			JsonData data = JsonMapper.ToObject (reader);

	
			string tmp;
			if (GetJSONStr (data, "shuffle", out tmp)) {
			
				Shuffle = JsonMapper.ToObject<int[]> (tmp);
			}

			if (GetJSONStr (data, "isUp", out tmp)) {
				CardStates = JsonMapper.ToObject<int[]> (tmp);
			}
	
			int res;
			if (GetJSONInt (data, "deckType", out res)) {
				Type = (GameType)res;
			}	

			if (GetJSONInt (data, "scoreType", out res)) {
				ScoreType = (ScoreType)res;
			}	

			if (GetJSONInt (data, "score", out res)) {
				Score = res;
			}

			if (GetJSONInt (data, "timeInSeconds", out res)) {
				Time = res;
			}	

			JsonData array = data ["statesArray"];
			if (array != null && array.IsArray) {			
				for (int i = 0; i < array.Count; i++) {
					AddSlotState (JsonMapper.ToObject<SlotState> (array [i].ToString ()));
				}
			}		

			if (History == null)
				History = new List<IUndoableUserMove> ();
			else
				History.Clear ();
		
			array = data ["history"];
			if (array != null && array.IsArray) {			
				for (int i = 0; i < array.Count; i++) {				
					History.Add (CreateHistoryEntry (array [i].ToString ()));
				}
			}	
			jsonLoaded = true;

		} catch {
			jsonLoaded = false;
		}
    }

	public override async Task Init ()
	{		
		fileName = System.IO.Path.Combine (Application.persistentDataPath, "states.json");
		jsonLoaded = false;
    }

	/// <summary>
	/// Creates the history undoable move entry using JSON representation of object.
	/// </summary>
	/// <returns>The history entry.</returns>
	/// <param name="json">Json string.</param>
	private IUndoableUserMove CreateHistoryEntry (string json)
	{		
		JsonData data = JsonMapper.ToObject (json.ToString ());
		IUndoableUserMove res = null;
		string type = data ["moveType"].ToString ();
		if (type == "card")
			res = new Move ();
		else if (type == "deck")
			res = new DeckMove ();

		if (res != null)
			res.FromJSON (data);

		return res;
	}

	private bool GetJSONInt (JsonData elem, string prop, out int res)
	{

		if (!elem.Contains (prop) || elem [prop] == null || !elem [prop].IsInt) {
			res = 0;
			return false;
		}

		return int.TryParse (elem [prop].ToString (), out res);
	}

	private bool GetJSONStr (JsonData elem, string prop, out string res)
	{
		if (!elem.Contains (prop) || elem [prop] == null) {
			res = null;
			return false;
		}
		res = elem [prop].ToString ();	

		return !(res == null || res == string.Empty);
	}

	public override bool HasState {
		get {
			return jsonLoaded;
		}
	}

	#endregion




}
