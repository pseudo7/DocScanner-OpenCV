using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;

public class Warp : MonoBehaviour
{
    public Texture2D scannedTexture;
    public RawImage croppedImage;
    public RawImage warpedImage;

    IEnumerator Start()
    {
        WarpTexture();
        yield return new WaitForEndOfFrame();
    }

    void WarpTexture()
    {
        Texture2D inputTexture = scannedTexture;

        croppedImage.texture = inputTexture;

        Mat inputMat = new Mat(inputTexture.height, inputTexture.width, CvType.CV_8UC4);

        Utils.texture2DToMat(inputTexture, inputMat);

        Mat dst_mat = new Mat(4, 1, CvType.CV_32FC2);

        double width = inputMat.cols();
        double height = inputMat.rows();

        List<Point> points = new List<Point>
        {
            new Point(0, 20),
            new Point(220, 0),
            new Point(30, height),
            new Point(width, 330),
        };

        Mat src_mat = Converters.vector_Point_to_Mat(points, CvType.CV_32F);

        dst_mat.put(0, 0, 0.0, 0.0, width, 0.0, 0.0, height, width, height);

        Mat perspectiveTransform = Imgproc.getPerspectiveTransform(src_mat, dst_mat);
        Mat outputMat = new Mat(new Size(width, height), CvType.CV_8UC4);

        Imgproc.warpPerspective(inputMat, outputMat, perspectiveTransform, new Size(width, height));
        Texture2D outputTexture = new Texture2D((int)(width), (int)(height), TextureFormat.RGB24, false);

        Utils.matToTexture2D(outputMat, outputTexture);
        warpedImage.texture = outputTexture;
    }

}
