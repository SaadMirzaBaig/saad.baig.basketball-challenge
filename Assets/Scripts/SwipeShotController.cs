using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SwipeShotController : MonoBehaviour
{
    [Header("Shot Selection")]
    [SerializeField]private BallThrow ballThrow;

    [Header("Swipe Settings")]
    [SerializeField]private float swipeThreshold = 100f;
    private float sliderSpeed = 800f; // how quivk to fill the slider
    [SerializeField] private float swipeTimeLimit = 2f; // Max time allowed for swipe

    [Header("Shot Zones (0-1 range)")]
    [Range(0f, 1f)]
    [SerializeField] private float perfectShotZoneStart = 0.4f; // perefet start
    [Range(0f, 1f)]
    [SerializeField] private float perfectShotZoneEnd = 0.7f;   // perfet end
    [Range(0f, 1f)]
    [SerializeField] private float backboardShotZoneStart = 0.7f; // bavkboard start  
    [Range(0f, 1f)]
    [SerializeField] private float backboardShotZoneEnd = 0.9f;   // bavkboard end
    [Range(0f, 1f)]
    [SerializeField] private float ringShotZoneStart = 0f; // ringshot start
    [Range(0f, 1f)]
    [SerializeField] private float ringShotZoneEnd = 0.38f; // ringshot end

    [Header("UI References")]
    [SerializeField] private Slider shotPowerSlider;
    [SerializeField] private Image sliderFill;
    [SerializeField] private TMP_Text shotTypeText;

    [Header("Visual Feedback")]
    [SerializeField] private Color normalZoneColor = Color.white;
    [SerializeField] private Color perfectZoneColor = Color.green;
    [SerializeField] private Color backboardZoneColor = Color.blue;
    [SerializeField] private Color awayZoneColor = Color.red;

    [Header("Audio")]
    [SerializeField] private AudioClip swipeStartSound;
    [SerializeField] private AudioClip shotSelectedSound;

    // Input tracking
    private bool isSwipeActive = false;

    private Vector2 swipeStartPosition;
    private Vector2 currentSwipePosition;
    private float swipeStartTime;
    private float currentSwipePower = 0f;

    // Components
    private AudioSource audioSource;

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


    private void Start()
    {
        InitializeComponents();
        SetupUI();
    }

    private void Update()
    {
        if (!CanAcceptInput()) return;

        HandleInput();
        UpdateUI();
    }

    private void InitializeComponents()
    {
        if (ballThrow == null)
            ballThrow = FindAnyObjectByType<BallThrow>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

    }

    private void SetupUI()
    {
        if (shotPowerSlider != null)
        {
            shotPowerSlider.minValue = 0f;
            shotPowerSlider.maxValue = 1f;
            shotPowerSlider.value = 0f;
            shotPowerSlider.interactable = false; // Controlled via swipe
        }

        if (shotTypeText != null)
            shotTypeText.text = GetShotTypeDisplayName(ShotType.None);

        if (sliderFill != null)
            sliderFill.color = normalZoneColor;
    }

    private bool CanAcceptInput()
    {

        // Don't accept input if ball is in flight
        if (ballThrow != null && ballThrow.ballRigidbody != null)
        {
            BallController ballController = ballThrow.ballRigidbody.GetComponent<BallController>();
            if (ballController != null && ballController.isInFlight)
                return false;
        }

        return true;
    }

    private void HandleInput()
    {
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

        PlaySound(swipeStartSound);

        Debug.Log("Swipe started");
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

        // Determine shot type based on power
        selectedShotType = GetShotTypeFromPower(currentSwipePower);

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

        // Execute the selected shot
        ExecuteShot(selectedShotType);

        isSwipeActive = false;

    }
    private void CancelSwipe()
    {
        isSwipeActive = false;
        currentSwipePower = 0f;
        selectedShotType = ShotType.None;

        Debug.Log("Swipe cancelled - not enough distance");
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
        else
        {
            return ShotType.Normal;
        }
    }

    private void ExecuteShot(ShotType shotType)
    {
        if (ballThrow == null)
        {
            Debug.LogError("BallThrow reference not assigned!");
            return;
        }

        PlaySound(shotSelectedSound);

        switch (shotType)
        {
            case ShotType.Ring:
                ballThrow.ThrowAtRing();
                Debug.Log("Ring Shot exeCuted");
                break;

            case ShotType.Perfect:
                ballThrow.ThrowPerfectShot();
                Debug.Log("Perfect Shot executed!");
                break;

            case ShotType.Backboard:
                ballThrow.ThrowBackboardShot();
                Debug.Log("Backboard Shot executed!");
                break;

            case ShotType.Normal:
                ballThrow.ThrowNormalShot();
                Debug.Log("Normal Shot executed!");
                break;

            case ShotType.Away:
                ballThrow.ThrowAwayShot();
                Debug.Log("Away Shot executed!");
                break;

            default:
                ballThrow.ThrowNormalShot();
                Debug.Log("Default Normal Shot executed!");
                break;
        }

        Invoke(nameof(ResetUIAfterDelay), 2f);
    }

    private void UpdateUI()
    {
        // Slider value follows model state
        if (shotPowerSlider != null)
        {
            shotPowerSlider.value = currentSwipePower;
        }

        // Slider color based on shot type
        UpdateSliderColor(selectedShotType);

        // Shot type label
        if (shotTypeText != null)
        {
            shotTypeText.text = GetShotTypeDisplayName(selectedShotType);
        }
    }

    private void UpdateSliderColor(ShotType shotType)
    {
        if (sliderFill == null) return;

        Color targetColor;

        switch (shotType)
        {
            case ShotType.Perfect:
                targetColor = perfectZoneColor;
                break;
            case ShotType.Backboard:
                targetColor = backboardZoneColor;
                break;
            case ShotType.Away:
                targetColor = awayZoneColor;
                break;
            case ShotType.Normal:
            case ShotType.None:
            default:
                targetColor = normalZoneColor;
                break;
        }

        sliderFill.color = targetColor;
    }

    private string GetShotTypeDisplayName(ShotType shotType)
    {
        switch (shotType)
        {
            case ShotType.Perfect: return "PERFECT SHOT";
            case ShotType.Backboard: return "BACKBOARD SHOT";
            case ShotType.Away: return "AWAY SHOT";
            case ShotType.Normal: return "NORMAL SHOT";
            default: return "SWIPE UP TO SHOOT";
        }
    }

    private void ResetUIAfterDelay()
    {

        // Reset the underlying state that drives the UI
        currentSwipePower = 0f;
        selectedShotType = ShotType.None;

        if (shotPowerSlider != null) shotPowerSlider.value = 0f;
        if (sliderFill != null) sliderFill.fillAmount = 0f;
        if (shotTypeText != null) shotTypeText.text = GetShotTypeDisplayName(selectedShotType);


        Debug.Log("Swipe UI reset after delay.");
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

}
