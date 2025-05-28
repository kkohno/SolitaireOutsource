using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PreviewImageController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
	public Camera uiCamera;
	public Canvas canvas;
	public Image cursorBorder;
	private bool isPressed;
	private Vector3 prevBorderPos;
	private Vector2 borderPressOffset;
	private float borderScale = 1f;

	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
		isPressed = false;
	}

	#endregion

	#region IPointerUpHandler implementation

	public void OnPointerUp (PointerEventData eventData)
	{
		isPressed = false;
	}

	#endregion

	#region IPointerDownHandler implementation

	public void OnPointerDown (PointerEventData eventData)
	{		
		isPressed = true;
		borderPressOffset = cursorBorder.transform.position - GetTouchPosition ();

	}

	#endregion

	/// <summary>
	/// Gets the source image component.
	/// </summary>
	/// <value>The source image.</value>
	public Image SourceImage {
		
		get {
			return GetComponent<Image> ();
		}

	}

	/// <summary>
	/// Places curson in the middle of the source image and resets its scale.
	/// </summary>
	public void Prepare ()
	{
		
		cursorBorder.rectTransform.anchoredPosition = SourceImage.rectTransform.sizeDelta / 2f;
		TryToChangeScale (borderScale = 1f);

	}

	/// <summary>
	/// Tries to change scale.
	/// </summary>
	/// <param name="newValue">New value.</param>
	private void TryToChangeScale (float newValue)
	{

		if (newValue < 1f)
			return;	

		float prevScale = borderScale;
		borderScale = newValue;	

		if (!CheckIfBorderInImage ()) {
			
			borderScale = prevScale;

		} else {
			
			cursorBorder.transform.localScale = Vector2.one * borderScale;

		}

	}

	/// <summary>
	/// Gets the current scale factor.
	/// </summary>
	/// <value>The scale factor.</value>
	public float ScaleFactor {
		
		get {
			
			return borderScale;

		}

	}

	/// <summary>
	/// Gets the size of the selected region in pixels.
	/// </summary>
	/// <value>The size of the region as array of two values. First value is width, second is height</value>
	public int[] RegionSize {
		
		get { 
			
			float rX = cursorBorder.rectTransform.sizeDelta.x * borderScale / SourceImage.rectTransform.sizeDelta.x;
			float rY = cursorBorder.rectTransform.sizeDelta.y * borderScale / SourceImage.rectTransform.sizeDelta.y;
			int[] res = new int[2];
			res [0] = (int)(rX * SourceImage.sprite.texture.width);
			res [1] = (int)(rY * SourceImage.sprite.texture.height);

			return res;
		}

	}

	/// <summary>
	/// Gets the selected region position.
	/// </summary>
	/// <value>The region position as array of two values. First value is X pos, second is Y pos.</value>
	public int[] RegionPosition {
		
		get { 
			
			int[] res = new int[2];
			res [0] = (int)(cursorBorder.rectTransform.anchoredPosition.x / SourceImage.rectTransform.sizeDelta.x * SourceImage.sprite.texture.width) - RegionSize [0] / 2;
			res [1] = (int)(cursorBorder.rectTransform.anchoredPosition.y / SourceImage.rectTransform.sizeDelta.y * SourceImage.sprite.texture.height) - RegionSize [1] / 2;
			return res;

		}

	}

	void Update ()
	{		

		#if UNITY_EDITOR
		if (Input.mouseScrollDelta.y != 0) {		

			TryToChangeScale (borderScale * 1 - Input.mouseScrollDelta.y / 10f);
		}
		#endif

		if (!isPressed)
			return;

		//it two touches - its scale mode
		if (Input.touchCount == 2) {
			
			// Store both touches.
			Touch touchZero = Input.GetTouch (0);
			Touch touchOne = Input.GetTouch (1);

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			TryToChangeScale (borderScale * (1f - deltaMagnitudeDiff / Screen.dpi * 3f));
		
		} else {			

			cursorBorder.transform.position = GetTouchPosition () + new Vector3 (borderPressOffset.x, borderPressOffset.y);

			//if new position is out of bounds - return prev value
			if (!CheckIfBorderInImage ())
				cursorBorder.transform.position = prevBorderPos;

			prevBorderPos = cursorBorder.transform.position;
				
		}
	}

	/// <summary>
	/// Checks if border is in preview image rect.
	/// </summary>
	/// <returns><c>true</c>, if border is in image rect, <c>false</c> otherwise.</returns>
	private bool CheckIfBorderInImage ()
	{

		float borderHalfWidth = cursorBorder.rectTransform.sizeDelta.x / 2f * borderScale;
		float borderHalfHeight = cursorBorder.rectTransform.sizeDelta.y / 2f * borderScale;

		return cursorBorder.rectTransform.anchoredPosition.x >= borderHalfWidth &&
		cursorBorder.rectTransform.anchoredPosition.x <= SourceImage.rectTransform.sizeDelta.x - borderHalfWidth &&
		cursorBorder.rectTransform.anchoredPosition.y >= borderHalfHeight &&
		cursorBorder.rectTransform.anchoredPosition.y <= SourceImage.rectTransform.sizeDelta.y - borderHalfHeight;
		
	}

	/// <summary>
	/// Gets the position of current user touch.
	/// </summary>
	/// <returns>The touch position.</returns>
	private Vector3 GetTouchPosition ()
	{	
		
		Vector3 pos;
		int tCount = Input.touchCount;

		#if UNITY_EDITOR
		pos = uiCamera.ScreenToWorldPoint (Input.mousePosition);
		#else
			if (Input.touchCount < 1)
				return Vector2.zero;	

			pos = uiCamera.ScreenToWorldPoint (Input.touches[tCount-1].position);
		#endif

		pos.z = canvas.planeDistance;
		return pos;

	}

}
