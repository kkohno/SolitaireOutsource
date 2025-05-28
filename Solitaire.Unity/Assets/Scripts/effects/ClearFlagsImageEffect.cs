using UnityEngine;

/// <summary>
/// Image effect that emulates frames overlaps
/// </summary>
public class ClearFlagsImageEffect : MonoBehaviour
{
	//material with shader which actually performs effect action
	private static Material m_Material = null;
	// is effect is started
	private bool started = false;

	/// <summary>
	/// Creates and gets the material with effect shader.
	/// </summary>
	/// <value>The material.</value>
	protected Material material {
		get {
			if (m_Material == null) {
				m_Material = new Material (Shader.Find ("Hidden/ClearFlagsImageEffect"));
				m_Material.hideFlags = HideFlags.DontSave;
			}
			return m_Material;
		}
	}

	public void OnDestroy ()
	{
		//free memory occupie by material
		if (m_Material) {
			DestroyImmediate (m_Material);
		}	
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="ClearFlagsImageEffect"/> is enabled.
	/// </summary>
	/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
	public bool Enabled {
		get{ return enabled; }
		set {			
			UnclearedCameraController.Instance.UnclearCamera.enabled = started = value;

			//every time we disable effect we clear camera texture
			if (!value)
				UnclearedCameraController.Instance.ClearRenderTexture ();
		}
	}

	/// <summary>
	/// This method merges main camera render texture with UnclearedCamera render texture using shader Blit.
	/// </summary>
	/// <param name="src">Source camera texture.</param>
	/// <param name="dst">Result texture.</param>
	void OnRenderImage (RenderTexture src, RenderTexture dst)
	{ 		
		//if effect disabled we just copy source texture to result texture without changes
		if (!started) {
			Graphics.Blit (src, dst);
			return;
		}

		// we just combine (overlap) previous frame of from ucleared camera with new one
		material.SetTexture ("_PrevFrame", UnclearedCameraController.Instance.Texture);
		Graphics.Blit (src, dst, material);
	}
}
