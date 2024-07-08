using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float fireRate = 0.75f;
    public GameObject bulletPrefab; // This can be removed if you're using pooling for bullets
    public Transform bulletPosition;
    private float nextFire; // Changed to private
    public AudioClip playerShootingAudio;
    public GameObject bulletFiringEffect;
    public int health = 100;
    public Slider healthBar;

    public delegate void EnemyKilled();
    public static event EnemyKilled OnEnemyKilled;

    private Transform playerTransform; // Cached player transform
    private AudioSource audioSource; // Cached AudioSource

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // Cache AudioSource component
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerTransform = other.transform; // Cache player transform when entering trigger
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerTransform = null; // Clear player transform when exiting trigger
        }
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            transform.LookAt(playerTransform); // Move LookAt to Update
            Fire(); // Call Fire in Update instead of OnTriggerStay
        }
    }

    void Fire()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            // Use Object Pooling instead of Instantiate
            GameObject bullet = ObjectPooler.Instance.SpawnFromPool("Bullet", bulletPosition.position, Quaternion.identity);
            bullet.GetComponent<BulletController>()?.InitializeBullet(transform.forward);

            if (audioSource && playerShootingAudio)
            {
                audioSource.PlayOneShot(playerShootingAudio); // Use cached AudioSource
            }

            VFXManager.instance.PlayVFX(bulletFiringEffect, bulletPosition.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            BulletController bullet = collision.gameObject.GetComponent<BulletController>();
            TakeDamage(bullet.damage);
        }
    }

    void TakeDamage(int damage)
    {
        health -= damage;

        healthBar.value = (float)health / 100; // Cast to float

        if (health <= 0)
        {
            EnemyDied();
        }
    }

    void EnemyDied()
    {
        gameObject.SetActive(false);

        if (OnEnemyKilled != null)
        {
            OnEnemyKilled.Invoke();
        }
    }

    private void OnEnable()
    {
        health = 100; // Reset health
        healthBar.value = 1f; // Reset health bar
    }
}
