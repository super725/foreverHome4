using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public float damage = 50f;
    public float range = 2f;
    public float swipeSpeed = 10f;
    public float swipeAngle = 90f;
    public KeyCode meleeKey = KeyCode.V;

    private bool isSwiping = false;
    private Quaternion initialRotation;
    private AudioSource weaponAudio;

    void Start()
    {
        initialRotation = transform.localRotation;
        weaponAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(meleeKey) && !isSwiping)
        {
            StartCoroutine(Swipe());
        }
    }

    IEnumerator Swipe()
    {
        isSwiping = true;
        weaponAudio.Play();

        // Rotate the weapon to the right.
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0f, swipeAngle, 0f);
        float elapsed = 0f;
        while (elapsed < swipeSpeed / 2f)
        {
            transform.localRotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed / (swipeSpeed / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Do a raycast to check if the weapon hits an enemy.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.right, out hit, range))
        {
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        // Rotate the weapon back to the initial position.
        targetRotation = initialRotation * Quaternion.Euler(0f, -swipeAngle, 0f);
        elapsed = 0f;
        while (elapsed < swipeSpeed / 2f)
        {
            transform.localRotation = Quaternion.Slerp(targetRotation, initialRotation, elapsed / (swipeSpeed / 2f));
            elapsed += Time.deltaTime;
            yield return null;
        }

        isSwiping = false;
    }
}
