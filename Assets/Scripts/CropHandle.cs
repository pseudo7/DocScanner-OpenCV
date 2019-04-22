using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropHandle : MonoBehaviour
{
    [SerializeField] PointerType pointerType = PointerType.NONE;

    static bool areValuesSet;
    static float yUpper;
    static float yLower;
    static float xUpper;
    static float xLower;

    static ImageCrop imageCrop;

    const int PADDING = 100;
    const int RADIUS = 100;

    bool insideXPadding;
    bool insideYPadding;

    Texture2D sourceTexture;
    void Awake()
    {
        if (!imageCrop) imageCrop = FindObjectOfType<ImageCrop>();
    }

    void Start()
    {
        if (!sourceTexture) sourceTexture = ImageCrop.CroppingTexture;

        if (!areValuesSet)
        {
            areValuesSet = true;
            xUpper = Screen.width - PADDING;
            xLower = PADDING;
            yUpper = Screen.height - PADDING;
            yLower = PADDING;
        }
    }

    Vector3 MousePosition { set; get; }

    /// <summary>
    /// Make sure to put Canvas to Screen Space - Overlay
    /// </summary>
    public void Drag()
    {
        MousePosition = Input.mousePosition;
        transform.position = MousePosition + PointerOffset * Screen.width / 40;
        CheckConstaints();
        UpdateClipTexture();
    }

    public void UpdateClipTexture()
    {
        imageCrop.UpdateClippedTexture(GetTextureAroundPoint(transform.GetChild(0).position));
    }

    /// <summary>
    /// Rotating objects so that the pointer stays inside always, adding offset so it doesn't jerk.
    /// </summary>
    void CheckConstaints()
    {
        bool aboveHalf = MousePosition.y > Screen.height / 2;
        bool leftHalf = MousePosition.x < Screen.width / 2;

        if (MousePosition.x > xUpper)
        {
            transform.localEulerAngles = new Vector3(0, 0, (Screen.width - MousePosition.x) / PADDING * (aboveHalf ? -45 : 45) + (aboveHalf ? 45 : -45));
            insideXPadding = true;
        }
        else if (MousePosition.x < xLower)
        {
            transform.localEulerAngles = new Vector3(0, 0, (MousePosition.x / PADDING) * (aboveHalf ? 45 : -45) + (aboveHalf ? -45 : 45));
            insideXPadding = true;
        }
        else insideXPadding = false;

        if (MousePosition.y > yUpper)
        {
            transform.localEulerAngles = new Vector3(0, 0, (Screen.height - MousePosition.y) / PADDING * (leftHalf ? -45 : 45) + (leftHalf ? 45 : -45));
            insideYPadding = true;
        }
        else if (MousePosition.y < yLower)
        {
            transform.localEulerAngles = new Vector3(0, 0, (MousePosition.y / PADDING) * (leftHalf ? 45 : -45) + (leftHalf ? -45 : 45));
            insideYPadding = true;
        }
        else insideYPadding = false;

        if (!insideXPadding && !insideYPadding)
            transform.localEulerAngles = new Vector3(0, 0, 0);
        else
        if (insideXPadding && insideYPadding)
            transform.localEulerAngles = new Vector3(0, 0, 0);

    }

    Texture2D GetTextureAroundPoint(Vector2 center)
    {
        Texture2D clippedTexture = new Texture2D(RADIUS, RADIUS);
        clippedTexture.SetPixels(sourceTexture.GetPixels((int)center.x - RADIUS / 2, (int)center.y - RADIUS / 2, RADIUS, RADIUS));
        clippedTexture.Apply();
        return clippedTexture;
    }

    Vector3 PointerOffset
    {
        get
        {
            switch (pointerType)
            {
                case PointerType.BOTTOM_LEFT:
                    return new Vector3(1, -1);
                case PointerType.BOTTOM_RIGHT:
                    return new Vector3(-1, -1);
                case PointerType.TOP_LEFT:
                    return new Vector3(1, 1);
                case PointerType.TOP_RIGHT:
                    return new Vector3(-1, 1);
                default: return Vector3.zero;
            }
        }
    }

    enum PointerType { NONE, BOTTOM_LEFT, BOTTOM_RIGHT, TOP_LEFT, TOP_RIGHT }
}
