using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class HowToPlay : MonoBehaviour
{
    public Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        SceneManager.LoadScene("GameScene"); 
    }

}
