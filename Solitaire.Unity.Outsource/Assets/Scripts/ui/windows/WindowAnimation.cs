using DG.Tweening;
using UnityEngine;

namespace ui.windows
{
	/// <summary>
	/// Window animation controller.
	/// </summary>
	public class WindowAnimation : MonoBehaviour
	{
	
		[Range (0.1f, 1f)]
		public float showDuration = 1f;
		[Range (0.1f, 1f)]
		public float hideDuration = 0.1f;
		private Tweener t_show;
		private Tweener t_hide;
		private RectTransform rt;
		private float initialScale = 0.1f;

		/// <summary>
		/// Hides the window and notify caller at the end
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void Hide (TweenCallback callback)
		{
			//if there is no rect transform on this object we cant run animation so just quit
			if (RT == null) {			
				return;
			}

			//force complette show animation
			if (t_show != null)
				t_show.Complete ();

			// create hide animation if it wasn't done before
			if (t_hide == null) {
				RT.localScale = Vector3.one;
				t_hide = rt.DOScale (Vector3.one * initialScale, hideDuration);		
			} 

			t_hide.OnComplete (callback);
			t_hide.Restart ();
		}

		/// <summary>
		/// Gets the RectTransform component of this gameObject.
		/// </summary>
		/// <value>Rect transform component.</value>
		private RectTransform RT {
			get { 
				if (rt == null)
					rt = GetComponent<RectTransform> ();

				return rt;
			}
		}

		/// <summary>
		/// Show this window.
		/// </summary>
		public void Show ()
		{
			if (RT == null) {			
				return;
			}

			// force complete hide animation
			if (t_hide != null)
				t_hide.Complete ();		
		
			//create show animation if needed, otherwise just start it
			if (t_show == null) {			
				RT.localScale = Vector3.one * initialScale;
				t_show = rt.DOScale (Vector3.one, showDuration);		
				t_show.SetEase (CardAnimationSettings.WindowCurve);
				t_show.Play ();
			} else
				t_show.Restart ();
		}
	}
}
