using UnityEngine;

namespace ui
{
	/// <summary>
	/// User interface variant controller apply orientation-dependent settings to rect transform object.
	/// </summary>
	public class UIVariant : MonoBehaviour
	{
		public CardLayout originalLayout;
		private Vector3[] originalValues;

		public Vector3 pos1;
		public Vector3 pos2;
		public Vector3 minAnchor;
		public Vector3 maxAnchor;
		public Vector3 pivot;
		public Vector3 rotation;
		public Vector3 scale;

		private RectTransform rt;

		/// <summary>
		/// Gets the rect transform component on object.
		/// </summary>
		/// <value>The rect transform.</value>
		private RectTransform RectTransform {
			get {
				if (rt == null)
					rt = GetComponent<RectTransform> ();
				return rt;
			}
		}

		/// <summary>
		/// Applies the original rect transform values.
		/// </summary>
		private void ApplyOriginal ()
		{		
			RectTransform.sizeDelta = originalValues [1];
			RectTransform.anchoredPosition3D = originalValues [0];
			RectTransform.anchorMin = originalValues [2];
			RectTransform.anchorMax = originalValues [3];
			RectTransform.pivot = originalValues [4];
			RectTransform.rotation = Quaternion.Euler (originalValues [5]);
			RectTransform.localScale = originalValues [6];
		}

		/// <summary>
		/// Applies the variative rect transform values.
		/// </summary>
		private void ApplyVariant ()
		{
			RectTransform.sizeDelta = pos2;
			RectTransform.anchoredPosition3D = pos1;
			RectTransform.anchorMin = minAnchor;
			RectTransform.anchorMax = maxAnchor;
			RectTransform.pivot = pivot;
			RectTransform.rotation = Quaternion.Euler (rotation);
			RectTransform.localScale = scale;
		}

		/// <summary>
		/// Saves the original rect transform values.
		/// </summary>
		private void SaveOriginal ()
		{
			originalValues = new Vector3[7];
			originalValues [0] = RectTransform.anchoredPosition3D;
			originalValues [1] = RectTransform.sizeDelta;
			originalValues [2] = RectTransform.anchorMin;
			originalValues [3] = RectTransform.anchorMax;
			originalValues [4] = RectTransform.pivot;
			originalValues [5] = RectTransform.rotation.eulerAngles;
			originalValues [6] = RectTransform.localScale;
		}

		public void SetVariant (CardLayout layout)
		{		
			//save original values if hasn't already done that
			if (originalValues == null) {
				SaveOriginal ();
			}	

			//apply layout-dependent rect transform values
			if (layout == originalLayout) {
				ApplyOriginal ();
			} else {
				ApplyVariant ();
			}
		}

	}
}
