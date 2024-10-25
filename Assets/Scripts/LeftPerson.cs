using UnityEngine;
using System.Collections;


public class LeftPerson : MonoBehaviour
{
    public Transform leftSpot;
    public Transform centerSpot;
    public Transform rightSpot;

    private int currentPositionIndex = 1;
    private Transform[] spots;

    [SerializeField] private float stopMovingTime = 1.0f;
    private float nextMoveTime = 0f;

    // Health of the LeftPerson
    [SerializeField] private int health = 10;

    // Duration for which the player turns red when hit
    [SerializeField] private float damageFlashDuration = 0.2f;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spots = new Transform[3] { leftSpot, centerSpot, rightSpot };
        transform.position = spots[currentPositionIndex].position;
        spriteRenderer = GetComponent<SpriteRenderer>();
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
    }

    private void MoveToSpot()
    {
        transform.position = spots[currentPositionIndex].position;
        nextMoveTime = Time.time + stopMovingTime;
    }

    public int GetCurrentPositionIndex()
    {
        return currentPositionIndex;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            // GAME OVER!!!!!!!




            Debug.Log("Player has been defeated!");
        }
        else
        {
            // Flash red to indicate damage
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
