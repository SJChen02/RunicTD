using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fortress : MonoBehaviour {

    [Header("Health")]

    public float health = 100f;
    public static int gold = 100;
    public TextMeshProUGUI goldAmount;
    public TextMeshProUGUI healthAmount;

    void Update() {
        SetText();
    }

    private void SetText() {
        goldAmount.text = "Gold: " + gold;
        healthAmount.text = "Health: " + health;
    }

    public void TakeDamage(float damage) {
        WaveTracker.totalEnemiesLeft -= 1;
        health -= damage;

        if (health <= 0) {
            Debug.Log("Fortress destroyed!");

            // Notify the GameManager that the game is over
            if (GameManager.instance != null) {
                GameManager.instance.GameOver();
            }

            Destroy(gameObject); 
        }
    }
}

