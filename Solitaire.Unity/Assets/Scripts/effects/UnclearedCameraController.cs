using System.Collections;
using ui;
using UnityEngine;

/// <summary>
/// Uncleared camera controller.
/// </summary>
public class UnclearedCameraController : MonoBehaviour
{

	public Camera original;
	private Camera unclearCamera;
	private static UnclearedCameraController instance;

	void Awake ()
	{
		instance = this;
		UnclearCamera.enabled = false;
	}

	public static UnclearedCameraController Instance {
		get{ return instance; }
	}

	void Start ()
	{
		UILayout.LayoutChanged += SyncWithOriginal;
	}

	void OnDestroy ()
	{
		UILayout.LayoutChanged -= SyncWithOriginal;
	}

	public Camera UnclearCamera {
		get { 
			if (unclearCamera == null) {
				unclearCamera = GetComponent<Camera> ();
			}
			return unclearCamera;
		}
	}

	public void ClearRenderTexture ()
	{		
		RenderTexture lastActive = RenderTexture.active;
		RenderTexture.active = UnclearCamera.targetTexture;
		GL.Clear (true, true, Color.clear);
		RenderTexture.active = lastActive;
	}

	/// <summary>
	/// Syncs uncleared camera when orientation, resolution or layout was changed
	/// </summary>
	public void SyncWithOriginal ()
	{		
		StartCoroutine (SyncRoutine ());
	}

	/// <summary>
	/// Synchronize uncleared camera parameters with main game camera
	/// </summary>
	private IEnumerator SyncRoutine ()
	{			
		
		UnclearCamera.orthographicSize = original.orthographicSize;
		UnclearCamera.farClipPlane = original.farClipPlane;
		UnclearCamera.nearClipPlane = original.nearClipPlane;
		UnclearCamera.transform.position = original.transform.position;
	
		//clear target texture
		if (UnclearCamera.targetTexture != null) {			
			UnclearCamera.targetTexture.Release ();
			UnclearCamera.targetTexture = null;
		}

		yield return null;		

		// create new target texture
		UnclearCamera.targetTexture = new RenderTexture (Screen.width, Screen.height, 16);
		ClearRenderTexture ();
	}

	/// <summary>
	/// Gets the camera target texture.
	/// </summary>
	/// <value>The texture.</value>
	public RenderTexture Texture {
		get{ return UnclearCamera.targetTexture; }
	}
}
