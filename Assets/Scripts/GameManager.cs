using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText; 

    private bool gameEnded = false;
    private float timer = 0f;

    public bool GameEnded => gameEnded;

    private void Update()
    {
        if (!gameEnded)
        {
            timer += Time.deltaTime;
            timerText.text = "Time: " + timer.ToString("F1") + "s";
        }
    }

    public void Victory()
    {
        if (gameEnded) return;
        gameEnded = true;

        GameData.PlayerWon = true;
        GameData.TimeTaken = timer;

        SceneManager.LoadScene("EndScene");
    }

    public void Defeat()
    {
        if (gameEnded) return;
        gameEnded = true;


        GameData.PlayerWon = false;
        GameData.TimeTaken = timer;

        SceneManager.LoadScene("EndScene");
    }
}
