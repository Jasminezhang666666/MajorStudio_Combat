using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class LeftPerson : MonoBehaviour
{
    public Transform leftSpot;
    public Transform centerSpot;
    public Transform rightSpot;

    private int currentPositionIndex = 1;
    private Transform[] spots;

    [SerializeField] private float stopMovingTime = 1.0f;
    private float nextMoveTime = 0f;

    public int health = 100; 
    private int maxHealth;

    public Slider healthSlider; 

    // Duration for which the player turns red when hit
    [SerializeField] private float damageFlashDuration = 0.2f;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spots = new Transform[3] { leftSpot, centerSpot, rightSpot };
        transform.position = spots[currentPositionIndex].position;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize health and maxHealth
        maxHealth = health;

        // Initialize the health slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }

    private void Update()
    {
        if (Time.time >= nextMoveTime)
        {
            if (Input.GetKeyDown(KeyCode.A) && currentPositionIndex > 0)
            {
                currentPositionIndex--;
                MoveToSpot();
            }

            if (Input.GetKeyDown(KeyCode.D) && currentPositionIndex < spots.Length - 1)
            {
                currentPositionIndex++;
                MoveToSpot();
            }
        }
    }

    private void MoveToSpot()
    {
        transform.position = spots[currentPositionIndex].position;
        nextMoveTime = Time.time + stopMovingTime;
    }

    public int GetCurrentPositionIndex()
    {
        return currentPositionIndex;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"Player health before damage: {health}");
        health -= damage;
        health = Mathf.Max(health, 0); 

        // Update health slider
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        Debug.Log($"Player health after damage: {health}");
        if (health <= 0)
        {
            // GAME OVER
            Debug.Log("Player has been defeated!");
            // Add code for player defeat (e.g., restart level, show game over screen)
        }
        else
        {
            // Flash red to indicate damage
            StartCoroutine(FlashRed());
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(damageFlashDuration);

        spriteRenderer.color = Color.white;
    }
}
