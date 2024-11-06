using UnityEngine;
using System.Collections;

public class RightPerson : MonoBehaviour
{
    public Sprite[] normalSprites;
    public Sprite[] withSpeedLineSprites;
    [SerializeField] private float delayTime = 0.5f;
    private SpriteRenderer spriteRenderer;
    private Coroutine spriteSwitchCoroutine;
    private LeftPerson leftPerson;
    [SerializeField] private float damageFlashDuration = 0.2f;
    private bool isInvincible = false;
    public GameObject shieldPrefab;
    private GameObject activeShield;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        leftPerson = GameObject.FindObjectOfType<LeftPerson>();
        if (leftPerson == null)
        {
            Debug.LogError("LeftPerson not found in the scene.");
        }
    }

    public void UpdateSpriteWithDelay(int circleId)
    {
        if (spriteSwitchCoroutine != null)
        {
            StopCoroutine(spriteSwitchCoroutine);
        }
        spriteSwitchCoroutine = StartCoroutine(SwitchToNormalSprite(circleId));
    }

    private IEnumerator SwitchToNormalSprite(int circleId)
    {
        if (circleId >= 0 && circleId < withSpeedLineSprites.Length)
        {
            spriteRenderer.sprite = withSpeedLineSprites[circleId];
        }
        yield return new WaitForSeconds(delayTime);
        if (circleId >= 0 && circleId < normalSprites.Length)
        {
            spriteRenderer.sprite = normalSprites[circleId];
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible)
            return;
        if (leftPerson != null)
        {
            leftPerson.TakeDamage(damage);
            StartCoroutine(FlashRed());
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
}
