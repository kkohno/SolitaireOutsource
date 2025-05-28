using UnityEngine;

namespace ui
{
	/// <summary>
	/// Sandwatch controller, that simply rotates gameobject on each frame.
	/// </summary>
	public class SandwatchController : MonoBehaviour
	{
		public Vector3 speed;

		void Update ()
		{		
			gameObject.transform.Rotate (speed);
		}
	}
}
