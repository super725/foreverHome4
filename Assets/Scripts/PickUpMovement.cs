using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobAndSpin : MonoBehaviour
{
    public float height = 0.5f;
    public float speed = 1.0f;
    public float rotationSpeed = 60.0f;

    private Vector3 startingPosition;

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        float newY = startingPosition.y + Mathf.Abs(Mathf.Sin(Time.time * speed));
        transform.position = new Vector3(startingPosition.x, newY, startingPosition.z);
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
