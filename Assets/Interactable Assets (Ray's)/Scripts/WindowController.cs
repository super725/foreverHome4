using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowController : MonoBehaviour
{
    private GameObject clearWindow, barredWindow;
    private bool opening;
    private void Awake()
    {
        InteractHandler.OnInteract += OnInteract;
        clearWindow = transform.Find("Window_Unbarred").gameObject;
        barredWindow = transform.Find("Window_Barred").gameObject;
        clearWindow.SetActive(true);
        barredWindow.SetActive(false);
    }

    void OnInteract(string caller)
    {
        if (caller == gameObject.name)
        {
            opening = !opening;
            if (opening)
            {
                clearWindow.SetActive(false);
                barredWindow.SetActive(true);
            }
            else
            {
                clearWindow.SetActive(true);
                barredWindow.SetActive(false);
            }
        }
    }
}
