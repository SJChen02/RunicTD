using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fortress : MonoBehaviour {

    [Header("Health")]

    public static int health = 100;
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

    // cannot call Destroy() directly in TakeHit(), so this is a workaround
    public static void DestroyFortress(GameObject fortress) {

        Destroy(fortress);

    }

    public static void TakeHit() {
        
        // considers the enemy gone, take away health from the fortress
        WaveTracker.totalEnemiesLeft -= 1;
        health -= 60;

        // ensures that health doesn't display as negative
        if (health < 0) {

            health = 0;

        }

        if (health <= 0) {

            // notify the GameManager that the game is over
            if (GameManager.instance != null) {

                GameManager.instance.GameOver();

            }

            // 'destroys' the fortress by making it invisible
            // the component is still active, so gold and health are still able to be updated
            GameObject.Find("Fortress").GetComponent<Renderer>().enabled = false; 
            // at present, the waves still continue after the fortress dies
        }

    }

}

