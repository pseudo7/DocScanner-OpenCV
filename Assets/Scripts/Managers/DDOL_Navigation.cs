﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class DDOL_Navigation : MonoBehaviour
{
    public static DDOL_Navigation Instance;

    public CanvasScaler canvasScaler;
    public GameObject blocker;
    public GameObject cropBorder;
    public Texture2D testingTexture;
    public StreamManager streamManager;
    public GameObject controlsParent;
    public RawImage capturedImage;
    event System.Action<string> Granted;
    event System.Action<string> Denied;
    event System.Action<string> DontAskDenied;

    public static Texture2D SavedTexture { private set; get; }
    public static Vector2Int TextureSizeDelta { private set; get; }

    void Awake()
    {
        Granted = new System.Action<string>(GrantedCallback);
        Denied = new System.Action<string>(DeniedCallback);
        DontAskDenied = new System.Action<string>(DontAskDeniedCallback);
        AndroidPermissionCallback permissionCallback = new AndroidPermissionCallback(Granted, Denied, DontAskDenied);

        AndroidPermissionsManager.RequestPermission(new string[] { Permission.Camera, Permission.ExternalStorageWrite }, permissionCallback);

        canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
        blocker.SetActive(!(Permission.HasUserAuthorizedPermission(Permission.Camera) && Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite)));

        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        capturedImage.gameObject.SetActive(false);
    }

    void GrantedCallback(string callback)
    {
        Debug.LogError("GrantedCallback: " + callback);
    }
    void DeniedCallback(string callback)
    {
        Debug.LogError("DeniedCallback: " + callback);
    }
    void DontAskDeniedCallback(string callback)
    {
        Debug.LogError("DontAskDeniedCallback: " + callback);
    }

    void Update()
    {
        CheckForBackPress();
    }

    void CheckForBackPress()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (SceneManager.GetActiveScene().buildIndex == 0)
                Application.Quit();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void CaptureTexture()
    {
        Texture2D capturedTexture = new Texture2D(StreamManager.WebcamSize.x, StreamManager.WebcamSize.y, TextureFormat.RGB24, false);

        StartCoroutine(Capture(capturedTexture));

        capturedImage.texture = capturedTexture;
        capturedImage.gameObject.SetActive(true);
        SavedTexture = capturedTexture;
    }

    public void ScanDocument()
    {
        if (!SavedTexture) SavedTexture = testingTexture;
        streamManager.WebCam.Stop();
        SceneManager.LoadScene(1);
    }

    IEnumerator Capture(Texture2D capturedTexture)
    {
        streamManager.WebCam.autoFocusPoint = null;
        controlsParent.SetActive(false);
        cropBorder.SetActive(false);

        Debug.Log("Texture: " + capturedTexture.width + "X" + capturedTexture.height);

        Color32[] colors = streamManager.WebCam.GetPixels32();
        capturedTexture.filterMode = FilterMode.Point;
        Debug.Log("Pixels: " + colors.Length);
        capturedTexture.SetPixels32(colors);
        yield return new WaitForEndOfFrame();
        capturedTexture.Apply();

        cropBorder.SetActive(true);
        controlsParent.SetActive(true);
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Pseudo/Capture")]
    static void Capture()
    {
        ScreenCapture.CaptureScreenshot(string.Format("{0}.png", System.DateTime.Now.Ticks.ToString()));
    }
#endif
}
