using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarpController : MonoBehaviour
{
    public RawImage warpedImage;
    public RectTransform widthHandle;
    public RectTransform heightHandle;

    Vector2 origSizeDelta;

    const float WIDTH = 1000;
    const float HEIGHT = 600;

    const float X_PADDING = 50;
    const float Y_PADDING = 50;

    const float X_OFFSET = 150;
    const float Y_OFFSET = 150;

    public void ResizeImage()
    {
        warpedImage.rectTransform.sizeDelta = new Vector2(widthHandle.position.x - X_PADDING, Screen.height - (heightHandle.position.y + Y_PADDING));
    }

    public void UpdateHandlers()
    {
        warpedImage.rectTransform.sizeDelta = new Vector2(WIDTH, HEIGHT);
        widthHandle.position = new Vector3(X_PADDING + WIDTH, Screen.height - (Y_PADDING + HEIGHT + Y_OFFSET));
        heightHandle.position = new Vector3(X_PADDING + WIDTH + X_OFFSET, Screen.height - (Y_PADDING + HEIGHT));
    }
}
