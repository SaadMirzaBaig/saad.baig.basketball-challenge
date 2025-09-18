using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardUI : MonoBehaviour
{
    public TMP_Text finalScoreText;
    public TMP_Text accuracyText;
    public TMP_Text perfectShotsText;
    public TMP_Text bonusCountText;

    public Button playAgainButton;
    public Button mainMenuButton;

    [Header("Star Rating")]
    public GameObject[] stars;
    public int[] scoreThresholds = { 20, 50, 100 };

    // Start is called before the first frame update
    void Start()
    {
        SetupButtons();
        DisplayResults();
    }
    
    private void SetupButtons()
    {
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        }

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    private void DisplayResults()
    {
        if (ScoreManager.Instance == null)
            return;

        GameStats stats = ScoreManager.Instance.GetFinalStats();

        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + stats.finalScore;

        if (accuracyText != null)
            accuracyText.text = $"Accuracy: {stats.accuracy:F1}%";

        if (perfectShotsText != null)
            perfectShotsText.text = $"Perfect Shots: {stats.perfectShots}";

        if (bonusCountText != null)
            bonusCountText.text = $"Backboard Bonuses: {stats.backboardBonuses}";

        DisplayStarRating(stats.finalScore);
    }


    private void DisplayStarRating(int score)
    {
        if (stars == null || stars.Length == 0)
            return;

        int earnedStars = 0;

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
                stars[i].SetActive(i < earnedStars);
            }
        }

    }


    private void OnPlayAgainClicked()
    {
        GameManager.Instance?.StartNewGame();
    }

    private void OnMainMenuClicked()
    {
        GameManager.Instance?.BackToMainMenu();
    }
}
