using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SwipeShotController : MonoBehaviour
{
    [Header("Shot Selection")]
    [SerializeField]private BallThrow ballThrow;

    [Header("Swipe Settings")]
    [SerializeField]private float swipeThreshold = 20f;
    [SerializeField] private float sliderSpeed = 900f; // how quivk to fill the slider
    [SerializeField] private float swipeTimeLimit = 0.5f; // Max time allowed for swipe

    [SerializeField] private float shotTypeDisplayDuration = 2.0f; // How long to show the shot type before resetting
    
    [Header("Shot Zones (0-1 range)")]
    [Range(0f, 1f)]
    [SerializeField] private float perfectShotZoneStart = 0.5f; // perefet start
    [Range(0f, 1f)]
    [SerializeField] private float perfectShotZoneEnd = 0.58f;   // perfet end
    [Range(0f, 1f)]
    [SerializeField] private float backboardShotZoneStart = 0.8f; // bavkboard start  
    [Range(0f, 1f)]
    [SerializeField] private float backboardShotZoneEnd = 0.9f;   // bavkboard end
    [Range(0f, 1f)]
    [SerializeField] private float ringShotZoneStart = 0f; // ringshot start
    [Range(0f, 1f)]
    [SerializeField] private float ringShotZoneEnd = 0.4f; // ringshot end



    [Header("UI References")]
    [SerializeField] private Slider shotPowerSlider;
    [SerializeField] private TMP_Text shotTypeText;

    [Header("Audio")]
    [SerializeField] private AudioClip swipeStartSound;
    [SerializeField] private AudioClip scoreSound;
    [SerializeField] private AudioClip ringHitSound;
    [SerializeField] private AudioClip backboardHitSound;

    // Input tracking
    private bool isSwipeActive = false;

    private Vector2 swipeStartPosition;
    private Vector2 currentSwipePosition;
    private float swipeStartTime;
    private float currentSwipePower = 0f;

    // Components
    private AudioSource audioSource;
    private BallController ballController;

    // Shot selection
    public enum ShotType
    {
        None,
        Ring,
        Normal,
        Perfect,
        Backboard,
        Away
    }

    private ShotType selectedShotType = ShotType.None;

    // UI caching to avoid per-frame allocations/updates
    private string lastShotTypeDisplayed;
    private float lastSliderValue = -1f;


    private void Start()
    {
        InitializeComponents();
        SetupUI();
    }
    
    private void Update()
    {
        // Only update UI when actively swiping
        if (isSwipeActive)
        {
            UpdateUI();
        }
        HandleInput();
    }


    private void InitializeComponents()
    {
        ValidateBallThrowReference();
        ValidateBallControllerReference();
        SetupAudioSource();
    }

    // Validates and sets up the BallThrow reference
    private void ValidateBallThrowReference()
    {
        if (ballThrow == null)
        {
            ballThrow = FindAnyObjectByType<BallThrow>();
            if (ballThrow == null)
            {
                Debug.LogError("SwipeShotController: No BallThrow component found in scene! Disabling component.");
                enabled = false;
                return;
            }
        }
    }

    // Validates and sets up the BallController reference
    private void ValidateBallControllerReference()
    {
        if (ballController == null)
        {
            ballController = FindAnyObjectByType<BallController>();
            if (ballController == null)
            {
                Debug.LogError("SwipeShotController: No BallController component found in scene! Disabling component.");
                enabled = false;
                return;
            }
        }
    }

    // Sets up the audio source component
    private void SetupAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

    }

    private void SetupUI()
    {

        if (shotTypeText != null)
        {
            lastShotTypeDisplayed = GetShotTypeDisplayName(ShotType.None);
            shotTypeText.text = lastShotTypeDisplayed;
        }

    }

    private void HandleInput()
    {
                // Block input when game is paused
        if (GameManager.Instance != null && GameManager.Instance.currentState == GameState.Paused)
        {
            return;
        }
        // Block input while ball is in flight
        if (ballController != null && ballController.isInFlight)
        {
            return;
        }
        // Mouse
        if (Input.GetMouseButtonDown(0))
        {
            StartSwipe(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0) && isSwipeActive)
        {
            UpdateSwipe(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) && isSwipeActive)
        {
            EndSwipe(Input.mousePosition);
        }

        // Touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    StartSwipe(touch.position);
                    break;

                case TouchPhase.Moved:
                    if (isSwipeActive)
                        UpdateSwipe(touch.position);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (isSwipeActive)
                        EndSwipe(touch.position);
                    break;
            }
        }
    }

    private void StartSwipe(Vector2 screenPosition)
    {
        if (isSwipeActive) return;

        swipeStartPosition = screenPosition;
        currentSwipePosition = screenPosition;
        
        swipeStartTime = Time.time;
        isSwipeActive = true;
        currentSwipePower = 0f;
       
        selectedShotType = ShotType.None;


        
    }

    private void UpdateSwipe(Vector2 screenPosition)
    {
        if (!isSwipeActive) return;

        currentSwipePosition = screenPosition;

        // Calculate swipe distance 
        Vector2 swipeDelta = currentSwipePosition - swipeStartPosition;
        float upwardDistance = swipeDelta.y;

        // Calculate the new potential power
        float swipePower = Mathf.Clamp01(upwardDistance / sliderSpeed);

        // Only increase the power, do not let it decrease
        if (swipePower > currentSwipePower)
        {
            currentSwipePower = swipePower;
        }

        // Check time limit
        if (Time.time - swipeStartTime > swipeTimeLimit)
        {
            EndSwipe(screenPosition);
        }
    }

    private void EndSwipe(Vector2 screenPosition)
    {
        if (!isSwipeActive) return;

        // Calculate final swipe distance
        Vector2 swipeDelta = screenPosition - swipeStartPosition;
        float upwardDistance = swipeDelta.y;

        if (upwardDistance < swipeThreshold)
        {
            CancelSwipe();
            return;
        }
        // Determine shot type based on power
        selectedShotType = GetShotTypeFromPower(currentSwipePower);

        // Update shot type display
        UpdateShotTypeDisplay();

        PlaySound(swipeStartSound);

        // Execute the selected shot
        ExecuteShot(selectedShotType);

        isSwipeActive = false;

    }
    private void CancelSwipe()
    {
        isSwipeActive = false;
        currentSwipePower = 0f;
        selectedShotType = ShotType.None;
        UpdateShotTypeDisplay();
    }

    private ShotType GetShotTypeFromPower(float power)
    {
        if(power >= ringShotZoneStart && power  <= ringShotZoneEnd)
        {
            return ShotType.Ring;
        }
        else if (power >= backboardShotZoneStart && power <= backboardShotZoneEnd)
        {
            return ShotType.Backboard;
        }
        else if (power >= perfectShotZoneStart && power <= perfectShotZoneEnd)
        {
            return ShotType.Perfect;
        }
        else if (power > backboardShotZoneEnd)
        {
            return ShotType.Away;
        }
        else if(power >=ringShotZoneEnd &&  power <= perfectShotZoneStart)
        {
            return ShotType.Normal;
        }
        else
        {
            return ShotType.Normal;
        }
    }

    private void ExecuteShot(ShotType shotType)
    {


        try
        {

            switch (shotType)
            {
                case ShotType.Ring:
                    ballThrow.ThrowAtRing();
                    break;

                case ShotType.Perfect:
                    ballThrow.ThrowPerfectShot();
                    break;

                case ShotType.Backboard:
                    ballThrow.ThrowBackboardShot();
                    break;

                case ShotType.Normal:
                    ballThrow.ThrowNormalShot();
                    break;

                case ShotType.Away:
                    ballThrow.ThrowAwayShot();
                    break;

                default:
                    Debug.LogWarning("SwipeShotController: Unknown shot type, defaulting to normal shot.");
                    ballThrow.ThrowNormalShot();
                    break;
            }

            Invoke(nameof(ResetUIAfterDelay), shotTypeDisplayDuration);
        }
        catch (System.Exception e)
        {
            Debug.LogError("SwipeShotController: Failed to execute shot: " + e.Message);
        }
    }

    private void UpdateUI()
    {
        // Slider value follows model state
        if (shotPowerSlider != null)
        {
            if (!Mathf.Approximately(lastSliderValue, currentSwipePower))
            {
                lastSliderValue = currentSwipePower;
                shotPowerSlider.value = currentSwipePower;
            }
        }

    }

    private void UpdateShotTypeDisplay()
    {
        if (shotTypeText != null)
        {
            string newShotTypeDisplay = GetShotTypeDisplayName(selectedShotType);
            
            // Only update if the text has changed to avoid unnecessary string allocations
            if (newShotTypeDisplay != lastShotTypeDisplayed)
            {
                lastShotTypeDisplayed = newShotTypeDisplay;
                shotTypeText.text = lastShotTypeDisplayed;
            }
        }
    }

    private string GetShotTypeDisplayName(ShotType shotType)
    {
        switch (shotType)
        {
            case ShotType.Perfect: 
                return "PERFECT SHOT";
            case ShotType.Backboard: 
                return "BACKBOARD SHOT";
            case ShotType.Away: 
                return "AWAY SHOT";
            case ShotType.Normal: 
                return "NORMAL SHOT";
            case ShotType.Ring:
                return "RING SHOT";
            default: 
                return "SWIPE UP TO SHOOT";
        }
    }

    private void ResetUIAfterDelay()
    {

        // Reset the underlying state that drives the UI
        currentSwipePower = 0f;
        selectedShotType = ShotType.None;

        lastSliderValue = -1f; // force next UpdateUI to write 0
        if (shotPowerSlider != null) shotPowerSlider.value = 0f;
        lastShotTypeDisplayed = GetShotTypeDisplayName(selectedShotType);
        if (shotTypeText != null) shotTypeText.text = lastShotTypeDisplayed;


    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Plays sound when ball scores
    public void PlayScoreSound()
    {
        PlaySound(scoreSound);
    }

    // Plays sound when ball hits the ring
    public void PlayRingHitSound()
    {
        PlaySound(ringHitSound);
    }

    // Plays sound when ball hits the backboard
    public void PlayBackboardHitSound()
    {
        PlaySound(backboardHitSound);
    }



    private void OnGameStateChanged(GameState newState)
    {
        // Cancel any active swipe when game is paused
        if (newState == GameState.Paused && isSwipeActive)
        {
            CancelSwipe();
        }
    }

}
