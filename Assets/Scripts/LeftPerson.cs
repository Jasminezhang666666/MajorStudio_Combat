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
    [SerializeField] private float damageFlashDuration = 0.2f;

    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite centerSprite;
    [SerializeField] private Sprite rightSprite;

    private bool isSwordEffectActive = false;
    [SerializeField] private float swordEffectDuration = 1.0f;
    [SerializeField] private float swordEffectCooldown = 2.0f;
    private float nextSwordEffectTime = 0f;

    private enum ActionStage { Attack, Deflect }
    private ActionStage currentStage = ActionStage.Attack;
    [SerializeField] private int bossDamageAmount = 2;

    private Boss boss;
    private bool isInvincible = false;
    public GameObject shieldPrefab;
    private GameObject activeShield;

    [SerializeField] private GameObject attackUI; // UI element for Attack stage
    [SerializeField] private GameObject deflectUI; // UI element for Deflect stage

    private void Start()
    {
        spots = new Transform[3] { leftSpot, centerSpot, rightSpot };
        transform.position = spots[currentPositionIndex].position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        maxHealth = health;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }

        boss = GameObject.FindObjectOfType<Boss>();
        if (boss == null)
        {
            Debug.LogError("Boss not found in the scene.");
        }

        UpdateUI(); // Initial UI update
        UpdateSprite(); // Initial sprite update based on starting position
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

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentStage == ActionStage.Deflect && Time.time >= nextSwordEffectTime && !isSwordEffectActive)
            {
                StartCoroutine(ActivateSwordEffect());
                nextSwordEffectTime = Time.time + swordEffectCooldown;
            }
            else if (currentStage == ActionStage.Attack)
            {
                AttackBoss();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleStage();
        }
    }

    private void MoveToSpot()
    {
        transform.position = spots[currentPositionIndex].position;
        nextMoveTime = Time.time + stopMovingTime;
        UpdateSprite(); // Update sprite based on new position
    }

    private void ToggleStage()
    {
        if (currentStage == ActionStage.Attack)
        {
            currentStage = ActionStage.Deflect;
            Debug.Log("Stage changed to Deflect.");
        }
        else
        {
            currentStage = ActionStage.Attack;
            Debug.Log("Stage changed to Attack.");
        }

        UpdateUI(); // Update UI after toggling the stage
    }

    private void UpdateUI()
    {
        attackUI.SetActive(currentStage == ActionStage.Attack);
        deflectUI.SetActive(currentStage == ActionStage.Deflect);
    }

    private void UpdateSprite()
    {
        switch (currentPositionIndex)
        {
            case 0:
                spriteRenderer.sprite = leftSprite;
                break;
            case 1:
                spriteRenderer.sprite = centerSprite;
                break;
            case 2:
                spriteRenderer.sprite = rightSprite;
                break;
        }
    }

    private void AttackBoss()
    {
        if (boss != null)
        {
            boss.TakeDamage(bossDamageAmount);
            Debug.Log("Boss took damage: " + bossDamageAmount);
        }
    }

    public int GetCurrentPositionIndex()
    {
        return currentPositionIndex;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
            return;
        if (currentStage != ActionStage.Deflect || !isSwordEffectActive)
        {
            health -= damage;
            health = Mathf.Max(health, 0);
            if (healthSlider != null)
            {
                healthSlider.value = health;
            }
            if (health <= 0)
            {
                Debug.Log("Player has been defeated!");
            }
            else
            {
                StartCoroutine(FlashRed());
            }
        }
    }

    public void RecoverHealth(int amount)
    {
        health += amount;
        health = Mathf.Min(health, maxHealth);
        if (healthSlider != null)
        {
            healthSlider.value = health;
        }
    }

    public IEnumerator ActivateShield(float duration)
    {
        isInvincible = true;
        if (shieldPrefab != null)
        {
            activeShield = Instantiate(shieldPrefab, transform);
            activeShield.transform.localPosition = Vector3.zero;
        }
        yield return new WaitForSeconds(duration);
        if (activeShield != null)
        {
            Destroy(activeShield);
        }
        isInvincible = false;
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(damageFlashDuration);
        spriteRenderer.color = Color.white;
    }

    private IEnumerator ActivateSwordEffect()
    {
        isSwordEffectActive = true;
        spriteRenderer.color = Color.blue;
        yield return new WaitForSeconds(swordEffectDuration);
        spriteRenderer.color = Color.white;
        isSwordEffectActive = false;
    }

    public bool IsSwordEffectActive()
    {
        return isSwordEffectActive;
    }
}
