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

    private Coroutine flashCoroutine;
    private Coroutine attackCoroutine;
    private Coroutine parryCoroutine;

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

    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource parrySound;

    private GameManager gameManager;

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

        animator = GetComponent<Animator>();
        animator.enabled = false;

        UpdateUI();
        UpdateSprite();

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
                if (parryCoroutine != null)
                {
                    StopCoroutine(parryCoroutine);
                    parryCoroutine = null;
                }
                if (parrySound != null)
                    parrySound.Play();
                parryCoroutine = StartCoroutine(PlayParryAnimation());
            }
            else if (currentStage == ActionStage.Attack)
            {
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                    attackCoroutine = null;
                }
                if (attackSound != null)
                    attackSound.Play();
                attackCoroutine = StartCoroutine(DisplayAttackSpriteSequence());
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
        }
        else
        {
            currentStage = ActionStage.Attack;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (attackUI != null)
            attackUI.SetActive(currentStage == ActionStage.Attack);
        if (deflectUI != null)
            deflectUI.SetActive(currentStage == ActionStage.Deflect);
    }

    private void UpdateSprite()
    {
        if (animator != null && animator.enabled)
            return;

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
                GameManager gameManager = FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.Defeat();
                }
            }
            else
            {
                if (flashCoroutine != null)
                {
                    StopCoroutine(flashCoroutine);
                }
                flashCoroutine = StartCoroutine(FlashRed());
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
            activeShield.transform.localScale = Vector3.one;
        }
        yield return new WaitForSeconds(duration);
        if (activeShield != null)
        {
            Destroy(activeShield);
        }
        isInvincible = false;
    }

    public IEnumerator ChangeColorTemporary(Color color, float duration)
    {
        spriteRenderer.color = color;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = Color.white;
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
        isSwordEffectActive = true;
        animator.enabled = true;

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

        animator.ResetTrigger(triggerName);
        animator.enabled = false;
        UpdateSprite();

        isSwordEffectActive = false;
        isParrying = false;
    }

    public bool IsSwordEffectActive()
    {
        return isSwordEffectActive;
    }

    public bool IsInvincible
    {
        get { return isInvincible; }
    }

}
