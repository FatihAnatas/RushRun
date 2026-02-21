using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro;

public class MainMenuController : MonoBehaviour
{

    void Start()
    {   
        Time.timeScale = 1f;
    }

    public void EndlessModeScene()
    {
        SceneManager.LoadScene("EndlessGameScene");
    }

    public void LevelModeScene()
    {
        SceneManager.LoadScene("LevelGameScene");
    }
}