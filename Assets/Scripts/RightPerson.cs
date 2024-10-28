using UnityEngine;
using System.Collections;

public class RightPerson : MonoBehaviour
{
    public Sprite[] normalSprites;        
    public Sprite[] withSpeedLineSprites;    
    [SerializeField] private float delayTime = 0.5f;  // Delay time before switching to normal sprite

    private SpriteRenderer spriteRenderer;
    private Coroutine spriteSwitchCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateSpriteWithDelay(int circleId)
    {
        // Stop any ongoing coroutine to prevent interference
        if (spriteSwitchCoroutine != null)
        {
            StopCoroutine(spriteSwitchCoroutine);
        }

        // switch sprites with delay
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

        // Display the normal sprite, if no other circle has been hovered
        if (circleId >= 0 && circleId < normalSprites.Length)
        {
            spriteRenderer.sprite = normalSprites[circleId];
        }
    }
}
