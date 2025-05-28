using UnityEngine;
using UnityEngine.UI;

public static class ColorExtensions
{ 

    public static void ChangeAlpha(this Image i, float newAlpha)
    {
		if (i == null)
			return;
		
        var c = i.color;
        i.color = new Color (c.r, c.g, c.b, newAlpha);
        //return ;
    }

	public static void ChangeAlpha(this Text i, float newAlpha)
	{
		var c = i.color;
		i.color = new Color (c.r, c.g, c.b, newAlpha);
		//return ;
	}

	public static bool IsEqual(this Color c1, Color c2){
		if (c1.a != c2.a)
			return false;
		if (c1.r != c2.r)
			return false;
		if (c1.g != c2.g)
			return false;
		if (c1.b != c2.b)
			return false;

		return true;
	}

	public static bool IsNear(this Color c1, Color c2, float thres){
		if (c1.a < 0.4f || c2.a < 0.4f)
			return false;

		if (Mathf.Abs ((c1.r + c1.g + c1.b) - (c2.r + c2.g + c2.b)) < thres)
			return true;

		if (Mathf.Abs (c1.r - c2.r) > thres)
			return false;
		if (Mathf.Abs (c1.g - c2.g) > thres)
			return false;
		if (Mathf.Abs (c1.b - c2.b) > thres)
			return false;
	
		return true;
	}

	public static void Fill<T>(this T[] array, T value, int start = 0, int finish=-1)
	{
		if (finish == -1) {
			finish = array.Length;
		}

		for(int i = start; i < finish; i++) 
		{
			array[i] = value;
		}
	}

	public static Color Inversed(this Color c){
		return new Color (1-c.r, 1-c.g, 1-c.b, c.a);
	}

	public static Color InversedBlack(this Color c){
		float av = (c.r + c.g + c.b) / 3;
		return new Color (1-av, 1-av, 1-av, c.a);
	}

}   

