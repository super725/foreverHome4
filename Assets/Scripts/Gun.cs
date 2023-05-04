using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Gun : MonoBehaviour
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

    public TextMeshProUGUI ammoText;

    private Transform cameraTransform;
    private bool canShoot = true;
    private bool isReloading = false;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        currentAmmo = maxAmmo;
        UpdateAmmoText();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    public void Shoot()
    {
        if (!canShoot || isReloading) // Check if shooting is currently allowed
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
        UpdateAmmoText();

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

        canShoot = false; // Disable shooting temporarily
        StartCoroutine(EnableShootingAfterCooldown());

        Destroy(bullet, 0.5f);
    }

    IEnumerator EnableShootingAfterCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true; // Enable shooting after the cooldown
    }

    void Reload()
    {
        if (isReloading || currentAmmo == maxAmmo) // Check if already reloading or ammo is full
        {
            return;
        }

        StartCoroutine(ReloadCoroutine());
    }

    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        canShoot = false;

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        UpdateAmmoText();

        isReloading = false;
        canShoot = true;
    }

    void UpdateAmmoText()
    {
        string gunName = gameObject.name; // Get the name of the current gun.
        ammoText.text = gunName + " Ammo: " + currentAmmo.ToString() + "/" + maxAmmo.ToString();

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
