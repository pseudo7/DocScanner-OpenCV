using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour
{
    public GameObject[] navigationPoints;

    void Update()
    {
        OnBackPress();
    }

    void OnBackPress()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (navigationPoints[3].activeInHierarchy)
            {
                navigationPoints[3].SetActive(false);
                navigationPoints[2].SetActive(true);
            }
            else if (navigationPoints[2].activeInHierarchy)
            {
                navigationPoints[2].SetActive(false);
                navigationPoints[1].SetActive(true);
            }
            else if (navigationPoints[1].activeInHierarchy)
            {
                navigationPoints[1].SetActive(false);
                navigationPoints[0].SetActive(true);
            }
            else UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
