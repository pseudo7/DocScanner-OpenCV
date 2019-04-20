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
    public Texture2D testingTexture;
    public StreamManager streamManager;
    public GameObject controlsParent;
    public RawImage capturedImage;

    public static Texture2D SavedTexture { private set; get; }

    void Awake()
    {
        blocker.SetActive(!Permission.HasUserAuthorizedPermission(Permission.Camera));

        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        capturedImage.gameObject.SetActive(false);
    }

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
        yield return new WaitForEndOfFrame();

        capturedTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        capturedTexture.Apply();

        controlsParent.SetActive(true);
    }
}
