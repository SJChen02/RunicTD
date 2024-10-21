using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb; //a reference to RigidBody

    [Header("Attributes")]
    [SerializeField] private float moveSpeed;  //read move speed of a object from unity (if we want standard move speed from another script then change this).

    [Header("Health")]
    public float Hp;

    private WaveManager waveManager;

    private Transform target;
    private int pathIndex = 0;


    public void TakeDamage(float amount)
    {
        Hp -= amount;
        if (Hp <= 0)
        {
            waveManager.waves[waveManager.currentWave].enemiesLeft--;
            //waveManager.waveTracker.EnemyKilled();
            Destroy(gameObject);
            FortressGold.gold += 20;
        }
    }


    public float totalDistance = 0;
    public void distanceCounter() //counts the total distance the object need to move
    {
        if (PathManager.main.path.Length > 1)
        {



            for (int i = 0; i < PathManager.main.path.Length - 1; i++)
            {

                Transform Item1 = PathManager.main.path[i]; //stores the coordinate of item in order
                Transform NextItem = PathManager.main.path[i + 1]; //stores the coordinate of next item

                float xDistance = Mathf.Abs(Item1.position.x - NextItem.position.x);
                float zDistance = Mathf.Abs(Item1.position.z - NextItem.position.z);
                totalDistance = totalDistance + xDistance + zDistance;
                //Debug.Log("X distance: " + xDistance + " Z distance: " + zDistance + ", Item1: " + Item1 + ", NextItem: " + NextItem + ", Total distance: " + totalDistance);
            }
        }
    }


    private Vector3 previousPosition;
    public float totalDistanceMoved;
    public void DistanceMoved() //counts how much the object moved
    {
        float distanceMoved = Vector3.Distance(previousPosition, transform.position);
        
        totalDistanceMoved += distanceMoved; // Update the total distance moved
        
        previousPosition = transform.position; // update previous position to the current position

        // test how much the enemy moved
        //Debug.Log("Distance moved this frame: " + distanceMoved);
        //Debug.Log("Total distance moved: " + totalDistanceMoved);
    }



    private void Start()
    {
        waveManager = GetComponentInParent<WaveManager>();
        target = PathManager.main.path[pathIndex]; //this is for enemy object to read through the array of waypoint to move to
        distanceCounter();


        //initialise the enemies starting position
        previousPosition = transform.position;
        totalDistanceMoved = 0f;

    }

    private void Update()
    {
        if (Vector3.Distance(target.position, transform.position) <= 1f)
        {
            pathIndex++;
           

            if(pathIndex == PathManager.main.path.Length)
            {
                Destroy(gameObject);
                return;
            } 
            else
            {
                target = PathManager.main.path[pathIndex];
            }
        }
        DistanceMoved();
    }

    private void FixedUpdate()
    {
        Vector3 direction = (target.position - transform.position);



        Vector3 movement = direction.normalized * moveSpeed;
        rb.velocity = new Vector3(movement.x, movement.y, movement.z);

    }
}
