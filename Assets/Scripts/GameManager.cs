using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 
using UnityEngine.UI; 

public class GameManager : MonoBehaviour {

    public static GameManager instance; 

    private bool gameIsOver = false;

    [Header("UI")]
    public GameObject gameOverPrefab; // Assign the Game Over UI Prefab

    private GameObject gameOverInstance; // instantiated prefab

    private void Awake() {
        // Singleton pattern to ensure only one GameManager exists
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make GameManager persistent across scenes
        }
        else {
            Destroy(gameObject);
        }
    }

    public void GameOver() {
        if (gameIsOver) return; // Prevent multiple game-over triggers

        gameIsOver = true;

        Debug.Log("Game Over! Player lost.");

        // Instantiate the Game Over UI if it doesn't already exist
        if (gameOverPrefab != null && gameOverInstance == null) {
            gameOverInstance = Instantiate(gameOverPrefab); 
        }

        // Stop the game
        Time.timeScale = 0f;
    }

}

