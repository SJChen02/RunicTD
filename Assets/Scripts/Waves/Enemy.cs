using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    // a reference to RigidBody
    [SerializeField] private Rigidbody rb;
    public Image healthBar; 

    [Header("Attributes")]
    // read move speed of a object from unity (if we want standard move speed from another script then change this)
    [SerializeField] private float moveSpeed; 
    public int health;
    private int maxHealth;

    public enum EnemyType { Fire, Water, Earth, Ice, Neutral, Wind }
    public EnemyType enemyType;
    private bool isSlowed = false;
    private bool isBurning = false;
    private bool isStunned = false;

    [SerializeField] private PathManager PathManager;

    public void SetPathManager(PathManager newPathManager) {
        PathManager = newPathManager;
    }

    // Apply a slow debuff to the enemy
    public void ApplySlowDebuff(float slowAmount, float duration) {
        if (!isSlowed)
        {
            StartCoroutine(SlowDebuff(slowAmount, duration));
        }
    }

    // Slow debuff to the enemy
    // The enemy's move speed is reduced by the slow amount for the duration of the debuff
    // After the duration, the enemy's move speed is set back to the original move speed
    private IEnumerator SlowDebuff(float slowAmount, float duration) {
        isSlowed = true;
        float originalMoveSpeed = moveSpeed;
        moveSpeed = moveSpeed * (1f - slowAmount);
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
        isSlowed = false;
    }

    // Apply a burn debuff to the enemy
    public void ApplyBurnDebuff(float burnDamage, float duration) {
        if (!isBurning)
        {
            StartCoroutine(BurnDebuff(burnDamage, duration));
        }
    }

    // Burn debuff to the enemy
    // The enemy takes damage every second for the duration of the debuff
    // The amount of damage taken is the burn damage
    private IEnumerator BurnDebuff(float burnDamage, float duration) {
        isBurning = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            TakeDamage(burnDamage);
            elapsedTime += 1f; // Apply burn damage every second
            yield return new WaitForSeconds(1f);
        }

        isBurning = false;
    }

    // Apply a stun debuff to the enemy
    public void ApplyStunDebuff(float duration) {
        if (!isStunned)
        {
            StartCoroutine(StunDebuff(duration));
        }
    }

    // Stun the enemy for a duration
    // The enemy's move speed is set to 0 for the duration of the stun
    // After the duration, the enemy's move speed is set back to the original move speed
    private IEnumerator StunDebuff(float duration) {
        isStunned = true;
        float originalMoveSpeed = moveSpeed;
        moveSpeed = 0f;
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
        isStunned = false;
    }
    
    // Take damage from a tower
    // The health of the enemy is reduced by the amount of damage taken
    // The health bar is updated to reflect the new health
    public void TakeDamage(float amount, string towerType = "Neutral") {
        int finalDamage = CalculateDamage(amount, towerType);
        health -= finalDamage;

        healthBar.fillAmount = (float)health / maxHealth;
    }

    // Calculate the damage based on the type advantage/disadvantage
    // Returns the final damage to be applied to the enemy
    private int CalculateDamage(float baseDamage, string towerType)
    {
        float multiplier = 1f;
        // Type advantage/disadvantage
        // Earth -> water -> fire -> wind -> earth
        switch (enemyType)
        {
            case EnemyType.Fire:
                if (towerType == "Fire") multiplier = 0.9f; // same type is 10% weaker
                if (towerType == "Water") multiplier = 1.2f; // advantage 20% stronger
                if (towerType == "Wind") multiplier = 0.5f; // Wind is weak against Fire
                break;

            case EnemyType.Water:
                if (towerType == "Water") multiplier = 0.9f;
                if (towerType == "Earth") multiplier = 1.2f;
                if (towerType == "Fire") multiplier = 0.5f;
                break;

            case EnemyType.Earth:
                if (towerType == "Earth") multiplier = 0.9f;
                if (towerType == "Wind") multiplier = 1.2f;
                if (towerType == "Water") multiplier = 0.5f;
                break;

            case EnemyType.Ice:
                break;
            case EnemyType.Wind:
                if (towerType == "Wind") multiplier = 0.9f;
                if (towerType == "Fire") multiplier = 1.2f;
                if (towerType == "Earth") multiplier = 0.5f;
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
        maxHealth = health;

        //spawner = GetComponentInParent<Spawner>();

        //this is for enemy object to read through the array of waypoint to move to
        target = PathManager.path[pathIndex]; 
        distanceCounter();

        // initialise the enemies starting position
        previousPosition = transform.position;
        totalDistanceMoved = 0f;

    }

    private void Update() {
        // If the health of the enemy is less than or equal to 0,
        // the enemy is killed and the enemy is unregistered from the wave tracker.
        if (health <= 0) {
            WaveTracker.EnemyKilled();
            WaveTracker.UnregisterEnemy(this);
            Destroy(gameObject);
        }
        
        // If the enemy reaches the end of the path, the enemy is destroyed and the fortress takes a hit.
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

    public float rotationSpeed = 2f;
    public bool firstRun = false;

    private void FixedUpdate() {

        Vector3 direction = (target.position - transform.position);
        Vector3 movement = direction.normalized * moveSpeed;
        rb.velocity = new Vector3(movement.x, movement.y, movement.z);

        if (direction != Vector3.zero)
        {
            Quaternion offset = Quaternion.Euler(0, 270, 0);
            Quaternion toRotation = Quaternion.LookRotation(-direction, Vector3.up) * offset;


            //Forces enemy object to instantly face the right direction when spawned
            if (firstRun == false) {
                transform.rotation = toRotation;
                firstRun = true;
            }

            //makes enemy turn smoothly with slower rotation speed
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime*10f); 
            
        }

    }

}
