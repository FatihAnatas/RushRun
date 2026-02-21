using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Oyun Ýçi UI")]
    public TextMeshProUGUI scoreText; 

    [Header("Game Over UI")]
    public TextMeshProUGUI gameOverHighScoreText;
    public GameObject gameOverPanel; 

    [Header("Ayarlar")]
    public float scoreCount;
    public float pointsPerSecond = 5f;
    public bool scoreIncreasing;

    private float highScoreCount;

    void Start()
    {
        highScoreCount = PlayerPrefs.GetFloat("HighScore", 0);
        scoreCount = 0;
        scoreIncreasing = true;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameOverHighScoreText != null) gameOverHighScoreText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (scoreIncreasing)
        {
            scoreCount += pointsPerSecond * Time.deltaTime;

            if (scoreCount > highScoreCount)
            {
                highScoreCount = scoreCount;
            }
        }

        scoreText.text = "Score: " + Mathf.RoundToInt(scoreCount);
    }

    public void StopScore()
    {
        scoreIncreasing = false;

        PlayerPrefs.SetFloat("HighScore", highScoreCount);
        PlayerPrefs.Save();

        if (gameOverHighScoreText != null)
        {
            gameOverHighScoreText.text = "Best: " + Mathf.RoundToInt(highScoreCount);
            gameOverHighScoreText.gameObject.SetActive(true);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}