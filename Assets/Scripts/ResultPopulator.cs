using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultPopulator : MonoBehaviour
{
    public RawImage grayImage;
    public RawImage resultImage;

    public Texture2D baseTexture;

    void Start()
    {
        resultImage.texture = Scanner.Instance.Scan(baseTexture);
        grayImage.texture = baseTexture; 
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene(0);
    }
}
