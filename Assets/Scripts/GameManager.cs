using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor; // Required for Editor-only functionality
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance for easy access

    private bool gameIsOver = false;

    [Header("UI")]
    public GameObject gameOverPanel; // Reference to the Game Over UI Panel in the scene

    private void Awake()
    {
        // Singleton pattern to ensure only one GameManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Make GameManager persistent across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameOver()
    {
        // Prevent multiple game-over triggers
        if (gameIsOver) return;

        gameIsOver = true;

        Debug.Log("Game Over! Player lost.");

        // Show the hidden Game Over UI panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); // Display the Game Over panel
        }

        // Stop the game (freeze time)
        Time.timeScale = 0f;

        // Start coroutine to wait for a few seconds and then exit the game
        StartCoroutine(ExitGameAfterDelay(5f)); // Wait for 5 seconds before exiting
    }

    private IEnumerator ExitGameAfterDelay(float delay)
    {
        // Wait for the specified delay (real-time wait since time is frozen in-game)
        yield return new WaitForSecondsRealtime(delay);

#if UNITY_EDITOR
        // Exit play mode in the Editor
        EditorApplication.isPlaying = false;
#else
        // Quit the application in a standalone build
        Application.Quit();
#endif
    }
}


