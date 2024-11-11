using UnityEngine;
using System.Linq;

public class BossAttack : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private int rightPersonHitDamage = 8;
    [SerializeField] private int leftPersonHitDamage = 3;
    [SerializeField] private int BossHitDamage = 10;

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
        targetPosition = target;

        // Set initial direction towards the target
        direction = (targetPosition - transform.position).normalized;
    }

    private void LateUpdate()
    {
        if (!targetReached)
        {
            // Move towards the target pos
            transform.position += direction * speed * Time.deltaTime;

            // Check if the projectile has reached the target
            if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
            {
                targetReached = true;

                transform.position = targetPosition;

                // Set new direction towards RightPerson
                direction = (rightPersonPosition - transform.position).normalized;
                movingTowardsRightPerson = true;
            }
        }
        else if (movingTowardsRightPerson)
        {
            // Update direction towards RightPerson
            direction = (rightPersonPosition - transform.position).normalized;

            // moving towards RightPerson
            transform.position += direction * speed * Time.deltaTime;

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
                    // Apply damage to the LeftPerson
                    leftPerson.TakeDamage(leftPersonHitDamage); 
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
                    boss.TakeDamage(BossHitDamage); // Decrease boss health
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
                rightPerson.TakeDamage(rightPersonHitDamage);
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
        }
    }


}
