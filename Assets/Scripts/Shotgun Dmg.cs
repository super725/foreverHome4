using UnityEngine;
using System.Collections;
public class ShotgunDmg : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public ParticleSystem muzzleFlash;
    public int maxAmmo = 30;
    public float reloadTime = 2f;

    [HideInInspector]
    public int currentAmmo;

    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        currentAmmo = maxAmmo;
    }

    public void Shoot()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        // Trigger the muzzle flash particle system.
        if (muzzleFlash != null && !muzzleFlash.isPlaying)
        {
            muzzleFlash.Play();
        }

        currentAmmo--;

        // Cast multiple rays in a cone shape from the center of the screen in the direction of the camera's forward vector.
        int numRays = 10; // number of rays to cast
        float coneAngle = 10f; // angle of the cone in degrees
        for (int i = 0; i < numRays; i++)
        {
            float angle = Random.Range(-coneAngle / 2f, coneAngle / 2f);
            Quaternion rotation = Quaternion.AngleAxis(angle, cameraTransform.right);
            Vector3 direction = rotation * cameraTransform.forward;

            Ray ray = new Ray(cameraTransform.position, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, range))
            {
                // Apply damage to the hit object if it has a collider.
                if (hit.collider != null)
                {
                    Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage);
                    }
                }
            }
        }
    }
}