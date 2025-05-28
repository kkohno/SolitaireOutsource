using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public enum DialogType{
	YesNo = 0,
	Ok
}

public class DialogController : MonoBehaviour {
	
	public Image[] bg;
	public Image yes;
	public Image no;

	public Text message;

	private Tweener s_animation;
	private Vector3 originalScale;
	private Action<bool> exitAction;
	private bool isReady;
	private DialogType type;
	private const float animSped = 0.4f;
	private const float duration = 2f;
	private Shadow txtShadow;
	private bool shown;

	public bool Ready{
		get{ return isReady;}
	}

	public void Show(string text, DialogType type, Action<bool> act=null){		
		if (shown)
			return;
		
		shown = true;
		this.type = type;	

		if(s_animation!=null)			
			s_animation.Kill ();
		
		Color c = Color.white;
		Color shadowColor;
		c.a = 0f;
		gameObject.SetActive (true);
		exitAction = act;	
		no.gameObject.SetActive(type.Equals(DialogType.YesNo));
		yes.gameObject.SetActive(type.Equals(DialogType.YesNo));
		yes.color = no.color = c;

		/*if (txtShadow == null)
			txtShadow = message.GetComponent<Shadow> ();

		shadowColor = txtShadow.effectColor;*/
			
		message.ChangeAlpha (0f);
		message.gameObject.SetActive (true);
		message.text = text;

		//Image curImage = null;

		for(int i = 0; i < bg.Length; i++){
			if ((int)type == i) {
				bg [i].gameObject.SetActive (true);
				//curImage = bg [i].GetComponent<Image> ();
				//curImage.color = c;				
			} else {
				bg [i].gameObject.SetActive (false);
			}
		}

		s_animation = DOTween.To (() => c.a, (x) => {	
			
			c.a = x;
			shadowColor.a = x;

			/*if(curImage!=null)
				curImage.color = c;	*/
			
				message.color = c;
			/*
				txtShadow.effectColor = shadowColor;*/

			if(type == DialogType.YesNo){
				yes.color = no.color = c;
			}

		}, 1f,animSped).OnComplete(()=>{
			
			isReady = true;
			if(type == DialogType.Ok){				
				StartCoroutine(CoroutineExtension.ExecuteAfterTime(duration, ()=>{
					Hide();
				}));
			}

		});

		s_animation.Play ();	
	}

	public void Hide(){
		
		if (!shown)
			return;
		
		shown = false;
		Color c = Color.white;
		Color shadowColor;
		c.a = 1f;

		if(s_animation != null)
			s_animation.Kill ();
		
		//Image curImage = bg [(int)type];

		//curImage.color = c;

		/*if (txtShadow == null)
			txtShadow = message.GetComponent<Shadow> ();

		shadowColor = txtShadow.effectColor;*/

		s_animation = DOTween.To (() => c.a, (x) => {			
			c.a = x;
			shadowColor.a = x;
			//curImage.color = c;	
			message.color = c;
			//txtShadow.effectColor = shadowColor;
			if(type == DialogType.YesNo){
				yes.color = no.color = c;
			}
		}, 0f,animSped).OnComplete(()=>{	
			isReady = false;		
			/*curImage.gameObject.SetActive(false);*/
			if(type == DialogType.YesNo){
				yes.gameObject.SetActive(false);
				no.gameObject.SetActive(false);
			}
			gameObject.SetActive (false);
		});
		s_animation.Play ();
	}

	public void Finish(bool arg){
		if (!isReady)
			return;

		Hide ();
		
		if (exitAction != null)
			exitAction (arg);	
	}
}
