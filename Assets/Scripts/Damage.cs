using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float damage = 10f;
    public float range = 100f;
    public void TakeDamage()
    {
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
                    Destroy(bullet, 0.5f);
                }
            }
        }
    }
}