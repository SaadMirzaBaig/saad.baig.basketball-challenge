using UnityEngine;

public class BasketDetector : MonoBehaviour
{
    [Header("Basket Detection Setup")]
    [SerializeField] private Transform hoopCenter;
    private float hoopRadius = Constants.HOOP_RADIUS;
    private float detectionHeight = Constants.DETECTION_HEIGHT;


    // Detection components
    private SphereCollider detectionTrigger;
    
    // To avoid repeated GetComponent calls
    private BallController ballController;
    private ShotTag shotTag;
    
    // Audio integration
    private SwipeShotController swipeShotController;


    private void Awake()
    {
        SetupBasketDetection();
        FindSwipeShotController();
    }
    
    // Finds and caches the SwipeShotController for audio integration
    private void FindSwipeShotController()
    {
        swipeShotController = FindAnyObjectByType<SwipeShotController>();
        if (swipeShotController == null)
        {
            Debug.LogWarning("BasketDetector: No SwipeShotController found in scene. Score audio will not play.");
        }
    }

    //Setup basket detection zone
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
        detectionTrigger.radius = hoopRadius * Constants.DETECTION_RADIUS_MULTIPLIER; // Slightly larger than hoop for reliable detection

        // Add trigger handler component
        BasketTriggerHandler triggerHandler = detectionZone.AddComponent<BasketTriggerHandler>();
        triggerHandler.Initialize(this);

  
    }

    //On ball enter detection zone
    public void OnBallEnterDetection(Collider ballCollider)
    {
        // to avoid repeated GetComponent calls
        if (ballController == null || ballController.gameObject != ballCollider.gameObject)
        {
            ballController = ballCollider.GetComponent<BallController>();
            shotTag = ballCollider.GetComponent<ShotTag>();
        }
        
        if (ballController == null) return;

        bool isPerfect = shotTag != null && shotTag.shotIntent == ShotTag.IntentType.Perfect;
        bool hasBackboardBonus = shotTag != null && shotTag.shotIntent == ShotTag.IntentType.Backboard;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.RegisterShot(
                isSuccessful: true,
                isPerfect: isPerfect,
                hasBackboardBonus: hasBackboardBonus
            );
        }

        // Play score sound
        if (swipeShotController != null)
        {
            swipeShotController.PlayScoreSound();
        }

        // Notify the ball that it scored
        ballController.OnBallScored();

    }


}
