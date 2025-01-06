using System.Collections;
using TMPro;
using UnityEngine;

public class Fortress : MonoBehaviour
{
    [Header("Health")]
    public static int health = 120;
    public static int mana = 150;
    public TextMeshProUGUI manaAmount;
    public TextMeshProUGUI healthAmount;
    public TextMeshProUGUI waveCount;


    private void Update()
    {
        SetText();
    }

    private void SetText()
    {
        manaAmount.text = mana.ToString();
        healthAmount.text = health.ToString();
        waveCount.text = "Wave: " + (WaveTracker.currentWave + 1);
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