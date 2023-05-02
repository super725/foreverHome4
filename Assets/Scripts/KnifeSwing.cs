using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeSwing : MonoBehaviour
{
    public float swingSpeed = 10f; // Adjust the speed of the swing
    public float swingDistance = 1f; // Adjust the distance of the swing
    public float damage = 10f; // Amount of damage to deal to enemies

    private bool isSwinging = false;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
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
            float moveAmount = swingSpeed * Time.deltaTime;
            currentPosition += moveAmount;

            Vector3 newPosition = originalPosition + new Vector3(-currentPosition, 0f, 0f);
            transform.localPosition = newPosition;

            yield return null;
        }

        isSwinging = false;
        ResetKnifePosition();

        DealDamageToEnemies();
    }

    void ResetKnifePosition()
    {
        transform.localPosition = originalPosition;
    }

    void DealDamageToEnemies()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, swingDistance);

        foreach (RaycastHit hit in hits)
        {
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
