using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    [Header("UI")]
    //Score text    
    public TMP_Text scoreText;
    //Game time text
    public TMP_Text timeText;
    //Pause button
    public Button pauseButton;
    //Pause panel
    public GameObject pausePanel;
    //Resume button
    public Button resumeButton;
    //Main menu button
    public Button mainMenuButton;
    //Bonus text

    public TMP_Text bonusText;
    //Bonus popup
    public GameObject bonusPopup;

    // to avoid string allocations
    private int lastScore = -1;
    private float lastTime = -1f;

    // Start is called before the first frame update
    void Start()
    {
        SetupButtons();
        SubscribeToEvents();
        InitializeUI();
    }

    //On destroy to unsubscribe from events
    void OnDestroy()
    {
        UnsubscribeFromEvents();   
    }


    //Setup all UI buttons
    private void SetupButtons()
    {
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseClicked);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    //Subscribe to all events
    private void SubscribeToEvents()
    {
        GameDataManager.OnScoreChanged += UpdateScore;
        GameDataManager.OnTimeChanged += UpdateTime;
        ScoreManager.OnBackboardBonus += ShowBonusPopup;
        ScoreManager.OnBonusPeriodStarted += OnBonusPeriodStarted;
        ScoreManager.OnBonusPeriodEnded += OnBonusPeriodEnded;
        GameManager.OnStateChanged += OnGameStateChanged;
    }

    //Unsubscribe from all events
    private void UnsubscribeFromEvents()
    {
        GameDataManager.OnScoreChanged -= UpdateScore;
        GameDataManager.OnTimeChanged -= UpdateTime;
        ScoreManager.OnBackboardBonus -= ShowBonusPopup;
        ScoreManager.OnBonusPeriodStarted -= OnBonusPeriodStarted;
        ScoreManager.OnBonusPeriodEnded -= OnBonusPeriodEnded;
        GameManager.OnStateChanged -= OnGameStateChanged;
    }

    //Initialize all UI elements
    private void InitializeUI()
    {
        // Initialize with current data from GameDataManager
        if (GameDataManager.Instance != null)
        {
            GameData currentData = GameDataManager.Instance.GetCurrentGameData();
            UpdateScore(currentData.currentScore);
            UpdateTime(currentData.remainingTime);
        }

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if(bonusPopup != null)
        {
            bonusPopup.SetActive(false);
        }

    }

    //Update the score text
    private void UpdateScore(int score)
    {
        if (scoreText != null && score != lastScore)
        {
            lastScore = score;
            scoreText.text = "Score: " + score;
        }
    }

    //Update the time text
    private void UpdateTime(float time)
    {
        if (timeText != null && GameManager.Instance && !Mathf.Approximately(time, lastTime))
        {
            lastTime = time;
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            timeText.text = $"Time: {minutes:00}:{seconds:00}";
            timeText.gameObject.SetActive(true);
        }
        else if (timeText != null)
        {
            timeText.gameObject.SetActive(false);
        }
    }

    //Show the bonus popup
    private void ShowBonusPopup(int bonusPoints)
    {
        // Only show the popup if we're not in a bonus period (to avoid overwriting the bonus period text)
        if(bonusPopup != null)
        {
            bonusPopup.SetActive(true);

            Invoke(nameof(HideBonusPopup), 2f);
        }
    }

    //Hide the bonus popup
    private void HideBonusPopup()
    {
        if(bonusPopup != null)
        {
            bonusPopup.SetActive(false);
        }
    }

    //Display the bonus points you can get when bonus period starts
    private void OnBonusPeriodStarted(int bonusPoints)
    {
        if(bonusText != null)
        {
            bonusText.text = bonusPoints + " BONUS!";
        }

        if(bonusPopup != null)
        {
            bonusPopup.SetActive(true);
        }
    }

    //Disable the bonus popup
    private void OnBonusPeriodEnded()
    {
        if(bonusPopup != null)
        {
            bonusPopup.SetActive(false);
        }
    }

    //On game state changed
    private void OnGameStateChanged(GameState state)
    {
        if(pausePanel != null)
        {
            pausePanel.SetActive(state == GameState.Paused);
        }
    }

    //On pause clicked
    private void OnPauseClicked()
    {
        GameManager.Instance?.PauseGame();
    }

    //On resume clicked
    private void OnResumeClicked()
    {
        GameManager.Instance?.ResumeGame();
    }

    //On main menu clicked
    private void OnMainMenuClicked()
    {
        GameManager.Instance?.BackToMainMenu();
    }

}
