using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    public TextMeshProUGUI titleText;  
    public TextMeshProUGUI timeText;   
    public Button restartButton;     
    
    private void Start()
    {
        titleText.text = GameData.PlayerWon ? "Victory! You defeated the boss!" : "Battle Lost...";

        timeText.text = "Time you used: " + GameData.TimeTaken.ToString("F1") + " seconds";

        restartButton.onClick.AddListener(RestartGame);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}
