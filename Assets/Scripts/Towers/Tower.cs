using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// attached to tower prefabs, keeps track of tower attributes and contains targeting methods
public class Tower : MonoBehaviour {

    // set during the targeting methods to decide where the bullet is fired
    private Transform target;
    //for animation controller to check if animation should be played or not
    public Transform CastCheck ; 

    [Header("Attributes")]
    public float range;
    public float fireRate; //per second
    public int path = 0;
    public int rank = 0;
    public int cost;
    public int sellValue;
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

    private float fireCountdown = 0f; //time until next shot
    private GameObject projectileBin;

    private void Awake() {
        // updating the tower with global buffs
        switch (towerName) {
            case "Earth Wizard":
                fireRate += FortressRunes.earthFireRateIncrease;
                range += FortressRunes.earthRangeIncrease;
                break;
            case "Fire Wizard":
                fireRate += FortressRunes.fireFireRateIncrease;
                range += FortressRunes.fireRangeIncrease;
                break;
            case "Wind Wizard":
                fireRate += FortressRunes.windFireRateIncrease;
                range += FortressRunes.windRangeIncrease;
                break;
            case "Water Wizard":
                fireRate += FortressRunes.waterFireRateIncrease;
                range += FortressRunes.waterRangeIncrease;
                break;
        }

        // setting the sellValue
        sellValue = (int)(0.75 * cost);
    }

    //------------------------------------------------------------------------------------------------------

    //targeting for close
    private void close() {
        float shortestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;

        // for each enemy in the activeEnemies list, check if the enemy is within range and if the distance is smaller than the shortest distance
        // if it is, set the enemy as the nearest enemy
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
            CastCheck = target;
            target = nearestEnemy.transform;
        }
        else {
            CastCheck = null; 
            target = null;
        }
    }

    //targeting for first
    private void first()
    {
        Enemy CloseToEndEnemy = null;
        float ClosestToEnd = Mathf.Infinity;

        // for each enemy in the activeEnemies list, check if the enemy is within range and if the distance left is smaller than the closest distance to the end
        // if it is, set the enemy as the closest enemy to the end and set target to that enemy
        foreach (Enemy enemy in WaveTracker.activeEnemies)
        {
            if (enemy == null) continue;

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            float distanceLeft = enemy.totalDistance - enemy.totalDistanceMoved;
            if (distanceLeft < ClosestToEnd && distanceToEnemy <= range)
            {
                ClosestToEnd = distanceLeft;
                CloseToEndEnemy = enemy;
            }

        }

        if (CloseToEndEnemy != null) // Finds enemy in range
        {
            CastCheck = target;
            target = CloseToEndEnemy.transform;
        }
        else
        {
            CastCheck = null;
            target = null;
        }
    }

    // targeting for last
    private void last() {
        float longestDistance = Mathf.NegativeInfinity;
        Enemy furthestEnemy = null; 

        // for each enemy in the activeEnemies list, check if the enemy is within range and if the distance left is bigger than the longest distance
        // if it is, set the enemy as the furthest enemy
        foreach (Enemy enemy in WaveTracker.activeEnemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position); 
            float distanceLeft = enemy.totalDistance - enemy.totalDistanceMoved; 

            if (distanceLeft > longestDistance && distanceToEnemy <= range)
            {                                   
                longestDistance = distanceLeft; 
                furthestEnemy = enemy;          
            }

            //Debug.Log("Checking enemy, distanceLeft: " + distanceLeft + " , longestDistance: " + longestDistance);
        }

        // Checks that there is a furthest enemy and within range
        if (furthestEnemy != null && Vector3.Distance(transform.position, furthestEnemy.transform.position) <= range) {
            CastCheck = target; 
            target = furthestEnemy.transform; 
        }
        else {
            CastCheck = null;
            target = null;
        }
    }

    //------------------------------------------------------------------------------------------------------

    void UpdateTarget() {

    }

    // Start is called before the first frame update
    void Start() {
        projectileBin = GameObject.FindWithTag("EntityBin");
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
        // find the direction to the target and rotate the tower towards it
        Quaternion offset = Quaternion.Euler(0, 270, 0);
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(-dir, Vector3.up) * offset;
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
                    SoundManager.PlaySound(SoundType.EarthCast, 0.1f);
                    bulletScript.SeekEarth(target,
                                           FortressRunes.earthDmgMulti * damage,
                                           FortressRunes.stunDurationMulti * stunDuration);
                    break;
                case "Fire Wizard":
                    SoundManager.PlaySound(SoundType.FireCast, 0.1f);
                    bulletScript.SeekFire(target,
                                          FortressRunes.fireDmgMulti * damage,
                                          FortressRunes.burnDurationMulti * burnDuration,
                                          FortressRunes.burnDmgMulti * burnDamage);
                    break;
                case "Wind Wizard":
                    SoundManager.PlaySound(SoundType.WindCast, 0.1f);
                    bulletScript.SeekWind(target,
                                          FortressRunes.windDmgMulti * damage,
                                          FortressRunes.critChanceMulti * critChance,
                                          FortressRunes.critDmgMulti * critDamage);
                    break;
                case "Water Wizard":
                    SoundManager.PlaySound(SoundType.WaterCast, 0.1f);
                    bulletScript.SeekWater(target,
                                           FortressRunes.waterDmgMulti * damage,
                                           FortressRunes.splashRadiusMulti * splashRadius);
                    break;
            }
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
