using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckController : MonoBehaviour, IPointerClickHandler
{
	

	public delegate void Clicked ();
	public event Clicked DeckClicked;

	public DeckController ()
	{
	}

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		if (DeckClicked != null)
			DeckClicked ();
	}

	#endregion
}


