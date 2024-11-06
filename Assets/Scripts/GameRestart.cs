using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameRestart : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText; // TextMeshPro UI element for displaying the timer
    private float timer = 0f;

    void Update()
    {
        // Update the timer
        timer += Time.deltaTime;

        // Display the timer in seconds with one decimal place
        timerText.text = "Time: " + timer.ToString("F1") + "s";

        // Check if the "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reload the current active scene and reset the timer
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnEnable()
    {
        // Reset timer each time the scene loads
        timer = 0f;
    }
}
