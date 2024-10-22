using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    // a reference to RigidBody
    [SerializeField] private Rigidbody rb; 

    [Header("Attributes")]
    // read move speed of a object from unity (if we want standard move speed from another script then change this)
    [SerializeField] private float moveSpeed; 
    public int health;

    private WaveManager waveManager;
    
    public void TakeDamage(int amount) {

        health -= amount;

        if (health <= 0) {

            //waveManager.waves[waveManager.currentWave].enemiesLeft--;
            waveManager.waveTracker.EnemyKilled();
            Destroy(gameObject);
            Fortress.gold += 10;

        }

    }

    public float totalDistance = 0;

    // counts the total distance the object need to move
    public void distanceCounter() { 
    
        if (PathManager.main.path.Length > 1) {
        
            for (int i = 0; i < PathManager.main.path.Length - 1; i++) {
                
                // stores the coordinate of item in order
                Transform Item1 = PathManager.main.path[i];

                // stores the coordinate of next item
                Transform NextItem = PathManager.main.path[i + 1]; 

                float xDistance = Mathf.Abs(Item1.position.x - NextItem.position.x);
                float zDistance = Mathf.Abs(Item1.position.z - NextItem.position.z);
                totalDistance = totalDistance + xDistance + zDistance;
                //Debug.Log("X distance: " + xDistance + " Z distance: " + zDistance + ", Item1: " + Item1 + ", NextItem: " + NextItem + ", Total distance: " + totalDistance);
            
            }
        
        }
    
    }

    private Vector3 previousPosition;
    public float totalDistanceMoved;

    // counts how much the object moved
    public void DistanceMoved() {

        float distanceMoved = Vector3.Distance(previousPosition, transform.position);

        // Update the total distance moved
        totalDistanceMoved += distanceMoved;

        // update previous position to the current position
        previousPosition = transform.position; 

        // test how much the enemy moved
        //Debug.Log("Distance moved this frame: " + distanceMoved);
        //Debug.Log("Total distance moved: " + totalDistanceMoved);

    }

    private Transform target;
    private int pathIndex = 0;

    private void Start() {

        waveManager = GetComponentInParent<WaveManager>();

        //this is for enemy object to read through the array of waypoint to move to
        target = PathManager.main.path[pathIndex]; 
        distanceCounter();

        // initialise the enemies starting position
        previousPosition = transform.position;
        totalDistanceMoved = 0f;

    }

    private void Update() {

        if (Vector3.Distance(target.position, transform.position) <= 1f) {

            pathIndex++;
           
            if (pathIndex == PathManager.main.path.Length) {

                Destroy(gameObject);
                Fortress.TakeHit();
                return;

            } 

            else {

                target = PathManager.main.path[pathIndex];

            }

        }

        DistanceMoved();

    }

    private void FixedUpdate() {

        Vector3 direction = (target.position - transform.position);
        Vector3 movement = direction.normalized * moveSpeed;
        rb.velocity = new Vector3(movement.x, movement.y, movement.z);

    }

}
