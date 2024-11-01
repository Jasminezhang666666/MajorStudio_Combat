using UnityEngine;
using System.Linq;

public class BossAttack : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Vector3 direction;
    private Vector3 targetPosition;
    private Vector3 rightPersonPosition;
    private Camera[] cameras;
    private bool isDeflected = false;
    private bool targetReached = false;
    private bool movingTowardsRightPerson = false;
    private Rigidbody rb;
    private GameObject rightPersonObj;

    private void Start()
    {
        cameras = Camera.allCameras;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Find the RightPerson in the scene
        rightPersonObj = GameObject.FindWithTag("RightPerson");
        if (rightPersonObj != null)
        {
            rightPersonPosition = rightPersonObj.transform.position;
        }
        else
        {
            Debug.LogError("RightPerson not found in the scene. Please make sure RightPerson has the correct tag.");
        }
    }

    public void SetTarget(Vector3 target)
    {
        // Store the target position
        targetPosition = target;

        // Set initial direction towards the target
        direction = (targetPosition - transform.position).normalized;

        // Debug to verify direction includes Z-axis
        Debug.Log("Initial Direction: " + direction);
    }

    private void LateUpdate()
    {
        if (!targetReached)
        {
            // Move towards the target position
            transform.position += direction * speed * Time.deltaTime;

            // Check if the projectile has reached the target
            if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
            {
                targetReached = true;

                // Snap to exact target position
                transform.position = targetPosition;

                // Set new direction towards RightPerson
                direction = (rightPersonPosition - transform.position).normalized;
                movingTowardsRightPerson = true;

                Debug.Log("Target reached. Now moving towards RightPerson.");
            }
        }
        else if (movingTowardsRightPerson)
        {
            // Update rightPersonPosition in case RightPerson moves
            rightPersonPosition = rightPersonObj.transform.position;

            // Update direction towards RightPerson
            direction = (rightPersonPosition - transform.position).normalized;

            // Continue moving towards RightPerson
            transform.position += direction * speed * Time.deltaTime;

            // Optionally, check if it has reached RightPerson
            if (Vector3.Distance(transform.position, rightPersonPosition) <= 0.1f)
            {
                // Trigger effects on RightPerson here
                // For example, you can call a method on RightPerson
                RightPerson rightPerson = rightPersonObj.GetComponent<RightPerson>();
                if (rightPerson != null)
                {
                    // Implement any effects on RightPerson
                }

                // Destroy the projectile
                Destroy(gameObject);
            }
        }

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
        else if (collision.gameObject.CompareTag("RightPerson"))
        {
            // Handle collision with RightPerson
            RightPerson rightPerson = collision.gameObject.GetComponent<RightPerson>();
            if (rightPerson != null)
            {
                // Implement any effects on RightPerson here
            }
            Destroy(gameObject);
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
