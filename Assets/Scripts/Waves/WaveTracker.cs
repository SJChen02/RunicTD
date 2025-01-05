using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTracker : MonoBehaviour {

    [SerializeField] private float countdown = 1f;
    public static List<Enemy> activeEnemies = new List<Enemy>();
    public static int totalEnemiesLeft;
    public static int currentWave = 0;
    public GameObject[] spawners;
    private static int spawnersCount;
    private int lastWave;
    private static bool readyToCountdown;
    private Spawner[] spScripts;
    public static bool gameWon = false;

    // Gets all the spawners in the scene and stores them in an array
    // then it gets the last wave number from the first spawner
    private void Start() {

        readyToCountdown = false;
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        spScripts = new Spawner[spawners.Length];

        for (int i = 0; i < spawners.Length; i++) {
            spScripts[i] = spawners[i].GetComponent<Spawner>();
        }
        
        lastWave = spScripts[0].waves.Length;

    }

    /* Updates the wave tracker
     * If the current wave is the last wave, it disables the script (win condition)
     * If the countdown is less than or equal to 0, it starts the next wave
     * If the total enemies left is 0, it increments the current wave and makes a report
     */
    private void Update() {
        if (currentWave >= lastWave) {
            Debug.Log("All waves completed");
            gameWon = true;
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

    // Registers and unregisters enemies in the activeEnemies list
    public static void RegisterEnemy(Enemy enemy) {activeEnemies.Add(enemy);}
    public static void UnregisterEnemy(Enemy enemy) {activeEnemies.Remove(enemy);}

    // Decrements the total enemies left
    public static void EnemyKilled() {totalEnemiesLeft--;}

    // Increments the total enemies left from each spawner
    public static void ReportEnemiesLeft(int amount) {

        totalEnemiesLeft += amount;
        spawnersCount++;

        if (spawnersCount == (GameObject.FindGameObjectsWithTag("Spawner")).Length) {
            spawnersCount = 0;
            readyToCountdown = true;
        }

    }
    private void OnEnable()
    {
        // Reset static variables on scene load
        activeEnemies.Clear();
        totalEnemiesLeft = 0;
        currentWave = 0;
        spawnersCount = 0;
        readyToCountdown = false;
        gameWon = false;
        Fortress.mana = 100;
        Fortress.health = 100;
    }
}
