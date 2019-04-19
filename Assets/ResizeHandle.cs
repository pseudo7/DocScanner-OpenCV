using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeHandle : MonoBehaviour
{
    [SerializeField] ResizeHandleType resizeHandle;

    public void Drag()
    {
        switch (resizeHandle)
        {
            case ResizeHandleType.WIDTH:
                transform.position = new Vector3(Input.mousePosition.x, transform.position.y);
                break;
            case ResizeHandleType.HEIGHT:
                transform.position = new Vector3(transform.position.x, Input.mousePosition.y);
                break;
        }
    }

    enum ResizeHandleType { NONE, WIDTH, HEIGHT }
}