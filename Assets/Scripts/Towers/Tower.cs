using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tower : MonoBehaviour
{
    // Basic tower defence tower unit which shoots at enemies targeting the enemy that is furthest down the path
    private Transform target;

    //default stat of the tower
    public double Defaultrange = 20f;
    public double DefaultfireRate = 1.25f;
    private double DefaultfireCountdown = 0f;

    [Header("Attributes")]
    public double range;
    public double fireRate; //per second
    public int cost;
    private double fireCountdown; //time until next shot

    [Header("Unity Setup Fields")]
    public string enemyTag = "Enemy";

    public float turnSpeed = 10f;

    public GameObject bulletPrefab;
    public Transform firePoint;
    private GameObject projectileBin;

    private void Awake()
    {
        // Initialize the attributes with default values
        range = Defaultrange;
        fireRate = DefaultfireRate;
        fireCountdown = DefaultfireCountdown;
    }

    //------------------------------------------------------------------------------------------------------
    //code for switching firing mode
    public enum Mode
    {
        first,
        close,
        last
    }
    public Mode Modes;

    //targeting for close
    private void close()
    {
        float shortestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;

        foreach (Enemy enemy in WaveTracker.activeEnemies)
        {
            if (enemy == null) continue;
            
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range) //found an enemy within range
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    //targeting for first
    private void first()
    {
        float shortestDistance = Mathf.Infinity;
        Enemy CloseToEndEnemy = null;
        float ClosestToEnd = Mathf.Infinity;

        foreach (Enemy enemy in WaveTracker.activeEnemies)
        {
            if (enemy == null) continue;

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                float distanceLeft = enemy.totalDistance - enemy.totalDistanceMoved;

                if (distanceLeft < ClosestToEnd)
                {
                    ClosestToEnd = distanceLeft;
                    CloseToEndEnemy = enemy;
                }

                shortestDistance = distanceToEnemy;
            }
        }

        if (CloseToEndEnemy != null && shortestDistance <= range) // Finds enemy in range
        {
            target = CloseToEndEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    private void last()
    {
        float longestDistance = Mathf.NegativeInfinity; //initialise the distance as negative infinity so "enemy" distance will actually replace this
        Enemy furthestEnemy = null; //enemy object thats furtherest from fortress

        foreach (Enemy enemy in WaveTracker.activeEnemies) //loop through array of "enemy" in list "enemies"
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position); //calculate the distance from tower to enemy
            float distanceLeft = enemy.totalDistance - enemy.totalDistanceMoved; //calculate the distance left to fortress

            if (distanceLeft > longestDistance && distanceToEnemy <= range) //if the enemy is within range and the distance left is bigger than the longest distance
            {                                   // if the current enemy object has a bigger distance than stored one... ->
                longestDistance = distanceLeft; //replace the distance 
                furthestEnemy = enemy;          //replace the enemy object with current enemy
            }

            //Debug.Log("Checking enemy, distanceLeft: " + distanceLeft + " , longestDistance: " + longestDistance); //for debugging
        }

        // Checks that there is a furthest enemy and within range
        if (furthestEnemy != null && Vector3.Distance(transform.position, furthestEnemy.transform.position) <= range) 
        {
            target = furthestEnemy.transform; //variable for bullet to travel to "this" enemy
        }
        else
        {
            target = null;
        }
    }

    //------------------------------------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        projectileBin = GameObject.FindWithTag("EntityBin");
        //temporary way to change firing mode
        //----------------------------------------------------------------------------
        Debug.Log("Selected Firing Mode: " + Modes);
        //----------------------------------------------------------------------------
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    void UpdateTarget()
    {


    }

    // Update is called once per frame
    void Update()
    {
        //switch case, switch the firing mode in real time
        switch (Modes)
        {
            case Mode.close:
                close();
                break;

            case Mode.first:
                first();
                break;

            case Mode.last:
                last();
                break;
        }

        if (target == null) {
            return;
        }
        //---------------------------------------------------------------------

        // Lock on target
        // Quaternion is how unity represents rotation, eulerAngles is a Vector3 that represents the rotation in degrees
        // Lerp is linear interpolation, it interpolates between two values (in this between current rotation and the target rotation)
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        GameObject bulletGO = (GameObject)Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, projectileBin.transform);//(GameObject) needed when wanting to store the result of Instantiate in a GameObject variable
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.Seek(target);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, (float)range);
    }
}
