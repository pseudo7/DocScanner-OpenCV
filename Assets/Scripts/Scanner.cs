using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;

public class Scanner
{
    static Scanner instance;

    public static Scanner Instance
    {
        get
        {
            if (instance == null)
                instance = new Scanner();
            return instance;
        }
    }

    public Texture2D Scan(Texture2D baseTexture)
    {

        Mat mainMat = new Mat(baseTexture.height, baseTexture.width, CvType.CV_8UC3);
        Mat grayMat = new Mat();


        //convert texture2d to matrix
        Utils.texture2DToMat(baseTexture, mainMat);
        //copy main matrix to grayMat
        mainMat.copyTo(grayMat);

        //convert color to gray
        Imgproc.cvtColor(grayMat, grayMat, Imgproc.COLOR_BGR2GRAY);
        //blur the image
        Imgproc.GaussianBlur(grayMat, grayMat, new Size(5, 5), 0);

        //thresholding make the image black and white
        Imgproc.threshold(grayMat, grayMat, 0, 255, Imgproc.THRESH_OTSU);
        //extract the edge of the image
        Imgproc.Canny(grayMat, grayMat, 50, 50);

        //prepare for the finding contours
        List<MatOfPoint> contours = new List<MatOfPoint>();
        //find the contour from canny edge image
        Imgproc.findContours(grayMat, contours, new Mat(), Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);


        List<MatOfPoint> tempTargets = new List<MatOfPoint>();
        for (int i = 0; i < contours.Count; i++)
        {
            MatOfPoint cp = contours[i];
            MatOfPoint2f cn = new MatOfPoint2f(cp.toArray());
            double p = Imgproc.arcLength(cn, true);

            MatOfPoint2f approx = new MatOfPoint2f();
            //convert contour to readable polygon
            Imgproc.approxPolyDP(cn, approx, 0.03 * p, true);

            // find a contour with 4 points
            if (approx.toArray().Length == 4)
            {

                MatOfPoint approxPt = new MatOfPoint();
                approx.convertTo(approxPt, CvType.CV_32S);

                float maxCosine = 0;
                for (int j = 2; j < 5; j++)
                {
                    Vector2 v1 = new Vector2((float)(approx.toArray()[j % 4].x - approx.toArray()[j - 1].x), (float)(approx.toArray()[j % 4].y - approx.toArray()[j - 1].y));
                    Vector2 v2 = new Vector2((float)(approx.toArray()[j - 2].x - approx.toArray()[j - 1].x), (float)(approx.toArray()[j - 2].y - approx.toArray()[j - 1].y));

                    float angle = Mathf.Abs(Vector2.Angle(v1, v2));
                    maxCosine = Mathf.Max(maxCosine, angle);
                }

                if (maxCosine < 135f)
                {
                    tempTargets.Add(approxPt);
                }

            }

        }

        if (tempTargets.Count > 0)
        {
            //get the first contour
            MatOfPoint approxPt = tempTargets[0];
            //making source mat

            Mat srcPointsMat = Converters.vector_Point_to_Mat(approxPt.toList(), CvType.CV_32F);

            //making destination mat
            List<Point> dstPoints = new List<Point>();
            dstPoints.Add(new Point(0, 0));
            dstPoints.Add(new Point(0, 512));
            dstPoints.Add(new Point(512, 512));
            dstPoints.Add(new Point(512, 0));
            Mat dstPointsMat = Converters.vector_Point_to_Mat(dstPoints, CvType.CV_32F);

            //make perspective transform
            Mat M = Imgproc.getPerspectiveTransform(srcPointsMat, dstPointsMat);
            Mat warpedMat = new Mat(mainMat.size(), mainMat.type());
            //crop and warp the image
            Imgproc.warpPerspective(mainMat, warpedMat, M, new Size(512, 512), Imgproc.INTER_LINEAR);

            warpedMat.convertTo(warpedMat, CvType.CV_8UC3);


            //create a empty final texture
            Texture2D finalTexture = new Texture2D(warpedMat.width(), warpedMat.height(), TextureFormat.RGB24, false);
            //convert matrix to texture 2d
            Utils.matToTexture2D(warpedMat, finalTexture);


            //Texture2D finalTexture = new Texture2D(mainMat.width(), mainMat.height(), TextureFormat.RGB24, false);
            //Utils.matToTexture2D(grayMat, finalTexture);
            return finalTexture;
        }
        return Texture2D.blackTexture;
    }

