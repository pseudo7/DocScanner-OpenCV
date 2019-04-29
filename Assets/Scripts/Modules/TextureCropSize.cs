using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureCropSize : MonoBehaviour
{
    public Dimm Dimmension { set; get; }
    public string DimmensionText { set; get; }
    public void SetDimmensions()
    {
        CropSizeManager.CurrentDimmension = Dimmension;
        CropSizeManager.Instance.SetCurrentDimmText(DimmensionText);
    }
}
