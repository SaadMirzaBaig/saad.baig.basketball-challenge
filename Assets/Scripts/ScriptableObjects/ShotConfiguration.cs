using UnityEngine;

[CreateAssetMenu(fileName = "ShotConfig", menuName = "Basketball/Shot Configuration")]
public class ShotConfiguration : ScriptableObject
{
    [Header("Perfect Shot Settings")]
    //Angle in degrees for perfect shots
    public float perfectShotAngle = 55f;

    //Flight time in seconds for perfect shots
    public float perfectShotFlightTime = 1.2f;

    [Header("Normal Shot Settings")]
    //Angle in degrees for normal shots
    public float normalShotAngle = 45f;

    //Flight time in seconds for normal shots
    public float normalShotFlightTime = 1.5f;

    [Header("Backboard Shot Settings")]
    //Angle in degrees for backboard shots
    public float backboardShotAngle = 55f;
    
    //Flight time in seconds for backboard shots
    public float backboardShotFlightTime = 1.3f;

    [Header("Ring Shot Settings")]
    //Angle in degrees for ring shots
    public float ringShotAngle = 55f;

    [Header("Away Shot Settings")]
    //Angle in degrees for away shots
    public float awayShotAngle = 40f;

    // Validates the configuration values
    private void OnValidate()
    {
        // Ensure angles are within reasonable range
        perfectShotAngle = Mathf.Clamp(perfectShotAngle, 30f, 80f);
        normalShotAngle = Mathf.Clamp(normalShotAngle, 30f, 80f);
        backboardShotAngle = Mathf.Clamp(backboardShotAngle, 30f, 80f);
        ringShotAngle = Mathf.Clamp(ringShotAngle, 30f, 80f);
        awayShotAngle = Mathf.Clamp(awayShotAngle, 20f, 70f);

        // Ensure flight times are positive
        perfectShotFlightTime = Mathf.Max(Constants.MIN_FLIGHT_TIME, perfectShotFlightTime);
        normalShotFlightTime = Mathf.Max(Constants.MIN_FLIGHT_TIME, normalShotFlightTime);
        backboardShotFlightTime = Mathf.Max(Constants.MIN_FLIGHT_TIME, backboardShotFlightTime);
    }
}
