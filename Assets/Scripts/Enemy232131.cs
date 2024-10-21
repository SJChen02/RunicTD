using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy23321 : MonoBehaviour
{
    [SerializeField] private float speed;
    
    private WaveManager waveManager;

    private float countdown = 5f; //every 5 seconds

    // Start is called before the first frame update
    void Start()
    {
        waveManager = GetComponentInParent<WaveManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        countdown -= Time.deltaTime;

        if (countdown <= 0)
        {
            Destroy(gameObject);

            Debug.Log("Enemy destroyed");
            //waveManager.waves[waveManager.currentWave].enemiesLeft--;
        }
    }
}
