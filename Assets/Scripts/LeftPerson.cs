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

    private bool isSwordEffectActive = false; // Track if sword effect is active
    [SerializeField] private float swordEffectDuration = 1.0f; // Duration of sword effect
    [SerializeField] private float swordEffectCooldown = 2.0f; // Cooldown time before the effect can be used again
    private float nextSwordEffectTime = 0f; // Time when the sword effect can be used again

    private enum ActionStage { Attack, Deflect }
    private ActionStage currentStage = ActionStage.Attack; // Initial stage

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

        // Activate sword effect when "W" key is pressed, and cooldown has passed
        if (Input.GetKeyDown(KeyCode.W) && Time.time >= nextSwordEffectTime && !isSwordEffectActive && currentStage == ActionStage.Deflect)
        {
            StartCoroutine(ActivateSwordEffect());
            nextSwordEffectTime = Time.time + swordEffectCooldown; // Set the next available time
        }

        // Change stage when Space key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleStage();
        }
    }

    private void MoveToSpot()
    {
        transform.position = spots[currentPositionIndex].position;
        nextMoveTime = Time.time + stopMovingTime;
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
    }

    public int GetCurrentPositionIndex()
    {
        return currentPositionIndex;
    }

    public void TakeDamage(int damage)
    {
        // Only take damage if not in Deflect mode or if sword effect is inactive
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
        else
        {
            Debug.Log("Attack deflected!");
        }
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
        spriteRenderer.color = Color.blue; // Change color to blue for sword effect

        yield return new WaitForSeconds(swordEffectDuration);

        spriteRenderer.color = Color.white; // Revert to original color
        isSwordEffectActive = false;
    }

    public bool IsSwordEffectActive()
    {
        return isSwordEffectActive;
    }
}
