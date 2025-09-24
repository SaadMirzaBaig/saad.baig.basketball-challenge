using UnityEngine;

public class BallThrow : MonoBehaviour
{
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

    private ShotTag ballShotTag;

    private void Start()
    {
        if (ballRigidbody.GetComponent<ShotTag>())
        {
            ballShotTag = ballRigidbody.GetComponent<ShotTag>();
        }
        else
        {
            ballRigidbody.gameObject.AddComponent<ShotTag>();
        }
    }
    /// <summary>
    /// For debuging the shots
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
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
    }

    // Core velocity solver
    // Different approaches to hit the target
    // The quadratic approach calculates angle and speed to have the perfect trajectory to the target
    void ThrowAtTarget_WithQuadraticApproach(Vector3 target)
    {
        if (ballRigidbody == null || startPoint == null)
        {
            Debug.LogError("Assign Rigidbody and StartPoint first!");
            return;
        }

        Vector3 startPos = startPoint.position;
        Vector3 displacement = target - startPos;
        Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);

        float distanceXZ = displacementXZ.magnitude;
        float height = displacement.y;
        float g = Mathf.Abs(Physics.gravity.y);

        // --- Step 1: Guaranteed min speed solver ---
        float a = 1f;
        float b = -2f * g * height;
        float c = -g * g * distanceXZ * distanceXZ;

        float discriminant = b * b - 4f * a * c;
        if (discriminant < 0f)
        {
            Debug.LogError("Target unreachable!");
            return;
        }

        float v2 = (-b + Mathf.Sqrt(discriminant)) / (2f * a);
        v2 = Mathf.Max(v2, 1e-6f);
        float v = Mathf.Sqrt(v2);

        // --- Step 2: Compute angles ---
        float underRoot = v2 * v2 - g * (g * distanceXZ * distanceXZ + 2 * height * v2);
        underRoot = Mathf.Max(underRoot, 0f);

        float sqrtRoot = Mathf.Sqrt(underRoot);
        float angle = Mathf.Atan((v2 - sqrtRoot) / (g * distanceXZ));

        // --- Step 3: Compute velocity vector ---
        Vector3 dirXZ = displacementXZ.normalized;
        Vector3 velocity = dirXZ * v * Mathf.Cos(angle);
        velocity.y = v * Mathf.Sin(angle);

        ballRigidbody.velocity = velocity;

        Debug.Log("Shot to " + target + " speed " + v + " angle " + angle * Mathf.Rad2Deg);
    }

    //The flight arc depends on the time taken to reach the target
    //Using for normal shot
    void ThrowAtTarget_WithTime(Vector3 targetPosition, float flightTime)
    {
        Vector3 start = startPoint.position;
        Vector3 r = targetPosition - start;            // displacement to target
        Vector3 v0 = (r / flightTime) - 0.5f * Physics.gravity * flightTime;
        ballRigidbody.velocity = v0;
    }

    //Calcualte the speed given the angle the ball will be thrown.
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

        ballRigidbody.velocity = v0;
    }



    [ContextMenu("Throw Perfect Shot")]
    public void ThrowPerfectShot()
    {
        ThrowAtTarget_WithAngle(hoopTarget.position, 55f);

        SetShotIntent(ShotTag.IntentType.Perfect);
    }

    public void ThrowBackboardShot()
    {
        ThrowAtTarget_WithAngle(backboardTarget.position, 55f);
        SetShotIntent(ShotTag.IntentType.Backboard);

    }

    public void ThrowNormalShot()
    {
        ThrowAtTarget_WithTime(normalTarget.position,1.5f);
        SetShotIntent(ShotTag.IntentType.Normal);

    }

    public void ThrowAtRing()
    {
        ThrowAtTarget_WithAngle(ringTarget.position, 55f);
    }

    public void ThrowAwayShot()
    {
        ThrowAtTarget_WithAngle(awayTarget.position, 40f);
        SetShotIntent(ShotTag.IntentType.Away);
    }


    void SetShotIntent(ShotTag.IntentType intent)
    {
        ballRigidbody.GetComponent<BallController>().isInFlight = true;
        ballShotTag.shotIntent = intent;
      
    }
}
