using UnityEngine;
using UnityEngine.EventSystems;

namespace ui.windows
{
	/// <summary>
	/// Additional buttons panel controller.
	/// </summary>
	public class AddPanelController : MonoBehaviour, IPointerClickHandler
	{

		public WindowAnimation window;

		public void OnPointerClick (PointerEventData eventData)
		{
			Hide ();
		}

		private bool shown = false;

		/// <summary>
		/// Changes the state of this window.
		/// </summary>
		public void ChangeState ()
		{
			if (shown)
				Hide ();
			else
				Show ();
		}

		public void Show ()
		{	
			if (shown)
				return;	
		
			shown = true;
			gameObject.SetActive (true);
			window.Show ();
		}

		public void Hide ()
		{		
			if (!shown)
				return;
		
			shown = false;
			window.Hide (() => {
				gameObject.SetActive (false);
			});
		}

	}
}
