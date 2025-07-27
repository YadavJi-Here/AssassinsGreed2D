using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isGameActive = true;
    public GameObject gameOverPanel;

    private void Start()
    {
        CacheGameOverPanel();
        isGameActive = true;
        Time.timeScale = 1f;
    }

    private void CacheGameOverPanel()
    {
        if (gameOverPanel == null)
        {
            gameOverPanel = GameObject.Find("GameOverPanel");

            if (gameOverPanel == null)
            {
                Debug.LogWarning("GameOverPanel not found in the scene. Make sure it's named exactly 'GameOverPanel'");
            }
        }
    }

    public void GameOver()
    {
        isGameActive = false;
        Time.timeScale = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isGameActive = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Fresh scene, fresh GameManager
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        isGameActive = true;
        SceneManager.LoadScene("Menu");
    }
}