using UnityEngine;
using UnityEngine.UI;

public class ToastMessage : MonoBehaviour {

	public Text text;
	private Image img;
	private float ratio;
	private float speed=1f;
	private RectTransform rt;
	void Start(){
		
	}

	public void Show(string msg, float initialValue = 1f){	
		if(img == null)	
			img = gameObject.GetComponent<Image> ();
		
		
		ratio = initialValue;
		text.ChangeAlpha(ratio);
		img.ChangeAlpha (ratio);
		text.text = msg;	

		gameObject.SetActive (true);
	}

	public void SetPosition(RectTransform dest){
		if (rt == null)
			rt = gameObject.GetComponent<RectTransform> ();
		
		rt.anchoredPosition = new Vector2 (0, CameraUtilities.switchToRectTransform(dest,rt).y);
	}



	public void DefaultPosition (){
		if (rt == null)
			rt = gameObject.GetComponent<RectTransform> ();
		
		rt.anchoredPosition = new Vector2 (0, 100);
	}

	void Update(){
		ratio -= speed * Time.deltaTime;

		img.ChangeAlpha (ratio);
		text.ChangeAlpha (ratio);

		if (ratio < 0.01f) {			
			gameObject.SetActive (false);
			DefaultPosition ();
		}
	}

}
