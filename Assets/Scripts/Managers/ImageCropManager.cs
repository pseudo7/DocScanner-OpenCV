using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;

public class ImageCropManager : MonoBehaviour
{
    [Header("Target Image")]
    public RawImage targetImage;
    public AspectRatioFitter targetRatioFitter;

    [Header("UI Components")]
    public CanvasScaler canvasScaler;
    public RawImage warpedImage;
    public RawImage filteredImage;
    public RawImage clippedImage;
    public GameObject cropControlPanel;
    public GameObject warpControlPanel;

    [Header("Point Transforms")]
    public Transform leftTop;
    public Transform rightTop;
    public Transform leftBottom;
    public Transform rightBottom;

    static Camera mainCam;
    static Texture2D warpedTexture;

    public static Texture2D CroppingTexture { private set; get; }

    void Awake()
    {
        if (!mainCam) mainCam = Camera.main;
        SetupFrame();
    }

    public void UpdateClippedTexture(Texture2D texture)
    {
        clippedImage.texture = texture;
    }

    public void CropTexture()
    {
        StartCoroutine(Cropping());
    }

    void SetupFrame()
    {
        canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);

        if (DDOL_Navigation.SavedTexture)
        {
            targetImage.texture = CroppingTexture = DDOL_Navigation.SavedTexture;
            targetRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            targetRatioFitter.aspectRatio = StreamManager.WebcamSize.x / (float)StreamManager.WebcamSize.y;
        }
        else Debug.LogError("Unable to capture the texture");
    }

    IEnumerator Cropping()
    {
        Vector2 leftBottomPos = GetResolutionBasedPosition(leftBottom.position);
        Vector2 leftTopPos = GetResolutionBasedPosition(leftTop.position);
        Vector2 rightBottomPos = GetResolutionBasedPosition(rightBottom.position);
        Vector2 rightTopPos = GetResolutionBasedPosition(rightTop.position);

        Mat mainMat = new Mat(StreamManager.WebcamSize.y, StreamManager.WebcamSize.x, CvType.CV_8UC3);

        Utils.texture2DToMat(CroppingTexture, mainMat);

        List<Point> srcPoints = new List<Point>
        {
            new Point(leftBottomPos.x, StreamManager.WebcamSize.y - leftBottomPos.y),
            new Point(rightBottomPos.x, StreamManager.WebcamSize.y - rightBottomPos.y),
            new Point(rightTopPos.x, StreamManager.WebcamSize.y - rightTopPos.y),
            new Point(leftTopPos.x, StreamManager.WebcamSize.y - leftTopPos.y),
        };

        Mat srcPointsMat = Converters.vector_Point_to_Mat(srcPoints, CvType.CV_32F);

        List<Point> dstPoints = new List<Point>
        {
            new Point(0, 0),
            new Point(CropSizeManager.CurrentDimmension.width, 0),
            new Point(CropSizeManager.CurrentDimmension.width, CropSizeManager.CurrentDimmension.height),
            new Point(0, CropSizeManager.CurrentDimmension.height),
        };

        Mat dstPointsMat = Converters.vector_Point_to_Mat(dstPoints, CvType.CV_32F);

        Mat M = Imgproc.getPerspectiveTransform(srcPointsMat, dstPointsMat);

        Mat warpedMat = new Mat(mainMat.size(), CvType.CV_8UC3);

        Imgproc.warpPerspective(mainMat, warpedMat, M, new Size(CropSizeManager.CurrentDimmension.width, CropSizeManager.CurrentDimmension.height));

        Texture2D finalTexture = new Texture2D(CropSizeManager.CurrentDimmension.width, CropSizeManager.CurrentDimmension.height, TextureFormat.RGB24, false);

        Utils.matToTexture2D(warpedMat, finalTexture);

        yield return new WaitForEndOfFrame();
        mainMat.Dispose();
        M.Dispose();
        warpedMat.Dispose();
        srcPointsMat.Dispose();
        dstPointsMat.Dispose();
        warpedImage.texture = finalTexture;
        finalTexture = null;
        System.GC.Collect();
        warpControlPanel.SetActive(true);
    }

    public void ColorEnhanced()
    {
        warpedTexture = new Texture2D(CropSizeManager.CurrentDimmension.width, CropSizeManager.CurrentDimmension.height, TextureFormat.RGB24, false);
        Graphics.CopyTexture(warpedImage.texture, warpedTexture);

        Mat initMat = new Mat(CropSizeManager.CurrentDimmension.height, CropSizeManager.CurrentDimmension.width, CvType.CV_8UC3);
        Utils.texture2DToMat(warpedTexture, initMat);

        initMat *= 1.15f;

        Utils.matToTexture2D(initMat, warpedTexture);
        initMat.Dispose();
        filteredImage.texture = warpedTexture;
        warpedTexture = null;
        System.GC.Collect();
        filteredImage.gameObject.SetActive(true);
    }

    public void OrignalTexture()
    {
        warpedTexture = new Texture2D(CropSizeManager.CurrentDimmension.width, CropSizeManager.CurrentDimmension.height, TextureFormat.RGB24, false);
        Graphics.CopyTexture(warpedImage.texture, warpedTexture);

        filteredImage.texture = warpedTexture;
        warpedTexture = null;
        System.GC.Collect();
        filteredImage.gameObject.SetActive(true);
    }

    Vector2 GetResolutionBasedPosition(Vector2 origPos)
    {
        float normalizedFactor = StreamManager.WebcamSize.y / (float)Screen.height;
        float width = (StreamManager.WebcamSize.x / (float)StreamManager.WebcamSize.y) * Screen.height;
        float offset = (Screen.width - width) / 2;

        Vector2 finalPos = new Vector2((origPos.x - offset) * normalizedFactor, origPos.y * normalizedFactor);

        Debug.Log(origPos);
        Debug.Log(finalPos);

        return finalPos;
    }

    public void GrayScaled()
    {
        warpedTexture = new Texture2D(CropSizeManager.CurrentDimmension.width, CropSizeManager.CurrentDimmension.height, TextureFormat.RGB24, false);
        Graphics.CopyTexture(warpedImage.texture, warpedTexture);

        Mat initMat = new Mat(CropSizeManager.CurrentDimmension.height, CropSizeManager.CurrentDimmension.width, CvType.CV_8UC3);
        Utils.texture2DToMat(warpedTexture, initMat);

        Imgproc.cvtColor(initMat, initMat, Imgproc.COLOR_BGR2GRAY);

        Utils.matToTexture2D(initMat, warpedTexture);
        initMat.Dispose();
        filteredImage.texture = warpedTexture;
        warpedTexture = null;
        System.GC.Collect();
        filteredImage.gameObject.SetActive(true);
    }

    public void BlackAndWhite()
    {
        warpedTexture = new Texture2D(CropSizeManager.CurrentDimmension.width, CropSizeManager.CurrentDimmension.height, TextureFormat.RGB24, false);
        Graphics.CopyTexture(warpedImage.texture, warpedTexture);

        Mat initMat = new Mat(CropSizeManager.CurrentDimmension.height, CropSizeManager.CurrentDimmension.width, CvType.CV_8UC3);
        Utils.texture2DToMat(warpedTexture, initMat);

        Imgproc.cvtColor(initMat, initMat, Imgproc.COLOR_BGR2GRAY);
        Imgproc.threshold(initMat, initMat, 127, 255, Imgproc.THRESH_BINARY);

        Utils.matToTexture2D(initMat, warpedTexture);
        initMat.Dispose();
        filteredImage.texture = warpedTexture;
        warpedTexture = null;
        System.GC.Collect();
        filteredImage.gameObject.SetActive(true);
    }

    public void EdgedWhite()
    {
        warpedTexture = new Texture2D(CropSizeManager.CurrentDimmension.width, CropSizeManager.CurrentDimmension.height, TextureFormat.RGB24, false);
        Graphics.CopyTexture(warpedImage.texture, warpedTexture);

        Mat initMat = new Mat(CropSizeManager.CurrentDimmension.height, CropSizeManager.CurrentDimmension.width, CvType.CV_8UC3);
        Utils.texture2DToMat(warpedTexture, initMat);

        Imgproc.cvtColor(initMat, initMat, Imgproc.COLOR_BGR2GRAY);
        Imgproc.adaptiveThreshold(initMat, initMat, 255, Imgproc.ADAPTIVE_THRESH_GAUSSIAN_C, Imgproc.THRESH_BINARY, 11, 2);

        Utils.matToTexture2D(initMat, warpedTexture);
        initMat.Dispose();
        filteredImage.texture = warpedTexture;
        warpedTexture = null;
        System.GC.Collect();
        filteredImage.gameObject.SetActive(true);
    }
}