using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float timeBetweenShots = 2.0f;

    private float nextFireTime = 0f;

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

        // Initialize the health slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void Update()
    {
        // Check if enough time has passed to fire again
        if (Time.time >= nextFireTime)
        {
            ShootAtPlayer();
            nextFireTime = Time.time + timeBetweenShots;
        }
    }

    // Determine the player's position
    private void ShootAtPlayer()
    {
        // Get the LeftPerson's current position index and its target position
        int playerPositionIndex = leftPerson.GetCurrentPositionIndex();

        Vector3 targetPosition = centerSpot.position;

        if (playerPositionIndex == 0)
        {
            targetPosition = leftSpot.position;
        }
        else if (playerPositionIndex == 2)
        {
            targetPosition = rightSpot.position;
        }

        // Maintain the Z-axis difference between the boss and the player
        targetPosition.z = leftPerson.transform.position.z;

        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<BossAttack>().SetTarget(targetPosition);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Ensure health doesn't go below 0
        Debug.Log("Boss took " + damage + " damage. Remaining health: " + currentHealth);

        // Update health slider
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            // Boss is defeated
            Debug.Log("Boss defeated!");
            // Add code for boss defeat (e.g., play animation, load next level, etc.)
            Destroy(gameObject); // Example action: destroy the boss object
        }
    }
}
