using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private Ragdoll ragdoll;

    void Start()
    {
        currentHealth = maxHealth;
        ragdoll = GetComponent<Ragdoll>();
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth != null)
        {
            currentHealth -= damage;
            Debug.Log("Enemy health: " + currentHealth);
            if (currentHealth <= 0)
            {
                ragdoll.ActivateRagdoll();
                Destroy(gameObject, 5f);
            }
        }
    }
}