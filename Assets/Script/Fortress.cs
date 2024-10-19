using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fortress : MonoBehaviour
{
    [Header("Health")]
    public float Hp = 100f; 

    public void TakeDamage(float amount)
    {
        Hp -= amount;
        Debug.Log("Fortress took damage! Current HP: " + Hp);

        if (Hp <= 0)
        {
            Debug.Log("Fortress destroyed!");

            // Notify the GameManager that the game is over
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver(); 
            }

            Destroy(gameObject); 
        }
    }
}

