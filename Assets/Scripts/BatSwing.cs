using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatSwing : MonoBehaviour
{
    public float swingSpeed = 10f; // Adjust the speed of the swing
    public float swingDistance = 1f; // Adjust the distance of the swing
    public float damage = 10f; // Amount of damage to deal to enemies

    private bool isSwinging = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isSwinging)
        {
            StartSwing();
        }
    }

    void StartSwing()
    {
        isSwinging = true;
        StartCoroutine(SwingCoroutine());
    }

    IEnumerator SwingCoroutine()
    {
        float currentPosition = 0f;

        while (currentPosition < swingDistance)
        {
            float swingStep = swingSpeed * Time.deltaTime;
            float xTarget = -currentPosition;
            float yTarget = currentPosition;
            Vector3 targetPosition = originalPosition + new Vector3(xTarget, yTarget, 0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, swingStep);

            float angle = Mathf.Lerp(0f, 180f, currentPosition / swingDistance);
            transform.localRotation = originalRotation * Quaternion.Euler(0f, 0f, angle);

            currentPosition += swingStep;
            yield return null;
        }

        // Reset position and rotation after the swing
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;

        isSwinging = false;
    }
}

