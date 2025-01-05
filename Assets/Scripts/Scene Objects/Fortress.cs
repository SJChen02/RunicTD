using System.Collections;
using TMPro;
using UnityEngine;

public class Fortress : MonoBehaviour
{
    [Header("Health")]
    public static int health = 100;
    public static int mana = 100;
    public TextMeshProUGUI manaAmount;
    public TextMeshProUGUI healthAmount;
    public TextMeshProUGUI waveCount;


    private void Update()
    {
        SetText();
    }

    private void SetText()
    {
        manaAmount.text = "Mana: " + mana;
        healthAmount.text = "Health: " + health;
        waveCount.text = "Wave: " + (WaveTracker.currentWave + 1);
    }

    public static void DestroyFortress(GameObject fortress)
    {
        Destroy(fortress);
    }

    public static void TakeHit()
    {
        // Considers the enemy gone, take away health from the fortress
        WaveTracker.totalEnemiesLeft -= 1;
        health -= 20;

        // Ensures that health doesn't display as negative
        if (health < 0)
        {
            health = 0;
        }

        if (health <= 0)
        {
            // 'Destroys' the fortress by making it invisible
            // The component is still active, so mana and health are still able to be updated
            GameObject.Find("Fortress").GetComponent<Renderer>().enabled = false;
        }
    }
}