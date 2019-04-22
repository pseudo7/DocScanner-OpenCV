using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage), typeof(AspectRatioFitter))]
public class StreamManager : MonoBehaviour
{
    public RawImage background;
    public AspectRatioFitter fitter;
    public AspectRatioFitter.AspectMode aspectMode = AspectRatioFitter.AspectMode.FitInParent;

    [Range(1, 100)]
    public int camQuality = 20;

    public bool useFrontCamera;
    public bool usePortrait;

    public WebCamTexture WebCam { private set; get; }

    void Start()
    {
        WebCamDevice[] camDevices = WebCamTexture.devices;

        if (camDevices.Length == 0)
        {
            Debug.LogWarning("No Cameras Available");
            return;
        }

        SetUpWebCam();

        if (!WebCam)
            useFrontCamera = !useFrontCamera;

        SetUpWebCam();

        if (!WebCam)
        {
            Debug.LogWarning("No Front Camera");
            return;
        }

        fitter.aspectMode = aspectMode;

        WebCam.Play();
        background.texture = WebCam;
    }

    void SetUpWebCam()
    {
        //int width = (WebCam.width / 100) * camQuality;
        //int height = (WebCam.height / 100) * camQuality;

        foreach (var cam in WebCamTexture.devices)
            if (useFrontCamera)
            {
                if (cam.isFrontFacing)
                    //WebCam = new WebCamTexture(cam.name, cam.availableResolutions[0].width, cam.availableResolutions[0].height);
                    WebCam = new WebCamTexture(cam.name, 16384, 16384);
            }
            else
            {
                if (!cam.isFrontFacing)
                    //WebCam = new WebCamTexture(cam.name, cam.availableResolutions[0].width, cam.availableResolutions[0].height);
                    WebCam = new WebCamTexture(cam.name, 16384, 16384);
            }
    }

    void Update()
    {
        fitter.aspectRatio = usePortrait ? WebCam.height / (float)WebCam.width : WebCam.width / (float)WebCam.height;
        background.rectTransform.localScale = new Vector3(1, WebCam.videoVerticallyMirrored ? -1 : 1, 1);
        background.rectTransform.localEulerAngles = new Vector3(0, 0, -WebCam.videoRotationAngle);
    }
}