    public Texture2D ScanTesting(Texture2D baseTexture)
    {

        Mat mainMat = new Mat(baseTexture.height, baseTexture.width, CvType.CV_8UC3);
        Mat grayMat = new Mat();

        //convert texture2d to matrix
        Utils.texture2DToMat(baseTexture, mainMat);
        //copy main matrix to grayMat
        mainMat.copyTo(grayMat);

        //convert color to gray
        Imgproc.cvtColor(grayMat, grayMat, Imgproc.COLOR_BGR2GRAY);
        //blur the image
        Imgproc.GaussianBlur(grayMat, grayMat, new Size(5, 5), 0);

        //thresholding make the image black and white
        Imgproc.threshold(grayMat, grayMat, 0, 255, Imgproc.THRESH_OTSU);
        //extract the edge of the image
        Imgproc.Canny(grayMat, grayMat, 50, 50);


        //prepare for the finding contours
        List<MatOfPoint> contours = new List<MatOfPoint>();
        //find the contour from canny edge image
        Imgproc.findContours(grayMat, contours, new Mat(), Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);

        /*
         * 
         * 
        List<MatOfPoint> tempTargets = new List<MatOfPoint>();
        for (int i = 0; i < contours.Count; i++)
        {
            MatOfPoint cp = contours[i];
            MatOfPoint2f cn = new MatOfPoint2f(cp.toArray());
            double p = Imgproc.arcLength(cn, true);

            MatOfPoint2f approx = new MatOfPoint2f();
            //convert contour to readable polygon
            Imgproc.approxPolyDP(cn, approx, 0.03 * p, true);

            // find a contour with 4 points
            if (approx.toArray().Length == 4)
            {

                MatOfPoint approxPt = new MatOfPoint();
                approx.convertTo(approxPt, CvType.CV_32S);

                float maxCosine = 0;
                for (int j = 2; j < 5; j++)
                {
                    Vector2 v1 = new Vector2((float)(approx.toArray()[j % 4].x - approx.toArray()[j - 1].x), (float)(approx.toArray()[j % 4].y - approx.toArray()[j - 1].y));
                    Vector2 v2 = new Vector2((float)(approx.toArray()[j - 2].x - approx.toArray()[j - 1].x), (float)(approx.toArray()[j - 2].y - approx.toArray()[j - 1].y));

                    float angle = Mathf.Abs(Vector2.Angle(v1, v2));
                    maxCosine = Mathf.Max(maxCosine, angle);
                }

                if (maxCosine < 135f)
                {
                    tempTargets.Add(approxPt);
                }

            }

        }

        if (tempTargets.Count > 0)
        {
            //get the first contour
            MatOfPoint approxPt = tempTargets[0];
            //making source mat
            Mat srcPointsMat = Converters.vector_Point_to_Mat(approxPt.toList(), CvType.CV_32F);

            //making destination mat
            List<Point> dstPoints = new List<Point>();
            dstPoints.Add(new Point(0, 0));
            dstPoints.Add(new Point(0, 512));
            dstPoints.Add(new Point(512, 512));
            dstPoints.Add(new Point(512, 0));
            Mat dstPointsMat = Converters.vector_Point_to_Mat(dstPoints, CvType.CV_32F);

            //make perspective transform
            Mat M = Imgproc.getPerspectiveTransform(srcPointsMat, dstPointsMat);
            Mat warpedMat = new Mat(mainMat.size(), mainMat.type());
            //crop and warp the image
            Imgproc.warpPerspective(mainMat, warpedMat, M, new Size(512, 512), Imgproc.INTER_LINEAR);
            warpedMat.convertTo(warpedMat, CvType.CV_8UC3);



            //create a empty final texture
            Texture2D finalTexture = new Texture2D(warpedMat.width(), warpedMat.height(), TextureFormat.RGB24, false);
            //convert matrix to texture 2d
            Utils.matToTexture2D(warpedMat, finalTexture);

                */

        Texture2D finalTexture = new Texture2D(mainMat.width(), mainMat.height(), TextureFormat.RGB24, false);
        Utils.matToTexture2D(grayMat, finalTexture);
        return finalTexture;
        //}
        //return Texture2D.blackTexture;
    }
}