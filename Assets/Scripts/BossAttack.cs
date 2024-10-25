using UnityEngine;
using System.Linq;

public class BossAttack : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Vector3 direction;

    private Camera[] cameras;

    private void Start()
    {
        // Find all cameras tagged as "MainCamera"
        cameras = GameObject.FindGameObjectsWithTag("MainCamera")
            .Select(go => go.GetComponent<Camera>())
            .Where(cam => cam != null)
            .ToArray();

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void SetTarget(Vector3 target)
    {
        // Calculate direction towards the target (X, Y, and Z) and normalize it
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
        // Check if the projectile hits the player (LeftPerson). They are both 3D rigid body and collider now.
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Projectile hit the player!");

            // LeftPerson TakeDamage anim
            LeftPerson leftPerson = collision.gameObject.GetComponent<LeftPerson>();
            if (leftPerson != null)
            {
                leftPerson.TakeDamage(1); // Adjust the damage amount as needed!!!!!!!!!
            }

            // Destroy the projectile after it hits the player
            Destroy(gameObject);
        }
    }
}
