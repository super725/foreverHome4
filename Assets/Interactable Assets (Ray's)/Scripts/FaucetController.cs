using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaucetController : MonoBehaviour
{
    private bool opening;
    
    private Transform leftKnob, rightKnob;
    private ParticleSystem waterStream;
    
    private readonly Vector3 openAngle = new(0f, 0f, 180f);
    private readonly Vector3 closeAngle = new(0f, 0f, 0f);

    void Awake()
    {
        InteractHandler.OnInteract += OnInteract;
        waterStream = transform.Find("WaterStream").gameObject.GetComponent<ParticleSystem>();
        leftKnob = transform.Find("LeftJoint");
        rightKnob = transform.Find("RightJoint");
    }

    void OnInteract(string caller)
    {
        if (caller == gameObject.name)
        {
            opening = !opening;
            if (opening)
            {
                waterStream.Play();
            }
            else
            {
                waterStream.Stop();
            }
            StopAllCoroutines();
            StartCoroutine(nameof(Animate));
        }
    }
    
    IEnumerator Animate()
    {
        float elapsedTime = 0;
        float waitTime = 1f;
        Quaternion leftStartRot = leftKnob.rotation;
        Quaternion rightStartRot = rightKnob.rotation;
        while (elapsedTime < waitTime)
        {
            leftKnob.rotation = Quaternion.Lerp(rightStartRot, Quaternion.Euler(opening ? openAngle : closeAngle), elapsedTime / waitTime);
            rightKnob.rotation = Quaternion.Lerp(leftStartRot, Quaternion.Euler(opening ? -openAngle : closeAngle), elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        leftKnob.rotation = Quaternion.Euler(opening ? openAngle : closeAngle);
        rightKnob.rotation = Quaternion.Euler(opening ? -openAngle : closeAngle);
        yield return null;
    }
}
