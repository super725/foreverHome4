using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private GameObject barredDoor, unbarredDoor;
    private bool opening, barred;

    private readonly Vector3 openAngle = new(0f, -85f, 0f);
    private readonly Vector3 closeAngle = new(0f, 0f, 0f);

    void Awake()
    {
        Transform Hinge = transform.Find("Hinge");
        unbarredDoor = Hinge.Find("Unbarred").gameObject;
        barredDoor = Hinge.Find("Barred").gameObject;
        unbarredDoor.SetActive(true);
        barredDoor.SetActive(false);
        InteractHandler.OnInteract += OnInteract;
    }

    void OnInteract(string caller)
    {
        if (caller == gameObject.name)
        {
            barred = !barred;
            ToggleBar();
            
            opening = !opening;
            StopAllCoroutines();
            StartCoroutine(nameof(Animate));
        }
    }
    
    void ToggleBar()
    {
        if (barred)
        {
            unbarredDoor.SetActive(false);
            barredDoor.SetActive(true);
        }
        else
        {
            unbarredDoor.SetActive(true);
            barredDoor.SetActive(false);
        }
    }
    
    IEnumerator Animate()
    {
        Transform scriptHolder = transform.Find("Hinge");
        float elapsedTime = 0;
        float waitTime = 1f;
        Quaternion currentRot = scriptHolder.rotation;
        while (elapsedTime < waitTime)
        {
            scriptHolder.rotation = Quaternion.Lerp(currentRot, Quaternion.Euler(opening ? openAngle : closeAngle), elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;

        }
        scriptHolder.rotation = Quaternion.Euler(opening ? openAngle : closeAngle);
        yield return null;
    }
}
