using System.Collections.Generic;

public class SlotState
{
	public Dictionary <string, object> Fields { get; set; }

	public int[] CardIndexes { get; set; }

	public string SlotName{ get; set; }

	public void AddField (string name, object value)
	{
		
		if (Fields == null)
			Fields = new Dictionary<string, object> ();
		
		Fields.Add (name, value);

	}

	public SlotState ()
	{
		
	}
}


