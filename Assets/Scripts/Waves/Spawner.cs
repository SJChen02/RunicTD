using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour {
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private PathManager pathManager;
    public Wave[] waves;
    //public WaveTracker waveTracker;

    private void Start() {
        // Set the enemiesLeft variable to the length of the enemies array
        foreach (Wave wave in waves)
        {
            wave.enemiesLeft = wave.enemyGroupCounts.Sum();
        }
        makeReport();
    }

    public void makeReport() {
        WaveTracker.ReportEnemiesLeft(waves[WaveTracker.currentWave].enemiesLeft);
    }

    public IEnumerator SpawnWave() { // IEnumerator is a type of function that can be paused and resumed 
        if (WaveTracker.currentWave < waves.Length) {
            for (int i = 0; i < waves[WaveTracker.currentWave].enemyGroups.Length; i++) {
                for (int j = 0; j < waves[WaveTracker.currentWave].enemyGroupCounts[i]; j++) {
                    Enemy spawnedEnemy = Instantiate(waves[WaveTracker.currentWave].enemyGroups[i], spawnPoint.transform);
                    spawnedEnemy.transform.SetParent(this.transform);
                    spawnedEnemy.SetPathManager(pathManager);
                    WaveTracker.RegisterEnemy(spawnedEnemy);
                    yield return new WaitForSeconds(waves[WaveTracker.currentWave].enemySpawnCooldown); // Pauses until the time has passed
                }
            }
        }

    }
}

[System.Serializable] // This attribute allows us to see the class in the inspector
public class Wave {
    public Enemy[] enemyGroups;
    public int[] enemyGroupCounts;
    public float enemySpawnCooldown;

    [HideInInspector] public int enemiesLeft; // "HideInInspector" hides the variable in the inspector
}