using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shotgun : MonoBehaviour
{
    public ShotgunDmg gun; // Reference to the ShotgunDmg script.
    public ParticleSystem muzzleFlash;
    public float sensitivity = 2.0f; // New variable for mouse sensitivity.

    private Transform cameraTransform;
    private bool isReloading = false; // New variable to track reloading state.
    private AudioSource gunAudio;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        gunAudio = GetComponent<AudioSource>();

        // Set the gun's position to match the camera's position, but at the same height.
        Vector3 position = transform.position;
        position.x = cameraTransform.position.x;
        position.y = transform.position.y;
        position.z = cameraTransform.position.z;
        transform.position = position;
    }

    void Update()
    {
        // Make the gun follow the camera's position, but stay at the same height.
        Vector3 position = transform.position;
        position.x = cameraTransform.position.x;
        position.z = cameraTransform.position.z;
        transform.position = position;

        // Rotate the gun based on mouse input.
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        transform.Rotate(Vector3.up, mouseX, Space.World);

        // Align the gun's forward vector with the camera's forward vector.
        transform.rotation = Quaternion.LookRotation(cameraTransform.forward);

        if (Input.GetButtonDown("Fire1") && !isReloading)
        {
            gun.Shoot();
            gunAudio.Play();
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && gun.currentAmmo < gun.maxAmmo)
        {
            isReloading = true;
            StartCoroutine(Reload());
        }

        // Stop the muzzle flash particle system if the mouse button is not pressed.
        if (muzzleFlash != null && muzzleFlash.isPlaying && !Input.GetButton("Fire1"))
        {
            muzzleFlash.Stop();
        }
    }

    IEnumerator Reload()
    {
        Debug.Log("Reloading...");

        // Move the gun up.
        Vector3 startPosition = transform.localPosition;
        Vector3 endPosition = startPosition + Vector3.up * 0.5f;
        float duration = 0.2f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Move the gun down.
        startPosition = transform.localPosition;
        endPosition = startPosition - Vector3.up * 0.5f;
        duration = 0.2f;
        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        gun.currentAmmo = gun.maxAmmo;
        isReloading = false;

        Debug.Log("Finished reloading.");
    }
}
