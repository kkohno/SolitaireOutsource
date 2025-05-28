using LitJson;

/// <summary>
/// Interface for user game move, which can be undone.
/// </summary>
public interface IUndoableUserMove
{
	void Undo ();

	bool CanUndo ();

	int Price{ get; }

	string ToJSON ();

	void FromJSON (JsonData data);

	string AsString{ get; }
}


