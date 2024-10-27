using UnityEngine;
using System.Linq;

public class BossAttack : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Vector3 direction;

    private Camera[] cameras;

    private bool hasCollided = false; 

    private void Start()
    {
        cameras = Camera.allCameras;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void SetTarget(Vector3 target)
    {
        // Calculate direction towards the target and normalize it
        direction = (target - transform.position).normalized;

        // Set the velocity of the Rigidbody to move continuously in the target direction
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = direction * speed;
    }

    private void Update()
    {
        if (!IsVisibleFromAnyCamera())
        {
            Destroy(gameObject);
        }
    }

    private bool IsVisibleFromAnyCamera()
    {
        foreach (Camera cam in cameras)
        {
            Vector3 viewportPoint = cam.WorldToViewportPoint(transform.position);
            // Check if the object is within the viewport of this camera
            if (viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
                viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
                viewportPoint.z > 0)
            {
                return true; // It is visible from this camera
            }
        }
        return false; // It is not visible from any of the cameras
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the projectile hits the player (LeftPerson)
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Projectile hit the player!");

            // LeftPerson takes damage
            LeftPerson leftPerson = collision.gameObject.GetComponent<LeftPerson>();
            if (leftPerson != null)
            {
                leftPerson.TakeDamage(10); // Deduct 10 HP
            }

            // Destroy the projectile after it hits the player
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            // Destroy the projectile if it hits the ground or any other object
            Destroy(gameObject);
        }
    }
}
