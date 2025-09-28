using UnityEngine;

[CreateAssetMenu(fileName = "BallConfig", menuName = "Basketball/Ball Configuration")]
public class BallConfiguration : ScriptableObject
{
    [Header("Physics Settings")]
    //Mass of the basketball
    public float mass = 0.6f;
    //Radius of the basketball
    public float radius = 0.12f;
    
    //Drag coefficient for the basketball
    public float drag = 0f;
    
    //Physics material for the basketball
    public PhysicMaterial ballPhysicsMaterial;

    [Header("Reset Settings")]
    //Maximum time the ball can be in flight before auto-reset
    public float maxFlightDuration = 2.5f;
    
    //Minimum Y position before ball is considered out of bounds
    public float minYResetThreshold = -2f;

    [Header("Validation")]
    //Enable validation warning
    public bool enableValidation = true;

    // Validates the configuration values
    private void OnValidate()
    {
        if (!enableValidation) return;

        // Validate mass
        if (mass <= 0f)
        {
            Debug.LogWarning("BallConfiguration: Mass must be greater than 0. Setting to default 0.6.");
            mass = Constants.DEFAULT_BALL_MASS;
        }
        else if (mass > Constants.MAX_BALL_MASS)
        {
            Debug.LogWarning("BallConfiguration: Mass seems unusually high. Consider if this is intended.");
        }

        // Validate radius
        if (radius <= 0f)
        {
            Debug.LogWarning("BallConfiguration: Radius must be greater than 0. Setting to default 0.12m.");
            radius = Constants.DEFAULT_BALL_RADIUS;
        }
        else if (radius > Constants.MAX_BALL_RADIUS)
        {
            Debug.LogWarning("BallConfiguration: Radius seems unusually large. Consider if this is intended.");
        }

        // Validate drag
        if (drag < 0f)
        {
            Debug.LogWarning("BallConfiguration: Drag cannot be negative. Setting to 0.");
            drag = Constants.DEFAULT_BALL_DRAG;
        }

        // Validate flight duration
        if (maxFlightDuration <= 0f)
        {
            Debug.LogWarning("BallConfiguration: Max flight duration must be greater than 0. Setting to default 2.5s.");
            maxFlightDuration = Constants.DEFAULT_MAX_FLIGHT_DURATION;
        }

        // Validate reset threshold
        if (minYResetThreshold > 0f)
        {
            Debug.LogWarning("BallConfiguration: Min Y reset threshold should typically be negative. Current value: " + minYResetThreshold);
        }
    }

}
