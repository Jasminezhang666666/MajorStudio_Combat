using UnityEngine;

public class RightPerson : MonoBehaviour
{
    public Sprite[] circleSprites; // Array of sprites for each circle

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateSprite(int circleId)
    {
        // Check if the circle ID is within the range
        if (circleId >= 0 && circleId < circleSprites.Length)
        {
            spriteRenderer.sprite = circleSprites[circleId];
        }
    }
}
