using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public RangedEnemyBehaviorSight behavior;
    [HideInInspector]
    public SkinnedMeshRenderer skinnedMeshRenderer;
    [HideInInspector]
    public float health;
    public float blinkIntensity;
    public float blinkDuration;
    float blinkTimer;

    private void Start()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer == null)
        {
            skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
        }
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    public void OnRaycastHit(Gun weapon, Vector3 direction)
    {
        Debug.Log("Hitbox was hit by a bullet!");
        blinkTimer = blinkDuration;
        TakeDamage(weapon.damage, direction);
    }

    public void TakeDamage(float damage, Vector3 direction)
    {
        // Apply damage logic here
        health -= damage;
        // Apply any other effects or behavior based on the damage

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        behavior.TakeDamage(health); // Notify the enemy behavior about the enemy's death
        Destroy(gameObject); // Destroy the hitbox object
    }

    private void Update()
    {
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        float intensity = lerp * blinkIntensity;
        skinnedMeshRenderer.material.color = Color.red * intensity;
    }
}
