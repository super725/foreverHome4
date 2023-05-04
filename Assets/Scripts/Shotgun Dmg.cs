using UnityEngine;
using System.Collections;
using TMPro;

public class ShotgunDmg : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float damage = 10f;
    public float range = 100f;
    public ParticleSystem muzzleFlash;
    public int maxAmmo = 30;
    public float reloadTime = 2f;
    public float shootCooldown = 0.5f;

    [HideInInspector]
    public int currentAmmo;

    private Transform cameraTransform;
    private bool canShoot = true;
    private bool isReloading = false;

    public TextMeshProUGUI ammoText; // Reference to the ammo text UI element.

    void Start()
    {
        cameraTransform = Camera.main.transform;
        currentAmmo = maxAmmo;

        UpdateAmmoText(); // Update the ammo text at the start.
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && canShoot && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }

    public void Shoot()
    {
        if (!canShoot || isReloading)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo!");
            canShoot = false;
            return;
        }

        if (muzzleFlash != null && !muzzleFlash.isPlaying)
        {
            muzzleFlash.Play();
        }

        currentAmmo--;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();

        if (bulletRigidbody != null)
        {
            bulletRigidbody.velocity = bulletSpawnPoint.forward * range;
        }

        Ray ray = new Ray(bulletSpawnPoint.position, bulletSpawnPoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            if (hit.collider != null)
            {
                Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }

        canShoot = false;
        StartCoroutine(EnableShootingAfterCooldown());

        Destroy(bullet, 0.5f);

        UpdateAmmoText(); // Update the ammo text after shooting.
    }

    IEnumerator EnableShootingAfterCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadTime);
        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, currentAmmo);
        currentAmmo -= ammoToReload;

        yield return new WaitForSeconds(reloadTime);
        currentAmmo += ammoToReload;
        isReloading = false;

        UpdateAmmoText(); // Update the ammo text after reloading.
    }

    void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            ammoText.text = "Ammo: " + currentAmmo.ToString() + "/" + maxAmmo.ToString();

            if (currentAmmo <= 0)
            {
                ammoText.color = Color.red; // Turn the text red when out of ammo.
            }
            else
            {
                ammoText.color = Color.white; // Reset the text color to white.
            }
        }
    }
}