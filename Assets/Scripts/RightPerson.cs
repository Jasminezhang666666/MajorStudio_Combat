using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RightPerson : MonoBehaviour
{
    public Sprite[] normalSprites;
    public Sprite[] withSpeedLineSprites;
    [SerializeField] private float delayTime = 0.5f;  // Delay time before switching to normal sprite

    private SpriteRenderer spriteRenderer;
    private Coroutine spriteSwitchCoroutine;

    // Added variables for health management
    public int health = 100;
    private int maxHealth;

    public Slider healthSlider; // Optional, if you want to display health

    [SerializeField] private float damageFlashDuration = 0.2f; // Duration of red flash

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize health
        maxHealth = health;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }

    public void UpdateSpriteWithDelay(int circleId)
    {
        // Stop any ongoing coroutine to prevent interference
        if (spriteSwitchCoroutine != null)
        {
            StopCoroutine(spriteSwitchCoroutine);
        }

        // Switch sprites with delay
        spriteSwitchCoroutine = StartCoroutine(SwitchToNormalSprite(circleId));
    }

    private IEnumerator SwitchToNormalSprite(int circleId)
    {
        // Display the withSpeedLine sprite first
        if (circleId >= 0 && circleId < withSpeedLineSprites.Length)
        {
            spriteRenderer.sprite = withSpeedLineSprites[circleId];
        }

        yield return new WaitForSeconds(delayTime);

        // Display the normal sprite
        if (circleId >= 0 && circleId < normalSprites.Length)
        {
            spriteRenderer.sprite = normalSprites[circleId];
        }
    }

    // New method to handle taking damage
    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Max(health, 0);

        // Update health slider if it exists
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }

        if (health <= 0)
        {
            Debug.Log("RightPerson has been defeated!");
            // Implement any defeat logic here (e.g., disable the object, play animation)
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red; // Change color to red
        yield return new WaitForSeconds(damageFlashDuration);
        spriteRenderer.color = Color.white; // Revert to original color
    }
}
