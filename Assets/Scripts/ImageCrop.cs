using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;

public class ImageCrop : MonoBehaviour
{
    [Header("UI Components")]
    public CanvasScaler canvasScaler;
    public RawImage targetImage;
    public RawImage warpedImage;
    public RawImage filteredImage;
    public Material lineMat;
    public GameObject cropControlPanel;
    public GameObject warpControlPanel;

    [Header("Point Transforms")]
    public Transform leftTop;
    public Transform rightTop;
    public Transform leftBottom;
    public Transform rightBottom;

    static Camera mainCam;

    Texture2D croppingTexture;

    void Awake()
    {
        if (!mainCam) mainCam = Camera.main;
        SetupFrame();
    }

    public void CropTexture()
    {
        StartCoroutine(Cropping());
    }

    void SetupFrame()
    {
        canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);

        if (DDOL_Navigation.SavedTexture)
            targetImage.texture = croppingTexture = DDOL_Navigation.SavedTexture;
        else
            StartCoroutine(Capture(croppingTexture = new Texture2D(Screen.width, Screen.height), new UnityEngine.Rect(0, 0, Screen.width, Screen.height)));
    }

    IEnumerator Cropping()
    {
        Mat mainMat = new Mat(Screen.height, Screen.width, CvType.CV_8UC3);

        Utils.texture2DToMat(croppingTexture, mainMat);

        List<Point> srcPoints = new List<Point>
        {
            new Point(leftBottom.position.x, Screen.height - leftBottom.position.y),
            new Point(rightBottom.position.x, Screen.height - rightBottom.position.y),
            new Point(rightTop.position.x, Screen.height - rightTop.position.y),
            new Point(leftTop.position.x, Screen.height - leftTop.position.y),
        };

        Mat srcPointsMat = Converters.vector_Point_to_Mat(srcPoints, CvType.CV_32F);

        List<Point> dstPoints = new List<Point>
        {
            new Point(0, 0),
            new Point(Screen.width, 0),
            new Point(Screen.width, Screen.height),
            new Point(0, Screen.height),
        };

        Mat dstPointsMat = Converters.vector_Point_to_Mat(dstPoints, CvType.CV_32F);

        Mat M = Imgproc.getPerspectiveTransform(srcPointsMat, dstPointsMat);

        Mat warpedMat = new Mat(mainMat.size(), CvType.CV_8UC3);

        yield return new WaitForEndOfFrame();

        Imgproc.warpPerspective(mainMat, warpedMat, M, new Size(Screen.width, Screen.height));

        Texture2D finalTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        Utils.matToTexture2D(warpedMat, finalTexture);

        yield return new WaitForEndOfFrame();

        warpedImage.texture = finalTexture;
        warpControlPanel.SetActive(true);
    }

    public void ColorEnhanced()
    {
        Texture2D warpedTexture = new Texture2D(warpedImage.mainTexture.width, warpedImage.mainTexture.height, TextureFormat.RGB24, false);
        Graphics.CopyTexture(warpedImage.texture, warpedTexture);

        Mat initMat = new Mat(warpedTexture.height, warpedTexture.width, CvType.CV_8UC4);
        Utils.texture2DToMat(warpedTexture, initMat);

        initMat *= 1.25f;
        initMat += Scalar.all(15);

        Utils.matToTexture2D(initMat, warpedTexture);
        filteredImage.texture = warpedTexture;
        filteredImage.gameObject.SetActive(true);
    }

    public void GrayScaled()
    {
        Texture2D warpedTexture = new Texture2D(warpedImage.mainTexture.width, warpedImage.mainTexture.height, TextureFormat.RGB24, false);
        Graphics.CopyTexture(warpedImage.texture, warpedTexture);

        Mat initMat = new Mat(warpedTexture.height, warpedTexture.width, CvType.CV_8UC4);
        Utils.texture2DToMat(warpedTexture, initMat);

        Imgproc.cvtColor(initMat, initMat, Imgproc.COLOR_BGR2GRAY);

        Utils.matToTexture2D(initMat, warpedTexture);
        filteredImage.texture = warpedTexture;
        filteredImage.gameObject.SetActive(true);
    }

    public void BlackAndWhite()
    {
        Texture2D warpedTexture = new Texture2D(warpedImage.mainTexture.width, warpedImage.mainTexture.height, TextureFormat.RGB24, false);
        Graphics.CopyTexture(warpedImage.texture, warpedTexture);

        Mat initMat = new Mat(warpedTexture.height, warpedTexture.width, CvType.CV_8UC4);
        Utils.texture2DToMat(warpedTexture, initMat);

        Imgproc.cvtColor(initMat, initMat, Imgproc.COLOR_BGR2GRAY);
        Imgproc.threshold(initMat, initMat, 127, 255, Imgproc.THRESH_BINARY);

        Utils.matToTexture2D(initMat, warpedTexture);
        filteredImage.texture = warpedTexture;
        filteredImage.gameObject.SetActive(true);
    }

    public void EdgedWhite()
    {
        Texture2D warpedTexture = new Texture2D(warpedImage.mainTexture.width, warpedImage.mainTexture.height, TextureFormat.RGB24, false);
        Graphics.CopyTexture(warpedImage.texture, warpedTexture);

        Mat initMat = new Mat(warpedTexture.height, warpedTexture.width, CvType.CV_8UC4);
        Utils.texture2DToMat(warpedTexture, initMat);

        Imgproc.cvtColor(initMat, initMat, Imgproc.COLOR_BGR2GRAY);
        Imgproc.adaptiveThreshold(initMat, initMat, 255, Imgproc.ADAPTIVE_THRESH_GAUSSIAN_C, Imgproc.THRESH_BINARY, 11, 2);

        Utils.matToTexture2D(initMat, warpedTexture);
        filteredImage.texture = warpedTexture;
        filteredImage.gameObject.SetActive(true);
    }

    IEnumerator Capture(Texture2D capturedTexture, UnityEngine.Rect rect)
    {
        cropControlPanel.SetActive(false);
        yield return new WaitForEndOfFrame();
        capturedTexture.ReadPixels(rect, 0, 0);
        capturedTexture.Apply();
        cropControlPanel.SetActive(true);
    }

}
