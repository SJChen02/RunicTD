using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTracker : MonoBehaviour
{
    [SerializeField] private float countdown = 5f;
    public static List<Enemy> activeEnemies = new List<Enemy>();
    public static int totalEnemiesLeft;
    public static int currentWave = 0;
    public GameObject[] spawners;
    private static int spawnersCount;
    private int lastWave;
    private Spawner[] spScripts;
    public static bool gameWon = false;

    private float countdownTimer;
    private bool isWaveInProgress = false;
    private bool isCountdownStarted = false;
    private static int nextWaveEnemyCount = 0;

    private void Start()
    {
        countdownTimer = countdown;
        Debug.Log("WaveTracker started.");

        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        spScripts = new Spawner[spawners.Length];

        for (int i = 0; i < spawners.Length; i++)
        {
            spScripts[i] = spawners[i].GetComponent<Spawner>();
        }

        lastWave = spScripts.Length > 0 ? spScripts[0].waves.Length : 0;

        // Reset everything at start
        ResetWaveState();
        StartWaveCountdown();
    }

    private void Update()
    {
        // Ensure totalEnemiesLeft never goes negative
        if (totalEnemiesLeft < 0)
        {
            Debug.LogWarning("Enemy count went negative, resetting to 0");
            totalEnemiesLeft = 0;
        }

        if (currentWave >= lastWave)
        {
            gameWon = true;
            enabled = false;
            return;
        }

        if (isWaveInProgress)
        {
            // If we're in a wave, check if all enemies are defeated
            if (totalEnemiesLeft == 0 && activeEnemies.Count == 0)
            {
                CompleteWave();
            }
        }
        else if (isCountdownStarted)
        {
            // Only countdown if we're not in a wave and have no active enemies
            if (totalEnemiesLeft == 0 && activeEnemies.Count == 0)
            {
                countdownTimer -= Time.unscaledDeltaTime;
                if (countdownTimer <= 0)
                {
                    StartWave();
                }
            }
        }
    }

    private void StartWave()
    {
        if (isWaveInProgress)
        {
            Debug.LogWarning("Attempted to start wave while another is in progress");
            return;
        }

        Debug.Log($"Starting wave {currentWave}");
        isWaveInProgress = true;
        isCountdownStarted = false;

        // Clear any stray enemies that might exist
        activeEnemies.Clear();
        totalEnemiesLeft = 0;

        foreach (Spawner spawner in spScripts)
        {
            spawner.StartCoroutine(spawner.SpawnWave());
        }
    }

    private void CompleteWave()
    {
        Debug.Log($"Wave {currentWave} completed");
        isWaveInProgress = false;
        currentWave++;

        if (currentWave < lastWave)
        {
            SoundManager.PlaySound(SoundType.WaveStart);

            // Reset for next wave
            activeEnemies.Clear();
            totalEnemiesLeft = 0;

            foreach (Spawner spawner in spScripts)
            {
                spawner.makeReport();
            }

            StartWaveCountdown();
        }
    }

    private void StartWaveCountdown()
    {
        countdownTimer = countdown;
        isCountdownStarted = true;
        Debug.Log($"Starting countdown for wave {currentWave}");
    }

    private void ResetWaveState()
    {
        activeEnemies.Clear();
        totalEnemiesLeft = 0;
        currentWave = 0;
        spawnersCount = 0;
        nextWaveEnemyCount = 0;
        isWaveInProgress = false;
        isCountdownStarted = false;
        gameWon = false;
    }

    public static void PrepareNextWave(int enemyCount)
    {
        nextWaveEnemyCount += enemyCount;
        spawnersCount++;

        if (spawnersCount == GameObject.FindGameObjectsWithTag("Spawner").Length)
        {
            spawnersCount = 0;
            nextWaveEnemyCount = 0;
        }
    }

    public static void RegisterEnemy(Enemy enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
            totalEnemiesLeft++;
            Debug.Log($"Enemy {enemy.name} registered. Active enemies: {activeEnemies.Count}, Total left: {totalEnemiesLeft}");
        }
    }

    public static void UnregisterEnemy(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            totalEnemiesLeft = Mathf.Max(0, totalEnemiesLeft - 1);  // Prevent negative values
            Debug.Log($"Enemy {enemy.name} unregistered. Active enemies: {activeEnemies.Count}, Total left: {totalEnemiesLeft}");
        }
    }

    public static void EnemyKilled()
    {
        totalEnemiesLeft = Mathf.Max(0, totalEnemiesLeft - 1);  // Prevent negative values
        Debug.Log($"Enemy killed. Total left: {totalEnemiesLeft}");
    }

    private void OnEnable()
    {
        ResetWaveState();
        Fortress.mana =175;
        Fortress.health = 120;
    }
}