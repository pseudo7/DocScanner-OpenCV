using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarpManager : MonoBehaviour
{
    public RawImage warpedImage;
    public RawImage filteredImage;
    public RectTransform widthHandle;
    public RectTransform heightHandle;

    Vector2 origSizeDelta;

    public static float WIDTH = 750;
    public static float HEIGHT;// = WIDTH * CropSizeManager.CurrentDimmension.height / (float)CropSizeManager.CurrentDimmension.width;// (StreamManager.WebcamSize.y / (float)StreamManager.WebcamSize.x);

    static float FILTER_WIDTH = 500;
    static float FILTER_HEIGHT;// = FILTER_WIDTH * CropSizeManager.CurrentDimmension.height / (float)CropSizeManager.CurrentDimmension.width;//(StreamManager.WebcamSize.y / (float)StreamManager.WebcamSize.x);

    public const float X_PADDING = 150;
    public const float Y_PADDING = 50;

    public const float X_OFFSET = 100;
    public const float Y_OFFSET = 100;

    public static float DeltaWidth { private set; get; }
    public static float DeltaHeight { private set; get; }

    void Start()
    {
        DeltaHeight = DeltaWidth = 1;
        //Debug.LogWarning("Orig\n" + warpedImage.mainTexture.width + " " + warpedImage.mainTexture.height);
    }

    public static void SetupDimm()
    {
        WIDTH = 750;
        HEIGHT = WIDTH * CropSizeManager.CurrentDimmension.height / (float)CropSizeManager.CurrentDimmension.width;// (StreamManager.WebcamSize.y / (float)StreamManager.WebcamSize.x);

        FILTER_WIDTH = 500;
        FILTER_HEIGHT = FILTER_WIDTH * CropSizeManager.CurrentDimmension.height / (float)CropSizeManager.CurrentDimmension.width;//(StreamManager.WebcamSize.y / (float)StreamManager.WebcamSize.x);
    }

    public void ResizeImage()
    {
        warpedImage.rectTransform.sizeDelta = new Vector2(widthHandle.position.x - X_PADDING, Screen.height - (heightHandle.position.y + Y_PADDING));
        DeltaWidth = warpedImage.rectTransform.rect.width / WIDTH;
        DeltaHeight = warpedImage.rectTransform.rect.height / HEIGHT;

        //Debug.LogWarning("WIDTH: " + WIDTH + "\nRect Width: " + warpedImage.rectTransform.rect.width);
        //Debug.LogWarning("HEIGHT: " + HEIGHT + "\nRect Height: " + warpedImage.rectTransform.rect.height);

        filteredImage.rectTransform.sizeDelta = new Vector2(FILTER_WIDTH * DeltaWidth, FILTER_HEIGHT * DeltaHeight);
    }

    public void UpdateHandlers()
    {
        SetupDimm();
        
        warpedImage.rectTransform.position = new Vector2(X_PADDING, Screen.height - Y_PADDING);
        warpedImage.rectTransform.sizeDelta = new Vector2(WIDTH, HEIGHT);

        filteredImage.rectTransform.position = new Vector2(X_PADDING, Y_PADDING);
        filteredImage.rectTransform.sizeDelta = new Vector2(FILTER_WIDTH, FILTER_HEIGHT);

        widthHandle.position = new Vector3(X_PADDING + WIDTH, Screen.height - (Y_PADDING + HEIGHT + Y_OFFSET));
        heightHandle.position = new Vector3(X_PADDING + WIDTH + X_OFFSET, Screen.height - (Y_PADDING + HEIGHT));
    }
}
