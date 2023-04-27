using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CabinetController : MonoBehaviour
{
    private bool opening;
    
    private Transform leftDoor, rightDoor;
    private readonly Vector3 leftOpenAngle = new(0f, 160f, 0f);
    private readonly Vector3 rightOpenAngle = new(0f,-160f,0f);
    private readonly Vector3 closeAngle = new(0f, 0f, 0f); 
    
    void Awake()
    {
        InteractHandler.OnInteract += OnInteract;
        leftDoor = transform.Find("Left Door");
        rightDoor = transform.Find("Right Door");
    }
    void OnInteract(string caller)
    {
        if (caller == gameObject.name)
        {
            opening = !opening;
            StopAllCoroutines();
            StartCoroutine(nameof(Animate));
        }
    }
    IEnumerator Animate()
    {
        float elapsedTime = 0;
        float waitTime = 1f;
        Quaternion leftStartRot = leftDoor.rotation;
        Quaternion rightStartRot = rightDoor.rotation;
        while (elapsedTime < waitTime)
        {
            leftDoor.rotation = Quaternion.Lerp(leftStartRot, Quaternion.Euler(opening ? leftOpenAngle : closeAngle), elapsedTime / waitTime);
            rightDoor.rotation = Quaternion.Lerp(rightStartRot, Quaternion.Euler(opening ? rightOpenAngle : closeAngle), elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rightDoor.rotation = Quaternion.Euler(opening ? rightOpenAngle : closeAngle);
        leftDoor.rotation = Quaternion.Euler(opening ? leftOpenAngle : closeAngle);
        yield return null;
    }
}

