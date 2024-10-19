using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb; // Reference to RigidBody

    [Header("Attributes")]
    [SerializeField] private float moveSpeed; // Read move speed of an object from Unity
    [SerializeField] private float FortressDamage;

    [Header("Health")]
    public float Hp;

    private WaveManager waveManager;
    private Transform target;
    private int pathIndex = 0;

    private void Start()
    {
        waveManager = GetComponentInParent<WaveManager>();
        target = PathManager.main.path[pathIndex]; // This is for the enemy to move through waypoints
    }

    public void TakeDamage(float amount)
    {
        Hp -= amount;
        if (Hp <= 0)
        {
            waveManager.waves[waveManager.currentWave].enemiesLeft--;
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Vector3.Distance(target.position, transform.position) <= 1f)
        {
            pathIndex++;

            if (pathIndex == PathManager.main.path.Length)
            {
                Destroy(gameObject); // Enemy reached the end, destroy the object
                return;
            }
            else
            {
                target = PathManager.main.path[pathIndex];
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 direction = (target.position - transform.position);
        Vector3 movement = direction.normalized * moveSpeed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z); // Maintain y velocity for gravity
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fortress"))
        {
            // Apply damage to the fortress when enemy enters the trigger zone
            Fortress fortress = other.GetComponent<Fortress>();
            if (fortress != null)
            {
                fortress.TakeDamage(FortressDamage); 
            }

            Destroy(gameObject);
        }
    }
}
