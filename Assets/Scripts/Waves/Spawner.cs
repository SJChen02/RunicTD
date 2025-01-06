using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private PathManager pathManager;
    public Wave[] waves;
    public float enemySpawnCooldown = 1f;
    private bool isSpawning = false;

    private void Start()
    {
        // Calculate the total number of enemies in the waves
        foreach (Wave wave in waves)
        {
            wave.enemiesLeft = wave.enemySets.Sum(detail => detail.count);
        }
        makeReport();
    }

    public void makeReport()
    {
        if (WaveTracker.currentWave < waves.Length)
        {
            // Just report the count for the next wave, don't add to totalEnemiesLeft yet
            WaveTracker.PrepareNextWave(waves[WaveTracker.currentWave].enemiesLeft);
        }
    }

    public IEnumerator SpawnWave()
    {
        // Check if we're already spawning or if we're past the last wave
        if (isSpawning || WaveTracker.currentWave >= waves.Length)
        {
            yield break;
        }

        isSpawning = true;
        Wave currentWave = waves[WaveTracker.currentWave];

        foreach (var enemySet in currentWave.enemySets)
        {
            for (int j = 0; j < enemySet.count; j++)
            {
                Enemy spawnedEnemy = Instantiate(enemySet.prefab, spawnPoint.transform);
                spawnedEnemy.transform.SetParent(this.transform);
                spawnedEnemy.SetPathManager(pathManager);
                WaveTracker.RegisterEnemy(spawnedEnemy);
                yield return new WaitForSeconds(enemySpawnCooldown);
            }
        }

        isSpawning = false;
        currentWave.enemiesLeft = 0; // Reset enemies left for this wave
    }
}

[System.Serializable]
public class Wave
{
    [System.Serializable]
    public struct EnemyDetail
    {
        public Enemy prefab;
        public int count;
    }
    public List<EnemyDetail> enemySets;
    [HideInInspector] public int enemiesLeft;
}