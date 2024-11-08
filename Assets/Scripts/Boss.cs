using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Boss : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject bossRainPrefab;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private float damageFlashDuration = 0.2f;

    private Color originalColor; // Store the original color here

    [SerializeField] private float timeBetweenShots = 2.0f;
    private float nextFireTime = 0f;

    [SerializeField] private float projectileChance = 0.33f;
    //[SerializeField] private float bossRainChance = 0.67f; 

    public Transform leftSpot;
    public Transform centerSpot;
    public Transform rightSpot;

    public int maxHealth = 100;
    public int currentHealth;

    [SerializeField] private LeftPerson leftPerson;
    public Slider healthSlider;

    private void Start()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Get and store the original color at the start

        // Initialize the health slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void Update()
    {
        // to fire again?
        if (Time.time >= nextFireTime)
        {
            ShootAtPlayer();
            nextFireTime = Time.time + timeBetweenShots;
        }
    }

    // Determine the player's position and attack type
    private void ShootAtPlayer()
    {
        // Get the LeftPerson's current position index and its target position
        int playerPositionIndex = leftPerson.GetCurrentPositionIndex();

        Vector3 targetPosition;
        if (playerPositionIndex == 0)
        {
            targetPosition = leftSpot.position;
        }
        else if (playerPositionIndex == 2)
        {
            targetPosition = rightSpot.position;
        }
        else
        {
            targetPosition = centerSpot.position;
        }

        // Adjust the probability of which attack to generate
        float randomValue = Random.value;
        if (randomValue <= projectileChance)
        {
            // projectile attack
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<BossAttack>().SetTarget(targetPosition);
        }
        else
        {
            // BossRain attack
            Vector3 spawnPosition = new Vector3(targetPosition.x, 6.37f, targetPosition.z);
            GameObject bossRain = Instantiate(bossRainPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void TakeDamage(int damage)
    {
        StartCoroutine(FlashPurple());

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Ensure health doesn't go below 0

        // Update health slider
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            // Boss is defeated
            Debug.Log("Boss defeated!");
            Destroy(gameObject); // Example action: destroy the boss object
        }
    }

    private IEnumerator FlashPurple()
    {
        spriteRenderer.color = new Color(0.494f, 0.337f, 0.604f); // Equivalent to hex #7E569A
        yield return new WaitForSeconds(damageFlashDuration);
        spriteRenderer.color = originalColor; // Return to the original color after flashing
    }
}
