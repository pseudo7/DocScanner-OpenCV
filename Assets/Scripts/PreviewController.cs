using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using OpenCVForUnity;
public class PreviewController : MonoBehaviour
{
    public RawImage filteredRawImage;
    public RawImage previewRawImage;
    public AspectRatioFitter ratioFitter;
    public GameObject saveDialog;
    public Sprite successSprite;
    public Sprite errorSprite;

    public void ShowPreview()
    {
        int width = filteredRawImage.mainTexture.width;
        int height = filteredRawImage.mainTexture.height;

        Debug.Log(WarpController.DeltaWidth + " " + WarpController.DeltaHeight);

        int newWidth = (int)(width * WarpController.DeltaWidth);
        int newHeight = (int)(height * WarpController.DeltaHeight);

        Texture2D warpedTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        Graphics.CopyTexture(filteredRawImage.mainTexture, warpedTexture);

        Mat warpedMat = new Mat(height, width, CvType.CV_8UC3);
        Utils.texture2DToMat(warpedTexture, warpedMat);

        Texture2D newTexture = new Texture2D(newWidth, newHeight, TextureFormat.RGB24, false);

        Imgproc.resize(warpedMat, warpedMat, new Size(newWidth, newHeight));
        Utils.matToTexture2D(warpedMat, newTexture);

        previewRawImage.texture = newTexture;
        ratioFitter.aspectRatio = newWidth / (float)newHeight;

        gameObject.SetActive(true);
    }

    public void RotateTexture(bool clockWise)
    {
        previewRawImage.texture = RotateTexture((Texture2D)previewRawImage.texture, clockWise);
        ratioFitter.aspectRatio = 1 / ratioFitter.aspectRatio;
    }

    public void Sharpen()
    {
        Texture2D warpedTexture = new Texture2D(previewRawImage.mainTexture.width, previewRawImage.mainTexture.height, TextureFormat.RGB24, false);
        Graphics.CopyTexture(previewRawImage.texture, warpedTexture);

        Mat initMat = new Mat(warpedTexture.height, warpedTexture.width, CvType.CV_8UC3);
        Utils.texture2DToMat(warpedTexture, initMat);
        Mat finalMat = new Mat(warpedTexture.height, warpedTexture.width, CvType.CV_8UC3);

        Imgproc.GaussianBlur(initMat, finalMat, new Size(0, 0), 3);


        Core.addWeighted(initMat, 1.5, finalMat, -.5, 0, finalMat);

        Utils.matToTexture2D(finalMat, warpedTexture);
        initMat.Dispose();
        finalMat.Dispose();
        previewRawImage.texture = warpedTexture;
    }

    public void SaveTextureToDisk()
    {
        Texture2D warpedTexture = new Texture2D(previewRawImage.mainTexture.width, previewRawImage.mainTexture.height, TextureFormat.RGB24, false);
        Graphics.CopyTexture(previewRawImage.texture, warpedTexture);

        string path = GetStorageDirectory();

        Debug.LogWarning("External Permission: " + Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite));

        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            try
            {
                byte[] textureData = warpedTexture.EncodeToPNG();
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                string fileName = System.DateTime.Now.Ticks + ".PNG";
                string filePath = Path.Combine(path, fileName);
                File.WriteAllBytes(filePath, textureData);
                Debug.Log("File: " + filePath);
                ScanMedia(fileName);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                ShowSaveSuccessDialog(false);
            }
            ShowSaveSuccessDialog(true);
        }
        else
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            ShowSaveSuccessDialog(false);
        }
    }

    void ShowSaveSuccessDialog(bool saveSuccessful)
    {
        StartCoroutine(ShowDialog(saveSuccessful));
    }

    IEnumerator ShowDialog(bool saveSuccessful)
    {
        saveDialog.transform.GetChild(0).GetComponent<Image>().sprite = saveSuccessful ? successSprite : errorSprite;
        saveDialog.transform.GetChild(0).GetComponent<Image>().color = saveSuccessful ? Color.green : Color.red;
        saveDialog.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = saveSuccessful ? "Save Successful!" : "Save Un-successful!";
        saveDialog.SetActive(true);
        yield return new WaitForSeconds(1);
        saveDialog.SetActive(false);
    }

    void ScanMedia(string fileName)
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        using (AndroidJavaClass jcUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject joActivity = jcUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject joContext = joActivity.Call<AndroidJavaObject>("getApplicationContext"))
        using (AndroidJavaClass jcMediaScannerConnection = new AndroidJavaClass("android.media.MediaScannerConnection"))
        using (AndroidJavaClass jcEnvironment = new AndroidJavaClass("android.os.Environment"))
        using (AndroidJavaObject joExDir = jcEnvironment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
        {
            string path = joExDir.Call<string>("toString") + "/DCIM/Pseudo/" + fileName;
            Debug.Log("search path : " + path);
            jcMediaScannerConnection.CallStatic("scanFile", joContext, new string[] { path }, new string[] { "image/png" }, null);
        }
    }

    string GetStorageDirectory()
    {
        if (Application.platform != RuntimePlatform.Android)
            return Application.persistentDataPath;
        using (AndroidJavaClass jcEnvironment = new AndroidJavaClass("android.os.Environment"))
        using (AndroidJavaObject joExDir = jcEnvironment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
            return joExDir.Call<string>("toString") + "/DCIM/Pseudo/";
    }

    public void ResetTexture()
    {
        ShowPreview();
    }

    Texture2D RotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];

        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w, TextureFormat.RGB24, false);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }
}
