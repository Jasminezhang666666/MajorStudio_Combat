using UnityEngine;
using System.Linq;

public class BossAttack : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Vector3 direction;
    private Camera[] cameras;
    private bool isDeflected = false;
    private Rigidbody rb;

    private void Start()
    {
        cameras = Camera.allCameras;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void SetTarget(Vector3 target)
    {
        // Set direction based on target, ensuring Z-axis is included
        direction = (target - transform.position).normalized;

        // Debug to verify direction includes Z-axis
        Debug.Log("Initial Direction: " + direction);
    }

    private void LateUpdate()
    {
        // Move the projectile along the set direction vector
        transform.position += direction * speed * Time.deltaTime;

        // Debug to verify Z-axis movement
        //Debug.Log("Current Position: " + transform.position + ", Direction: " + direction);

        // Destroy if out of camera view
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
            if (viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
                viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
                viewportPoint.z > 0)
            {
                return true;
            }
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LeftPerson leftPerson = collision.gameObject.GetComponent<LeftPerson>();
            if (leftPerson != null)
            {
                if (leftPerson.IsSwordEffectActive())
                {
                    // Deflect projectile back toward the boss
                    DeflectTowardsBoss();
                }
                else
                {
                    // Apply damage to the player and destroy the projectile
                    leftPerson.TakeDamage(10); // Adjust damage value as needed
                    Destroy(gameObject);
                }
            }
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {
            if (isDeflected)
            {
                Boss boss = collision.gameObject.GetComponent<Boss>();
                if (boss != null)
                {
                    boss.TakeDamage(1); // Decrease boss health
                }
                Destroy(gameObject);
            }
        }
    }

    private void DeflectTowardsBoss()
    {
        if (!isDeflected)
        {
            isDeflected = true;
            direction = -direction;
            Debug.Log("Deflected Direction: " + direction);
        }
    }
}
