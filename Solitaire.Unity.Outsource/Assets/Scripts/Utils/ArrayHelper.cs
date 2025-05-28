public static class ArrayHelper
{
	/// <summary>
	/// Calculates pseudo hash for given int array
	/// </summary>
	/// <returns>The pseudo hash value.</returns>
	/// <param name="arr">Array of integers</param>
	public static int GetPseudoHash (this int[] arr)
	{
		int hc = arr.Length;
		for (int i = 0; i < arr.Length; ++i) {
			hc = unchecked(hc * 314159 + arr [i]);
		}
		return hc;
	}
}


