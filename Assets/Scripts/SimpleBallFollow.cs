using UnityEngine;

public class SimpleBallFollow : MonoBehaviour
{
    [SerializeField] private Rigidbody ballRigidbody;
    [SerializeField] private Transform targetPoint;

    private float distanceToTarget;
    private bool isFollowing = true;

    //the fixed offset from the ball when following
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 3f, -5f);

    [Range(0.01f, 1f)]
    //how smoothly the camera follows the ball
    [SerializeField] private float smoothSpeed = 0.125f;

    //Camera stops following the ball at this distance
    [SerializeField] private float stopFollowDistance = 10f;

    //fixed position for the camera once it stops following
    [SerializeField] private Vector3 finalCameraPosition = new Vector3(0f, 6f, -10f);

    //the final point the camera should look at
    [SerializeField] private Vector3 finalLookAtPoint = new Vector3(0f, 3f, 0f);


    private void Start()
    {
        // Basic safety check
        if (ballRigidbody == null || targetPoint == null)
        {
            Debug.LogError("Ball Rigidbody or Target Point is not assigned in the BallFollowCamera script.");
        }

        // Initialize the camera's rotation to look towards the target
        if (targetPoint != null)
        {
            transform.LookAt(targetPoint);
        }
    }

    // LateUpdate is called after all Update functions have been called
    private void LateUpdate()
    {
        if (ballRigidbody == null || targetPoint == null) return;

        if (isFollowing)
        {
            // distance calculation to avoid repeated Vector3.Distance calls
            distanceToTarget = Vector3.Distance(transform.position, targetPoint.position);

            if (distanceToTarget <= stopFollowDistance)
            {
                // stop following the ball
                isFollowing = false;
            }
            else
            {
                FollowBall();
            }
        }

        else
        {
            TransitionToFixedView();
        }
    }

    private void FollowBall()
    {
        Vector3 desiredPosition = ballRigidbody.position + followOffset;

        // Use Vector3.Lerp for smooth transition
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Keep looking at the target
        transform.LookAt(targetPoint);
    }

    private void TransitionToFixedView()
    {
        // Smoothly move to the final position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, finalCameraPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Smoothly adjust the look direction
        Quaternion targetRotation = Quaternion.LookRotation(finalLookAtPoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed);
    }

}