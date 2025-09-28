using UnityEngine;

public class BallThrow : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private ShotConfiguration shotConfig;

    [Header("Transforms")]
    [SerializeField] private Transform startPoint;

    [Header ("Targets")]
    [SerializeField] private Transform hoopTarget;       // Perfect shot target
    [SerializeField] private Transform backboardTarget;  // Backboard hit point
    [SerializeField] private Transform normalTarget;      //Normal  hit point
    [SerializeField] private Transform ringTarget;       //Ring Hit - no point
    [SerializeField] private Transform awayTarget;       //Away Shot- no point

    [Header("Rigidbody")]
    public Rigidbody ballRigidbody;
    
    //Shot tag to identify the shot intent
    private ShotTag ballShotTag;

    //Ball controller to start the ball flight
    private BallController ballController;


    private void Start()
    {
        ValidateComponents();
    }

    // Validates all required components and sets up references    
    private void ValidateComponents()
    {
        // Validate shot configuration
        if (shotConfig == null)
        {
            Debug.LogError("BallThrow: ShotConfiguration is not assigned! Using default values.");
            CreateDefaultShotConfig();
        }

        if (ballRigidbody == null)
        {
            Debug.LogError("BallThrow: ballRigidbody reference is missing! Disabling component.");
            enabled = false;
            return;
        }

        ballController = ballRigidbody.GetComponent<BallController>();
        if (ballController == null)
        {
            Debug.LogError("BallThrow: BallController component not found on ballRigidbody! Disabling component.");
            enabled = false;
            return;
        }

        ballShotTag = ballRigidbody.GetComponent<ShotTag>();
        if (ballShotTag == null)
        {
            Debug.LogWarning("BallThrow: ShotTag component not found, adding one automatically.");
            ballShotTag = ballRigidbody.gameObject.AddComponent<ShotTag>();
        }

        // Validate target transforms
        ValidateTargetTransforms();
    }


    // Validates that all required target transforms are assigned
    private void ValidateTargetTransforms()
    {
        if (startPoint == null)
        {
            Debug.LogError("BallThrow: startPoint transform is not assigned!");
        }

        if (hoopTarget == null)
        {
            Debug.LogError("BallThrow: hoopTarget transform is not assigned!");
        }

        if (backboardTarget == null)
        {
            Debug.LogError("BallThrow: backboardTarget transform is not assigned!");
        }

        if (normalTarget == null)
        {
            Debug.LogError("BallThrow: normalTarget transform is not assigned!");
        }

        if (ringTarget == null)
        {
            Debug.LogError("BallThrow: ringTarget transform is not assigned!");
        }

        if (awayTarget == null)
        {
            Debug.LogError("BallThrow: awayTarget transform is not assigned!");
        }
    }

    // Create default shot configuration if not assigned
    private void CreateDefaultShotConfig()
    {
        shotConfig = ScriptableObject.CreateInstance<ShotConfiguration>();
        shotConfig.perfectShotAngle = 55f;
        shotConfig.normalShotAngle = 45f;
        shotConfig.backboardShotAngle = 55f;
        shotConfig.ringShotAngle = 55f;
        shotConfig.awayShotAngle = 40f;
    }

    // For debugging the shots from the editor
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            ThrowNormalShot();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ThrowPerfectShot();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ThrowBackboardShot();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ThrowAtRing();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ThrowAwayShot();
        }
