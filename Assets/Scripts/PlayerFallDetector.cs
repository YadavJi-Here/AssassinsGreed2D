using UnityEngine;

public class PlayerFallDetector : MonoBehaviour
{
    public float fallYLimit = -3f; // You can tweak this based on how far below the platform you want
    private bool isGameOver = false;

    private GameManager gameManager;

    void Start()
    {
        // Get reference to GameManager
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (!isGameOver && transform.position.y < fallYLimit)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over!");

        if (gameManager != null)
        {
            gameManager.isGameActive = false;
            Time.timeScale = 0f;

            if (gameManager.gameOverPanel != null)
            {
                gameManager.gameOverPanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("GameOverPanel not assigned in GameManager!");
            }
        }
        else
        {
            Debug.LogWarning("GameManager not found in PlayerFallDetector!");
        }
    }
}
