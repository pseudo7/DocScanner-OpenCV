using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropHandle : MonoBehaviour
{
    [SerializeField] PointerType pointerType;
    public void Drag()
    {
        transform.position = Input.mousePosition + PointerOffset * Screen.width / 40;
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
