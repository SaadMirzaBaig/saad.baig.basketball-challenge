using UnityEngine;
using System.Collections.Generic;

public class BallController : MonoBehaviour
{
    public PhysicMaterial ballPhysicsMaterial;

    [Header("Ball State")]
    public bool isInFlight = false;
    public bool hasScored = false;

    private Rigidbody rb;
    private Vector3 startPosition;

    private int currentPos = 0;
    [SerializeField] private List<Transform> newBallPosition;

    private void Awake()
    {
        SetupBallComponents();
        SetNewPosition();
        //startPosition = transform.position;
    }

    private void SetupBallComponents()
    {
        // Setup Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.mass = 0.6f; // Standard basketball mass
        rb.drag = 0; // Basketball drag coefficient
        rb.useGravity = true;

        // Setup Collider
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = 0.12f; // Standard basketball radius
        }

        // Apply physics material if provided
        if (ballPhysicsMaterial != null)
        {
            sphereCollider.material = ballPhysicsMaterial;
        }

        // Set tag for identification
        gameObject.tag = Constants.BALL_TAG;
    }


    private void FixedUpdate()
    {
        if (isInFlight)
        {

            // the speed is slowing down
            if (rb.velocity.magnitude < 2 )
            {

                Debug.Log("Ball missed resetting the position");
                ResetBall();
            }
        }
    }

 
    public void OnBallScored()
    {
        hasScored = true;
        Debug.Log("GOAL! Ball scored!");
    }
    
    public void ResetBall()
    {
        //If the ball is missed Resgister it with missed target
        if (isInFlight && !hasScored)
        {
            ScoreManager.Instance?.RegisterShot(
                isSuccessful: false,
                isPerfect: false,
                hasBackboardBonus: false
            );
        }

        // Reset position
        SetNewPosition();

        // Reset physics
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset state
        isInFlight = false;
        hasScored = false;

        Debug.Log("Ball reset to starting position");
    }

    private void SetNewPosition()
    {
        Debug.Log("setting up new position");
        if(currentPos < newBallPosition.Count)
        {
            transform.position = newBallPosition[currentPos].position;
            currentPos++;
        }
        else
        {
            currentPos = 0;
            transform.position = newBallPosition[currentPos].position;

        }

    }
}