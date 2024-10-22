using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTracker : MonoBehaviour {

    [SerializeField] private float countdown = 1f;
    public static int totalEnemiesLeft;
    public static int currentWave =0;
    public GameObject[] spawners;
    private static int spawnersCount;
    private int lastWave;
    private static bool readyToCountdown;
    private Spawner[] spScripts;

    private void Start() {

        readyToCountdown = false;
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        spScripts = new Spawner[spawners.Length];

        for (int i = 0; i < spawners.Length; i++) {
            spScripts[i] = spawners[i].GetComponent<Spawner>();
        }
        
        lastWave = spScripts[0].waves.Length;

    }

    private void Update() {
        if (currentWave >= lastWave) {
            Debug.Log("All waves completed");
            enabled = false; // Disables the script
            return;
        }

        if (readyToCountdown) {countdown -= Time.deltaTime;}

        if (totalEnemiesLeft == 0) {
            currentWave++;
            if (currentWave < lastWave) {
                foreach (Spawner spawner in spScripts) {
                    //Debug.Log("Making report");
                    spawner.makeReport();
                }
            }
                
        }

        if (countdown <= 0) {
            Debug.Log("Starting next wave");
            countdown = 2f;
            readyToCountdown = false;

            foreach (Spawner spawner in spScripts) {
                spawner.StartCoroutine(spawner.SpawnWave());
            }

            Debug.Log("All spawners have started their waves");
        }
    }

    public static void EnemyKilled() {totalEnemiesLeft--;}

    public static void ReportEnemiesLeft(int amount) {

        totalEnemiesLeft += amount;
        spawnersCount++;

        if (spawnersCount == (GameObject.FindGameObjectsWithTag("Spawner")).Length) {
            spawnersCount = 0;
            readyToCountdown = true;
        }

    }

}
