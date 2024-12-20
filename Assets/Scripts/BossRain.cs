using UnityEngine;

public class BossRain : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f; 
    [SerializeField] private int leftPersonHitDamage = 10;
    [SerializeField] private GameObject hitLeftSoundPrefab;

    private void Update()
    {
        // Move downwards
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        // Destroy if Y position is less than -2.56
        if (transform.position.y < -2.56f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("Colliding");

        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if the collided object is LeftPerson and apply damage
            LeftPerson leftPerson = collision.gameObject.GetComponent<LeftPerson>();
            if (leftPerson != null)
            {
                Instantiate(hitLeftSoundPrefab, transform.position, Quaternion.identity);
                leftPerson.TakeDamage(leftPersonHitDamage);
                Destroy(gameObject);
            }
        }
    }
}