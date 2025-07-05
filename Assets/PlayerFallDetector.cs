using UnityEngine;

public class PlayerFallDetector : MonoBehaviour
{
    public float fallYLimit = -3f; // You can tweak this based on how far below the platform you want

    private bool isGameOver = false;

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
        // Optionally freeze game or show UI
        Time.timeScale = 0;

        // If using UI:
        // gameOverPanel.SetActive(true);
    }
}
