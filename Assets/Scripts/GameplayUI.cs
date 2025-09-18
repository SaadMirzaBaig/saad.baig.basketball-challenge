using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timeText;
    public TMP_Text shotsText;
    public Button pauseButton;
    public GameObject pausePanel;
    public Button resumeButton;
    public Button mainMenuButton;

    public TMP_Text bonusText;
    public GameObject bonusPopup;

    // Start is called before the first frame update
    void Start()
    {
        SetupButtons();
        SubscribeToEvents();
        InitializeUI();
    }

    // Update is called once per frame
    void OnDestroy()
    {
        UnsubscribeFromEvents();   
    }

    private void SetupButtons()
    {
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseClicked);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    private void SubscribeToEvents()
    {
        ScoreManager.OnScoreUpdated += UpdateScore;
        ScoreManager.OnShotStatsUpdated += UpdateShotStats;
        ScoreManager.OnBackboardBonus += ShowBonusPopup;
        GameManager.OnTimeUpdated += UpdateTime;
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    private void UnsubscribeFromEvents()
    {
        ScoreManager.OnScoreUpdated -= UpdateScore;
        ScoreManager.OnShotStatsUpdated -= UpdateShotStats;
        ScoreManager.OnBackboardBonus -= ShowBonusPopup;
        GameManager.OnTimeUpdated -= UpdateTime;
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    private void InitializeUI()
    {
        UpdateScore(0);
        UpdateTime(GameManager.Instance ? GameManager.Instance.gameTime : 60f);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if(bonusPopup != null)
        {
            bonusPopup.SetActive(false);
        }

    }

    private void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    private void UpdateTime(float time)
    {
        if (timeText != null && GameManager.Instance)
        {
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

    private void UpdateShotStats(int successful, int total)
    {
        // This can be used for additional UI elements showing shooting statistics
        Debug.Log($"Shot Stats: {successful}/{total}");
    }

    private void ShowBonusPopup(int bonusPoints)
    {
        if(bonusText != null)
        {
            bonusText.text = bonusPoints + " BONUS!";
        }

        if(bonusPopup != null)
        {
            bonusPopup.SetActive(true);

            Invoke(nameof(HideBonusPopup), 2f);
        }
    }

    private void HideBonusPopup()
    {
        if(bonusPopup != null)
        {
            bonusPopup.SetActive(false);
        }
    }

    private void OnGameStateChanged(GameState state)
    {
        if(pausePanel != null)
        {
            pausePanel.SetActive(state == GameState.Paused);
        }
    }

    private void OnPauseClicked()
    {
        GameManager.Instance?.PauseGame();
    }

    private void OnResumeClicked()
    {
        GameManager.Instance?.ResumeGame();
    }

    private void OnMainMenuClicked()
    {
        GameManager.Instance?.BackToMainMenu();
    }

}
