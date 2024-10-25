using UnityEngine;


public class Boss : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;

    // Time interval between shots (in frames)
    [SerializeField] private float timeBetweenShots = 2.0f;

    private float nextFireTime = 0f;

    public Transform leftSpot;
    public Transform centerSpot;
    public Transform rightSpot;

    [SerializeField] private LeftPerson leftPerson;


    private void Update()
    {
        // Check if enough time has passed to fire again
        if (Time.time >= nextFireTime)
        {
            ShootAtPlayer();
            nextFireTime = Time.time + timeBetweenShots;
        }
    }

    // determine the player's position
    private void ShootAtPlayer()
    {
        // Get the LeftPerson's current position index, and its target position
        int playerPositionIndex = leftPerson.GetCurrentPositionIndex();

        Vector3 targetPosition = centerSpot.position;

        if (playerPositionIndex == 0)
        {
            targetPosition = leftSpot.position;
        }
        else if (playerPositionIndex == 2)
        {
            targetPosition = rightSpot.position;
        }

        // Maintain the Z-axis difference between the boss and the player
        targetPosition.z = leftPerson.transform.position.z;

        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<BossAttack>().SetTarget(targetPosition);
    }



}