#endif
    }

    //The flight arc depends on the time taken to reach the target
    //Using for normal shot 
    void ThrowAtTarget_WithTime(Vector3 targetPosition, float flightTime)
    {
        Vector3 start = startPoint.position;
        Vector3 r = targetPosition - start;            // displacement to target
        Vector3 v0 = (r / flightTime) - 0.5f * flightTime * Physics.gravity;
        
        // Reset velocity first to avoid conflicts
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;

        ballRigidbody.velocity = v0;

    }

    //Calculate the speed given the angle the ball will be thrown.
    //Using for Perfect/Backboard shot
    void ThrowAtTarget_WithAngle(Vector3 targetPosition, float angleDeg)
    {
        var gVec = Physics.gravity;
        float g = Mathf.Abs(gVec.y);

        Vector3 start = startPoint.position;
        Vector3 r = targetPosition - start;
        Vector3 rXZ = new Vector3(r.x, 0f, r.z);

        float d = rXZ.magnitude;
        float h = r.y;

        float theta = angleDeg * Mathf.Deg2Rad;
        float cosT = Mathf.Cos(theta);
        float sinT = Mathf.Sin(theta);

        float denom = 2f * cosT * cosT * (d * Mathf.Tan(theta) - h);
        if (denom <= 1e-6f) { Debug.LogError("Angle too low/high for this target."); return; }

        float v2 = g * d * d / denom;
        if (v2 <= 0f) { Debug.LogError("Unreachable with this angle."); return; }

        float v = Mathf.Sqrt(v2);
        Vector3 dirXZ = (d > 1e-6f) ? rXZ.normalized : Vector3.forward;

        Vector3 v0 = dirXZ * (v * cosT);
        v0.y = v * sinT;

        // Reset velocity first to avoid conflicts
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        
        ballRigidbody.velocity = v0;


    }

    // Validates that all components needed for throwing are available
    // Returns true if all components are valid, false otherwise
    private bool ValidateThrowComponents()
    {
        if (ballRigidbody == null)
        {
            Debug.LogError("BallThrow: Cannot throw - ballRigidbody is null!");
            return false;
        }

        if (ballController == null)
        {
            Debug.LogError("BallThrow: Cannot throw - ballController is null!");
            return false;
        }

        if (ballController.isInFlight)
        {
            Debug.LogWarning("BallThrow: Cannot throw - ball is already in flight!");
            return false;
        }

        return true;
    }

    [ContextMenu("Throw Perfect Shot")]
    //Throw perfect shot
    public void ThrowPerfectShot()
    {
        if (!ValidateThrowComponents()) return;

        try
        {
            ThrowAtTarget_WithAngle(hoopTarget.position, shotConfig.perfectShotAngle);
            SetShotIntent(ShotTag.IntentType.Perfect, true);
        }
        catch (System.Exception e)
        {
            Debug.LogError("BallThrow: Failed to throw perfect shot: " + e.Message);
        }
    }

    //Throw backboard shot
    public void ThrowBackboardShot()
    {
        if (!ValidateThrowComponents()) return;

        try
        {
            ThrowAtTarget_WithAngle(backboardTarget.position, shotConfig.backboardShotAngle);
            SetShotIntent(ShotTag.IntentType.Backboard, true);
        }
        catch (System.Exception e)
        {
            Debug.LogError("BallThrow: Failed to throw backboard shot: " + e.Message);
        }
    }

    //Throw normal shot
    public void ThrowNormalShot()
    {
        if (!ValidateThrowComponents()) return;

        try
        {
            ThrowAtTarget_WithTime(normalTarget.position, shotConfig.normalShotFlightTime);
            SetShotIntent(ShotTag.IntentType.Normal, true);
        }
        catch (System.Exception e)
        {
            Debug.LogError("BallThrow: Failed to throw normal shot: " + e.Message);
        }
    }

    //Throw ring shot
    public void ThrowAtRing()
    {
        if (!ValidateThrowComponents()) return;

        try
        {
            ThrowAtTarget_WithAngle(ringTarget.position, shotConfig.ringShotAngle);
            SetShotIntent(ShotTag.IntentType.Ring, false);
        }
        catch (System.Exception e)
        {
            Debug.LogError("BallThrow: Failed to throw ring shot: " + e.Message);
        }
    }

    //Throw away shot
    public void ThrowAwayShot()
    {
        if (!ValidateThrowComponents()) return;

        try
        {
            ThrowAtTarget_WithAngle(awayTarget.position, shotConfig.awayShotAngle);
            SetShotIntent(ShotTag.IntentType.Away, false);
        }
        catch (System.Exception e)
        {
            Debug.LogError("BallThrow: Failed to throw away shot: " + e.Message);
        }
    }


   

    // Set the shot intent and starts ball flight
    // intent: The type of shot being attempted
    // isScored: Whether this shot should be considered successful
    void SetShotIntent(ShotTag.IntentType intent, bool isScored)
    {
        if (ballController != null)
        {
            ballController.StartFlight(isScored);
        }

        if (ballShotTag != null)
        {
            ballShotTag.shotIntent = intent;
        }
    }

    
    

}
