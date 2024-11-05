using UnityEngine;
using System.Collections;

public class RightPerson : MonoBehaviour
{
    public Sprite[] normalSprites;
    public Sprite[] withSpeedLineSprites;
    [SerializeField] private float delayTime = 0.5f;  // Delay time before switching to normal sprite

    private SpriteRenderer spriteRenderer;
    private Coroutine spriteSwitchCoroutine;

    private LeftPerson leftPerson;

    [SerializeField] private float damageFlashDuration = 0.2f; // Duration of red flash

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Find the LeftPerson in the scene
        leftPerson = GameObject.FindObjectOfType<LeftPerson>();
        if (leftPerson == null)
        {
            Debug.LogError("LeftPerson not found in the scene.");
        }
    }

    public void UpdateSpriteWithDelay(int circleId)
    {
        // Stop any ongoing coroutine
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

    public void TakeDamage(int damage)
    {
        if (leftPerson != null)
        {
            leftPerson.TakeDamage(damage); // Apply damage to LeftPerson
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
