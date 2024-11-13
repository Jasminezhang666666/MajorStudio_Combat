using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    public Button startButton;
    public Button howToPlayButton;

    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
        howToPlayButton.onClick.AddListener(ShowHowToPlay);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("GameScene"); 
    }

    private void ShowHowToPlay()
    {
        SceneManager.LoadScene("HowToPlayScene"); 
    }
}
