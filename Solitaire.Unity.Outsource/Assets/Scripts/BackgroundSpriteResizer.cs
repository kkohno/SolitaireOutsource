using ui;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class makes sure that background is fit to the screen and uses valid images for current orientation
/// </summary>
public class BackgroundSpriteResizer : MonoBehaviour, IPointerClickHandler
{

	public MovingElement botPanel;

	/// <summary>
	/// Image that shows state of bottom panel with buttons
	/// </summary>
	public GameObject expandIcon;

	/// <summary>
	/// Sprite of left side of the background.
	/// </summary>
	public SpriteRenderer leftSide;

	/// <summary>
	/// Sprite of right side of the background.
	/// </summary>
	public SpriteRenderer rightSide;

	/// <summary>
	/// Handles the pointer click event on this object.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public void OnPointerClick (PointerEventData eventData)
	{		
		// each click to background hides or shows bottom panel
		botPanel.Shown = !botPanel.Shown;

		// we show expand icon only if botom pannel is hidden
		// to let user know that there is additional buttons hidden on the bottom of the screen
		expandIcon.SetActive (!expandIcon.activeSelf);
	}

	void Awake ()
	{

		// subscribe to background loading event
		// to change background sprites when user changes background theme
		ThemeManager.BackgroundLoaded += OnBgChanged;
	}

	/// <summary>
	/// Resize background images to fit the whole screen of given size
	/// </summary>
	/// <param name="size">Current size of screen.</param>
	public void RefreshLayout (Vector2 size)
	{		

		// apply current sprite images according to orientation and theme
		leftSide.sprite = rightSide.sprite = CurrentSprite;

		//calculate half of screen width
		float halfW = size.x / 2f;

		//fit left side of bg to 100% of screen height and 50% of screen width
		leftSide.transform.localScale = new Vector3 (leftSide.transform.localScale.x * (halfW + 0.01f) / leftSide.bounds.size.x,
			leftSide.transform.localScale.y * size.y / leftSide.bounds.size.y, 
			1f);

		//fit right side as mirror reflection of left side
		rightSide.transform.localScale = new Vector3 (-leftSide.transform.localScale.x, leftSide.transform.localScale.y, leftSide.transform.localScale.z);

		// place bg sprites on the center of the screen
		leftSide.transform.localPosition = new Vector3 (-halfW / 2f, 0f, 0f);
		rightSide.transform.localPosition = new Vector3 (halfW / 2f, 0f, 0f);

		//fit collider to part of screen
		//collider is needed to handle click events

		float ratio = 2.5f;
		size.y = size.y / ratio;
		float offset = (ratio - 1) / 2f;

		GetComponent<BoxCollider2D> ().size = size;
		GetComponent<BoxCollider2D> ().offset = new Vector2 (0f, -size.y * offset);
	}

	/// <summary>
	/// Handles the background changed event.
	/// </summary>
	void OnBgChanged ()
	{		
		//apply new images to background sprites
		leftSide.sprite = rightSide.sprite = CurrentSprite;

		// calculate average color of current bg image and set this color as camera clear color
		// this makes transitions to look more smooth (without black regions)
		Camera.main.backgroundColor = ThemeManager.TextureAvgColor (leftSide.sprite.texture);
	}

	/// <summary>
	/// Gets the current background sprite valid for current orientation.
	/// </summary>
	/// <value>The current background sprite.</value>
	private Sprite CurrentSprite {		
		get { 			
			return UILayout.Instance.LayoutType == CardLayout.Vertical ? ThemeManager.BGSpriteVert : ThemeManager.BGSpriteHor;
		}
	}
}
