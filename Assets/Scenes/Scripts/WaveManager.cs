using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private Enemy e1;
    [SerializeField] private Enemy e2;
    [SerializeField] private float countdown;
    [SerializeField] private GameObject spawnPoint;
    public Wave[] waves;
    public int currentWave = 0;
    private bool readyToCountdown;

    private void Start()
    {
        readyToCountdown = true;
        waves = new Wave[]
        {
            new Wave
            {
            enemies = new Enemy[] { e1, e1 },
            enemySpawnCooldown = 1.0f,
            waveCooldown = 5.0f
            },
            new Wave
            {
            enemies = new Enemy[] { e1, e1, e1, e2, e2, e1, e1, e1, e1, e1 },
            enemySpawnCooldown = 1.5f,
            waveCooldown = 6.0f
            },
            // Add more waves as needed
        };
    
        // Set the enemiesLeft variable to the length of the enemies array
        foreach (Wave wave in waves)
        {
            wave.enemiesLeft = wave.enemies.Length;
        }
    }

    private void Update()
    {
        if (currentWave >= waves.Length)
        {
            Debug.Log("All waves completed");
            enabled = false; // Disables the script
            return;
        }

        if (readyToCountdown == true)
        {
            countdown -= Time.deltaTime;
        }

        if (countdown <= 0)
        {
            readyToCountdown = false;
            countdown = waves[currentWave].waveCooldown;
            StartCoroutine(SpawnWave());
        }

        if (waves[currentWave].enemiesLeft == 0)
        {
            readyToCountdown = true;
            currentWave++;
        }
    }

    private IEnumerator SpawnWave() // IEnumerator is a type of function that can be paused and resumed
    {
        if (currentWave < waves.Length)
        {
            Debug.Log("Starting wave " + (currentWave + 1));
            foreach (Enemy enemy in waves[currentWave].enemies)
            {
                Enemy spawnedEnemy = Instantiate(enemy, spawnPoint.transform);
                spawnedEnemy.transform.SetParent(spawnPoint.transform);
                yield return new WaitForSeconds(waves[currentWave].enemySpawnCooldown); // Pauses until the time has passed
            }
        }

    }
}

[System.Serializable] // This attribute allows us to see the class in the inspector
public class Wave
{
    public Enemy[] enemies;
    public float enemySpawnCooldown;
    public float waveCooldown;

    [HideInInspector] public int enemiesLeft; // "HideInInspector" hides the variable in the inspector

}