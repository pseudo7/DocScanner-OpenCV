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
                    WarpManager.X_PADDING, WarpManager.X_PADDING + WarpManager.WIDTH),
                    transform.position.y);
                break;
            case ResizeHandleType.HEIGHT:
                transform.position = new Vector3(transform.position.x, Mathf.Clamp(Input.mousePosition.y,
                Screen.height - (WarpManager.HEIGHT + WarpManager.Y_PADDING), Screen.height - WarpManager.Y_PADDING));
                break;
        }
    }

    enum ResizeHandleType { NONE, WIDTH, HEIGHT }
}