using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI endScoreText; 
    public TextMeshProUGUI endHighScoreText;
    void Start()
    {
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenMenu()
    {
        
        SceneManager.LoadScene("MainMenu");
    }

    public void GameOver()
    {
        ScoreManager sm = FindFirstObjectByType<ScoreManager>();

        if (sm != null)
        {
            sm.StopScore();

            int finalScore = Mathf.RoundToInt(sm.scoreCount);
            float savedRecord = PlayerPrefs.GetFloat("HighScore", 0);

            endScoreText.text = "Score: " + finalScore;
            endHighScoreText.text = "High Score: " + Mathf.RoundToInt(savedRecord);
        }

        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}