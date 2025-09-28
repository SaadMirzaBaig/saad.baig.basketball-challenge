using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardUI : MonoBehaviour
{
    [Header("UI Text")]
    public TMP_Text finalScoreText;
    public TMP_Text accuracyText;
    public TMP_Text perfectShotsText;
    public TMP_Text bonusCountText;

    public Button playAgainButton;
    public Button mainMenuButton;

    [Header("Star Rating")]
    public GameObject[] stars;
    

    void Start()
    {
        SetupButtons();
        DisplayResults();
    }
    
    //Setup all UI buttons
    private void SetupButtons()
    {
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        }

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    //Display the final results and Star rating
    private void DisplayResults()
    {
        if (ScoreManager.Instance == null)
            return;

        GameData stats = ScoreManager.Instance.GetFinalStats();

        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + stats.currentScore;

        if (accuracyText != null)
            accuracyText.text = $"Accuracy: {stats.GetAccuracy():F1}%";

        if (perfectShotsText != null)
            perfectShotsText.text = $"Perfect Shots: {stats.perfectShots}";

        if (bonusCountText != null)
            bonusCountText.text = $"Backboard Bonuses: {stats.backboardBonuses}";

        DisplayStarRating(stats.currentScore);
    }


    //Display the star rating
    private void DisplayStarRating(int score)
    {
        if (stars == null || stars.Length == 0)
        {
            Debug.LogWarning("RewardUI: Stars array is null or empty!");
            return;
        }

        if (ConfigurationManager.Instance == null)
        {
            Debug.LogError("RewardUI: ConfigurationManager.Instance is null!");
            return;
        }

        int earnedStars = 0;

        int[] scoreThresholds = ConfigurationManager.Instance.GetScoreThresholds();
        
        for (int i = 0; i < scoreThresholds.Length; i++)
        {
            if (score >= scoreThresholds[i])
            {
                earnedStars = i + 1;
            }
        }

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
            {
                bool shouldShow = i < earnedStars;
                stars[i].SetActive(shouldShow);
            }
            else
            {
                Debug.LogWarning($"RewardUI: Star {i + 1} is null!");
            }
        }
    }


    //On play again clicked
    private void OnPlayAgainClicked()
    {
        GameManager.Instance?.StartNewGame();
    }

    //On main menu clicked
    private void OnMainMenuClicked()
    {
        GameManager.Instance?.BackToMainMenu();
    }
}
