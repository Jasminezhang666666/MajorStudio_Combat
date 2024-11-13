using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Boss : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject bossRainPrefab;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private float damageFlashDuration = 0.2f;
    private Color originalColor;
    [SerializeField] private float timeBetweenShots = 2.0f;
    private float nextFireTime = 0f;
    [SerializeField] private float projectileChance = 0.33f;

    public Transform leftSpot;
    public Transform centerSpot;
    public Transform rightSpot;

    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider;

    [SerializeField] private LeftPerson leftPerson;

    private Animator animator;
    private GameManager gameManager;

    [SerializeField] private float attackAnimationDelay = 0.7f; 

    private void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        animator = GetComponent<Animator>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }

    private void Update()
    {
        if (gameManager != null && gameManager.GameEnded)
            return;

        if (Time.time >= nextFireTime)
        {
            ShootAtPlayer();
            nextFireTime = Time.time + timeBetweenShots;
        }
    }

    private void ShootAtPlayer()
    {
        int playerPositionIndex = leftPerson.GetCurrentPositionIndex();

        Vector3 targetPosition = playerPositionIndex == 0 ? leftSpot.position :
                                 playerPositionIndex == 2 ? rightSpot.position : centerSpot.position;

        float randomValue = Random.value;
        if (randomValue <= projectileChance)
        {
            animator.SetTrigger("AttackTrigger"); // Play attack animation
            StartCoroutine(FireProjectileAfterDelay(targetPosition)); 
        }
        else
        {
            Vector3 spawnPosition = new Vector3(targetPosition.x, 6.37f, targetPosition.z);
            Instantiate(bossRainPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private IEnumerator FireProjectileAfterDelay(Vector3 targetPosition)
    {
        yield return new WaitForSeconds(attackAnimationDelay); 
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<BossAttack>().SetTarget(targetPosition);
    }

    public void TakeDamage(int damage)
    {
        StartCoroutine(FlashPurple());

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.Victory();
            }
            Destroy(gameObject);
        }
    }


    private IEnumerator FlashPurple()
    {
        spriteRenderer.color = new Color(0.494f, 0.337f, 0.604f);
        yield return new WaitForSeconds(damageFlashDuration);
        spriteRenderer.color = originalColor;
    }
}
