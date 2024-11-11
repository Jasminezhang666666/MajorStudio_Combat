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
    [SerializeField] private Sprite leftAttackSprite;
    [SerializeField] private Sprite centerAttackSprite;
    [SerializeField] private Sprite rightAttackSprite;
    [SerializeField] private Sprite leftAttackStopSprite;
    [SerializeField] private Sprite centerAttackStopSprite;
    [SerializeField] private Sprite rightAttackStopSprite;
    [SerializeField] private float attackSpriteDuration = 0.5f;

    private bool isAttacking = false;
    private bool isSwordEffectActive = false;
    [SerializeField] private float swordEffectDuration = 1.0f;

    private enum ActionStage { Attack, Deflect }
    private ActionStage currentStage = ActionStage.Attack;
    [SerializeField] private int bossDamageAmount = 2;

    private Boss boss;
    private bool isInvincible = false;
    public GameObject shieldPrefab;
    private GameObject activeShield;

    [SerializeField] private GameObject attackUI;
    [SerializeField] private GameObject deflectUI;

    private Animator animator;
    private bool isParrying = false;
    [SerializeField] private float parryAnimationDuration = 1.0f;

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

        animator = GetComponent<Animator>();
        animator.enabled = false; // Disable Animator by default

        UpdateUI();
        UpdateSprite();
    }

    private void Update()
    {
        if (!isParrying && Time.time >= nextMoveTime)
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
            if (currentStage == ActionStage.Deflect && !isParrying)
            {
                StopAllCoroutines();
                StartCoroutine(PlayParryAnimation());
            }
            else if (currentStage == ActionStage.Attack)
            {
                StopAllCoroutines();
                StartCoroutine(DisplayAttackSpriteSequence());
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
        UpdateSprite();
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

        UpdateUI();
    }

    private void UpdateUI()
    {
        attackUI.SetActive(currentStage == ActionStage.Attack);
        deflectUI.SetActive(currentStage == ActionStage.Deflect);
    }

    private void UpdateSprite()
    {
        if (animator != null && animator.enabled)
            return; // Skip updating the sprite if Animator is active

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

    private IEnumerator DisplayAttackSpriteSequence()
    {
        isAttacking = true;
        Sprite originalSprite = spriteRenderer.sprite;

        switch (currentPositionIndex)
        {
            case 0:
                spriteRenderer.sprite = leftAttackSprite;
                break;
            case 1:
                spriteRenderer.sprite = centerAttackSprite;
                break;
            case 2:
                spriteRenderer.sprite = rightAttackSprite;
                break;
        }

        yield return new WaitForSeconds(attackSpriteDuration);

        if (!Input.GetKey(KeyCode.W))
        {
            switch (currentPositionIndex)
            {
                case 0:
                    spriteRenderer.sprite = leftAttackStopSprite;
                    break;
                case 1:
                    spriteRenderer.sprite = centerAttackStopSprite;
                    break;
                case 2:
                    spriteRenderer.sprite = rightAttackStopSprite;
                    break;
            }

            yield return new WaitForSeconds(attackSpriteDuration);
        }

        spriteRenderer.sprite = originalSprite;
        isAttacking = false;
    }

    public int GetCurrentPositionIndex()
    {
        return currentPositionIndex;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || isParrying)
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

    private IEnumerator PlayParryAnimation()
    {
        isParrying = true;
        isSwordEffectActive = true; // Activate sword effect
        spriteRenderer.color = Color.blue; // Visual indication of sword effect
        animator.enabled = true; // Enable the Animator

        string triggerName = "";
        switch (currentPositionIndex)
        {
            case 0:
                triggerName = "LeftParry";
                break;
            case 1:
                triggerName = "CenterParry";
                break;
            case 2:
                triggerName = "RightParry";
                break;
        }

        animator.SetTrigger(triggerName);

        yield return new WaitForSeconds(parryAnimationDuration);

        animator.ResetTrigger(triggerName); // Reset the trigger
        animator.enabled = false;
        UpdateSprite(); // Update the sprite

        spriteRenderer.color = Color.white; // Reset sprite color
        isSwordEffectActive = false; // Deactivate sword effect
        isParrying = false;
    }

    public bool IsSwordEffectActive()
    {
        return isSwordEffectActive;
    }

}
