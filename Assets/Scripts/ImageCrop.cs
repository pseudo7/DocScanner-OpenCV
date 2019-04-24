using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;

public class ImageCrop : MonoBehaviour
{
    [Header("Target Image")]
    public RawImage targetImage;
    public AspectRatioFitter targetRatioFitter;

    [Header("UI Components")]
    public CanvasScaler canvasScaler;
    public RawImage warpedImage;
    public RawImage filteredImage;
    public RawImage clippedImage;
    public Material lineMat;
    public GameObject cropControlPanel;
    public GameObject warpControlPanel;

    [Header("Point Transforms")]
    public Transform leftTop;
    public Transform rightTop;
    public Transform leftBottom;
    public Transform rightBottom;

    static Camera mainCam;
    static Texture2D warpedTexture;

    //CropHandle[] cropHandles;
    public static Texture2D CroppingTexture { private set; get; }

    void Awake()
    {
        if (!mainCam) mainCam = Camera.main;
        //cropHandles = FindObjectsOfType<CropHandle>();
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
        //StartCoroutine(Capture(CroppingTexture = new Texture2D(Screen.width, Screen.height), new UnityEngine.Rect(0, 0, Screen.width, Screen.height)));
    }

    IEnumerator Cropping()
    {
        Vector2 leftBottomPos = GetResolutionBasedPosition(leftBottom.position);
        Vector2 leftTopPos = GetResolutionBasedPosition(leftTop.position);
        Vector2 rightBottomPos = GetResolutionBasedPosition(rightBottom.position);
        Vector2 rightTopPos = GetResolutionBasedPosition(rightTop.position);

        //float minValX, minValY, maxValX, maxValY;
        //minValX = minValY = float.MaxValue;
        //maxValX = maxValY = float.MinValue;

        //foreach (CropHandle handle in cropHandles)
        //{
        //    Vector3 handlePos = handle.transform.position;

        //    if (handlePos.x < minValX) minValX = handlePos.x;
        //    if (handlePos.y < minValY) minValY = handlePos.y;

        //    if (handlePos.x > maxValX) maxValX = handlePos.x;
        //    if (handlePos.y > maxValY) maxValY = handlePos.y;
        //}

        Mat mainMat = new Mat(StreamManager.WebcamSize.y, StreamManager.WebcamSize.x, CvType.CV_8UC3);

        Utils.texture2DToMat(CroppingTexture, mainMat);

        List<Point> srcPoints = new List<Point>
        {
            new Point(leftBottomPos.x, StreamManager.WebcamSize.y - leftBottomPos.y),
            new Point(rightBottomPos.x, StreamManager.WebcamSize.y - rightBottomPos.y),
            new Point(rightTopPos.x, StreamManager.WebcamSize.y - rightTopPos.y),
            new Point(leftTopPos.x, StreamManager.WebcamSize.y - leftTopPos.y),
            //new Point(leftBottom.position.x, StreamManager.WebcamSize.y - leftBottom.position.y),
            //new Point(rightBottom.position.x, StreamManager.WebcamSize.y - rightBottom.position.y),
            //new Point(rightTop.position.x, StreamManager.WebcamSize.y - rightTop.position.y),
            //new Point(leftTop.position.x, StreamManager.WebcamSize.y - leftTop.position.y),
        };

        Mat srcPointsMat = Converters.vector_Point_to_Mat(srcPoints, CvType.CV_32F);

        List<Point> dstPoints = new List<Point>
        {
            //new Point(0, 0),
            //new Point(Screen.width, 0),
            //new Point(Screen.width, Screen.height),
            //new Point(0, Screen.height),
            new Point(0, 0),
            new Point(StreamManager.WebcamSize.x, 0),
            new Point(StreamManager.WebcamSize.x, StreamManager.WebcamSize.y),
            new Point(0, StreamManager.WebcamSize.y),
            //new Point(minValX, minValY),
            //new Point(maxValX, minValY),
            //new Point(maxValX, maxValY),
            //new Point(minValX, maxValY),
        };

        Mat dstPointsMat = Converters.vector_Point_to_Mat(dstPoints, CvType.CV_32F);

        Mat M = Imgproc.getPerspectiveTransform(srcPointsMat, dstPointsMat);

        Mat warpedMat = new Mat(mainMat.size(), CvType.CV_8UC3);

        yield return new WaitForEndOfFrame();

        Imgproc.warpPerspective(mainMat, warpedMat, M, new Size(StreamManager.WebcamSize.x, StreamManager.WebcamSize.y));

        Texture2D finalTexture = new Texture2D(StreamManager.WebcamSize.x, StreamManager.WebcamSize.y, TextureFormat.RGB24, false);

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
        warpedTexture = new Texture2D(StreamManager.WebcamSize.x, StreamManager.WebcamSize.y, TextureFormat.RGB24, false);
        Graphics.CopyTexture(warpedImage.texture, warpedTexture);

        Mat initMat = new Mat(StreamManager.WebcamSize.y, StreamManager.WebcamSize.x, CvType.CV_8UC3);
        Utils.texture2DToMat(warpedTexture, initMat);

        initMat *= 1.25f;
        initMat += Scalar.all(15);

        Utils.matToTexture2D(initMat, warpedTexture);
        initMat.Dispose();
        filteredImage.texture = warpedTexture;
        warpedTexture = null;
        System.GC.Collect();
        filteredImage.gameObject.SetActive(true);
    }

    public void OrignalTexture()
    {
        warpedTexture = new Texture2D(StreamManager.WebcamSize.x, StreamManager.WebcamSize.y, TextureFormat.RGB24, false);
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
        warpedTexture = new Texture2D(StreamManager.WebcamSize.x, StreamManager.WebcamSize.y, TextureFormat.RGB24, false);
        Graphics.CopyTexture(warpedImage.texture, warpedTexture);

        Mat initMat = new Mat(StreamManager.WebcamSize.y, StreamManager.WebcamSize.x, CvType.CV_8UC3);
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
        warpedTexture = new Texture2D(StreamManager.WebcamSize.x, StreamManager.WebcamSize.y, TextureFormat.RGB24, false);
        Graphics.CopyTexture(warpedImage.texture, warpedTexture);

        Mat initMat = new Mat(StreamManager.WebcamSize.y, StreamManager.WebcamSize.x, CvType.CV_8UC3);
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
        warpedTexture = new Texture2D(StreamManager.WebcamSize.x, StreamManager.WebcamSize.y, TextureFormat.RGB24, false);
        Graphics.CopyTexture(warpedImage.texture, warpedTexture);

        Mat initMat = new Mat(StreamManager.WebcamSize.y, StreamManager.WebcamSize.x, CvType.CV_8UC3);
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

    //IEnumerator Capture(Texture2D capturedTexture, UnityEngine.Rect rect)
    //{
    //    cropControlPanel.SetActive(false);
    //    yield return new WaitForEndOfFrame();
    //    capturedTexture.ReadPixels(rect, 0, 0);
    //    capturedTexture.Apply();
    //    cropControlPanel.SetActive(true);
    //}
}
