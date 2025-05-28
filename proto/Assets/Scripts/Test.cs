using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour, IPointerDownHandler, IPointerClickHandler {
	
	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		Debug.Log ("test click1");
	}

	#endregion

	#region IPointerDownHandler implementation

	public void OnPointerDown (PointerEventData eventData)
	{
		Debug.Log ("test click2");
	}

	#endregion




}
