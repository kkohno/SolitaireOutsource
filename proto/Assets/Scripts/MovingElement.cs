using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingElement : MonoBehaviour {

	public Vector2 shownPos;
	public Vector2 hidenPos;
	public bool DisableOnHide = true;
	[Range(0.1f,3f)]
	public float AnimationTime = 1f;

	private Tweener t_move;
	private RectTransform rt;
	private bool state;

	public void Switch(){
		Shown = !Shown;
	}

	public bool Shown{
		set{
			if (state == value) {				
				return;
			}

			state = value;

			if (t_move != null && t_move.IsActive()) {
				t_move.Pause ();
				t_move.Kill ();
			}

			if (rt == null)
				rt = gameObject.GetComponent<RectTransform> ();
			
			if(state)
				gameObject.SetActive(true);

			t_move = DOTween.To (() => rt.anchoredPosition, (v) => rt.anchoredPosition = v, state ? shownPos : hidenPos, AnimationTime).OnComplete(()=>{
				if(!state && DisableOnHide)
					gameObject.SetActive(false);				
			});

			//TODO set active  = false когда элемент за пределами видимой области экрана
			t_move.Play ();
		}

		get{ return state;}
	}
}
