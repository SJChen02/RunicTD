using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Spawner : MonoBehaviour {
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private PathManager pathManager;
    public Wave[] waves;
    public float enemySpawnCooldown = 1f;

    private void Start() {
        // Calculate the total number of enemies in the waves
        foreach (Wave wave in waves)
        {
            wave.enemiesLeft = wave.enemySets.Sum(detail => detail.count);
        }
        makeReport();
    }

    // makeReport() is called by WaveTracker when the wave is over to track each Spawners enemies left
    public void makeReport() {
        WaveTracker.ReportEnemiesLeft(waves[WaveTracker.currentWave].enemiesLeft);
    }

    /* SpawnWave() is a coroutine that spawns enemies in the scene. It is called by WaveTracker when the wave is over.
     * It spawns enemies in the scene with a delay of enemySpawnCooldown between each enemy.
     * It also registers the spawned enemies in the WaveTracker. (used in tower targetting)
     */
    public IEnumerator SpawnWave() { // IEnumerator is a type of function that can be paused and resumed 
        if (WaveTracker.currentWave < waves.Length)
        {
            foreach (var enemySet in waves[WaveTracker.currentWave].enemySets)
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
        }

    }
}

/* Wave is a class that holds the details of the enemies that will be spawned in a wave.*/
[System.Serializable] // This attribute allows us to see the class in the inspector
public class Wave {
    [System.Serializable]
    public struct EnemyDetail
    {
        public Enemy prefab;
        public int count;
    }

    public List<EnemyDetail> enemySets;


    [HideInInspector] public int enemiesLeft; // "HideInInspector" hides the variable in the inspector
}