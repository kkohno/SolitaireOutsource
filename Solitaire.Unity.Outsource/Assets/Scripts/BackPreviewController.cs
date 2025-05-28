using System;
using ui.windows;
using UnityEngine;

public class BackPreviewController : MonoBehaviour
{

    public GameObject loading;
    public Canvas canvas;
    public PreviewImageController preview;

    private Action<bool, Texture2D> resultCallback;
    private float scaleFactor = 1f;
    private int[] textureSize = new int[] { 144, 220 };

    /// <summary>
    /// Creates the new card back texture and returns it in callback.
    /// </summary>
    /// <param name="callback">Callback, where operation result and texture will be provided</param>
    public void CreateBack(Action<bool, Texture2D> callback)
    {
        resultCallback = callback;

        //subscribe on events
        Unimgpicker.Instance.Completed += ImagePathReceived;
        Unimgpicker.Instance.Failed += PickAborted;

        //show native gallery to pick image
        Unimgpicker.Instance.Show();
    }

    /// <summary>
    /// Image picker cancel actions.
    /// </summary>
    /// <param name="clearTexture">If set to <c>true</c> then clear loaded texture.</param>
    public void Cancel(bool clearTexture)
    {
        resultCallback(false, null);
        FinalActions(clearTexture);
    }

    /// <summary>
    /// Image picker apply actions.
    /// </summary>
    public void Apply()
    {
        //create texture which contain new card back
        Texture2D resTex = new Texture2D(preview.RegionSize[0], preview.RegionSize[1], TextureFormat.ARGB32, false);

        //crop texture according to region positon and size
        resTex.SetPixels(preview.SourceImage.sprite.texture.GetPixels(preview.RegionPosition[0], preview.RegionPosition[1], preview.RegionSize[0], preview.RegionSize[1]));

        // scale texture to standard size
        TextureScaler.Scale(resTex, textureSize[0], textureSize[1], FilterMode.Bilinear);
        //make it corners round
        TextureScaler.RoundCorners(resTex, 15);
        //add standard card borders to this texture
        TextureScaler.Merge(resTex, ThemeManager.CardBorder);

        resTex.Apply();

        resultCallback(true, resTex);
        FinalActions(true);
    }

    /// <summary>
    /// Handles pick aborted event
    /// </summary>
    /// <param name="reason">Error msg or other failure reason.</param>
    private void PickAborted(string reason)
    {
        Cancel(false);
    }

    /// <summary>
    /// Handles success picker event.
    /// </summary>
    /// <param name="path">Image path.</param>
    private void ImagePathReceived(string path)
    {
        gameObject.SetActive(true);


        //load picture with given path and prepare editor UI
        LoadImage("file://" + path, () => {
            //disable rotations during image edit
            OrientationHandler.RotationEnabled = false;
            preview.Prepare();
            loading.SetActive(false);
        });
    }

    /// <summary>
    /// Loads the image with specified path into preview object and fits it to current screen resolution.
    /// </summary>
    /// <returns>The image.</returns>
    /// <param name="path">Path.</param>
    /// <param name="callback">Callback.</param>
    private async void LoadImage(string path, Action callback)
    {
        loading.SetActive(true);
        var bytes = await WWWUtils.GetBytesFromURL(path);

        Texture2D tex = new Texture2D(0, 0);
        tex.LoadImage(bytes);

        if (tex.width <= textureSize[0] ||
           tex.height <= textureSize[1]) {

            Destroy(tex);
            Cancel(false);
            return;
        }

        preview.SourceImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2f);
        preview.SourceImage.rectTransform.sizeDelta = new Vector2(tex.width, tex.height);

        FitToLayout();

        callback();


        loading.SetActive(false);
    }

    /// <summary>
    /// Final action, which must be performed after using this class
    /// </summary>
    /// <param name="clearTexture">If set to <c>true</c> then clear loaded texture.</param>
    private void FinalActions(bool clearTexture)
    {
        //we clear loaded texture to free memory
        //but only if its exists and if clearTexture is true
        if (preview.SourceImage.sprite != null && clearTexture) {
            Destroy(preview.SourceImage.sprite.texture);
            preview.SourceImage.sprite = null;
        }

        gameObject.SetActive(false);
        loading.SetActive(false);

        //unsubscribe from events
        Unimgpicker.Instance.Completed -= ImagePathReceived;
        Unimgpicker.Instance.Failed -= PickAborted;

        //return rotation to its original state according to user settings
        OrientationHandler.RotationEnabled = GameSettings.Instance.Data.ChangeOrient;
    }

    /// <summary>
    /// Fits preview iamge to current screen size.
    /// </summary>
    private void FitToLayout()
    {
        if (((float)preview.SourceImage.sprite.texture.width / Screen.width) >
            ((float)preview.SourceImage.sprite.texture.height / Screen.height)) {
            scaleFactor = (float)Screen.width / preview.SourceImage.sprite.texture.width;
        }
        else {
            scaleFactor = (float)Screen.height / preview.SourceImage.sprite.texture.height;
        }

        preview.SourceImage.rectTransform.sizeDelta = preview.SourceImage.rectTransform.sizeDelta * scaleFactor / canvas.scaleFactor;
    }

}
