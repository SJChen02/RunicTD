using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameObject spawnPoint;
    public Wave[] waves;
    public WaveTracker waveTracker;

    private void Start()
    {
        // Set the enemiesLeft variable to the length of the enemies array
        foreach (Wave wave in waves)
        {
            wave.enemiesLeft = wave.enemyGroupCounts.Sum();
        }
        makeReport();
    }

    private void Update()
    {

    }
    public void makeReport()
    {
        waveTracker.ReportEnemiesLeft(waves[waveTracker.currentWave].enemiesLeft);
    }

    public IEnumerator SpawnWave() // IEnumerator is a type of function that can be paused and resumed
    {
        if (waveTracker.currentWave < waves.Length)
        {
            for (int i = 0; i < waves[waveTracker.currentWave].enemyGroups.Length; i++)
            {
                for (int j = 0; j < waves[waveTracker.currentWave].enemyGroupCounts[i]; j++)
                {
                    Enemy spawnedEnemy = Instantiate(waves[waveTracker.currentWave].enemyGroups[i], spawnPoint.transform);
                    spawnedEnemy.transform.SetParent(spawnPoint.transform);
                    yield return new WaitForSeconds(waves[waveTracker.currentWave].enemySpawnCooldown); // Pauses until the time has passed
                }
            }
        }

    }
}

[System.Serializable] // This attribute allows us to see the class in the inspector
public class Wave
{
    public Enemy[] enemyGroups;
    public int[] enemyGroupCounts;
    public float enemySpawnCooldown;

    [HideInInspector] public int enemiesLeft; // "HideInInspector" hides the variable in the inspector

}