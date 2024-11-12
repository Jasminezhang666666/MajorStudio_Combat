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

    [SerializeField] private GameObject hitBossSoundPrefab;
    [SerializeField] private GameObject hitLeftSoundPrefab;
    [SerializeField] private GameObject hitRightSoundPrefab;

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
        direction = (targetPosition - transform.position).normalized;
    }

    private void LateUpdate()
    {
        if (!targetReached)
        {
            transform.position += direction * speed * Time.deltaTime;
            if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
            {
                targetReached = true;
                transform.position = targetPosition;
                direction = (rightPersonPosition - transform.position).normalized;
                movingTowardsRightPerson = true;
            }
        }
        else if (movingTowardsRightPerson)
        {
            direction = (rightPersonPosition - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }

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
                    DeflectTowardsBoss();
                }
                else
                {
                    Instantiate(hitLeftSoundPrefab, transform.position, Quaternion.identity);
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
                    Instantiate(hitBossSoundPrefab, transform.position, Quaternion.identity);
                    boss.TakeDamage(BossHitDamage);
                }
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("RightPerson"))
        {
            RightPerson rightPerson = collision.gameObject.GetComponent<RightPerson>();
            if (rightPerson != null)
            {
                Instantiate(hitRightSoundPrefab, transform.position, Quaternion.identity);
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
