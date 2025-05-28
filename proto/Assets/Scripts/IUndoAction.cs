using System;


public interface IUndoAction
{	
	void Debug();
	void Undo ();
	bool CanUndo ();
}


