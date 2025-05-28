using UnityEngine;


    public class CameraUtilities
    {
        /// <summary>
        /// Smoothes a Vector3 that represents euler angles.
        /// </summary>
        /// <param name="current">The current Vector3 value.</param>
        /// <param name="target">The target Vector3 value.</param>
        /// <param name="velocity">A refernce Vector3 used internally.</param>
        /// <param name="smoothTime">The time to smooth, in seconds.</param>
        /// <returns>The smoothed Vector3 value.</returns>
        public static Vector3 SmoothDampEuler(Vector3 current, Vector3 target, ref Vector3 velocity, float smoothTime)
        {
            Vector3 v;

            v.x = Mathf.SmoothDampAngle(current.x, target.x, ref velocity.x, smoothTime);
            v.y = Mathf.SmoothDampAngle(current.y, target.y, ref velocity.y, smoothTime);
            v.z = Mathf.SmoothDampAngle(current.z, target.z, ref velocity.z, smoothTime);

            return v;
        }

        /// <summary>
        /// Multiplies each element in Vector3 v by the corresponding element of w.
        /// </summary>
        public static Vector3 MultiplyVectors(Vector3 v, Vector3 w)
        {
            v.x *= w.x;
            v.y *= w.y;
            v.z *= w.z;

            return v;
        }


		public static  Vector2 switchToRectTransform(RectTransform from, RectTransform to)
		{
			Vector2 localPoint;
			Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * from.pivot.x + from.rect.xMin, from.rect.height * from.pivot.y + from.rect.yMin);
			Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
			screenP += fromPivotDerivedOffset;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
			Vector2 pivotDerivedOffset = new Vector2(to.rect.width * to.pivot.x + to.rect.xMin, to.rect.height * to.pivot.y + to.rect.yMin);
			return to.anchoredPosition + localPoint - pivotDerivedOffset;
		}
    }