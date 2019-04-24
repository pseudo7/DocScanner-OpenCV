using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CropSizeManager : MonoBehaviour
{
    public static CropSizeManager Instance;

    [SerializeField] ToggleGroup sizeToggleGroup;
    [SerializeField] TextMeshProUGUI currentDimmText;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform itemSpawnParent;

    public static Dimm CurrentDimmension { set; get; }
    public void SetCurrentDimmText(string dimmensionText)
    {
        currentDimmText.text = dimmensionText;
    }
    const int MAX_ITEMS = 6;

    void Awake()
    {
        if (!Instance) Instance = this;
        PopulateItems();
        ReActivatePaperItems();
        CurrentDimmension = GetPaperPixels(ISOPaperType.A2);
        SetCurrentDimmText(string.Format("Current Size\n{0} - {1}", ISOPaperType.A2, GetPaperPixels(ISOPaperType.A2)));
    }
    public void RePopulateItems(int id)
    {
        ClearAllItems();
        switch (id)
        {
            case 0:
                ReActivatePaperItems();
                break;
            case 1:
                ReActivateWideScreenItems();
                break;
            case 2:
                ReActivateLegacyScreenItems();
                break;
        }
    }

    void ClearAllItems()
    {
        foreach (Transform child in itemSpawnParent)
            child.gameObject.SetActive(false);
    }

    void ReActivateWideScreenItems()
    {
        System.Array items = System.Enum.GetValues(typeof(WideScreenResolution));

        for (int i = 0; i < items.Length - 1; i++)
        {
            itemSpawnParent.GetChild(i).gameObject.SetActive(true);
            WideScreenResolution item = (WideScreenResolution)items.GetValue(i + 1);
            TextureCropSize cropSize = itemSpawnParent.GetChild(i).GetComponent<TextureCropSize>();
            cropSize.Dimmension = GetWideScreenPixels(item);
            cropSize.DimmensionText = string.Format("Current Size\n{0} - {1}", item, cropSize.Dimmension);
            itemSpawnParent.GetChild(i).GetChild(0).GetComponent<Text>().text = "\t\t" + item;
            itemSpawnParent.GetChild(i).GetChild(1).GetComponent<Text>().text = GetWideScreenMeasurements(item);
        }
        items = null;
        System.GC.Collect();
    }

    void ReActivatePaperItems()
    {
        System.Array items = System.Enum.GetValues(typeof(ISOPaperType));

        for (int i = 0; i < items.Length - 1; i++)
        {
            itemSpawnParent.GetChild(i).gameObject.SetActive(true);
            ISOPaperType item = (ISOPaperType)items.GetValue(i + 1);
            TextureCropSize cropSize = itemSpawnParent.GetChild(i).GetComponent<TextureCropSize>();
            cropSize.Dimmension = GetPaperPixels(item);
            cropSize.DimmensionText = string.Format("Current Size\n{0} - {1}", item, cropSize.Dimmension);
            itemSpawnParent.GetChild(i).GetChild(0).GetComponent<Text>().text = "\t\t" + item;
            itemSpawnParent.GetChild(i).GetChild(1).GetComponent<Text>().text = GetPaperMeasurements(item);
        }
        items = null;
        System.GC.Collect();
    }

    void ReActivateLegacyScreenItems()
    {
        System.Array items = System.Enum.GetValues(typeof(LegacyScreenResolution));

        for (int i = 0; i < items.Length - 1; i++)
        {
            itemSpawnParent.GetChild(i).gameObject.SetActive(true);
            LegacyScreenResolution item = (LegacyScreenResolution)items.GetValue(i + 1);
            TextureCropSize cropSize = itemSpawnParent.GetChild(i).GetComponent<TextureCropSize>();
            cropSize.Dimmension = GetLegacyScreenPixels(item);
            cropSize.DimmensionText = string.Format("Current Size\n{0} - {1}", item, cropSize.Dimmension);
            itemSpawnParent.GetChild(i).GetChild(0).GetComponent<Text>().text = "\t\t" + item;
            itemSpawnParent.GetChild(i).GetChild(1).GetComponent<Text>().text = GetLegacyScreenMeasurements(item);
        }
        items = null;
        System.GC.Collect();
    }

    void PopulateItems()
    {
        for (int i = 0; i < MAX_ITEMS; i++)
            Instantiate(itemPrefab, itemSpawnParent);
    }

    Dimm GetLegacyScreenPixels(LegacyScreenResolution screenResolution)
    {
        switch (screenResolution)
        {
            case LegacyScreenResolution.L_600:
                return new Dimm(800, 600);
            case LegacyScreenResolution.L_768:
                return new Dimm(1024, 768);
            case LegacyScreenResolution.L_1024:
                return new Dimm(1366, 1024);
            default: return new Dimm(0, 0);
        }
    }
    Dimm GetWideScreenPixels(WideScreenResolution screenResolution)
    {
        switch (screenResolution)
        {
            case WideScreenResolution.L_540P:
                return new Dimm(960, 540);
            case WideScreenResolution.L_720P:
                return new Dimm(1280, 720);
            case WideScreenResolution.L_1080P:
                return new Dimm(1920, 1080);
            case WideScreenResolution.L_2K:
                return new Dimm(2048, 1152);
            case WideScreenResolution.L_1440P:
                return new Dimm(2560, 1440);
            case WideScreenResolution.L_4K:
                return new Dimm(3840, 2160);
            default: return new Dimm(0, 0);
        }
    }

    /// <summary>
    /// Get paper size in pixels of 150 DPI
    /// </summary>
    /// <param name="paperType"></param>
    /// <returns>Dimmensions of the Paper in Pixels</returns>
    Dimm GetPaperPixels(ISOPaperType paperType)
    {
        switch (paperType)
        {
            case ISOPaperType.A2:
                return new Dimm(3508, 2480);
            case ISOPaperType.A3:
                return new Dimm(2480, 1754);
            case ISOPaperType.A4:
                return new Dimm(1754, 1240);
            case ISOPaperType.A5:
                return new Dimm(1240, 874);
            case ISOPaperType.A6:
                return new Dimm(874, 620);
            case ISOPaperType.A7:
                return new Dimm(620, 437);
            default:
                return new Dimm(0, 0);
        }
    }

    string GetLegacyScreenMeasurements(LegacyScreenResolution screenResolution)
    {
        string dimm = "Pixels";

        switch (screenResolution)
        {
            case LegacyScreenResolution.L_600:
                return string.Format("{0}x{1} : {2}", 800, 600, dimm);
            case LegacyScreenResolution.L_768:
                return string.Format("{0}x{1} : {2}", 1024, 768, dimm);
            case LegacyScreenResolution.L_1024:
                return string.Format("{0}x{1} : {2}", 1366, 1024, dimm);
            default:
                return string.Format("{0}x{1} : {2}", 0, 0, dimm);
        }
    }

    string GetWideScreenMeasurements(WideScreenResolution screenResoultion)
    {
        string dimm = "Pixels";

        switch (screenResoultion)
        {
            case WideScreenResolution.L_540P:
                return string.Format("{0}x{1} : {2}", 960, 540, dimm);
            case WideScreenResolution.L_720P:
                return string.Format("{0}x{1} : {2}", 1280, 720, dimm);
            case WideScreenResolution.L_1080P:
                return string.Format("{0}x{1} : {2}", 1920, 1080, dimm);
            case WideScreenResolution.L_2K:
                return string.Format("{0}x{1} : {2}", 2048, 1152, dimm);
            case WideScreenResolution.L_1440P:
                return string.Format("{0}x{1} : {2}", 2560, 1440, dimm);
            case WideScreenResolution.L_4K:
                return string.Format("{0}x{1} : {2}", 3840, 2160, dimm);
            default:
                return string.Format("{0}x{1} : {2}", 0, 0, dimm);
        }
    }

    string GetPaperMeasurements(ISOPaperType paperType)
    {
        string dimm = "mm";

        switch (paperType)
        {
            case ISOPaperType.A2:
                return string.Format("{0}x{1} : {2}", 420, 594, dimm);
            case ISOPaperType.A3:
                return string.Format("{0}x{1} : {2}", 297, 420, dimm);
            case ISOPaperType.A4:
                return string.Format("{0}x{1} : {2}", 210, 297, dimm);
            case ISOPaperType.A5:
                return string.Format("{0}x{1} : {2}", 148, 210, dimm);
            case ISOPaperType.A6:
                return string.Format("{0}x{1} : {2}", 105, 148, dimm);
            case ISOPaperType.A7:
                return string.Format("{0}x{1} : {2}", 74, 105, dimm);
            default:
                return string.Format("{0}x{1} : {2}", 0, 0, dimm);
        }
    }

}
[System.Serializable]
public struct Dimm
{
    public int width;
    public int height;

    public Dimm(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
    public override string ToString()
    {
        return string.Format("{0} X {1}", width, height);
    }
}
public enum LegacyScreenResolution { NONE, L_600, L_768, L_1024 }
public enum WideScreenResolution { NONE, L_540P, L_720P, L_1080P, L_2K, L_1440P, L_4K }
public enum ISOPaperType { NONE, A2, A3, A4, A5, A6, A7 }