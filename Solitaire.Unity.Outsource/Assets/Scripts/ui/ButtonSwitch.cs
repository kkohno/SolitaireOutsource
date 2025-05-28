using TMPro;
using ui.windows;
using UnityEngine;
using UnityEngine.UI;

namespace ui
{
	/// <summary>
	/// Button switch with two positions (on\off).
	/// </summary>
	public class ButtonSwitch : MonoBehaviour
	{
		public Sprite onImage;
		public Sprite offImage;
		public TMP_Text label;

		public delegate void ButtonSwitchEventHandler (bool value);

		public event ButtonSwitchEventHandler StateChanged;

		private bool state;
		private bool active = true;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ButtonSwitch"/> is active.
		/// </summary>
		/// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
		public bool Active {
			get{ return active; }
			set {
				//innactive switch is half transparent and doesnt react to clicks
				gameObject.GetComponent<Image> ().color = (active = value) ? Color.white : new Color (1f, 1f, 1f, 0.5f);
				gameObject.GetComponent<Button> ().interactable = value;
			}
		}

		/// <summary>
		/// Changes the state of this switch to the opposite one.
		/// </summary>
		public void ChangeState ()
		{
			if (!active)
				return;
		
			State = !State;

			if (StateChanged != null) {			
				StateChanged (state);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="ButtonSwitch"/> is on.
		/// </summary>
		/// <value><c>true</c> if on; otherwise, <c>false</c>.</value>
		public bool State {
			set {					
				ApplyStateSprite (state = value);
				UpdateLabel ();
			}
			get { return state; }
		}

		/// <summary>
		/// Updates the label of this switch according to localisation settings.
		/// </summary>
		public void UpdateLabel ()
		{
			if (label != null)
				label.text = GameSettings.BoolToOnOffStr (state);
		}

		/// <summary>
		/// Applies sprite to switch image according to state argument
		/// </summary>
		/// <param name="arg">If set to <c>true</c> than onImage is used, otherwise offImage is used.</param>
		private void ApplyStateSprite (bool arg)
		{			
			gameObject.GetComponent<Image> ().sprite = state ? onImage : offImage;
		}
	}
}
