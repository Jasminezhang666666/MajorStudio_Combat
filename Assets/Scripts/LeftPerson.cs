using UnityEngine;

public class LeftPerson : MonoBehaviour
{
    public Transform leftSpot;
    public Transform centerSpot;
    public Transform rightSpot;

    // Current position index: 0 = left, 1 = center, 2 = right
    private int currentPositionIndex = 1;

    private Transform[] spots;

    // Time to wait before allowing another move
    [SerializeField] private float stopMovingTime = 1.0f;
    private float nextMoveTime = 0f;

    private void Start()
    {
        spots = new Transform[3] { leftSpot, centerSpot, rightSpot };

        // starting position at the center
        transform.position = spots[currentPositionIndex].position;
    }

    private void Update()
    {
        // time check
        if (Time.time >= nextMoveTime)
        {
            // Move left
            if (Input.GetKeyDown(KeyCode.A) && currentPositionIndex > 0)
            {
                currentPositionIndex--;
                MoveToSpot();
            }

            // Move right
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
}
