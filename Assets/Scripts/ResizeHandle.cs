using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeHandle : MonoBehaviour
{
    [SerializeField] ResizeHandleType resizeHandle = ResizeHandleType.NONE;

    public void Drag()
    {
        switch (resizeHandle)
        {
            case ResizeHandleType.WIDTH:
                transform.position = new Vector3(Mathf.Clamp(Input.mousePosition.x,
                    WarpController.X_PADDING, WarpController.X_PADDING + WarpController.WIDTH),
                    transform.position.y);
                break;
            case ResizeHandleType.HEIGHT:
                transform.position = new Vector3(transform.position.x, Mathf.Clamp(Input.mousePosition.y,
                Screen.height - (WarpController.HEIGHT + WarpController.Y_PADDING), Screen.height - WarpController.Y_PADDING));
                break;
        }
    }

    enum ResizeHandleType { NONE, WIDTH, HEIGHT }
}