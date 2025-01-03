using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// attached to tower prefabs, keeps track of tower attributes and contains targeting methods
public class Tower : MonoBehaviour {

    // set during the targeting methods to decide where the bullet is fired
    private Transform target;

    //default stat of the tower
    public float Defaultrange = 20f;
    public float DefaultfireRate = 1.25f;
    private float DefaultfireCountdown = 0f;

    [Header("Attributes")]
    public int rank = 0;
    public int cost;
    public int sellValue;
    public float range;
    public float fireRate; //per second
    public string targeting = "First";
    public string towerName;
    public GameObject tilePlacedOn;

    [Header("Bullet Attributes")]
    public float damage = 40;
    public int bulletSpeed;

    [Header("Elemental Attributes")]
    public float stunDuration = 1f;
    public float burnDuration = 3f;
    public float burnDamage = 5f;
    public float critChance = 0.2f;  // 20% chance to land a critical hit
    public float critDamage = 1.5f;  // 50% extra damage on crit (1.5 means 150%)
    public float splashRadius = 20f;

    [Header("Unity Setup Fields")]
    public string enemyTag = "Enemy";
    public float turnSpeed = 10f;
    public Transform firePoint;
    public GameObject bulletPrefab;

    private float fireCountdown; //time until next shot
    private GameObject projectileBin;

    private void Awake() {
        // Initialize the attributes with default values
        range = Defaultrange;
        fireRate = DefaultfireRate;
        fireCountdown = DefaultfireCountdown;

        // setting the sellValue
        sellValue = (int)(0.75 * cost);
    }

    //------------------------------------------------------------------------------------------------------

    //targeting for close
    private void close() {
        float shortestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;

        foreach (Enemy enemy in WaveTracker.activeEnemies) {
            if (enemy == null) continue;

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance) {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range) //found an enemy within range
        {
            target = nearestEnemy.transform;
        }
        else {
            target = null;
        }
    }

    //targeting for first
    private void first() {
        float shortestDistance = Mathf.Infinity;
        Enemy CloseToEndEnemy = null;
        float ClosestToEnd = Mathf.Infinity;

        foreach (Enemy enemy in WaveTracker.activeEnemies) {
            if (enemy == null) continue;

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance) {
                float distanceLeft = enemy.totalDistance - enemy.totalDistanceMoved;

                if (distanceLeft < ClosestToEnd) {
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
        else {
            target = null;
        }
    }

    private void last() {
        float longestDistance = Mathf.NegativeInfinity; //initialise the distance as negative infinity so "enemy" distance will actually replace this
        Enemy furthestEnemy = null; //enemy object thats furthest from fortress

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
        if (furthestEnemy != null && Vector3.Distance(transform.position, furthestEnemy.transform.position) <= range) {
            target = furthestEnemy.transform; //variable for bullet to travel to "this" enemy
        }
        else {
            target = null;
        }
    }

    //------------------------------------------------------------------------------------------------------

    void UpdateTarget() {

    }

    // Start is called before the first frame update
    void Start() {
        projectileBin = GameObject.FindWithTag("EntityBin");
        //temporary way to change firing mode
        //----------------------------------------------------------------------------
        //Debug.Log("Selected Firing Mode: " + Modes);
        //----------------------------------------------------------------------------
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    // Update is called once per frame
    void Update() {
        //switch case, switch the firing mode in real time
        switch (targeting) {
            case "Close":
                close();
                break;

            case "First":
                first();
                break;

            case "Last":
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

        if (fireCountdown <= 0f) {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot() {
        // creating a bullet
        GameObject bulletGo = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, projectileBin.transform);

        // accessing the bullet script attached to the instantiated bullet
        Bullet bulletScript = bulletGo.GetComponent<Bullet>();

        if (bulletScript != null) {
            switch (towerName) {
                case "Earth Wizard":
                    bulletScript.SeekEarth(target, damage, stunDuration);
                    break;
                case "Fire Wizard":
                    bulletScript.SeekFire(target, damage, burnDuration, burnDamage);
                    break;
                case "Wind Wizard":
                    bulletScript.SeekWind(target, damage);
                    break;
                case "Water Wizard":
                    bulletScript.SeekWater(target, damage, splashRadius);
                    break;
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
