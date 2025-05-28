using UnityEngine;

public class TextureScaler
{
	/// <summary>
	/// Scales the texture data of the given texture.
	/// </summary>
	/// <param name="tex">Texure to scale</param>
	/// <param name="width">New width</param>
	/// <param name="height">New height</param>
	/// <param name="mode">Filtering mode</param>
	public static void Scale (Texture2D tex1, int width, int height, FilterMode mode = FilterMode.Trilinear)
	{
		Rect texR = new Rect (0, 0, width, height);
		_gpu_scale (tex1, width, height, mode);

		// Update new texture
		tex1.Reinitialize (width, height);
		tex1.ReadPixels (texR, 0, 0, true);

	}

	/// <summary>
	/// Merge the specified textures in one (the first one)
	/// </summary>
	/// <param name="bot">Bottom layer texture. It will be modified.</param>
	/// <param name="top">Top layer texture.</param>
	public static void Merge (Texture2D bot, Texture2D top)
	{
		var cols1 = bot.GetPixels ();
		var cols2 = top.GetPixels ();
		float a;
		for (var i = 0; i < cols1.Length; ++i) {
			a = cols1 [i].a;
			cols1 [i] = Color.Lerp (cols1 [i], cols2 [i], cols2 [i].a);
			cols1 [i].a = a;
		}

		bot.SetPixels (cols1);

	}

	/// <summary>
	/// Rounds the corners of given texture.
	/// </summary>
	/// <param name="tex">Texture.</param>
	/// <param name="r">The radius in pixels.</param>
	public static void RoundCorners (Texture2D tex, int r)
	{
		Color[] c = tex.GetPixels (0, 0, tex.width, tex.height);

		int h = tex.height;
		int w = tex.width;
		Color p;

		Vector2 fromCorner = Vector2.zero;
		Vector2 radius = Vector2.one * r;

		for (int i = 0; i < (h * w); i++) {
			int y = Mathf.FloorToInt (((float)i) / ((float)w));
			int x = Mathf.FloorToInt (((float)i - ((float)(y * w))));

			fromCorner.Set ((w / 2) - Mathf.Abs ((w / 2) - x),
				(h / 2) - Mathf.Abs ((h / 2) - y));
		
			p = c [i];

			if (fromCorner.x + fromCorner.y < r &&
			    Vector2.Distance (radius, fromCorner) > r) {
				p = Color.clear;
			}

			tex.SetPixel (x, y, p);
		}
	}

	// Internal unility that renders the source texture into the RTT - the scaling method itself.
	static void _gpu_scale (Texture2D src, int width, int height, FilterMode fmode)
	{
		//We need the source texture in VRAM because we render with it
		src.filterMode = fmode;
		src.Apply (true);       

		//Using RTT for best quality and performance. Thanks, Unity 5
		RenderTexture rtt = new RenderTexture (width, height, 32);

		//Set the RTT in order to render to it
		Graphics.SetRenderTarget (rtt);

		//Setup 2D matrix in range 0..1, so nobody needs to care about sized
		GL.LoadPixelMatrix (0, 1, 1, 0);

		//Then clear & draw the texture to fill the entire RTT.
		GL.Clear (true, true, new Color (0, 0, 0, 0));
		Graphics.DrawTexture (new Rect (0, 0, 1, 1), src);
		//Graphics.DrawTexture(new Rect(0,0,1,1),second);
	}
}