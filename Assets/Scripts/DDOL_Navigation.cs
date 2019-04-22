using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class DDOL_Navigation : MonoBehaviour
{
    public static DDOL_Navigation Instance;

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

    void Awake()
    {
        Granted = new System.Action<string>(GrantedCallback);
        Denied = new System.Action<string>(DeniedCallback);
        DontAskDenied = new System.Action<string>(DontAskDeniedCallback);

        AndroidPermissionCallback permissionCallback = new AndroidPermissionCallback(Granted, Denied, DontAskDenied);

        AndroidPermissionsManager.RequestPermission(new string[] { Permission.Camera, Permission.ExternalStorageWrite }, permissionCallback);

        blocker.SetActive(!(Permission.HasUserAuthorizedPermission(Permission.Camera) && Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite)));

        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
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

    /*
    void Start()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
            dialog = new GameObject();
        }
#endif
    }

    void OnGUI()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            // The user denied permission to use the microphone.
            // Display a message explaining why you need it with Yes/No buttons.
            // If the user says yes then present the request again
            // Display a dialog here.
            dialog.AddComponent<PermissionsRationaleDialog>();
            return;
        }
        else if (dialog != null)
        {
            Destroy(dialog);
        }
#endif

        // Now you can do things with the microphone
    }*/

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void CaptureTexture()
    {
        Texture2D capturedTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        StartCoroutine(Capture(capturedTexture));

        capturedImage.texture = capturedTexture;
        capturedImage.gameObject.SetActive(true);
        SavedTexture = capturedTexture;
    }

    public void ScanDocument()
    {
        if (!SavedTexture) SavedTexture = testingTexture;
        SceneManager.LoadScene(1);
    }

    public void ResetTexture()
    {
        capturedImage.gameObject.SetActive(false);
        SavedTexture = null;
    }

    IEnumerator Capture(Texture2D capturedTexture)
    {
        controlsParent.SetActive(false);
        cropBorder.SetActive(false);
        yield return new WaitForEndOfFrame();

        Debug.Log("Cam Texture: " + streamManager.WebCam.width + "X" + streamManager.WebCam.height);

        capturedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        capturedTexture.Apply();

        cropBorder.SetActive(true);
        controlsParent.SetActive(true);
    }
}
