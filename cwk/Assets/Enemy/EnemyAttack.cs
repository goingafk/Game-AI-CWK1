using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject projectilePrefab; // The projectile prefab to be fired
    public Transform firePoint; // The point from where the projectile will be fired
    public float fireRate = 1f; // Time between each shot in seconds
    public float range = 10f; // The range within which the enemy will fire at the player
    public float projectileSpeed = 10f; // Speed of the projectile
    public Transform target; // The target (e.g., the player)

    private float nextFireTime;

    private void Update()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= range && Time.time >= nextFireTime)
        {
            FireProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void FireProjectile()
    {
        // Instantiate the projectile at the fire point
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        // Set the projectile's velocity towards the target
        Vector3 direction = (target.position - firePoint.position).normalized;
        rb.velocity = direction * projectileSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a red sphere around the enemy to visualize the attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
