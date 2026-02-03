using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    public float scoreCount;
    public float pointsPerSecond = 5f;
    public bool scoreIncreasing;

    private float highScoreCount;

    void Start()
    {
        
        highScoreCount = PlayerPrefs.GetFloat("HighScore", 0);
        highScoreText.text = "High Score: " + Mathf.RoundToInt(highScoreCount);
        scoreCount = 0;
        scoreIncreasing = true;
    }

    void Update()
    {
        if (scoreIncreasing)
        {
            scoreCount += pointsPerSecond * Time.deltaTime;

            
            if (scoreCount > highScoreCount)
            {
                highScoreCount = scoreCount;
                highScoreText.text = "High Score: " + Mathf.RoundToInt(highScoreCount);
            }
        }

        scoreText.text = "Score: " + Mathf.RoundToInt(scoreCount);
    }

   
    public void StopScore()
    {
        scoreIncreasing = false;

        
        PlayerPrefs.SetFloat("HighScore", highScoreCount);
        PlayerPrefs.Save();
    }

    void UpdateHighScoreText()
    {
        highScoreText.text = "High Score: " + Mathf.RoundToInt(highScoreCount);
    }
}