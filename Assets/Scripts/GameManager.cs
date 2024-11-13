using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private Boss boss;
    private bool gameEnded = false;
    private float timer = 0f;

    public bool GameEnded => gameEnded;

    [SerializeField] private AudioSource bossDie;

    private void Start()
    {
        boss = FindObjectOfType<Boss>();
    }

    private void Update()
    {
        if (!gameEnded)
        {
            timer += Time.deltaTime;
            timerText.text = "Time: " + timer.ToString("F1") + "s";
        }
    }

    public void Defeat()
    {
        if (gameEnded) return;
        gameEnded = true;


        GameData.PlayerWon = false;
        GameData.TimeTaken = timer;

        SceneManager.LoadScene("EndScene");
    }

    public void Victory()
    {
        if (gameEnded) return;
        gameEnded = true;

        GameData.PlayerWon = true;
        GameData.TimeTaken = timer;

        if (boss != null)
        {
            boss.PlayDyingAnimation();
            StartCoroutine(WaitForBossDeath()); 
        }
    }

    private IEnumerator WaitForBossDeath()
    {
        yield return new WaitForSeconds(0.7f);
        bossDie.Play();
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("EndScene");
    }
}
