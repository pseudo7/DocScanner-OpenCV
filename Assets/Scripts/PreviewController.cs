﻿using System.IO;
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

    void Start()
    {

    }

    public void ShowPreview()
    {
        int width = filteredRawImage.mainTexture.width;
        int height = filteredRawImage.mainTexture.height;

        Debug.Log(WarpController.DeltaWidth + " " + WarpController.DeltaHeight);

        int newWidth = (int)(width * WarpController.DeltaWidth);
        int newHeight = (int)(height * WarpController.DeltaHeight);

        Texture2D warpedTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        Graphics.CopyTexture(filteredRawImage.mainTexture, warpedTexture);

        Mat warpedMat = new Mat(height, width, CvType.CV_8UC4);
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

        Mat initMat = new Mat(warpedTexture.height, warpedTexture.width, CvType.CV_8UC4);
        Utils.texture2DToMat(warpedTexture, initMat);
        Mat finalMat = new Mat(warpedTexture.height, warpedTexture.width, CvType.CV_8UC4);

        Imgproc.GaussianBlur(initMat, finalMat, new Size(0, 0), 3);

        Core.addWeighted(initMat, 1.5, finalMat, -.5, 0, finalMat);

        //double[] scalarValues = new double[] { -1, -1, -1, -1, 9 - 1, -1, -1, -1 };
        //Scalar scalar = new Scalar(scalarValues);

        //Mat kernel = new Mat(3, 3, CvType.CV_8UC4, scalar);
        //Imgproc.filter2D(initMat, initMat, -1, kernel);

        Utils.matToTexture2D(finalMat, warpedTexture);

        previewRawImage.texture = warpedTexture;
    }

    public void SaveTextureToDisk()
    {
        Texture2D warpedTexture = new Texture2D(previewRawImage.mainTexture.width, previewRawImage.mainTexture.height, TextureFormat.RGB24, false);
        Graphics.CopyTexture(previewRawImage.texture, warpedTexture);

        string path = Path.Combine(Application.persistentDataPath, "Pseudo");

        Debug.Log("Directory: " + path);

        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            byte[] textureData = warpedTexture.EncodeToPNG();
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            string filePath = Path.Combine(path, System.DateTime.Now.Ticks + ".PNG");
            File.WriteAllBytes(filePath, textureData);
            Debug.Log("File: " + filePath);
        }
        else Permission.RequestUserPermission(Permission.ExternalStorageWrite);
    }

    public void ResetTexture()
    {
        Texture2D warpedTexture = new Texture2D(previewRawImage.mainTexture.width, previewRawImage.mainTexture.height, TextureFormat.RGB24, false);
        Graphics.CopyTexture(previewRawImage.texture, warpedTexture);
        previewRawImage.texture = warpedTexture;
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
