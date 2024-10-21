using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveTracker : MonoBehaviour
{
    private int totalEnemiesLeft;
    private int waveManagersCount;
    [SerializeField] private float countdown = 1f;
    private bool readyToCountdown;
    public int currentWave =0;
    public GameObject[] waveManagers;
    private int lastWave;

    private void Start()
    {
        readyToCountdown = false;
        waveManagers = GameObject.FindGameObjectsWithTag("Spawner");
        lastWave = waveManagers[0].GetComponent<WaveManager>().waves.Length-1;
    }

    private void Update()
    {
        if (currentWave >= lastWave)
        {
            Debug.Log("All waves completed");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            enabled = false; // Disables the script
            return;
        }

        if (readyToCountdown)
        {
            countdown -= Time.deltaTime;
        }

        if (totalEnemiesLeft == 0)
        {
            currentWave++;
            
            if (currentWave < lastWave)
            {
                foreach (GameObject waveManagerObject in waveManagers)
                {
                    WaveManager waveManager = waveManagerObject.GetComponent<WaveManager>();
                    Debug.Log("Making report");
                    waveManager.makeReport();
                }
            }
                
        }

        if (countdown <= 0)
        {
            Debug.Log("Starting next wave");
            countdown = 2f;
            readyToCountdown = false;

            foreach (GameObject waveManagerObject in waveManagers)
            {
                WaveManager waveManager = waveManagerObject.GetComponent<WaveManager>();
                waveManager.StartCoroutine(waveManager.SpawnWave());
            }
            Debug.Log("All spawners have started their waves");
        }
    }

    public void EnemyKilled()
    {
        totalEnemiesLeft--;
    }

    public void ReportEnemiesLeft(int amount)
    {
        totalEnemiesLeft += amount;
        waveManagersCount++;
        if (waveManagersCount == (GameObject.FindGameObjectsWithTag("Spawner")).Length)
        {
            waveManagersCount = 0;
            readyToCountdown = true;
        }
    }


}
