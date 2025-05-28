using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ui
{
	/// <summary>
	/// Custom button with additional animations and events.
	/// </summary>
	public class CustomButton : Button
	{
	
		public UnityEvent OnPress;
		public bool scaleOnClick;
		private Tweener t_scale;

		public override void OnPointerDown (PointerEventData eventData)
		{
			if (!interactable)
				return;

			//notify subscribers about onPress event
			if (OnPress != null)
				OnPress.Invoke ();

			//is animation enabled we must create it
			if (scaleOnClick) {
			
				if (t_scale == null) {
					//scale pressed button
					t_scale = targetGraphic.gameObject.transform.DOScale (Vector2.one * 1.2f, 0.1f);
					t_scale.SetAutoKill (false);
				} else
					t_scale.Rewind ();
			

				t_scale.PlayForward ();
			}
		}

		public override void OnPointerUp (PointerEventData eventData)
		{
		
			if (!interactable)
				return;

			if (scaleOnClick) {
				if (t_scale == null)
					return;
				else
					t_scale.Complete ();

				//play scale animation backwards when user released a buttons
				t_scale.PlayBackwards ();
			}

		}
	}
}
