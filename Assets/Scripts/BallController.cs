using UnityEngine;
using System.Collections.Generic;

public class BallController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private BallConfiguration ballConfig;

    [Header("Ball State")]
    public bool isInFlight = false;
    public bool hasScored = false;

    [SerializeField] private List<Transform> newBallPosition;

    private float flightStartTime;
    private Rigidbody rb;
    
    //Current position index
    private int currentPos = 0;

    private void Awake()
    {
        SetupBallComponents();
        SetNewPosition();
    }

    private void SetupBallComponents()
    {
        // Validate configuration
        if (ballConfig == null)
        {
            Debug.LogError("BallController: BallConfiguration is not assigned! Using default values.");
            CreateDefaultConfig();
        }

        // Setup Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.mass = ballConfig.mass;
        rb.drag = ballConfig.drag;
        rb.useGravity = true;

        // Setup Collider
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        }
        sphereCollider.radius = ballConfig.radius;

        // Apply physics material if provided
        if (ballConfig.ballPhysicsMaterial != null)
        {
            sphereCollider.material = ballConfig.ballPhysicsMaterial;
        }

        // Set tag for identification
        gameObject.tag = Constants.BALL_TAG;
    }

    //Create default configuration
    private void CreateDefaultConfig()
    {
        ballConfig = ScriptableObject.CreateInstance<BallConfiguration>();
        ballConfig.mass = Constants.DEFAULT_BALL_MASS;
        ballConfig.radius = Constants.DEFAULT_BALL_RADIUS;
        ballConfig.drag = Constants.DEFAULT_BALL_DRAG;
        ballConfig.maxFlightDuration = Constants.DEFAULT_MAX_FLIGHT_DURATION;
        ballConfig.minYResetThreshold = Constants.DEFAULT_MIN_Y_RESET_THRESHOLD;
    }


    private void FixedUpdate()
    {
        if (isInFlight)
        {
            // Out-of-bounds check
            if (transform.position.y < ballConfig.minYResetThreshold)
            {
                ResetBall();
                return;
            }

            // Flight timeout to avoid hanging balls
            if (Time.time - flightStartTime > ballConfig.maxFlightDuration)
            {
                ResetBall();
                return;
            }
        }
    }

    //On ball scored
    public void OnBallScored()
    {
        hasScored = true;
    }


    //Start ball flight
    public void StartFlight(bool willScore)
    {
        isInFlight = true;
        hasScored = willScore;
        flightStartTime = Time.time;
        
        // Update centralized data
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.UpdateBallState(isInFlight, hasScored);
        }
    }
    
    //Reset ball properties
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

        SetNewPosition();

        // Reset physics
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset state
        isInFlight = false;
        hasScored = false;
        
        // Update GameDataManager
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.UpdateBallState(isInFlight, hasScored);
        }

        
    }

    //Set new position
    private void SetNewPosition()
    {
        
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