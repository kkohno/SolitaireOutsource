using DG.Tweening;
using UnityEngine;

namespace ui
{
	/// <summary>
	/// Moving element controller which shows and hide gameobject by moving it between specified positions.
	/// </summary>
	public class MovingElement : MonoBehaviour
	{

		public Vector2 shownPos;
		public Vector2 hidenPos;
		public bool DisableOnHide = true;
		[Range (0.1f, 3f)]
		public float AnimationTime = 1f;

		private Tweener t_move;
		private bool state;

		public void Switch ()
		{
			Shown = !Shown;
		}

		public RectTransform Rect {
			get{ return gameObject.GetComponent<RectTransform> (); }
		}

		public bool Shown {
			set {
			
				//if state is already has specified value
				if (state == value) {				
					return;
				}

				state = value;

				//disable and kill previous animation
				if (t_move != null && t_move.IsActive ()) {
					t_move.Pause ();
					t_move.Kill ();
				}

				// enable gameobject if new value is true
				if (state)
					gameObject.SetActive (true);
			
				//create moving animation from one position to another
				//depending on new value
				t_move = DOTween.To (() => Rect.anchoredPosition, (v) => Rect.anchoredPosition = v, state ? shownPos : hidenPos, AnimationTime).OnComplete (() => {
					if (!state && DisableOnHide)
						gameObject.SetActive (false);				
				});
		
				t_move.Play ();
			}

			get{ return state; }
		}
	}
}
