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

    public enum EnemyType { Fire, Water, Earth, Ice, Neutral }
    public EnemyType enemyType;
    private bool isSlowed = false;
    private bool isBurning = false;
    private bool isStunned = false;

    [SerializeField] private PathManager PathManager;

    //private Spawner spawner;

    public void SetPathManager(PathManager newPathManager) {
        PathManager = newPathManager;
    }

    public void ApplySlowDebuff(float slowAmount, float duration) {
        if (!isSlowed)
        {
            StartCoroutine(SlowDebuff(slowAmount, duration));
        }
    }

    private IEnumerator SlowDebuff(float slowAmount, float duration) {
        isSlowed = true;
        float originalMoveSpeed = moveSpeed;
        moveSpeed = moveSpeed * (1f - slowAmount);
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
        isSlowed = false;
    }

    public void ApplyBurnDebuff(float burnDamage, float duration) {
        if (!isBurning)
        {
            StartCoroutine(BurnDebuff(burnDamage, duration));
        }
    }

    private IEnumerator BurnDebuff(float burnDamage, float duration) {
        isBurning = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            TakeDamage((int)burnDamage);
            elapsedTime += 1f; // Apply burn damage every second
            yield return new WaitForSeconds(1f);
        }

        isBurning = false;
    }

    public void ApplyStunDebuff(float duration) {
        if (!isStunned)
        {
            StartCoroutine(StunDebuff(duration));
        }
    }

    private IEnumerator StunDebuff(float duration) {
        isStunned = true;
        float originalMoveSpeed = moveSpeed;
        moveSpeed = 0f;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
        isStunned = false;
    }
    
    public void TakeDamage(int amount, string towerType = "Neutral") {

        int finalDamage = CalculateDamage(amount, towerType);
        health -= finalDamage;

        if (health <= 0) {
            WaveTracker.EnemyKilled();
            WaveTracker.UnregisterEnemy(this);
            Destroy(gameObject);
            Fortress.gold += 10;
        }

    }

    private int CalculateDamage(int baseDamage, string towerType)
    {
        float multiplier = 1f;
        // EDIT THIS SWITCH STATEMENT TO ADD MORE ENEMY TYPES
        switch (enemyType)
        {
            case EnemyType.Fire:
                if (towerType == "Fire") multiplier = 0.9f; // same type is 10% weaker
                if (towerType == "Water") multiplier = 1.2f; // advantage 20% stronger
                if (towerType == "Ice") multiplier = 0.5f; // Rock is weak against Fire
                break;

            case EnemyType.Water:
                break;

            case EnemyType.Earth:
                break;

            case EnemyType.Ice:
                break;

            default: // Neutral type
                multiplier = 1f;
                break;
        }

        return Mathf.RoundToInt(baseDamage * multiplier);
    }
    
    public float totalDistance = 0;

    // counts the total distance the object need to move
    public void distanceCounter() { 
    
        if (PathManager.path.Length > 1) {
        
            for (int i = 0; i < PathManager.path.Length - 1; i++) {
                
                // stores the coordinate of item in order
                Transform Item1 = PathManager.path[i];

                // stores the coordinate of next item
                Transform NextItem = PathManager.path[i + 1]; 

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

        //spawner = GetComponentInParent<Spawner>();

        //this is for enemy object to read through the array of waypoint to move to
        target = PathManager.path[pathIndex]; 
        distanceCounter();

        // initialise the enemies starting position
        previousPosition = transform.position;
        totalDistanceMoved = 0f;

    }

    private void Update() {

        if (Vector3.Distance(target.position, transform.position) <= 1f) {

            pathIndex++;
           
            if (pathIndex == PathManager.path.Length) {

                Destroy(gameObject);
                WaveTracker.UnregisterEnemy(this);
                Fortress.TakeHit();
                return;

            } 

            else {

                target = PathManager.path[pathIndex];

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
