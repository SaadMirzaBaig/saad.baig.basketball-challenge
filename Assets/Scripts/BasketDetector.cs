using UnityEngine;

public class BasketDetector : MonoBehaviour
{
    [Header("Basket Detection Setup")]
    public Transform hoopCenter;
    public float hoopRadius = 0.23f;
    public float detectionHeight = 0.5f;


    // Detection components
    private SphereCollider detectionTrigger;


    private void Awake()
    {
        SetupBasketDetection();
    }

    private void SetupBasketDetection()
    {
        // Create detection zone below the hoop
        GameObject detectionZone = new GameObject("BasketDetectionZone");
        detectionZone.transform.parent = transform;

        // Position the detection zone below hoop center
        if (hoopCenter != null)
        {
            detectionZone.transform.position = hoopCenter.position + Vector3.down * detectionHeight;
        }
        else
        {
            // If no hoop center specified, use this GameObject's position
            detectionZone.transform.position = transform.position + Vector3.down * detectionHeight;
            Debug.LogWarning("No hoop center assigned - using BasketDetector position");
        }

        // Setup trigger collider for detection
        detectionTrigger = detectionZone.AddComponent<SphereCollider>();
        detectionTrigger.isTrigger = true;
        detectionTrigger.radius = hoopRadius * 1.1f; // Slightly larger than hoop for reliable detection

        // Add trigger handler component
        BasketTriggerHandler triggerHandler = detectionZone.AddComponent<BasketTriggerHandler>();
        triggerHandler.Initialize(this);

        Debug.Log("Basket detection setup complete");
    }

    public void OnBallEnterDetection(Collider ballCollider)
    {
    
        BallController ball = ballCollider.GetComponent<BallController>();
        if (ball == null) return;


        Debug.Log("Ball found! HasScored: " + ball.HasScored +  ", IsInFlight: " +ball.IsInFlight);


        var tag = ballCollider.GetComponent<ShotTag>();
        bool isPerfect;
        bool hasBackboardBonus;

        isPerfect = tag.shotIntent == ShotTag.IntentType.Perfect;
        hasBackboardBonus = tag.shotIntent == ShotTag.IntentType.Backboard;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.RegisterShot(
                isSuccessful: true,
                isPerfect: isPerfect,
                hasBackboardBonus: hasBackboardBonus
            );
        }

        // Notify the ball that it scored
        ball.OnBallScored();

        Debug.Log("BASKET! Ball successfully entered the hoop with tag ! " + tag.shotIntent);
    }

    public void OnBallExitDetection(Collider ballCollider)
    {

        Debug.Log("Ball exited basket detection zone");

    }

}
