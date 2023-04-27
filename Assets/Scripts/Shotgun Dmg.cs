using UnityEngine;
using System.Collections;

public class ShotgunDmg : MonoBehaviour
{
    public Transform bulletSpawnPoint; // Reference to the bullet spawn point.
    public GameObject bulletPrefab; // Reference to the bullet prefab.
    public float damage = 10f;
    public float range = 100f;
    public ParticleSystem muzzleFlash;
    public int maxAmmo = 30;
    public float reloadTime = 2f;
    public float shootCooldown = 0.5f; // Cooldown period between shots.

    [HideInInspector]
    public int currentAmmo;

    private Transform cameraTransform;
    private bool canShoot = true; // Flag to track if shooting is allowed.

    void Start()
    {
        cameraTransform = Camera.main.transform;
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            Shoot();
        }
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

        // Spawn the bullet prefab at the bullet spawn point position and rotation.
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

        // Set the bullet's velocity to move straight in the shooting direction.
        if (bulletRigidbody != null)
        {
            bulletRigidbody.velocity = bulletSpawnPoint.forward * range;
        }

        // Cast a ray from the bullet spawn point in the direction of its forward vector.
        Ray ray = new Ray(bulletSpawnPoint.position, bulletSpawnPoint.forward);
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

        canShoot = false; // Disable shooting temporarily.
        StartCoroutine(EnableShootingAfterCooldown()); // Enable shooting after the cooldown period.

        Destroy(bullet, 0.5f); // Destroy the bullet after half a second
    }

    IEnumerator EnableShootingAfterCooldown()
    {
        yield return new WaitForSeconds(shootCooldown); // Wait for the cooldown period.
        canShoot = true; // Enable shooting.
    }
}
