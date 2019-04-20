using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarpController : MonoBehaviour
{
    public RawImage warpedImage;
    public RawImage filteredImage;
    public RectTransform widthHandle;
    public RectTransform heightHandle;

    Vector2 origSizeDelta;

    public const float WIDTH = 750;
    public const float HEIGHT = 450;

    const float FILTER_WIDTH = 500;
    const float FILTER_HEIGHT = 300;

    public const float X_PADDING = 150;
    public const float Y_PADDING = 50;

    public const float X_OFFSET = 100;
    public const float Y_OFFSET = 100;

    public void ResizeImage()
    {
        warpedImage.rectTransform.sizeDelta = new Vector2(widthHandle.position.x - X_PADDING, Screen.height - (heightHandle.position.y + Y_PADDING));
    }

    public void UpdateHandlers()
    {
        //Debug.LogFormat("Width: {0}, Height: {1}", WIDTH, HEIGHT);
        //Debug.LogFormat("X Padding: {0}, Y Padding: {1}", X_PADDING, Y_PADDING);
        //Debug.LogFormat("X Offset: {0}, Y Offset: {1}", X_OFFSET, Y_OFFSET);

        warpedImage.rectTransform.position = new Vector2(X_PADDING, Screen.height - Y_PADDING);
        warpedImage.rectTransform.sizeDelta = new Vector2(WIDTH, HEIGHT);

        filteredImage.rectTransform.position = new Vector2(X_PADDING, Y_PADDING);
        filteredImage.rectTransform.sizeDelta = new Vector2(FILTER_WIDTH, FILTER_HEIGHT);

        widthHandle.position = new Vector3(X_PADDING + WIDTH, Screen.height - (Y_PADDING + HEIGHT + Y_OFFSET));
        heightHandle.position = new Vector3(X_PADDING + WIDTH + X_OFFSET, Screen.height - (Y_PADDING + HEIGHT));
    }
}
